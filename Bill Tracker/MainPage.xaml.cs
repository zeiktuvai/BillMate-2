using Bill_Tracker.Dialog;
using Bill_Tracker.Pages;
using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.ViewModel.Providers;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//using Windows.ApplicationModel.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Bill_Tracker
{
    public sealed partial class MainPage : Page
    {
        //Control variable for being logged in
        public static bool _loggedin = false;
        //Control variable for the whats new page.
        //<TODO: ALWAYS UPDATE THIS>!!!!!!
        public static bool _showWhatsNew = true;
        //^^^^^^^ UPDATE!!!!! ^^^^^
                
        private string _menNumLateBill;
        public static string _billSelect = "All";
        public static string _monthSelect = "d30";

        public static bool _taskExecCurSession = false;

        public string menNumLateBill
        {
            get { return _menNumLateBill; }

            set {
               _menNumLateBill = value;
               lblMenDueBill.Text = _menNumLateBill;
                if (int.Parse(_menNumLateBill) > 0)
                {
                    menDueBill.Visibility = Visibility.Visible;
                    menDueBillNotification.Visibility = Visibility.Visible;
                } else
                {
                    menDueBill.Visibility = Visibility.Collapsed;
                    menDueBillNotification.Visibility = Visibility.Collapsed;
                }
            }
        }
        public MainPage()
        {
            this.InitializeComponent();
        }
    /// <summary>
    /// Tasks to condct on loaded.
    /// 
    /// </summary>
    private async void BillMate_Loaded(object sender, RoutedEventArgs e)
        {            
            await CheckDataSource().ConfigureAwait(true);

            VersionDataProvider VDP = new VersionDataProvider();
            VDP.checkTable();

            lstMen.SelectedIndex = 0;
            mainFrame.Navigate(typeof(Home));

            Window.Current.Activate();
            windowCust();

            Application.Current.Suspending += new SuspendingEventHandler(billm_suspnd);
            Application.Current.Resuming += new EventHandler<object>(billm_resume);

            if (ConfigurationManager.GetLocalSetting("loginEnbld").ToString() == "True")
            {
                if (_loggedin != true)
                {
                    this.Frame.Navigate(typeof(Login));
                }
                else
                {
                    whatsNew();
                }
            }
            else
            {
               whatsNew();
            }


            //Auto backup code
            if (ConfigurationManager.CheckLocalSetting("AutoBkup") && ConfigurationManager.GetLocalSetting("AutoBkup").ToString() == "True")
            {
                var bakupDate = ConfigurationManager.GetLocalSetting("AutoBkupDate");

                if (bakupDate == null || DateTime.Today.Date > DateTime.Parse(bakupDate.ToString()).AddDays(30))
                {
                    try
                    {
                        OneDriveProvider ODP = new OneDriveProvider();
                        var result = await ODP.UploadToOneDrive();
                        if (result)
                        {
                            ConfigurationManager.SetLocalSetting("AutoBkupDate", DateTime.Today.Date.ToString());
                        }                        
                    }
                    catch (IOException ex)
                    {
                        var dialog = new Windows.UI.Popups.MessageDialog("An error is keeping auto backup from working properly, please check network connection and ensure you are logged in to OneDrive.", "Auto Backup error");
                        dialog.Options = Windows.UI.Popups.MessageDialogOptions.None;
                        await dialog.ShowAsync();
                    }
                }
            }
        }


        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            menSplit.IsPaneOpen = !menSplit.IsPaneOpen;
        }

        /// <summary>
        /// Apply window customizations for desktop or phone
        /// </summary>
        private void windowCust()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                //CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                //ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                //titleBar.ButtonBackgroundColor = Colors.Transparent;
                //titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.LightSteelBlue;
                    titleBar.BackgroundColor = Colors.LightSteelBlue;
                }
            }

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {

                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = Colors.LightSteelBlue;
                    menSplit.IsPaneOpen = false;
                }
            }
        }

        private void lstMen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Home.IsSelected)
            {
                navigatePage(typeof(Home));
                tbxTitle.Text = "";
            }
            if (Bills.IsSelected)
            {
                navigatePage(typeof(billList));
                tbxTitle.Text = "Bills";
            }           
            if (History.IsSelected)
            {
                navigatePage(typeof(BillManagement));
                tbxTitle.Text = "Bill Management";
            }
            if (SaveGoal.IsSelected)
            {
                navigatePage(typeof(Goals));
                tbxTitle.Text = "Savings Goals";
            }
            if (Single.IsSelected)
            {
                navigatePage(typeof(SinglePay));
                tbxTitle.Text = "One-Time Payment";
            }
            if (Tools.IsSelected)
            {
                navigatePage(typeof(finTools));
                tbxTitle.Text = "Tools";
            }

        }


        private void lstSttngs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Feedback.IsSelected)
            {
                navigatePage(typeof(Feedback.Feedback));
                tbxTitle.Text = "";
            }
            if (Settings.IsSelected)
            {
                navigatePage(typeof(sttngs));
                tbxTitle.Text = "";
            }
        }

    

        /// <summary>
        /// If there is no DB file or on first run, sets up DB file in appdata
        /// </summary>
        //TODO: Get this out of here some how
        private async Task CheckDataSource()
        {
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("bills.db3") == null)
            {
            SQLiteRawDataProvider RDP = new SQLiteRawDataProvider();
            await RDP.CreateSQLiteDB().ConfigureAwait(true);
            }
        }


        private void bttnSttngs_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(typeof(sttngs), null, new DrillInNavigationTransitionInfo());
        }
        private void mainFrame_Navigated(object sender, NavigationEventArgs e)
        {            
            switch (e.SourcePageType.Name)
            {
                case "Home":
                    lstMen.SelectedItem = "Home";
                    lstSttngs.SelectedItem = null;
                    break;

                case "billList":
                    lstMen.SelectedItem = "Bills";
                    lstSttngs.SelectedItem = null;
                    break;
                case "acctLedger":
                    lstMen.SelectedItem = "Ledger";
                    lstSttngs.SelectedItem = null;
                    break;

                case "Single":
                    lstMen.SelectedItem = "Single";
                    lstSttngs.SelectedItem = null;
                    break;
                case "payHistory":
                    lstMen.SelectedItem = "History";
                    lstSttngs.SelectedItem = null;
                    break;
                case "Tracker":
                    lstMen.SelectedItem = "Debt";
                    lstSttngs.SelectedItem = null;
                    break;
                case "Goals":
                    lstMen.SelectedItem = "SaveGoal";
                    lstSttngs.SelectedItem = null;
                    break;
                case "accntDetail":                    
                    lstMen.SelectedItem = null;
                    lstSttngs.SelectedItem = null;
                    break;
                case "Feedback":
                    lstSttngs.SelectedItem = "Feedback";
                    lstMen.SelectedItem = null;
                    break;
                case "sttngs":                    
                    lstMen.SelectedItem = null;
                    lstSttngs.SelectedItem = "Settings";
                    break;
                case "Help":
                    lstMen.SelectedItem = null;
                    lstSttngs.SelectedItem = null;
                    tbxTitle.Text = "Help";
                    break;
                case "finTools":
                    lstMen.SelectedItem = "Tools";
                    lstSttngs.SelectedItem = null;
                    break;
                default:
                    break;
            }            
        }

        private void billm_resume(Object sender, Object e)
        {
            Windows.Storage.ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Object value = localsettings.Values["location"];
            var page = typeof(billList);
            //TODO: Update resume functionality
            switch (value.ToString())
            {
                case "billList":
                    page = typeof(billList);
                    break;              
                default:
                    page = typeof(billList);
                    break;
            }

             mainFrame.Navigate(page);
        }

        private void billm_suspnd(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            localsettings.Values["location"] = mainFrame.SourcePageType.Name;
            _loggedin = false;

        }

        /// <summary>
        /// Displays the "Whats New" page if the current release version is not listed in app data.
        /// </summary>
        private async void whatsNew()
        {
            if (_showWhatsNew is true)
            {
                Package pack = Package.Current;
                PackageId packId = pack.Id;
                PackageVersion pver = packId.Version;
                string vers = string.Format("{0}.{1}.{2}", pver.Major, pver.Minor, pver.Build);

                if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("version"))
                {
                    ConfigurationManager.SetLocalSetting("version", "");
                }
                Windows.Storage.ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localsettings.Values["version"].ToString() != vers) 
                    {
                        whatsNew newDia = new whatsNew();
                        await newDia.ShowAsync();                
                }
            }
            
        }   
        
        private void navigatePage(System.Type navPage)
        {
           mainFrame.Navigate(navPage, null, new DrillInNavigationTransitionInfo());
           
        }
    }
    
  
}
