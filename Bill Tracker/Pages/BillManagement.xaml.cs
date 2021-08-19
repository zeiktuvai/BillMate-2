using Bill_Tracker.Dialog;
using Bill_Tracker.UI;
using BillMate.Services.ViewModel.Models;
using BillMate.Services.ViewModel.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


namespace Bill_Tracker
{
    public sealed partial class BillManagement : Page
    {
        internal BillDataProvider BP { get; set; }
        internal PaymentDataProvider PP { get; set; }
        List<Payment> _payments;
        List<Bill> _bills;
        Bill _selectedBill;
        string _timeSpan;
        string _selBillFreq = "ALL";
        string _selCategory = "ALL";

        public BillManagement()
        {
            this.InitializeComponent();
            BP = new BillDataProvider();
            PP = new PaymentDataProvider();
            _payments = PP.GetPaymentData();
            _bills = BP.GetBillData("srtByDte",0,true);
            List<string> arch = new List<string>();
            bttnBack.Visibility = Visibility.Collapsed;
            fillCatBox();
            histList.ItemsSource = _bills.OrderBy(o => o.Name);            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                Bill inBill = (Bill)e.Parameter;
                histList.ItemsSource = _bills.FindAll(b => b.Name == inBill.Name);
                histList.SelectedIndex = 0;
                dayAll.IsChecked = true;
                day30.IsChecked = false;
                bttnBack.Visibility = Visibility.Visible;
                _timeSpan = "ALL";
                updateList();
            }
        }

        private void fillCatBox()
        {
            cbxCat.Items.Add("All");
            List<string> catgories = new List<string>();
            foreach (Bill item in _bills)
            {
                if (!catgories.Contains(item.Category.ToString()))
                {
                    catgories.Add(item.Category);
                    cbxCat.Items.Add(item.Category);
                }
            }
            cbxCat.SelectedIndex = 0;
        }

        private void updateList()
        {
            List<Payment> sourceList = new List<Payment>();
            sourceList.Clear();
          
            sourceList = _payments.FindAll(b => b.BillID == _selectedBill.BillID);

            switch (_timeSpan)
            {
                case "30":
                    sourceList = sourceList.FindAll(d => d.payDate >= (DateTime.Now.Date).AddDays(-30));
                    break;
                case "60":
                    sourceList = sourceList.FindAll(d => d.payDate >= (DateTime.Now.Date).AddDays(-60));
                    break;
                case "90":
                    sourceList = sourceList.FindAll(d => d.payDate >= (DateTime.Now.Date).AddDays(-90));
                    break;
                case "180":
                    sourceList = sourceList.FindAll(d => d.payDate >= (DateTime.Now.Date).AddDays(-180));
                    break;
                case "365":
                    sourceList = sourceList.FindAll(d => d.payDate >= (DateTime.Now.Date).AddDays(-365));
                    break;
                case "ALL":                    
                    break;                
            }


            lstPayments.ItemsSource = sourceList.OrderByDescending(o => o.payDate);

            if (sourceList.Count == 0)
            {
                stkNoItems.Visibility = Visibility.Visible;
                lstPayments.Visibility = Visibility.Collapsed;
            }
            else
            {
                stkNoItems.Visibility = Visibility.Collapsed;
                lstPayments.Visibility = Visibility.Visible;
            }
            if (sourceList != null)
            {
                cmdbr.Visibility = Visibility.Visible;
                stckBillOpt.Visibility = Visibility.Visible;
            }
        }


        private async void histUnArch_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBill.Reminder == "Archived")
            {
                var dialog = new Windows.UI.Popups.MessageDialog(string.Format("{0} will be un-archived and begin to display in the bill list. " +
                    "Do you wish to un-archive?", _selectedBill.Name), "Un-Archive Bill?");
                dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 0;

                var action = await dialog.ShowAsync();

                if (action.Id.ToString() == "1")
                {
                    _selectedBill.Reminder = "";
                    BP.UpdateBill(_selectedBill);                  
                }
            } else
            {
                var dialog = new Windows.UI.Popups.MessageDialog(string.Format("{0} will be archived and no longer display in the bill list or be tracked. " +
                   "Do you wish to archive this bill?", _selectedBill.Name), "Archive Bill?");
                dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 0;

                var action = await dialog.ShowAsync();

                if (action.Id.ToString() == "1")
                {
                    _selectedBill.Reminder = "Archived";
                    BP.UpdateBill(_selectedBill);               
                }
            }
        }

      
        private void setBillListType()
        {
            List<Bill> filtBillFreq = new List<Bill>();
            List<Bill> filtBillCat = new List<Bill>();
            

            if (_selBillFreq == "All")
            {
                filtBillFreq = _bills.OrderBy(o => o.Name).ToList();
            } else if (_selBillFreq == "Archived")
            {
                filtBillFreq = _bills.Where(b => b.Reminder == _selBillFreq).ToList().OrderBy(o => o.Name).ToList();
            } else
            {
                filtBillFreq = _bills.Where(b => b.Frequency == _selBillFreq).ToList().OrderBy(o => o.Name).ToList();                      
            }
            
            if (_selCategory == "All")
            {
                filtBillCat = filtBillFreq;
            } else
            {
                filtBillCat = filtBillFreq.Where(c => c.Category == cbxCat.SelectedItem.ToString()).OrderBy(o => o.Name).ToList();
            }

            histList.ItemsSource = filtBillCat;
        }



        private async void delPayment(Payment pay)
        {
            string delMessage = string.Format("Do you wish to delete the payment on {0} for the date of {1}", pay.billName, pay.dispPayDate);


            var dialog = new Windows.UI.Popups.MessageDialog(delMessage, "Delete Payment?");
            dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var action = await dialog.ShowAsync();

            if (action.Id.ToString() == "1")
            {
                PP.DeletePayment(pay);
                _payments = PP.GetPaymentData();
                updateList();
            }
        }


        #region UI Buttons
        private async void BttnAdd_Click(object sender, RoutedEventArgs e)
        {
            addBill addBillDiag = new addBill(this.Frame);
            await addBillDiag.ShowAsync();
        }
        private async void BttnBillDel_Click(object sender, RoutedEventArgs e)
        {
            Bill theBill = _selectedBill;
            string delMessage = "";

            switch (theBill.Frequency)
            {
                case "Monthly":
                    delMessage = theBill.Name + " will be deleted, do you wish to continue?";
                    break;
                case "Bi-Monthly":
                    delMessage = "Both instances of " + theBill.Name + " will be deleted, do you wish to continue?";
                    break;
                case "Weekly":
                    delMessage = "All instances of " + theBill.Name + " will be deleted, do you wish to continue?";
                    break;
            }

            var dialog = new Windows.UI.Popups.MessageDialog(delMessage, "Delete Bill?");
            dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var action = await dialog.ShowAsync();

            if (action.Id.ToString() == "1")
            {
                BP.DeleteBill(theBill);
                this.Frame.Navigate(typeof(BillManagement));
            }
        }
        private async void bttnBillEdit_Click(object sender, RoutedEventArgs e)
        {
            editBill eb = new editBill(_selectedBill);
            await eb.ShowAsync();
            this.Frame.Navigate(typeof(BillManagement));
        }
        private void day30_Click(object sender, RoutedEventArgs e)
        {
            _timeSpan = "30";
            updateList();
            day60.IsChecked = false;
            day90.IsChecked = false;
            day180.IsChecked = false;
            day365.IsChecked = false;
            dayAll.IsChecked = false;
            
        }
        private void day60_Click(object sender, RoutedEventArgs e)
        {
            _timeSpan = "60";
            updateList();
            day30.IsChecked = false;
            day90.IsChecked = false;
            day180.IsChecked = false;
            day365.IsChecked = false;
            dayAll.IsChecked = false;
        }
        private void day90_Click(object sender, RoutedEventArgs e)
        {
            _timeSpan = "90";
            updateList();
            day30.IsChecked = false;
            day60.IsChecked = false;
            day180.IsChecked = false;
            day365.IsChecked = false;
            dayAll.IsChecked = false;
        }
        private void day180_Click(object sender, RoutedEventArgs e)
        {
            _timeSpan = "180";
            updateList();
            day30.IsChecked = false;
            day60.IsChecked = false;
            day90.IsChecked = false;
            day365.IsChecked = false;
            dayAll.IsChecked = false;
        }
        private void day365_Click(object sender, RoutedEventArgs e)
        {
            _timeSpan = "365";
            updateList();
            day30.IsChecked = false;
            day60.IsChecked = false;
            day90.IsChecked = false;
            day180.IsChecked = false;
            dayAll.IsChecked = false;
        }
        private void dayAll_Click(object sender, RoutedEventArgs e)
        {
            _timeSpan = "ALL";
            updateList();
            day30.IsChecked = false;
            day60.IsChecked = false;
            day90.IsChecked = false;
            day180.IsChecked = false;
            day365.IsChecked = false;
        }
        private void cbxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string Selected = (((sender as ComboBox).SelectedItem) as ComboBoxItem).Tag.ToString();
            if (histList != null)
            {
                histList.SelectedItem = null;
                switch (Selected)
                {
                    case "AL":
                        _selBillFreq = "All";
                        setBillListType();
                        break;
                    case "MO":
                        _selBillFreq = "Monthly";
                        setBillListType();
                        break;
                    case "BI":
                        _selBillFreq = "Bi-Monthly";
                        setBillListType();
                        break;
                    case "WK":
                        _selBillFreq = "Weekly";
                        setBillListType();
                        break;
                    case "AN":
                        _selBillFreq = "Annual";
                        setBillListType();
                        break;
                    case "AR":
                        _selBillFreq = "Archived";
                        setBillListType();
                        break;
                    default:
                        break;
                }
            }
        }
        private void DelItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            delPayment((Payment)(args.SwipeControl.DataContext));
        }

        private void DelPay_Click(object sender, RoutedEventArgs e)
        {
            delPayment((Payment)(sender as Button).DataContext);
        }
        private void BttnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(billList), null, new DrillInNavigationTransitionInfo());
        }
        #endregion

        #region UI Interactions
        private void histList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (histList.SelectedItem != null)
            {
                Bill selBill = ((sender as ListView).SelectedItem as Bill);

                lblBillName.Text = selBill.Name;
                lblBillCat.Text = selBill.Category;
                lblBillDue.Text = selBill.baseAmount.ToCurrencyString();
                lblBillFreq.Text = selBill.Frequency;
                txtBillAmn.Visibility = Visibility.Visible;
                txtBillCa.Visibility = Visibility.Visible;

                if (selBill.Reminder == "Archived")
                {
                    histUnArch.IsChecked = true;
                }
                else
                {
                    histUnArch.IsChecked = false;
                }
                _selectedBill = selBill;
                _timeSpan = "ALL";
                day30.IsChecked = false;
                day60.IsChecked = false;
                day90.IsChecked = false;
                day180.IsChecked = false;
                day365.IsChecked = false;
                dayAll.IsChecked = true;
                updateList();
            }
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
        private void CbxCat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selCategory = cbxCat.SelectedItem.ToString();
            setBillListType();
        }
        private void bttnHlp_Click(object sender, RoutedEventArgs e)
        {
            UIFunctions.showHelp(this.Frame);
        }
        #endregion
    }
}
