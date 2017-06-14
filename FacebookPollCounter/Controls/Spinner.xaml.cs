using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FacebookPollCounter.Controls
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class Spinner : UserControl
    {
        public Spinner()
        {
            InitializeComponent();
        }

        public string LoadingText
        {
            get { return (string)GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadingText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register(nameof(LoadingText), typeof(string), typeof(Spinner), new PropertyMetadata("Loading..."));
    }
}
