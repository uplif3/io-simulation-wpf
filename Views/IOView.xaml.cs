using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace io_simulation_wpf.Views
{
    public partial class IOView : UserControl
    {
        public IOView()
        {
            InitializeComponent();
            this.Focusable = true;
            this.Loaded += (s, e) => Keyboard.Focus(this);

            // KeyUp über PreviewKeyUp abfangen
            this.PreviewKeyUp += IOView_PreviewKeyUp;
        }

        private void IOView_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Leite KeyUp-Ereignisse an das ViewModel weiter
            if (DataContext is ViewModels.IOViewModel vm &&
                vm.HandleKeyUp.CanExecute(e.Key.ToString()))
            {
                vm.HandleKeyUp.Execute(e.Key.ToString());
                e.Handled = true;
            }
        }
    }
}
