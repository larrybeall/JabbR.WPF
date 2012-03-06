using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using Jabbr.WPF.Infrastructure;
using Caliburn.Micro;

namespace Jabbr.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public JabbrManager JabbrManager { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            JabbrManager = IoC.Get<JabbrManager>();
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            if(System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debug.WriteLine(unobservedTaskExceptionEventArgs.Exception.Message);
            else
            {
                MessageBox.Show(unobservedTaskExceptionEventArgs.Exception.Message);
            }
            unobservedTaskExceptionEventArgs.SetObserved();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            JabbrManager.Disconnect();
            base.OnExit(e);
        }
    }
}
