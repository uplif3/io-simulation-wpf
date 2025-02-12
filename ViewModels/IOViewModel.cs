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
        public IOModel Model { get; }
        private SerialPortService port;

        public IOViewModel(SerialPortService port)
        {
            this.Model = new IOModel();
            this.port = port;

            ToggleSwitchCommand = new RelayCommand(idx => ToggleSwitch(int.Parse(idx.ToString())));
            ToggleButtonCommand = new RelayCommand(idx => ToggleButton(int.Parse(idx.ToString())));

        }

        public ICommand ToggleSwitchCommand { get; }
        private void ToggleSwitch(int idx)
        {
            switch(idx)
            {
                case 0: Model.Switch0 = !Model.Switch0; break;
                case 1: Model.Switch1 = !Model.Switch1; break;
                case 2: Model.Switch2 = !Model.Switch2; break;
                case 3: Model.Switch3 = !Model.Switch3; break;
                case 4: Model.Switch4 = !Model.Switch4; break;
                case 5: Model.Switch5 = !Model.Switch5; break;
                case 6: Model.Switch6 = !Model.Switch6; break;
                case 7: Model.Switch7 = !Model.Switch7; break;
            }

            string hexVal = Model.GetSwitchStateInHex();
            port.WriteLine($"d1{hexVal}");
        }

        public ICommand ToggleButtonCommand { get; }
        private void ToggleButton(int idx)
        {
            switch (idx)
            {
                case 0: Model.Button0 = !Model.Button0; break;
                case 1: Model.Button1 = !Model.Button1; break;
                case 2: Model.Button2 = !Model.Button2; break;
                case 3: Model.Button3 = !Model.Button3; break;
            }
            string hexVal = Model.GetButtonStateInHex();
            port.WriteLine($"d3{hexVal}");
        }

        public void ProcessData(string cmd, string data)
        {
            Console.WriteLine($"recieved: {data}");

            if (data.StartsWith("d1"))
            {
                Model.SetSwitchStateFromHex(data.Substring(2));
            }
            else if (data.StartsWith("d3"))
            {
                Model.SetButtonStateFromHex(data.Substring(2));
            }
            else if (cmd is "d0")
            {
                Model.SetLedStateFromHex(data);
            }
        }
    }
}
