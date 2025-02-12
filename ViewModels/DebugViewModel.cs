using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using io_simulation_wpf.Models;

namespace io_simulation_wpf.ViewModels
{
    public class DebugViewModel
    {
        private DebugModel _model;

        public DebugViewModel()
        {
            _model = new DebugModel();
        }

        public ObservableCollection<string> Messages => _model.Messages;

        public void AddDebugMessage(string msg)
        {
            _model.Messages.Add(msg);
        }
    }
}
