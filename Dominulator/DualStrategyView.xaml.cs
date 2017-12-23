using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dominulator
{
    public sealed partial class DualStrategyView : UserControl
    {
        public DualStrategyView()
        {
            this.InitializeComponent();
        }

        internal AppDataContext appDataContext
        {
            get
            {
                return this.AppDataContext;
            }
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

        private void ClearStrategy1ButtonClick(object sender, RoutedEventArgs e)
        {
            this.appDataContext.player1Strategy.Clear();
            this.appDataContext.UpdateSimulationStep();
        }

        private void ClearStrategy2ButtonClick(object sender, RoutedEventArgs e)
        {
            this.appDataContext.player2Strategy.Clear();
            this.appDataContext.UpdateSimulationStep();
        }

        private void SimulateGameButtonClick(object sender, RoutedEventArgs e)
        {
            this.appDataContext.SimulateGameButtonClick();
        }
    }
}
