using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dominulator
{
    public sealed partial class CompactyStrategyView : UserControl
    {
        public CompactyStrategyView()
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

        private void StrategyButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.ViewStrategiesFullScreen();
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
    }
}

