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

        public IOViewModel(SerialPortService port)
        {
            this.Model = new IOModel();
            this.port = port;

            this.Model.PropertyChanged += Model_PropertyChanged;

        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e is null || e.PropertyName is null) throw new NullReferenceException("Unknown Prop changed?");
            // Prüfe, ob ein Schalter geändert wurde:
            if (e.PropertyName.StartsWith("Switch"))
            {
                string hexVal = Model.GetSwitchStateInHex();
                port.WriteLine($"d01{hexVal}");
            }
            // Falls du auch Button-Änderungen verarbeiten möchtest:
            else if (e.PropertyName.StartsWith("Button"))
            {
                string hexVal = Model.GetButtonStateInHex();
                System.Diagnostics.Debug.WriteLine($"d02{hexVal}");
                port.WriteLine($"d02{hexVal}");
            }
        }

        public void ProcessLedPacket(string data) => Model.SetLedStateFromHex(data);

    }
}
