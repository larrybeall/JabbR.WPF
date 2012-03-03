using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace Jabbr.WPF.Authentication
{
    /// <summary>
    /// Interaction logic for AuthenticationWindowView.xaml
    /// </summary>
    public partial class AuthenticationWindowView : MetroWindow
    {
        private bool _initializing = true;
        public AuthenticationWindowView()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            authWidget.SizeChanged += AuthWidgetOnSizeChanged;
        }

        private void AuthWidgetOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (sizeChangedEventArgs.HeightChanged && !Height.Equals(sizeChangedEventArgs.NewSize.Height))
                Height = sizeChangedEventArgs.NewSize.Height + 50;

            if (sizeChangedEventArgs.WidthChanged && !Width.Equals(sizeChangedEventArgs.NewSize.Width))
                Width = sizeChangedEventArgs.NewSize.Width + 15;
        }
    }
}
