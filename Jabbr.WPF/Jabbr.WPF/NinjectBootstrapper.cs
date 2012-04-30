using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using JabbR.Client;
using Jabbr.WPF.Infrastructure.Services;
using Ninject;
using SignalR.Client.Transports;

namespace Jabbr.WPF
{
    public class NinjectBootstrapper : Bootstrapper<ShellViewModel>
    {
        private IKernel _kernel;

        protected override void Configure()
        {
            _kernel = new StandardKernel();
            _kernel.Load(Assembly.GetExecutingAssembly());

            if (Directory.Exists(@".\modules"))
                _kernel.Load(@".\modules\*.dll");

            IEnumerable<Assembly> assemblies = _kernel.GetModules()
                .Select(module => module.GetType().Assembly)
                .Distinct();

            IEnumerable<Assembly> toObserve = assemblies.Except(AssemblySource.Instance);

            AssemblySource.Instance.AddRange(toObserve);

            RegisterApplicationBindings();
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance = _kernel.Get(service, key);
            return instance;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _kernel.GetAll(service);
        }

        private void RegisterApplicationBindings()
        {
            _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            _kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

            _kernel.Bind<JabbRClient>().ToMethod(
                context => new JabbRClient("http://jabbr.net", new LongPollingTransport())).InSingletonScope();

            _kernel.Bind<ShellViewModel>().ToSelf().InSingletonScope();
            _kernel.Bind<AuthenticationService>().ToSelf().InSingletonScope();
            _kernel.Bind<UserService>().ToSelf().InSingletonScope();
            _kernel.Bind<MessageService>().ToSelf().InSingletonScope();
            _kernel.Bind<RoomService>().ToSelf().InSingletonScope();
        }
    }
}