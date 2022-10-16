﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Automatic_Fan_Controller
{
    public  class Controller : INotifyPropertyChanged
    {
        private readonly SerialPort _serialPort = new();
        private readonly DispatcherTimer _serialTimer = new();

        private bool _isAutoMode = true;
        private bool _isManualMode = false;
        private bool _isSearchingPort = true;
        private bool _isPortFound = true;
        private bool _isConnected = false;
        private bool _isReady = false;
        private int _peopleCount = 0;
        private int _temperature = 0;
        private int _activationTemp = 25;
        private int _fanSpeed = 0;
        private int _startFanSpeed = 50;

        public Controller()
        {
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
        }

        public bool IsAutoMode
        {
            get { return _isAutoMode; }
            set 
            { 
                _isAutoMode = value;
                OnPropertyChanged("IsAutoMode");
                SendDataToArduino();
            }
        }

        public bool IsManualMode
        {
            get { return _isManualMode; }
            set
            {
                _isManualMode = value;
                OnPropertyChanged("IsManualMode");
                SendDataToArduino();
            }
        }

        public bool IsSearchingPort
        { 
            get { return _isSearchingPort; }
            set
            {
                _isSearchingPort = value;
                OnPropertyChanged("IsSearchingPort");
            }
        }

        public bool IsPortFound
        {
            get { return _isPortFound; }
            set
            {
                _isPortFound = value;
                OnPropertyChanged("IsPortFound");
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }

        public bool IsReady
        {
            get { return _isReady; }
            set
            {
                _isReady = value;
                OnPropertyChanged("IsReady");
            }
        }

        public int PersonCount
        {
            get { return _peopleCount; }
            set 
            { 
                _peopleCount = value;
                OnPropertyChanged("PeopleCount");
            }
        }

        public int Temperature
        {
            get { return _temperature; }
            set 
            { 
                _temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        public int ActivationTemp
        {
            get { return _activationTemp; }
            set 
            { 
                _activationTemp = value;
                OnPropertyChanged("ActivationTemp");
                SendDataToArduino();
            }
        }

        public int FanSpeed
        {
            get { return _fanSpeed; }
            set 
            { 
                _fanSpeed = value;
                OnPropertyChanged("FanSpeed");
                SendDataToArduino();
            }
        }

        public int StartFanSpeed
        { 
            get { return _startFanSpeed; }
            set 
            {
                if (value > 0 && value < 100)
                {
                    _startFanSpeed = value;
                    OnPropertyChanged("StartFanSpeed");
                    SendDataToArduino();
                }
            }
        }

        public async void ConnectArduinoPortAsync()
        {
            IsSearchingPort = true;
            await Task.Delay(5000);

            string? arduinoPort = GetArduinoPort();

            if (arduinoPort is not null)
            {
                _serialPort.PortName = arduinoPort;
                _serialPort.BaudRate = 9600;
                _serialPort.Open();

                IsPortFound = true;
                IsConnected = true;
                IsSearchingPort = false;
                await Task.Delay(2000);
                IsReady = true;
            }
            else
            {
                IsPortFound = false;
                IsConnected = false;
                IsSearchingPort = false;
            }
        }

        private static string? GetArduinoPort()
        {
            ManagementScope connectionScope = new();
            SelectQuery serialQuery = new("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new(connectionScope, serialQuery);
            try
            {
                foreach (ManagementObject item in searcher.Get().Cast<ManagementObject>())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException)
            {
                // Do nothing 
            }

            return null;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string serialData = _serialPort.ReadLine();
            ParseDataFromSerial(serialData);
        }

        private void ParseDataFromSerial(string serialData)
        {
            // Will receive only the temperature, fan speed,
            // and person count data from the arduino.
            //
            // Data format: 6,38,70
            //
            //      6 - person count
            //      38 - temperature
            //      70 - fan speed

            string[] data = serialData.Split(',',3);

            PersonCount = int.Parse(data[0]);
            Temperature = int.Parse(data[1]);
            FanSpeed = int.Parse(data[3]);
        }

        private void SendDataToArduino()
        {
            if (_serialPort.IsOpen)
            {
                if (IsAutoMode)
                {
                    _serialPort.WriteLine($"A{ActivationTemp}{StartFanSpeed}{FanSpeed}");
                }
                else
                {
                    _serialPort.WriteLine($"M{ActivationTemp}{StartFanSpeed}{FanSpeed}");
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
