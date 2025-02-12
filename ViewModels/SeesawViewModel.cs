using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using io_simulation_wpf.Models;

namespace io_simulation_wpf.ViewModels
{
    class SeesawViewModel
    {
        public DebugModel Model { get; }

        public SeesawViewModel()
        {
            Model = new DebugModel();
        }

        public void AddDebugMessage(string msg)
        {
            Model.Messages.Add(msg);
        }
    }
}
