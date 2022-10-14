using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Automatic_Fan_Controller
{
    public  class Controller : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isAutoMode = true;
        private bool _isManualMode = false;
        private bool _isSearchingPort = true;
        private bool _isPortFound = true;
        private bool _isConnected = false;
        private int _peopleCount = 0;
        private int _temperature = 0;
        private int _activationTemp = 25;
        private int _fanSpeed = 0;
        private int _startFanSpeed = 50;

        public bool IsAutoMode
        {
            get { return _isAutoMode; }
            set 
            { 
                _isAutoMode = value;
                OnPropertyChanged("IsAutoMode");
            }
        }

        public bool IsManualMode
        {
            get { return _isManualMode; }
            set
            {
                _isManualMode = value;
                OnPropertyChanged("IsManualMode");
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

        public int PeopleCount
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
            }
        }

        public int FanSpeed
        {
            get { return _fanSpeed; }
            set 
            { 
                _fanSpeed = value;
                OnPropertyChanged("FanSpeed");
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
                }
            }
        }

        public string? GetArduinoPort()
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
                /* Do Nothing */
            }

            return null;
        }

        public void ParseDataFromSerial(string serialData)
        {
            /*  Serial Data Format: A0536802550
             * 
             *  1st Character: Mode = "A" or "M"
             *  
             *  By 2 digits in order:
             *      People Count = 05
             *      Temperature = 36
             *      Fan Speed = 80
             *      Activation Temp = 25
             *      Start Fan Speed = 50
             */

            IsAutoMode = serialData[..1] == "A";
            PeopleCount = int.Parse(serialData.Substring(1, 2));
            Temperature = int.Parse(serialData.Substring(3, 2));
            FanSpeed = int.Parse(serialData.Substring(5, 2));

            //ActivationTemp = int.Parse(serialData.Substring(7, 2));
            //StartFanSpeed = int.Parse(serialData.Substring(9, 2));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
