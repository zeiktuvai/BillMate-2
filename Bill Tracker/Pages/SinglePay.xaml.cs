using Bill_Tracker.Dialog;
using BillMate.Services.ViewModel.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Bill_Tracker.UI;
using BillMate.Services.ViewModel.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Pages
{  
    public sealed partial class SinglePay : Page
    {
        internal OTPDataProvider OP { get; set; }
        public SinglePay()
        {
            this.InitializeComponent();
            OP = new OTPDataProvider();
            refrshGrid(false);
        }
        private void refrshGrid(bool showPaid)
        {
            List<OneTime> OTPList;

            if (showPaid == false)
            {
                OTPList = OP.GetOTPTable().Where(p => p.Paid == 0).ToList();
                gvsOtp.ItemsSource = OTPList;
                bttnPaid.IsOn = false;
                lblOTPTotal.Text = OTPList.FindAll(t => t.Paid == 0).Sum(o => o.PayAmount).ToCurrencyString();
            } else
            {
                OTPList = OP.GetOTPTable().OrderBy(p => p.Paid).ToList();
                gvsOtp.ItemsSource = OTPList;
                lblOTPTotal.Text = OTPList.Sum(o => o.PayAmount).ToCurrencyString();
            }
        }

        private async void BttnAdd_Click(object sender, RoutedEventArgs e)
        {
            addBill aBillDiag = new addBill(this.Frame, true);
            await aBillDiag.ShowAsync();
            refrshGrid(false);
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UIFunctions.pointerEnter(sender, e);
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UIFunctions.pointerExit(sender, e);
        }


        private async void BttnEditOtp_Click(object sender, RoutedEventArgs e)
        {
            addBill edBillDiag = new addBill((OneTime)(sender as Button).DataContext);
            await edBillDiag.ShowAsync();
            refrshGrid(false);
        }

        private async void BttnDelOtp_Click(object sender, RoutedEventArgs e)
        {
            OneTime theOTP = (OneTime)(sender as Button).DataContext;

            var dialog = new Windows.UI.Popups.MessageDialog("Would you like to delete " + theOTP.Name + "?", "Delete Single One Time Bill?");
            dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var action = await dialog.ShowAsync();

            if (action.Id.ToString() == "1")
            {
                OP.DeleteOTP(theOTP);                
                refrshGrid(false);
            }
        }

        private async void BttnPayOtp_Click(object sender, RoutedEventArgs e)
        {
            OneTime theOTP = (OneTime)(sender as Button).DataContext;
            if (theOTP.Paid == 0)
            {
                PayBill pbdiag = new PayBill(theOTP);
                await pbdiag.ShowAsync();
                refrshGrid(false);
            } else
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Woul you like to mark " + theOTP.Name + " as not paid?", "Undo payment?");
                dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 0;

                var action = await dialog.ShowAsync();

                if (action.Id.ToString() == "1")
                {
                    theOTP.Paid = 0;
                    OP.UpdateOTP(theOTP);
                    refrshGrid(false);
                }
            }

        }

        private void BttnPaid_Toggled(object sender, RoutedEventArgs e)
        {
            if (bttnPaid.IsOn)
            {
                refrshGrid(true);
            }
            else
            {
                refrshGrid(false);
            }
        }

        private async void BttnClear_Click(object sender, RoutedEventArgs e)
        {
            List<OneTime> paidOTP = OP.GetOTPTable().FindAll(o => o.Paid == 1).ToList();

            var dialog = new Windows.UI.Popups.MessageDialog("Would you like to delete all paid items?", "Clear all paid?");
            dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var action = await dialog.ShowAsync();

            if (action.Id.ToString() == "1")
            {
                foreach (OneTime item in paidOTP)
                {
                    OP.DeleteOTP(item);
                }
            }
            refrshGrid(false);
        }

        private void bttnHlp_Click(object sender, RoutedEventArgs e)
        {
            UIFunctions.showHelp(this.Frame);
        }
    }
}
