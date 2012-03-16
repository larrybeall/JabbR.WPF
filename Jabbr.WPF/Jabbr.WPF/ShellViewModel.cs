using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Authentication;
using Jabbr.WPF.Infrastructure;

namespace Jabbr.WPF
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly ServiceLocator _serviceLocator;

        public ShellViewModel(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;

            var login = _serviceLocator.GetViewModel<LoginViewModel>();
            ActivateItem(login);
            Items.Add(login);
        }
    }
}
