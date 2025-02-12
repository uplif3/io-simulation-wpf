using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace io_simulation_wpf.Models
{
    public class IOModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _led0;
        public bool Led0
        {
            get => _led0;
            set
            {
                _led0 = value;
                OnPropertyChanged();
            }
        }

        private bool _led1;
        public bool Led1
        {
            get => _led1;
            set
            {
                _led1 = value;
                OnPropertyChanged();
            }
        }

        private bool _led2;
        public bool Led2
        {
            get => _led2;
            set
            {
                _led2 = value;
                OnPropertyChanged();
            }
        }

        private bool _led3;
        public bool Led3
        {
            get => _led3;
            set
            {
                _led3 = value;
                OnPropertyChanged();
            }
        }

        private bool _led4;
        public bool Led4
        {
            get => _led4;
            set
            {
                _led4 = value;
                OnPropertyChanged();
            }
        }

        private bool _led5;
        public bool Led5
        {
            get => _led5;
            set
            {
                _led5 = value;
                OnPropertyChanged();
            }
        }

        private bool _led6;
        public bool Led6
        {
            get => _led6;
            set
            {
                _led6 = value;
                OnPropertyChanged();
            }
        }

        private bool _led7;
        public bool Led7
        {
            get => _led7;
            set
            {
                _led7 = value;
                OnPropertyChanged();
            }
        }

        private bool _switch0;
        public bool Switch0
        {
            get => _switch0;
            set
            {
                _switch0 = value;
                OnPropertyChanged();
            }
        }

        private bool _switch1;
        public bool Switch1
        {
            get => _switch1;
            set
            {
                _switch1 = value;
                OnPropertyChanged();
            }
        }

        private bool _switch2;
        public bool Switch2
        {
            get => _switch2;
            set
            {
                _switch2 = value;
                OnPropertyChanged();
            }
        }

        private bool _switch3;
        public bool Switch3
        {
            get => _switch3;
            set
            {
                _switch3 = value;
                OnPropertyChanged();
            }
        }

        private bool _switch4;
        public bool Switch4
        {
            get => _switch4;
            set
            {
                _switch4 = value;
                OnPropertyChanged();
            }
        }

        private bool _switch5;
        public bool Switch5
        {
            get => _switch5;
            set
            {
                _switch5 = value;
                OnPropertyChanged();
            }
        }

        private bool _switch6;
        public bool Switch6
        {
            get => _switch6;
            set
            {
                _switch6 = value;
                OnPropertyChanged();
            }
        }

        private bool _switch7;
        public bool Switch7
        {
            get => _switch7;
            set
            {
                _switch7 = value;
                OnPropertyChanged();
            }
        }

        private bool _button0;
        public bool Button0
        {
            get => _button0;
            set
            {
                _button0 = value;
                OnPropertyChanged();
            }
        }

        private bool _button1;
        public bool Button1
        {
            get => _button1;
            set
            {
                _button1 = value;
                OnPropertyChanged();
            }
        }

        private bool _button2;
        public bool Button2
        {
            get => _button2;
            set
            {
                _button2 = value;
                OnPropertyChanged();
            }
        }

        private bool _button3;
        public bool Button3
        {
            get => _button3;
            set
            {
                _button3 = value;
                OnPropertyChanged();
            }
        }

        private int _scale0;
        public int Scale0
        {
            get => _scale0;
            set
            {
                _scale0 = value;
                OnPropertyChanged();
            }
        }

        private int _scale1;
        public int Scale1
        {
            get => _scale1;
            set
            {
                _scale1 = value;
                OnPropertyChanged();
            }
        }

        public void SetLedStateFromHex(string hexVal)
        {
            byte val = byte.Parse(hexVal, System.Globalization.NumberStyles.HexNumber);
            Led0 = (val & (1 << 0)) != 0;
            Led1 = (val & (1 << 1)) != 0;
            Led2 = (val & (1 << 2)) != 0;
            Led3 = (val & (1 << 3)) != 0;
            Led4 = (val & (1 << 4)) != 0;
            Led5 = (val & (1 << 5)) != 0;
            Led6 = (val & (1 << 6)) != 0;
            Led7 = (val & (1 << 7)) != 0; 
            // Debug-Ausgabe
            System.Diagnostics.Debug.WriteLine($"SetLedStateFromHex: {hexVal} -> {Convert.ToString(val, 2).PadLeft(8, '0')}");
        }

        public void SetSwitchStateFromHex(string hexVal)
        {
            byte val = byte.Parse(hexVal, System.Globalization.NumberStyles.HexNumber);
            Switch0 = (val & (1 << 0)) != 0;
            Switch1 = (val & (1 << 1)) != 0;
            Switch2 = (val & (1 << 2)) != 0;
            Switch3 = (val & (1 << 3)) != 0;
            Switch4 = (val & (1 << 4)) != 0;
            Switch5 = (val & (1 << 5)) != 0;
            Switch6 = (val & (1 << 6)) != 0;
            Switch7 = (val & (1 << 7)) != 0;
        }

        public void SetButtonStateFromHex(string hexVal)
        {
            byte val = byte.Parse(hexVal, System.Globalization.NumberStyles.HexNumber);
            Button0 = (val & (1 << 0)) != 0;
            Button1 = (val & (1 << 1)) != 0;
            Button2 = (val & (1 << 2)) != 0;
            Button3 = (val & (1 << 3)) != 0;
        }

        public string GetButtonStateInHex()
        {
            byte val = 0;
            if (Button0) val |= (1 << 0);
            if (Button1) val |= (1 << 1);
            if (Button2) val |= (1 << 2);
            if (Button3) val |= (1 << 3);
            var res = val.ToString("x2");
            System.Diagnostics.Debug.WriteLine(res);
            return res;
        }

        public string GetSwitchStateInHex()
        {
            byte val = 0;
            if (Switch0) val |= (1 << 0);
            if (Switch1) val |= (1 << 1);
            if (Switch2) val |= (1 << 2);
            if (Switch3) val |= (1 << 3);
            if (Switch4) val |= (1 << 4);
            if (Switch5) val |= (1 << 5);
            if (Switch6) val |= (1 << 6);
            if (Switch7) val |= (1 << 7);
            return val.ToString("X2");
        }

    }
}
