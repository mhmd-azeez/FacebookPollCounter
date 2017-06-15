using FacebookPollCounter.Helpers;
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

namespace FacebookPollCounter.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        string _redirectUrl = "https://www.facebook.com/connect/login_success.html";
        public Login()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string appId = "108678209685280";

            browser.Source = new Uri($"https://www.facebook.com/v2.9/dialog/oauth?client_id={appId}&redirect_uri={ _redirectUrl }&response_type=token&display=popup");
        }

        private void browser_Navigated(object sender, NavigationEventArgs e)
        {
            var link = browser.Source.ToString();
            if (link.StartsWith(_redirectUrl))
            {
                link = link.Replace(_redirectUrl, string.Empty);
                var parameters = System.Web.HttpUtility.ParseQueryString(link);
                if (parameters["#access_token"] != null)
                {
                    (Application.Current as App).Settings.AccessToken = parameters["#access_token"];
                    (Application.Current as App).Settings.TokenExpirationDate = DateTime.Now + TimeSpan.FromSeconds(int.Parse(parameters["expires_in"]));

                    this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("The app needs your permission to be able to work properly");
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
