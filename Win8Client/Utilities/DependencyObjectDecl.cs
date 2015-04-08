using Windows.UI.Xaml;
using System.ComponentModel;    

namespace Win8Client
{
    delegate void PropertyChangedEvent(object owner);

    class DependencyObjectDecl<T>
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

    class DependencyObjectDecl<T, T2>
        : DependencyObject, INotifyPropertyChanged
        where T2 : DependencyPolicy<T>, new()
    {
        private readonly object parent;
        public event PropertyChangedEventHandler PropertyChanged;

        public DependencyObjectDecl(object parent)
        {
            this.parent = parent;
        }

        private static T2 Policy = new T2();        

        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register(
            "Value",
            typeof(T),
            typeof(AppDataContext),
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

    interface DependencyPolicy<T>
    {
        T DefaultValue
        {
            get;
        }
    }

    class DefaultTrue
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

    class DefaultFalse
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

    class DefaultEmptyString
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

    class DefaultDoubleZero
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

    class DefaultEmptyStrategyDescription
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
