using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using Jabbr.WPF.Infrastructure;

namespace Jabbr.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            System.Diagnostics.Debugger.Break();
            unobservedTaskExceptionEventArgs.SetObserved();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            JabbrManager.Instance.Disconnect();
            base.OnExit(e);
        }
    }
}
