// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;

namespace FluentEditorShared.ViewCommon
{
    public class VisualStateAdapter
    {
        private List<(VisualStateGroup group, Action<VisualStateGroup, VisualState, VisualState> onChanging, Action<VisualStateGroup, VisualState, VisualState> onChanged)> _groupListeners = new List<(VisualStateGroup group, Action<VisualStateGroup, VisualState, VisualState> onChanging, Action<VisualStateGroup, VisualState, VisualState> onChanged)>();

        private FrameworkElement _containerElement = null;
        public FrameworkElement ContainerElement
        {
            get { return _containerElement; }
        }

        public VisualState GetActiveStateForGroup(string groupName)
        {
            var group = GetGroupByName(groupName);
            if (group != null)
            {
                return group.CurrentState;
            }
            return null;
        }

        public VisualStateGroup GetGroupByName(string groupName)
        {
            if (_containerElement == null)
            {
                return null;
            }
            IList<VisualStateGroup> groups = VisualStateManager.GetVisualStateGroups(_containerElement);
            foreach (VisualStateGroup group in groups)
            {
                if (group.Name == groupName)
                {
                    return group;
                }
            }
            return null;
        }

        public VisualStateGroup GetGroupByStateName(string stateName)
        {
            if (_containerElement == null)
            {
                return null;
            }
            IList<VisualStateGroup> groups = VisualStateManager.GetVisualStateGroups(_containerElement);
            foreach (VisualStateGroup group in groups)
            {
                foreach (VisualState state in group.States)
                {
                    if (state.Name == stateName)
                    {
                        return group;
                    }
                }
            }
            return null;
        }

        public void AddGroupListeners(string groupName, Action<VisualStateGroup, VisualState, VisualState> onChanging, Action<VisualStateGroup, VisualState, VisualState> onChanged)
        {
            if (_containerElement == null)
            {
                throw new Exception("Cannot add group listeners while not attached");
            }
            var group = GetGroupByName(groupName);
            if (group == null)
            {
                throw new ArgumentException("Could not find group with name " + groupName, "groupName");
            }

            _groupListeners.Add((group, onChanging, onChanged));
        }

        // This makes the big assumption that new states and groups are not being added or removed after Attach is called
        public void Attach(FrameworkElement containerElement)
        {
            Detach();

            _containerElement = containerElement;

            if (_containerElement == null)
            {
                return;
            }

            _containerElement.Unloaded -= _containerElement_Unloaded;
            _containerElement.Unloaded += _containerElement_Unloaded;

            IList<VisualStateGroup> groups = VisualStateManager.GetVisualStateGroups(_containerElement);
            foreach (VisualStateGroup group in groups)
            {
                group.CurrentStateChanging -= Group_CurrentStateChanging;
                group.CurrentStateChanged -= Group_CurrentStateChanged;
                group.CurrentStateChanging += Group_CurrentStateChanging;
                group.CurrentStateChanged += Group_CurrentStateChanged;
            }
        }

        private void _containerElement_Unloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        private void Group_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState == null || string.IsNullOrEmpty(e.NewState.Name))
            {
                return;
            }

            var group = GetGroupByStateName(e.NewState.Name);
            if (group == null)
            {
                return;
            }

            var listener = _groupListeners.SingleOrDefault((a) => a.group == group);
            listener.onChanged?.Invoke(group, e.OldState, e.NewState);
        }

        private void Group_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState == null || string.IsNullOrEmpty(e.NewState.Name))
            {
                return;
            }

            var group = GetGroupByStateName(e.NewState.Name);
            if (group == null)
            {
                return;
            }

            var listener = _groupListeners.SingleOrDefault((a) => a.group == group);
            listener.onChanging?.Invoke(group, e.OldState, e.NewState);
        }

        public void Detach()
        {
            var containerElement = _containerElement;
            _containerElement = null;

            _groupListeners.Clear();

            if (containerElement != null)
            {
                containerElement.Unloaded -= _containerElement_Unloaded;
                IList<VisualStateGroup> groups = VisualStateManager.GetVisualStateGroups(containerElement);
                foreach (VisualStateGroup group in groups)
                {
                    group.CurrentStateChanging -= Group_CurrentStateChanging;
                    group.CurrentStateChanged -= Group_CurrentStateChanged;
                }
            }
        }
    }
}
