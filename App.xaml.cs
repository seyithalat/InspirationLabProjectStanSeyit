using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InspirationLabProjectStanSeyit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Database initialization removed; handled elsewhere if needed
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Database cleanup removed; handled elsewhere if needed
            base.OnExit(e);
        }
    }
}
