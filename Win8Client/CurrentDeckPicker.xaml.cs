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
    public sealed partial class CurrentDeckPicker : UserControl
    {
        public event Action CurrentCardsChanged;

        public CurrentDeckPicker()
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
                return this.CurrentCardsListView.SelectedItems;
            }
        }

        private void CurrentCardsListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            Win8Client.Strategies.PrepareDragAndDrop(e);
            this.CurrentCardsListView.SelectedItems.Clear();
        }     

        public void Randomize10Cards()
        {
            var selectedItems = this.CurrentCardsListView.SelectedItems.Select(item => (DominionCard)item).ToArray<DominionCard>();
            this.appDataContext.CurrentDeck.Generate10Random(this.appDataContext.AllCards.Cards, itemsToReplace: selectedItems);
            if (this.CurrentCardsChanged != null)
                this.CurrentCardsChanged();            
        }

        private void RandomizeButtonClick(object sender, RoutedEventArgs e)
        {
            Randomize10Cards();
        }   

        private void SortCurrentByName(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentDeck.SortByName();
            this.appDataContext.CurrentDeck.UpdateUIFromUIThread();
        }

        private void SortCurrentByCost(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentDeck.SortByCost();
            this.appDataContext.CurrentDeck.UpdateUIFromUIThread();
        }

        private void SortCurrentByExpansion(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentDeck.SortByExpansion();
            this.appDataContext.CurrentDeck.UpdateUIFromUIThread();
        }        
    }
}
