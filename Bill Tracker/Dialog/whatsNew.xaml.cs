using BillMate.Services.ViewModel.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Dialog
{
    public sealed partial class whatsNew : ContentDialog
    {
        string _vers;
        public whatsNew()
        {
            this.InitializeComponent();
            PackageVersion pver = ((Package.Current).Id).Version;
            string versin = string.Format("{0}.{1}.{2}", pver.Major, pver.Minor, pver.Build);
            this.Title = "What's New in version " + versin;
            _vers = versin;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ConfigurationManager.SetLocalSetting("version", _vers);
            Hide();
        }

    }
}
