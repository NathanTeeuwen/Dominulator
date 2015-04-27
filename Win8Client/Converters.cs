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

namespace Win8Client
{
    public class ComparisonToIntegerConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var comparison = (Dominion.Strategy.Description.Comparison)value;
            return (int)comparison;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int intValue = (int)value;
            return (Dominion.Strategy.Description.Comparison)intValue;
        }
    }

    public class CountSourceToIntegerConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var comparison = (Dominion.Strategy.Description.CountSource)value;
            return (int)comparison;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int intValue = (int)value;
            return (Dominion.Strategy.Description.CountSource)intValue;
        }
    }

    public class BoolToVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = (bool)value;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToInVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = (bool)value;
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class CurrentDeckVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (PageConfig)value;
            return cardVisibility == PageConfig.CurrentDeck ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    
    public class AllCardVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (PageConfig)value;
            return cardVisibility == PageConfig.AllCards ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StrategyVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (PageConfig)value;
            return cardVisibility == PageConfig.Strategy ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingsVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (PageConfig)value;
            return cardVisibility == PageConfig.Settings ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ReportVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (PageConfig)value;
            return cardVisibility == PageConfig.Report ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingsButtonVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (SettingsButtonVisibility)value;
            return cardVisibility == SettingsButtonVisibility.Settings ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BackButtonVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (PageConfig)value;
            return cardVisibility == PageConfig.CurrentDeck ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }       

    public class EnumToIndexConverter<T, T2> : IValueConverter
    {
        KeyValuePair<T, T2>[] conversions;

        protected static KeyValuePair<T, T2> Pair(T t, T2 str) { return new KeyValuePair<T, T2>(t, str); }

        public EnumToIndexConverter(params KeyValuePair<T, T2>[] conversions)
        {
            this.conversions = conversions;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var typedValue = (T)value;

            foreach (var conversion in this.conversions)
            {
                if (conversion.Key.Equals(typedValue))
                    return conversion.Value;
            }

            throw new System.Exception("Key not found: " + typedValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var typedValue = (T2)value;

            foreach (var conversion in this.conversions)
            {
                if (conversion.Value.Equals(typedValue))
                    return conversion.Key;
            }

            throw new System.Exception("Conversion not found: " + typedValue);
        }
    }

    public class OpenningSplitConverter
        : EnumToIndexConverter<Dominion.StartingCardSplit, int>
    {
        public OpenningSplitConverter()
            : base(
                Pair(Dominion.StartingCardSplit.Random, 0),
                Pair(Dominion.StartingCardSplit.Split25, 1),
                Pair(Dominion.StartingCardSplit.Split34, 2),
                Pair(Dominion.StartingCardSplit.Split43, 3),
                Pair(Dominion.StartingCardSplit.Split52, 4)
            )
        {

        }
    }      
}