using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using io_simulation_wpf.Models;
using io_simulation_wpf.Services;

namespace io_simulation_wpf.ViewModels
{
    public class IOViewModel
    {
        public IOModel Model { get; }
        private SerialPortService port;

        private readonly HashSet<Key> _pressedKeys = new HashSet<Key>();

        public ICommand HandleKeyDown { get; }
        public ICommand HandleKeyUp { get; }

        public IOViewModel(SerialPortService port)
        {
            this.Model = new IOModel();
            this.port = port;

            this.Model.PropertyChanged += Model_PropertyChanged;

            HandleKeyDown = new RelayCommand(param => ProcessKeyDown((Key)Enum.Parse(typeof(Key), param.ToString())));
            HandleKeyUp = new RelayCommand(param => ProcessKeyUp((Key)Enum.Parse(typeof(Key), param.ToString())));

        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e is null || e.PropertyName is null) throw new NullReferenceException("Unknown Prop changed?");
            if (e.PropertyName.StartsWith("Switch"))
            {
                string hexVal = Model.GetSwitchStateInHex();
                port.WriteLine($"d01{hexVal}");
            }
            else if (e.PropertyName.StartsWith("Button"))
            {
                string hexVal = Model.GetButtonStateInHex();
                System.Diagnostics.Debug.WriteLine($"d02{hexVal}");
                port.WriteLine($"d02{hexVal}");
            }
            else if (e.PropertyName.Equals("Scale0"))
            {
                string hexVal = Model.GetScale0InHex();
                port.WriteLine($"d0a{hexVal}");
            }
            else if (e.PropertyName.Equals("Scale1"))
            {
                string hexVal = Model.GetScale1InHex();
                port.WriteLine($"d0b{hexVal}");
            }
        }

        public void ProcessLedPacket(string data) => Model.SetLedStateFromHex(data);

        private void HandleButtonClick(int index)
        {
            ToggleButton(index);
            SendButtonStatus();
        }

        private void ProcessKeyDown(Key key)
        {
            if (_pressedKeys.Contains(key))
                return; // Verhindert wiederholte Eingaben, solange die Taste noch gedrückt ist

            int index = KeyToButtonIndex(key);
            if (index >= 0)
            {
                _pressedKeys.Add(key);
                SetButtonState(index, true); // Button "drücken" (an)
                SendButtonStatus();
            }
        }

        private void ProcessKeyUp(Key key)
        {
            int index = KeyToButtonIndex(key);
            if (index >= 0)
            {
                _pressedKeys.Remove(key);
                SetButtonState(index, false); // Button loslassen (aus)
                SendButtonStatus();
            }
        }


        private void ToggleButton(int index)
        {
            switch (index)
            {
                case 0: Model.Button0 = !Model.Button0; break;
                case 1: Model.Button1 = !Model.Button1; break;
                case 2: Model.Button2 = !Model.Button2; break;
                case 3: Model.Button3 = !Model.Button3; break;
            }
        }

        private int KeyToButtonIndex(Key key)
        {
            return key switch
            {
                Key.D1 => 3,
                Key.D2 => 2,
                Key.D3 => 1,
                Key.D4 => 0,
                _ => -1
            };
        }

        private void SendButtonStatus()
        {
            string hexValue = Model.GetButtonStateInHex();
            port.WriteLine($"d02{hexValue}");
            System.Diagnostics.Debug.WriteLine($"Gesendet: d02{hexValue}");
        }

        private void SetButtonState(int index, bool state)
        {
            switch (index)
            {
                case 0: Model.Button0 = state; break;
                case 1: Model.Button1 = state; break;
                case 2: Model.Button2 = state; break;
                case 3: Model.Button3 = state; break;
            }
        }


    }
}
