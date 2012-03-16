using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

namespace Jabbr.WPF.Infrastructure
{
    public class ServiceLocator
    {
        private readonly IKernel _kernel;

        public ServiceLocator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public TViewModel GetViewModel<TViewModel>()
        {
            return _kernel.Get<TViewModel>();
        }
    }
}
