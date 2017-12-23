using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dominulator
{
    public sealed partial class SideBySideStrategyView : UserControl
    {
        public SideBySideStrategyView()
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

        public static void PrepareDragAndDrop(DragItemsStartingEventArgs e)
        {
            var cardListAsString = string.Join(",", e.Items.Select(card => ((DominionCard)card).dominionCard.name));

            e.Data.SetData("text", cardListAsString);
        }

        private void ClearStrategyButtonClick(object sender, RoutedEventArgs e)
        {
            this.appDataContext.currentStrategy.Value.Clear();
            this.appDataContext.UpdateSimulationStep();
        }

        private void PlayerRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (this.appDataContext.IsPlayer1StrategyChecked.Value)
            {
                this.appDataContext.CurrentStrategy.Value = this.appDataContext.player1Strategy;
            }
        }

        private void PlayerRadioButtonChecked2(object sender, RoutedEventArgs e)
        {
            if (this.appDataContext.IsPlayer2StrategyChecked.Value)
            {
                this.appDataContext.CurrentStrategy.Value = this.appDataContext.player2Strategy;
            }
        }

        private void StrategyButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.ViewStrategiesFullScreen();
        }
    }
}

