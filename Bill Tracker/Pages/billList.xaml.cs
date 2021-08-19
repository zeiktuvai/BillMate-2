using Bill_Tracker.Dialog;
using Bill_Tracker.Global;
using Bill_Tracker.UI;
using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.ViewModel.Models;
using BillMate.Services.ViewModel.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;


//Remove all future scheduled bill stuff
//Remove the details pane

namespace Bill_Tracker
{

    public sealed partial class billList : Page
    {
        public List<Bill> BillsList { get; set; }
        public Queue<Bill> billQueue = new Queue<Bill>();
        //public Bill selBill { get; set; }
        public bool pointerIn { get; set; }
        public List<String> Days = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        public BillDataProvider BP { get; set; }
        public PaymentDataProvider PP { get; set; }

        public billList()
        {
            this.InitializeComponent();

            BP = new BillDataProvider();
            PP = new PaymentDataProvider();
            InitializeBillPage(Global.Globals._WORKING_MONTH);
            getStarted_Check();
        }

        private void InitializeBillPage(int dateShift = 0)
        {
            // Set the current, -1 and +1 date buttons
            bttnCurMo.Content = DateTime.Now.Date.ToString("MMMM");
            bttnLastMo.Content = DateTime.Now.AddMonths(-1).ToString("MMMM");
            bttnNextMo.Content = DateTime.Now.AddMonths(1).ToString("MMMM");

            // Get list of bills from the data source.
            BillsList = BP.GetBillData(getSortOrder(), dateShift);
                        
            //Sort bill list by date or category
            cvsBills.Source = BP.SortBillData(BillsList, getSortOrder());
            lvBills.SelectedItem = null;

            // Find any late bills and return a count.
            var late = BillsList.FindAll(b => b.isPastDue == true).Count.ToString();
            // Define what the main page window is, and set any late function on the main page.
            MainPage mainPage = (Window.Current.Content as Frame).Content as MainPage;
            mainPage.menNumLateBill = late;

            // Highlight the button for the month currently selected
            switch (Globals._WORKING_MONTH)
            {
                case -1:
                    bttnLastMo.BorderThickness = new Thickness(2, 2, 2, 2);
                    bttnCurMo.BorderThickness = new Thickness(0, 0, 0, 0);
                    bttnNextMo.BorderThickness = new Thickness(0, 0, 0, 0);
                    break;

                case 0:
                    bttnLastMo.BorderThickness = new Thickness(0, 0, 0, 0);
                    bttnCurMo.BorderThickness = new Thickness(2, 2, 2, 2);
                    bttnNextMo.BorderThickness = new Thickness(0, 0, 0, 0);
                    break;

                case 1:
                    bttnLastMo.BorderThickness = new Thickness(0, 0, 0, 0);
                    bttnCurMo.BorderThickness = new Thickness(0, 0, 0, 0);
                    bttnNextMo.BorderThickness = new Thickness(2, 2, 2, 2);
                    break;
            }

            // Set data text blocks
            lblTotal.Text = BillsList.Sum(x => x.Amount).ToCurrencyString();
            lblPaid.Text = BillsList.FindAll(b => b.isPaid == true).Sum(x => x.Amount).ToCurrencyString();
            lblBillCnt.Text = BillsList.Count.ToString();
            lblRemain.Text = BillsList.FindAll(b => b.isPaid == false).Sum(x => x.baseAmount).ToCurrencyString();
        }

        private void getStarted_Check()
        {
            if (BillsList.Count == 0)
            {
                stkNoItems.Visibility = Visibility.Visible;
                lvBills.Visibility = Visibility.Collapsed;
            }
            else
            {
                stkNoItems.Visibility = Visibility.Collapsed;
                lvBills.Visibility = Visibility.Visible;
            }
        }


        /// <summary>
        /// Retrieves the sorting order from app data settings
        /// </summary>
        /// <returns> [String] sortOrder </returns>
        private string getSortOrder()
        {
            //Windows.Storage.ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var value = ConfigurationManager.GetLocalSetting("sortOrder");
            if (value == null)
            {
                ConfigurationManager.SetLocalSetting("sortOrder", "srtByDte");
                value = "strByDte";
            }
            if (value.ToString() == "srtByCat")
            {
                tglSort.IsOn = true;
            }
            return value.ToString();
        }

       //Pay clicked on menu flyout of bill item.

        #region bill actions
        private void shareBill (Bill bill)
        {
            billQueue.Enqueue(bill);
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
            DataTransferManager dtm = DataTransferManager.GetForCurrentView();

            dtm.DataRequested += dtm_datareq;
        }      
        private async void payBill (Bill bill)
        {
            Dialog.PayBill dialog = new Dialog.PayBill(bill);
            await dialog.ShowAsync();
            string result = dialog.Result;

            if (result != "Cancel")
            {
                InitializeBillPage(Global.Globals._WORKING_MONTH);
            }
        }

        private void manageBillPay (Bill bill)
        {
            this.Frame.Navigate(typeof(BillManagement), bill);
        }
        #endregion

        #region UI Button Clicks
        private void PayBill_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                ((((sender as Button).Parent as StackPanel).Parent as FlyoutPresenter).Parent as Popup).IsOpen = false;
            }
            payBill((Bill)(sender as Button).DataContext);
        }
        private void bttnQPay_Click(object sender, RoutedEventArgs e)
        {
            if (Global.Globals._WORKING_MONTH != -1)
            {
                Bill qpBill = (Bill)(sender as Button).DataContext;                
                Payment qpay = new Payment();

                if (Global.Globals._WORKING_MONTH == 1)
                {
                    qpay = new Payment() { amount = qpBill.baseAmount, BillID = qpBill.BillID, dispPayDate = (qpBill.Frequency == "Bi-Monthly") ? qpBill.dueDate.ToString("d") : qpBill.dueDate.Date.ToString("d") };
                }
                else
                {
                    qpay = new Payment() { amount = qpBill.baseAmount, BillID = qpBill.BillID, dispPayDate = (qpBill.Frequency == "Bi-Monthly") ? qpBill.dueDate.ToString("d") : DateTime.Now.Date.ToString("d") };
                }

                PP.CreatePayment(qpay);                
                this.Frame.Navigate(typeof(billList));
            }
        }
        private void shrBillBttn_Click(object sender, RoutedEventArgs e)
        {
            shareBill((Bill)(sender as Button).DataContext);
        }        
        private void ManageBillPay_Click(object sender, RoutedEventArgs e)
        {
            manageBillPay((Bill)(sender as Button).DataContext);
        }        
        private void SwipeShare_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            shareBill((Bill)args.SwipeControl.DataContext);
        }
        private void SwipePay_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            payBill((Bill)args.SwipeControl.DataContext);
        }
        private void SwipeManage_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            manageBillPay((Bill)args.SwipeControl.DataContext);
        }
        private void BttnLastMo_Click(object sender, RoutedEventArgs e)
        {
            Global.Globals._WORKING_MONTH = -1;
            InitializeBillPage(Global.Globals._WORKING_MONTH);
        }
        private void BttnCurMo_Click(object sender, RoutedEventArgs e)
        {
            Global.Globals._WORKING_MONTH = 0;
            InitializeBillPage(Global.Globals._WORKING_MONTH);
        }
        private void BttnNextMo_Click(object sender, RoutedEventArgs e)
        {
            Global.Globals._WORKING_MONTH = 1;
            InitializeBillPage(Global.Globals._WORKING_MONTH);
        }
        private async void bttnAdd_Click(object sender, RoutedEventArgs e)
        {
            addBill addBillDiag = new addBill(this.Frame);
            await addBillDiag.ShowAsync();
        }
        private void tglSort_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.cvsBills.Source != null)
            {
                ToggleSwitch toggle = sender as ToggleSwitch;
                if (!toggle.IsOn == true && ConfigurationManager.GetLocalSetting("sortOrder").ToString() == "srtByDte")
                {

                }
                if (toggle.IsOn == true)
                {
                    ConfigurationManager.SetLocalSetting("sortOrder", "srtByCat");
                } else
                {
                    ConfigurationManager.SetLocalSetting("sortOrder", "srtByDte");
                }

                InitializeBillPage(Global.Globals._WORKING_MONTH);
            }
        }
        #endregion

        #region UI Actions
        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UIFunctions.pointerEnter(sender, e);
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UIFunctions.pointerExit(sender, e);
        }
        private void lblDte_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (this.cvsBills.Source != null)
            {
                if (tglSort.IsOn == true)
                {
                    ConfigurationManager.SetLocalSetting("sortOrder", "srtByCat");
                } else
                {
                    ConfigurationManager.SetLocalSetting("sortOrder", "srtByDte");
                }
                this.Frame.Navigate(typeof(billList));
            }
        }
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UIFunctions.swipe(sender, e);
        }
        //private void DetailCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        //{
        //    pointerIn = true;
        //}
        private void bttnHlp_Click(object sender, RoutedEventArgs e)
        {
            UIFunctions.showHelp(this.Frame);
        }
        #endregion

        private void dtm_datareq(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs e)
        {
            Bill bill = billQueue.Dequeue();
            e.Request.Data.Properties.Title = "BillMate: Sharing bill " + bill.Name;
            string message = "Date Due: " + bill.dueDate.ToString() + "\r\n Amount Due: " + bill.baseAmount.ToString() + "\r\n Frequency: " + bill.Frequency + "\r\n Category: " + bill.Category;
            if (bill.isPastDue == true) { message += "\r\n Bill Past Due"; }
            e.Request.Data.SetText(message);
        }
    }

    //convert properties to true/false
    public class visibilityBoolConv : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string returnVal = "";

            if ((string)parameter == "postColor")
            {
                switch (value.ToString().ToLower())
                {
                    case "true":
                        return "PaleGreen";
                }   
            }

            switch (value.ToString().ToLower())
            {
                case "true":
                    return "Visible";
                case "false":
                    return "Collapsed";
                default:
                    returnVal = "Collapsed";
                    break;
            }
            
            return returnVal;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return "Visible";
                else
                    return "Collapsed";
            }
            return "Collapsed";
        }
    }

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((double)value).ToCurrencyString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}