using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dominulator
{
    public sealed partial class CompactStrategy : UserControl
    {
        public CompactStrategy()
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
            typeof(CompactStrategy), new PropertyMetadata(null));

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
    }
}
