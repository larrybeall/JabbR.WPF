using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;

namespace Jabbr.WPF
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
        }

        private void TitleBarMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
                DragMove();

            if (e.ClickCount == 2 && (ResizeMode == ResizeMode.CanResizeWithGrip || ResizeMode == ResizeMode.CanResize))
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed
                && e.LeftButton == MouseButtonState.Pressed && WindowState == WindowState.Maximized)
            {
                // Calcualting correct left coordinate for multi-screen system.
                Point mouseAbsolute = PointToScreen(Mouse.GetPosition(this));
                double width = RestoreBounds.Width;
                double left = mouseAbsolute.X - width / 2;

                // Aligning window's position to fit the screen.
                double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
                left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

                var mousePosition = e.MouseDevice.GetPosition(this);

                // When dragging the window down at the very top of the border,
                // move the window a bit upwards to avoid showing the resize handle as soon as the mouse button is released
                Top = mousePosition.Y < 5 ? -5 : mouseAbsolute.Y - mousePosition.Y;
                Left = left;

                // Restore window to normal state.
                WindowState = WindowState.Normal;

                DragMove();
            }
        }
    }
}
