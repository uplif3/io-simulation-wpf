using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using io_simulation_wpf.Models;

namespace io_simulation_wpf.ViewModels
{
    public class DebugViewModel
    {
        public DebugModel Model { get; }

        public DebugViewModel()
        {
            Model = new DebugModel();
        }

        public void AddDebugMessage(string msg)
        {
            Model.Messages.Add(msg);
        }
    }
}
