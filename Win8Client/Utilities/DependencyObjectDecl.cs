using Windows.UI.Xaml;
using System.ComponentModel;    

namespace Win8Client
{
    delegate void PropertyChangedEvent(object owner);

    public class DependencyObjectDecl<T>
        : DependencyObject, INotifyPropertyChanged        
    {
        private readonly object parent;
        public event PropertyChangedEventHandler PropertyChanged;

        public DependencyObjectDecl(object parent)
        {
            this.parent = parent;
        }        

        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register(
            "Value",
            typeof(T),
            typeof(AppDataContext),
            new PropertyMetadata(default(T), PropertyChangedCallback)
        );

        public T Value
        {
            get
            {
                return (T)this.GetValue(DependencyProperty);
            }

            set
            {
                this.SetValue(DependencyProperty, value);
            }
        }

        static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DependencyObjectDecl<T> typed = (DependencyObjectDecl<T>)d;
            if (typed.PropertyChanged != null)
            {
                typed.PropertyChanged(typed.parent, new PropertyChangedEventArgs("Value"));
            }
        }
    }
 
    public class DependencyObjectDeclWithSettings<T, T2>
        : DependencyObjectDecl<T, T2>
        where T2 : DependencyPolicy<T>, new()
    {
        public readonly string settingsName;

        public DependencyObjectDeclWithSettings(object parent, string settingsName)
            : base(parent)
        {
            this.settingsName = settingsName;

            object currentValue = Windows.Storage.ApplicationData.Current.LocalSettings.Values[this.settingsName]; 
            if (currentValue == null)
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[this.settingsName] = Policy.DefaultValue;
            }
            else
            {
                this.Value = (T)currentValue;
            }

            this.PropertyChanged += UpdateLocalSettings_PropertyChanged;
        }

        void UpdateLocalSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
 	        Windows.Storage.ApplicationData.Current.LocalSettings.Values[this.settingsName] = this.Value;
        }
    }

    public class DependencyObjectDecl<T, T2>
        : DependencyObject, INotifyPropertyChanged
        where T2 : DependencyPolicy<T>, new()
    {
        private readonly object parent;
        public event PropertyChangedEventHandler PropertyChanged;

        public DependencyObjectDecl(object parent)
        {
            this.parent = parent;
        }

        protected static T2 Policy = new T2();        

        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register(
            "Value",
            typeof(T),
            typeof(DependencyObjectDecl<T, T2>),
            new PropertyMetadata(Policy.DefaultValue, PropertyChangedCallback)
        );

        public T Value
        {
            get
            {
                return (T)this.GetValue(DependencyProperty);
            }

            set
            {
                this.SetValue(DependencyProperty, value);
            }
        }                

        static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DependencyObjectDecl<T, T2> typed = (DependencyObjectDecl<T, T2>)d;
            if (typed.PropertyChanged != null)
            {
                typed.PropertyChanged(typed.parent, new PropertyChangedEventArgs("Value"));
            }
        }
    }

    public interface DependencyPolicy<T>
    {
        T DefaultValue
        {
            get;
        }
    }

    public class DefaultTrue
        : DependencyPolicy<bool>
    {
        public bool DefaultValue
        {
            get
            {
                return true;
            }
        }
    }

    public class DefaultFalse
        : DependencyPolicy<bool>
    {
        public bool DefaultValue
        {
            get
            {
                return false;
            }
        }
    }

    public class DefaultSplit4
        : DependencyPolicy<Dominion.StartingCardSplit>
    {
        public Dominion.StartingCardSplit DefaultValue
        {
            get
            {
                return Dominion.StartingCardSplit.Split43;
            }
        }
    }

    public class DefaultEmptyString
        : DependencyPolicy<string>
    {
        public string DefaultValue
        {
            get
            {
                return "";
            }
        }
    }

    public class DefaultDoubleZero
        : DependencyPolicy<double>
    {
        public double DefaultValue
        {
            get
            {
                return 0;
            }
        }
    }

    public class DefaultEmptyStrategyDescription
        : DependencyPolicy<StrategyDescription>
    {
        public StrategyDescription DefaultValue
        {
            get
            {
                return new StrategyDescription();
            }
        }
    }   
}
