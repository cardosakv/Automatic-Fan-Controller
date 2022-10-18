using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Automatic_Fan_Controller
{
    public partial class MainWindow : Window
    {
        private readonly Controller _controller = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _controller;
            _controller.ConnectArduinoPortAsync();
        }

        private void RadioAutoMode_Checked(object sender, RoutedEventArgs e)
        {
            _controller.IsManualMode = false;
            _controller.IsAutoMode = true;
        }

        private void RadioManualMode_Checked(object sender, RoutedEventArgs e)
        {
            _controller.IsManualMode = true;
            _controller.IsAutoMode = false;
        }

        private void BtnMinusTemperature_Click(object sender, RoutedEventArgs e)
        {
            _controller.ActivationTemp--;
        }

        private void BtnAddTemperature_Click(object sender, RoutedEventArgs e)
        {
            _controller.ActivationTemp++;
        }

        private void BtnMinusStartFanSpeed_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartFanSpeed--;
        }

        private void BtnAddStartFanSpeed_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartFanSpeed++;
        }

        private void BtnFanSpeed_Off_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 0;
        }

        private void BtnFanSpeed_Max_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 100;
        }

        private void BtnFanSpeed_20_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 20;
        }

        private void BtnFanSpeed_40_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 40;
        }

        private void BtnFanSpeed_60_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 60;
        }

        private void BtnFanSpeed_80_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 80;
        }

        private void LblStartFanSpeed_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (lblStartFanSpeed.Text != string.Empty)
            {
                _controller.FanSpeed = int.Parse(lblStartFanSpeed.Text);
            }
        }

        private void LblActivationTemperature_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (lblActivationTemperature.Text != string.Empty)
            {
                _controller.ActivationTemp = int.Parse(lblActivationTemperature.Text);
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Toolbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void lblStartFanSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lblStartFanSpeed.Text != string.Empty)
            {
                _controller.StartFanSpeed = int.Parse(lblStartFanSpeed.Text);
            }
        }

        private void lblActivationTemperature_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lblActivationTemperature.Text != string.Empty)
            {
                _controller.ActivationTemp = int.Parse(lblActivationTemperature.Text);
            }
        }
    }
}
