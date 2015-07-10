using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FiloStreamer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool _isMoving;
        private Point _lastPoint;

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var window = button.DataContext as Window;
            window.WindowState = System.Windows.WindowState.Minimized;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var window = button.DataContext as Window;
            window.Close();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window window = (sender as FrameworkElement).DataContext as Window;
            _isMoving = true;
            _lastPoint = e.GetPosition((sender as FrameworkElement));
            (sender as FrameworkElement).CaptureMouse();
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isMoving && sender == System.Windows.Input.Mouse.Captured)
            {
                Window window = (sender as FrameworkElement).DataContext as Window;
                Point current = e.GetPosition((sender as FrameworkElement));
                window.Top += current.Y - this._lastPoint.Y;
                window.Left += current.X - this._lastPoint.X;
            }
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isMoving = false;
            (sender as FrameworkElement).ReleaseMouseCapture();
        }
    }
}
