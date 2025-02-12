using System;
using System.Collections.Generic;
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
        private IOModel model = new IOModel();
        private SerialPortService port;

        public IOViewModel(SerialPortService port)
        {
            this.port = port;

            ToggleSwitchCommand = new RelayCommand(idx => ToggleSwitch(int.Parse(idx.ToString())));
            ToggleButtonCommand = new RelayCommand(idx => ToggleButton(int.Parse(idx.ToString())));

        }

        public ICommand ToggleSwitchCommand { get; }
        private void ToggleSwitch(int idx)
        {
            switch(idx)
            {
                case 0: model.Switch0 = !model.Switch0; break;
                case 1: model.Switch1 = !model.Switch1; break;
                case 2: model.Switch2 = !model.Switch2; break;
                case 3: model.Switch3 = !model.Switch3; break;
                case 4: model.Switch4 = !model.Switch4; break;
                case 5: model.Switch5 = !model.Switch5; break;
                case 6: model.Switch6 = !model.Switch6; break;
                case 7: model.Switch7 = !model.Switch7; break;
            }

            string hexVal = model.GetSwitchStateInHex();
            port.WriteLine($"d1{hexVal}");
        }

        public ICommand ToggleButtonCommand { get; }
        private void ToggleButton(int idx)
        {
            switch (idx)
            {
                case 0: model.Button0 = !model.Button0; break;
                case 1: model.Button1 = !model.Button1; break;
                case 2: model.Button2 = !model.Button2; break;
                case 3: model.Button3 = !model.Button3; break;
            }
            string hexVal = model.GetButtonStateInHex();
            port.WriteLine($"d3{hexVal}");
        }

        public void ParseData(string data)
        {
            Console.WriteLine($"recieved: {data}");

            if (data.StartsWith("d1"))
            {
                model.SetSwitchStateFromHex(data.Substring(2));
            }
            else if (data.StartsWith("d3"))
            {
                model.SetButtonStateFromHex(data.Substring(2));
            }
            else if (data.StartsWith("d0"))
            {
                model.SetLedStateFromHex(data.Substring(2));
            }
        }
    }
}
