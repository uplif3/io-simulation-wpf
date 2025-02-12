using System.Collections.ObjectModel;
using io_simulation_wpf.Models;

namespace io_simulation_wpf.ViewModels
{
    public class LogViewModel
    {
        public LogModel Model { get; }

        public LogViewModel()
        {
            this.Model = new LogModel();
        }

        public void AddLogMessage(string msg)
        {
            Model.Messages.Add(msg);
        }
    }
}
