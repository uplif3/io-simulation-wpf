using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace io_simulation_wpf.Models
{
    public class LogModel
    {
        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();
    }
}
