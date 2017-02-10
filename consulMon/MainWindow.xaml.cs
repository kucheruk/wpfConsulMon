using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ConsulMon.ViewModels;

namespace ConsulMon
{
    public partial class MainWindow
    {
        private bool _dragging;
        private Rect GetCurrentScreen() => GetCurrentWorkingArea();

        private Rect GetCurrentWorkingArea()
        {
            var screen = WpfScreen.GetScreenFrom(this);
            return screen.WorkingArea;
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AppViewmodel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetInitialWindowPosition();
            InitializeWindowPositionMonitoring();
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _dragging = true;
                DragMove();
            }
        }

        private void InitializeWindowPositionMonitoring()
        {
            var timer = new DispatcherTimer();
            timer.Tick += (sender, args) =>
            {
                //TODO: sticky edges
                if (!_dragging)
                {
                    var screen = GetCurrentScreen();
                    if (Left > (screen.Left + screen.Width) - Width)
                        Left = (screen.Left + screen.Width) - Width;
                    else if (Left < screen.Left)
                        Left = screen.Left;

                    if (Top > screen.Top + screen.Height - Height)
                        Top = screen.Top + screen.Height - Height;
                    else if (Top < screen.Top)
                        Top = screen.Top;
                }
            };

            timer.Interval = new TimeSpan(0, 0, 0, 10);
            timer.Start();
        }

        private void SetInitialWindowPosition()
        {
            Left = GetCurrentScreen().Width - Width;
            Top = GetCurrentScreen().Height / 2d - Height / 2;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                _dragging = false;
        }

        private void MainWindow_OnActivated(object sender, EventArgs e)
        {
            Topmost = true;
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            Topmost = true;
            Activate();
        }
    }
    
}