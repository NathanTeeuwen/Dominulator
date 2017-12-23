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
using Generic = System.Collections.Generic;
using System.ComponentModel;

namespace Dominulator
{
    public sealed partial class MainPage : Page
    {
        public static Random random = new System.Random();
        public static AppDataContext appDataContextStatic;

        public AppDataContext appDataContext;

        public MainPage()
        {
            MainPage.appDataContextStatic = new AppDataContext(this);
            this.appDataContext = MainPage.appDataContextStatic;

            this.InitializeComponent();

            this.DataContext = this.appDataContext;

            this.CurrentCards.CurrentCardsChanged += this.AllCards.UpdateAllCardsListSelection;
            this.appDataContext.CurrentDeck.PropertyChanged += UpdateCommonCardsFromKingdom;
            this.appDataContext.StrategyReport.PropertyChanged += StrategyReport_PropertyChanged;
            this.appDataContext.UseShelters.PropertyChanged += UpdateCommonCardsFromKingdom;
            this.appDataContext.UseColonyPlatinum.PropertyChanged += UpdateCommonCardsFromKingdom;

            this.appDataContext.CurrentDeck.SortByCost();
            this.appDataContext.AllCards.SortByName();

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            appDataContext.ShelterCards.PopulateShelters();
            appDataContext.ColonyPlatinumCards.PopulateColonyPlatinum();

            appDataContext.AllCards.Populate().ContinueWith(
                delegate (System.Threading.Tasks.Task task)
                {
                    this.CurrentCards.Randomize10Cards();
                }, uiScheduler);
        }

        public void UpdateAllCardsListSelection()
        {
           this.AllCards.UpdateAllCardsListSelection();
        }

        internal void UpdateCommonCardsFromKingdom(object sender, PropertyChangedEventArgs e)
        {
            Dominion.GameConfig gameConfig = this.appDataContext.GetGameConfig();
            this.appDataContext.CommonCards.PopulateCommon(gameConfig);
        }

        internal static bool GenerateRandom(
            int targetCount,
            ref DominionCard baneCard,
            IList<DominionCard> resultList,
            IList<DominionCard> sourceList,
            IEnumerable<DominionCard> allCards,
            IEnumerable<DominionCard> itemsToReplace)
        {
            bool isReplacingItems = itemsToReplace != null && itemsToReplace.Any() && sourceList.Count <= targetCount;
            bool isReducingItems = itemsToReplace != null && itemsToReplace.Any() && sourceList.Count > targetCount;
            var cardPicker = new Dominion.UniqueCardPicker(allCards.Select(c => c.dominionCard), MainPage.random);

            bool isCleanRoll = false;

            if (isReplacingItems)
            {
                cardPicker.ExcludeCards(itemsToReplace.Select(c => c.dominionCard));
            }

            baneCard = DominionCard.Create(cardPicker.GetCard(c => c.DefaultCoinCost == 2 || c.DefaultCoinCost == 3));

            if (isReplacingItems)
            {
                foreach (DominionCard cardToReplace in itemsToReplace)
                {
                    for (int i = 0; i < resultList.Count; ++i)
                    {
                        if (resultList[i] == cardToReplace)
                        {
                            var nextCard = cardPicker.GetCard(c => true);
                            if (nextCard == null)
                            {
                                resultList.Remove(cardToReplace);
                                i--;  // do this index again
                            }
                            else
                            {
                                resultList[i] = DominionCard.Create(nextCard);
                            }
                        }
                    }
                }
            }
            else if (sourceList.Count < targetCount && sourceList.Count > 0)
            {
                var listRemoved = new List<DominionCard>();
                foreach (var card in resultList)
                {
                    if (!sourceList.Contains(card))
                        listRemoved.Add(card);
                }

                foreach (var card in listRemoved)
                {
                    resultList.Remove(card);
                }
            }
            else if (isReducingItems)
            {
                foreach (var card in itemsToReplace)
                {
                    resultList.Remove(card);
                }
            }
            else
            {
                resultList.Clear();
                isCleanRoll = true;
            }

            while (resultList.Count < targetCount)
            {
                Dominion.Card currentCard = cardPicker.GetCard(c => true);
                if (currentCard == null)
                    break;
                resultList.Add(DominionCard.Create(currentCard));
            }

            return isCleanRoll;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentPageConfig.Value = PageConfig.Settings;
            this.appDataContext.SettingsButtonVisibility.Value = SettingsButtonVisibility.Back;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentPageConfig.Value = PageConfig.Settings;
            this.appDataContext.SettingsButtonVisibility.Value = SettingsButtonVisibility.Back;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentPageConfig.Value = PageConfig.CurrentDeck;
            this.appDataContext.SettingsButtonVisibility.Value = SettingsButtonVisibility.Settings;
        }

        private void AllCardsButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentPageConfig.Value =
                this.appDataContext.CurrentPageConfig.Value == PageConfig.CurrentDeck ? PageConfig.AllCards : PageConfig.CurrentDeck;
        }

        private void SaveReportButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = this.appDataContext.Player1Name.Value + " vs " + this.appDataContext.Player2Name.Value;
            picker.DefaultFileExtension = ".html";
            picker.FileTypeChoices.Add("Web Page", new string[] { ".html" });
            string htmlString = this.appDataContext.StrategyReport.Value;
            picker.PickSaveFileAsync().AsTask().ContinueWith((storageFile) =>
            {
                Windows.Storage.StorageFile result = storageFile.Result;
                Windows.Storage.FileIO.WriteTextAsync(result, htmlString);
            });
        }

        void StrategyReport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ResultsWebView.NavigateToString(this.appDataContext.StrategyReport.Value);
        }

        private void GoogleButtonClick(object sender, RoutedEventArgs e)
        {
            var uriBuilder = new System.Text.StringBuilder();
            uriBuilder.Append("https://www.google.com/?gws_rd=ssl#safe=off&q=dominion");

            foreach (Dominion.Card card in this.CurrentCards.GetSelectedCardsAndClear(fShouldClear: false))
            {
                uriBuilder.Append("+");
                uriBuilder.Append(card.name.Replace(" ", "%20"));
            }

            string uriToLaunch = uriBuilder.ToString();
            var uri = new Uri(uriToLaunch);
            Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
