using Windows.UI.Xaml;

namespace Win8Client
{
    delegate void PropertyChangedEvent(object owner);

    class DependencyObjectDecl<T, T2>
        : DependencyObject
        where T2 : DependencyPolicy<T>, new()
    {
        private readonly object parent;
        public event PropertyChangedEvent Changed;

        public DependencyObjectDecl(object parent)
        {
            this.parent = parent;
        }

        private static T2 Policy = new T2();        

        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register(
            "Value",
            typeof(T),
            typeof(AppDataContext),
            new PropertyMetadata(Policy.DefaultValue, PropertyChanged)
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

        static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DependencyObjectDecl<T, T2> typed = (DependencyObjectDecl<T, T2>)d;
            if (typed.Changed != null)
            {
                typed.Changed(typed.parent);
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

}
