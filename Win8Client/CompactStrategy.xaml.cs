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
