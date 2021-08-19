using BillMate.Services.ViewModel.Configuration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }

        private void login()
        {
            if (ConfigurationManager.GetLocalSetting("usrPasswrd").ToString() == sttngs.hash(tbxPass.Password))
            {
                MainPage._loggedin = true;
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        private void bttnLoginP_Click(object sender, RoutedEventArgs e)
        {
            login();
        }

        private void tbxPass_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                login();
            }
        }
    }
}
