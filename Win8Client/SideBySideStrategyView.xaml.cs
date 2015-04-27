using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Win8Client
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
            this.appDataContext.currentStrategy.Value.CardAcceptanceDescriptions.Clear();
        }

        private void SimulateGameButtonClick(object sender, RoutedEventArgs e)
        {
            this.appDataContext.SimulateGameButtonClick();
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

    }   
}
