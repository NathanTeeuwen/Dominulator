using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Win8Client
{
    public sealed partial class AllCardsPicker : UserControl
    {
        public AllCardsPicker()
        {
            this.InitializeComponent();
        }

        private AppDataContext appDataContext
        {
            get
            {
                return this.AppDataContext;
            }
        }

        internal AppDataContext AppDataContext
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

        public IList<object> SelectedItems
        {
            get
            {
                return this.AllCardsListView.SelectedItems;
            }
        }

        private void AllCardsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.AppDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates)
                return;

            this.AppDataContext.CurrentDeck.UpdateOriginalCards(e.AddedItems.Select(item => (DominionCard)item), e.RemovedItems.Select(item => (DominionCard)item));
        }

        private void SortAllByName(object sender, RoutedEventArgs e)
        {            
            this.appDataContext.AllCards.SortByName();         
        }

        private void SortAllByCost(object sender, RoutedEventArgs e)
        {            
            this.appDataContext.AllCards.SortByCost();            
        }

        private void SortAllByExpansion(object sender, RoutedEventArgs e)
        {            
            this.appDataContext.AllCards.SortByExpansion();         
        }

        
        
    }
}
