// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.ControlPalette.Model;
using FluentEditor.Model;
using FluentEditor.OuterNav;
using FluentEditorShared;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace FluentEditor
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            lock (_initLock)
            {
                if (_state != AppState.FirstLaunch)
                {
                    if (e.PrelaunchActivated == false)
                    {
                        Window.Current.Activate();
                    }
                }
                else
                {
                    _ = FirstTimeInit(e);
                }
            }
        }

        private async Task FirstTimeInit(LaunchActivatedEventArgs e)
        {
            lock(_initLock)
            {
                _state = AppState.Initializing;
            }

            await SetupDependencies();

            Window.Current.Content = new OuterNavPage(_outerNavViewModel);

            lock(_initLock)
            {
                _state = AppState.Running;
            }

            if (e.PrelaunchActivated == false)
            {
                Window.Current.Activate();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            lock (_initLock)
            {
                if (_state == AppState.Running)
                {
                    var deferral = e.SuspendingOperation.GetDeferral();
                    var suspendTask = SuspendApp(_mainNavModel, _paletteModel);
                    suspendTask.ContinueWith((t) =>
                    {
                        deferral.Complete();
                    });
                }
            }
        }

        private async Task SuspendApp(IMainNavModel mainModel, IControlPaletteModel controlPaletteModel)
        {
            await controlPaletteModel.HandleAppSuspend();
            await mainModel.HandleAppSuspend();
        }

        private async Task SetupDependencies()
        {
            var stringProvider = new StringProvider(Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse());
            var exportProvider = new ControlPaletteExportProvider();

            var paletteModel = new ControlPaletteModel();
            await paletteModel.InitializeData(stringProvider, stringProvider.GetString("ControlPaletteDataPath"));

            var navModel = new MainNavModel(stringProvider);
            await navModel.InitializeData(stringProvider.GetString("MainDataPath"), paletteModel, exportProvider);

            lock (_initLock)
            {
                _stringProvider = stringProvider;
                _exportProvider = exportProvider;

                _paletteModel = paletteModel;

                _mainNavModel = navModel;
                _outerNavViewModel = new OuterNavViewModel(_mainNavModel.NavItems, _mainNavModel.DefaultNavItem);
            }
        }

        private object _initLock = new object();
        private enum AppState { FirstLaunch, Initializing, Running }
        private AppState _state = AppState.FirstLaunch;

        private StringProvider _stringProvider;
        private ControlPaletteExportProvider _exportProvider;
        private IMainNavModel _mainNavModel;
        private IControlPaletteModel _paletteModel;

        private OuterNavViewModel _outerNavViewModel;
    }
}
