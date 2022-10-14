using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly SerialPort _serialPort = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _controller;

            _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            ConnectArduinoPortAsync();
        }

        private async void ConnectArduinoPortAsync()
        {
            _controller.IsSearchingPort = true;
            await Task.Delay(3000);

            string? arduinoPort = _controller.GetArduinoPort();

            if (arduinoPort is not null)
            {
                _serialPort.PortName = arduinoPort;
                _serialPort.BaudRate = 9600;
                _serialPort.Open();

                _controller.IsPortFound = true;
                _controller.IsConnected = true;
            }

            _controller.IsSearchingPort = false;
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string controllerData = _serialPort.ReadLine();
            _controller.ParseDataFromSerial(controllerData);
        }

       
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void radioAutoMode_Checked(object sender, RoutedEventArgs e)
        {
            _controller.IsManualMode = false;
            _controller.IsAutoMode = true;
        }

        private void radioManualMode_Checked(object sender, RoutedEventArgs e)
        {
            _controller.IsManualMode = true;
            _controller.IsAutoMode = false;
        }

        private void btnMinusTemperature_Click(object sender, RoutedEventArgs e)
        {
            _controller.ActivationTemp--;
        }

        private void btnAddTemperature_Click(object sender, RoutedEventArgs e)
        {
            _controller.ActivationTemp++;
        }

        private void btnMinusStartFanSpeed_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartFanSpeed--;
        }

        private void btnAddStartFanSpeed_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartFanSpeed++;
        }

        private void btnFanSpeed_Off_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 0;
        }

        private void btnFanSpeed_Max_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 99;
        }

        private void btnFanSpeed_30_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 30;
        }

        private void btnFanSpeed_50_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 50;
        }

        private void btnFanSpeed_70_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 70;
        }

        private void btnFanSpeed_90_Click(object sender, RoutedEventArgs e)
        {
            _controller.FanSpeed = 90;
        }

        private void lblStartFanSpeed_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^1-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void lblActivationTemperature_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
