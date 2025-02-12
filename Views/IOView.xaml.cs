using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace io_simulation_wpf.Views
{
    /// <summary>
    /// Interaktionslogik für IOPanelView.xaml
    /// </summary>
    public partial class IOView : UserControl
    {
        public IOView()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                if (this.DataContext == null)
                    System.Diagnostics.Debug.WriteLine("IOView DataContext is NULL!");
                else
                    System.Diagnostics.Debug.WriteLine($"IOView DataContext: {this.DataContext.GetType().Name}");
            };
        }
    }
}
