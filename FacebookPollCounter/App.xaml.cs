using FacebookPollCounter.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FacebookPollCounter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Settings = Settings.Load();
        }

        public Settings Settings { get; private set; }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Save();
        }
    }
}
