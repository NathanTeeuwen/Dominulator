using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dominulator
{
    public sealed partial class Strategy : UserControl
    {
        public Strategy()
        {
            this.InitializeComponent();
        }

        internal StrategyDescription StrategyDescription
        {
            get
            {
                return (StrategyDescription)(base.DataContext);
            }

            set
            {
                this.DataContext = value;
            }
        }

        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register(
            "AppDataContext",
            typeof(AppDataContext),
            typeof(Strategy), new PropertyMetadata(null));

        public AppDataContext AppDataContext
        {
            get
            {
                return (AppDataContext)this.GetValue(DependencyProperty);
            }

            set
            {
                this.SetValue(DependencyProperty, value);
            }
        }

        private async void CurrentCardsListView_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetView().AvailableFormats.Contains("text"))
                return;

            string cardNames = (string)await e.Data.GetView().GetTextAsync("text");
            Dominion.Card[] cards = cardNames.Split(',').Select(cardName => DominionCard.Create(cardName).dominionCard).ToArray();
            Dominion.Strategy.Description.StrategyDescription strategy = this.StrategyDescription.ConvertToDominionStrategy();
            strategy = strategy.AddCardsToPurchaseOrder(cards);
            this.StrategyDescription.PopulateFrom(strategy);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as CardAcceptanceButton;
            CardAcceptanceDescription descr = button.CardAcceptanceDescription;
            StrategyDescription strategy = button.StrategyDescription;
            strategy.PurchaseOrderDescriptions.Remove(descr);
        }
    }

    public class CardAcceptanceButton
        : Button
    {
        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register(
            "StrategyDescription",
            typeof(StrategyDescription),
            typeof(CardAcceptanceButton), new PropertyMetadata(null));

        public StrategyDescription StrategyDescription
        {
            get
            {
                return (StrategyDescription)this.GetValue(DependencyProperty);
            }

            set
            {
                this.SetValue(DependencyProperty, value);
            }
        }

        public CardAcceptanceDescription CardAcceptanceDescription
        {
            get
            {
                return (CardAcceptanceDescription)this.DataContext;
            }
        }
    }
}
