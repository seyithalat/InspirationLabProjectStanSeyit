using System;
using System.Windows;
using Microsoft.Win32;

namespace InspirationLabProjectStanSeyit
{

    public partial class App : Application
    {
        public App()
        {
            SetBrowserFeatureControl();
            // Database initialization removed; handled elsewhere if needed
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Start with the main window instead of login
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void SetBrowserFeatureControl()
        {
            string appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION"))
            {
                key.SetValue(appName, 11001, RegistryValueKind.DWord); // 11001 = IE11 mode
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Database cleanup removed; handled elsewhere if needed
            base.OnExit(e);
        }
    }
}