using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure.Services;

namespace Jabbr.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AuthenticationService _authenticationService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            // due to the fact that you have to subscribe to all events
            // from jabbr that you would like to know about before a connection
            // we will initialize all singleton service instances at startup
            InitializeServices();

            _authenticationService = IoC.Get<AuthenticationService>();
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender,
                                                            UnobservedTaskExceptionEventArgs
                                                                unobservedTaskExceptionEventArgs)
        {
            if (Debugger.IsAttached)
                Debug.WriteLine(unobservedTaskExceptionEventArgs.Exception.Message);
            else
            {
                MessageBox.Show(unobservedTaskExceptionEventArgs.Exception.Message);
            }
            unobservedTaskExceptionEventArgs.SetObserved();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _authenticationService.SignOut();
            base.OnExit(e);
        }

        private void InitializeServices()
        {
            IoC.Get<RoomService>();
            IoC.Get<UserService>();
            IoC.Get<AuthenticationService>();
            IoC.Get<MessageService>();
        }
    }
}