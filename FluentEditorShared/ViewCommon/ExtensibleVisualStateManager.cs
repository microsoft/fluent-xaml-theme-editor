// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FluentEditorShared.ViewCommon
{
    public class ExtensibleVisualStateManager : VisualStateManager
    {
        protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            if (templateRoot == null)
            {
                return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
            }
            else
            {
                IControlTemplateExtension extension = ExtensibleVisualStateManagerProperties.GetVisualStateManagerExtension(templateRoot);

                if (extension != null)
                {
                    bool callCore = false;
                    bool retVal = extension.HandleStateChange(out callCore, control, templateRoot, stateName, group, state, useTransitions);
                    if (callCore)
                    {
                        return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
                    }
                    else
                    {
                        return retVal;
                    }
                }
                else
                {
                    return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
                }
            }
        }
    }

    public interface IControlTemplateExtension
    {
        bool HandleStateChange(out bool callGoToStateCore, Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions);
    }

    public class PassthroughExtension : IControlTemplateExtension
    {
        public bool HandleStateChange(out bool callGoToStateCore, Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            if (control is IControlTemplateExtension controlHandler)
            {
                return controlHandler.HandleStateChange(out callGoToStateCore, control, templateRoot, stateName, group, state, useTransitions);
            }
            else
            {
                callGoToStateCore = true;
                return true;
            }
        }
    }

    public static class ExtensibleVisualStateManagerProperties
    {
        private static object _stateManagersLock = new object();
        private static Dictionary<CoreDispatcher, ExtensibleVisualStateManager> _stateManagers = new Dictionary<CoreDispatcher, ExtensibleVisualStateManager>();

        public static ExtensibleVisualStateManager GetManagerForDispatcher(CoreDispatcher dispatcher)
        {
            if (!dispatcher.HasThreadAccess)
            {
                throw new InvalidOperationException("Current thread does not match provided dispatcher");
            }
            lock (_stateManagersLock)
            {
                if (_stateManagers.ContainsKey(dispatcher))
                {
                    return _stateManagers[dispatcher];
                }
                else
                {
                    ExtensibleVisualStateManager manager = new ExtensibleVisualStateManager();
                    _stateManagers.Add(dispatcher, manager);
                    return manager;
                }
            }
        }

        // In the event that we need to clear out the manager for a given Dispatcher
        // If closing a window with its own Dispatcher thread this might be needed
        public static void ClearResourcesForDispatcher(CoreDispatcher dispatcher)
        {
            lock (_stateManagersLock)
            {
                if (_stateManagers.ContainsKey(dispatcher))
                {
                    _stateManagers.Remove(dispatcher);
                }
            }
        }

        public static readonly DependencyProperty ControlTemplateExtensionProperty = DependencyProperty.Register("VisualStateManagerExtension", typeof(IControlTemplateExtension), typeof(ExtensibleVisualStateManagerProperties), new PropertyMetadata(null, new PropertyChangedCallback(OnControlTemplateExtensionPropertyChanged)));

        private static void OnControlTemplateExtensionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                IControlTemplateExtension newValue = e.NewValue as IControlTemplateExtension;

                if (newValue != null)
                {
                    VisualStateManager.SetCustomVisualStateManager(element, GetManagerForDispatcher(element.Dispatcher));
                }
                else
                {
                    VisualStateManager.SetCustomVisualStateManager(element, null);
                }
            }
        }

        public static IControlTemplateExtension GetVisualStateManagerExtension(DependencyObject d)
        {
            return d.GetValue(ControlTemplateExtensionProperty) as IControlTemplateExtension;
        }

        public static void SetVisualStateManagerExtension(DependencyObject d, IControlTemplateExtension value)
        {
            d.SetValue(ControlTemplateExtensionProperty, value);
        }
    }
}
