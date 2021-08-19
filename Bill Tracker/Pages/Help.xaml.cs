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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Help : Page
    {
        public Help()
        {
            this.InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem sel = (sender as ListView).SelectedItem as ListViewItem;
            switch (sel.Name)
            {
                case "Bills":
                    BillsBlade.IsOpen = true;
                    break;
                case "Ledger":
                    LedgerBlade.IsOpen = true;
                    break;
                case "OTP":
                    OTPBlade.IsOpen = true;
                    break;
                case "MGMT":
                    MGMTBlade.IsOpen = true;
                    break;
                case "Goal":
                    GoalBlade.IsOpen = true;
                    break;
            }
        }
    }
}
