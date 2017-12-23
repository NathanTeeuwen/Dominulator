using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dominulator
{
    public sealed partial class SimulationResults : UserControl
    {
        public SimulationResults()
        {
            this.InitializeComponent();
        }

        public AppDataContext AppDataContext
        {
            get
            {
                return (AppDataContext)(base.DataContext);
            }

            set
            {
                this.DataContext = value;
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            this.AppDataContext.ShowReport();
        }

    }
}
