using System.Collections.ObjectModel;

namespace io_simulation_wpf.ViewModels
{
    public class LogViewModel
    {
        public ObservableCollection<string> LogMessages { get; } = new ObservableCollection<string>();

        public void AddLogMessage(string msg)
        {
            LogMessages.Add(msg);
        }
    }
}
