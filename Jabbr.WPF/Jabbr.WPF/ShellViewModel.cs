using System;

namespace Jabbr.WPF {
    using System.ComponentModel.Composition;
    using System.Windows;
using Caliburn.Micro;
    using Jabbr.WPF.Authentication;

    [Export(typeof(IShell))]
    public class ShellViewModel : IShell
    {
        private readonly IWindowManager _windowManager;

        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        
        public void Test()
        {
            //var manager = Infrastructure.JabbrManager.Instance;
            var viewModel = new AuthWindowViewModel();
            //_windowManager.ShowWindow(viewModel);
            _windowManager.ShowDialog(viewModel);
        }
    }
}
