using Bill_Tracker.Global;
using Bill_Tracker.UI;
using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.ViewModel.Models;
using BillMate.Services.ViewModel.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Bill_Tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Ledger : Page
    {        
        private string billBdgtSettngName = "billBdgtAmt";
        private double billBdgtAmnt = 0;
        internal PaymentDataProvider PP { get; set; } = new PaymentDataProvider();
        public Ledger()
        {
            this.InitializeComponent();
            initializePage();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainPage mainPage = (Window.Current.Content as Frame).Content as MainPage;
            BillDataProvider BillProvider = new BillDataProvider();                   
            mainPage.menNumLateBill = BillProvider.GetBillData("srtByDte", Globals._WORKING_MONTH).Where(b => b.isPastDue == true).Count().ToString();
            BillProvider = null;
        }

        private void initializePage()
        {
            double billBdgt = 0;
            double amtPosted = 0;

            if (ConfigurationManager.CheckLocalSetting(billBdgtSettngName) == false)
            {
                billBdgt = 1000;
            }
            else
            {
                billBdgt = double.Parse(ConfigurationManager.GetLocalSetting(billBdgtSettngName).ToString());
            }

            billBdgtAmnt = billBdgt;

            tbxBillBdgt.Text = billBdgt.ToString();
            
            List<Payment> oPay = PP.GetPaymentData().Where(m => DateTime.Parse(m.dispPayDate).Month == DateTime.Now.Month)
                                   .Where(y => DateTime.Parse(y.dispPayDate).Year == DateTime.Now.Year)
                                   .OrderBy(x => x.dispPayDate).ToList();
            List<Payment> paysource = oPay.OrderBy(s => s.payDate).ToList();

            foreach (var item in paysource)
            {
                billBdgt = billBdgt - double.Parse(item.amount.ToString());
                item.ledgerBalance = billBdgt;
                item._ledgerBalance = billBdgt.ToCurrencyString();
                if (item.ledgerBalance < 0)
                {
                    item._ledgerBalanceColor = "Red";
                }
                else
                {
                    item._ledgerBalanceColor = "Black";
                }
                if (item.posted == true)
                {
                    item._ledgerLinePosted = "Visible";
                    amtPosted += item.amount;
                }
                else
                {
                    item._ledgerLinePosted = "Collapsed";
                }



            }

            lblARem.Text = billBdgt.ToCurrencyString();
            lblAPo.Text = amtPosted.ToCurrencyString();
            lblABudget.Text = billBdgtAmnt.ToCurrencyString();
            lstPayments.ItemsSource = paysource;

            int notPosted = 0;
            foreach (Payment item in lstPayments.Items.ToList())
            {
                if (item.posted == false)
                {
                    notPosted++;
                }
            }

            if (notPosted == 0)
            {
                ldgPostAll.Label = "Un-Post All";
            }
            else
            {
                ldgPostAll.Label = "Post All";
            }

            if (oPay.Count == 0)
            {
                stkNoItems.Visibility = Visibility.Visible;
                lstPayments.Visibility = Visibility.Collapsed;
            }
            else
            {
                stkNoItems.Visibility = Visibility.Collapsed;
                lstPayments.Visibility = Visibility.Visible;
            }
        }

         private void tbxBillBdgt_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            string value = tbxBillBdgt.Text;

            foreach (char digit in value)
            {
                if (!char.IsDigit(digit))
                {
                    var pos = value.IndexOf(digit);
                    value = value.Remove(value.IndexOf(digit));
                    tbxBillBdgt.Text = value;
                }
            }

            if (value.Contains("."))
            {
                int pPos = value.IndexOf(".");
                string main = value.Substring(0, pPos);
                string dec = value.Substring(pPos + 1);

                if (dec.Contains("."))
                {
                    dec = dec.Remove(dec.IndexOf("."), 1);
                }

                if (dec.Length > 2)
                {
                    dec = dec.Substring(0, 2);
                }

                string amnt = main + "." + dec;
                tbxBillBdgt.Text = amnt;
            }
            tbxBillBdgt.SelectionStart = tbxBillBdgt.Text.Length;
        }

        private void ldgBttnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbxBillBdgt.Text))
            {
                tbxBillBdgt.Text = ConfigurationManager.GetLocalSetting(billBdgtSettngName).ToString();
            }
            else
            {
                if (tbxBillBdgt.Text == billBdgtAmnt.ToString())
                {
                    tbxBillBdgt.Text = ConfigurationManager.CheckLocalSetting(billBdgtSettngName) ?
                                       ConfigurationManager.GetLocalSetting(billBdgtSettngName).ToString() :
                                       tbxBillBdgt.Text;
                    ConfigurationManager.SetLocalSetting(billBdgtSettngName, tbxBillBdgt.Text);        
                } else
                {                
                    ConfigurationManager.SetLocalSetting(billBdgtSettngName, tbxBillBdgt.Text);
                    btnSettings.Flyout.Hide();
                    lblABudget.Text = tbxBillBdgt.Text.ToCurrencyString();
                    initializePage();
                }
            }
            btnSettings.Flyout.Hide();
        }

        private void ldgBttnCncl_Click(object sender, RoutedEventArgs e)
        {
            btnSettings.Flyout.Hide();
        }

        private void postPayment(Payment pay)
        {
            pay.posted = !pay.posted;           
            PP.UpdatePayment(pay);
            initializePage();
        }

        private async void delPayment(Payment pay)
        {
            string diaText = string.Format("Payment \"{0}\" on {1} for the amount of:  {2}, \n will be deleted. do you wish to continue?",
             pay.billName, (DateTime.Parse(pay.dispPayDate)).ToString("MM/dd/yyyy"), string.Format("{0:F2}", pay.amount));
            var dialog = new Windows.UI.Popups.MessageDialog(diaText, "Delete Payment?");
            dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var action = await dialog.ShowAsync();

            if (action.Id.ToString() == "1")
            {
                PP.DeletePayment(pay);
            }
            initializePage();
        }

        private void ldgPost_Click(object sender, RoutedEventArgs e)
        {
            postPayment((Payment)(sender as Button).DataContext);            
        }


        private void ldgPostAll_Click(object sender, RoutedEventArgs e)
        {
            var list = lstPayments.Items.ToList();
            var notPost = list.Where(p => (p as Payment).posted == false).ToList();
            if (notPost.Count > 0)
            {
                foreach (Payment item in notPost)
                {
                    item.posted = !item.posted;
                    PP.UpdatePayment(item);
                }
                ldgPostAll.InvalidateArrange();
            }
            else
            {
                foreach (Payment item in list)
                {
                    item.posted = !item.posted;
                    PP.UpdatePayment(item);
                }
            }
            initializePage();
        }

        private void ldgDeletePay_Click(object sender, RoutedEventArgs e)
        {            
            delPayment((Payment)(sender as Button).DataContext);         
        }

        private void SwipeDel_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            delPayment((Payment)args.SwipeControl.DataContext);
        }

        private void SwipePost_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            postPayment((Payment)args.SwipeControl.DataContext);
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UIFunctions.pointerEnter(sender, e);
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UIFunctions.pointerExit(sender, e);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UIFunctions.swipe(sender, e);
        }

        private void bttnHlp_Click(object sender, RoutedEventArgs e)
        {
            UIFunctions.showHelp(this.Frame);
        }
    }

}
