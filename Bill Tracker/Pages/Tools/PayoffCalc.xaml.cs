using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.ViewModel.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Pages.Tools
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PayoffCalc : Page
    {

        List<PayoffAccount> acctList = new List<PayoffAccount>();
        bool autoSave = false;
        public PayoffCalc()
        {
            this.InitializeComponent();

            getSavedPayoff();
            if (ConfigurationManager.CheckLocalSetting("PayoffCalcAutoSave"))
            {
                autoSave = bool.Parse(ConfigurationManager.GetLocalSetting("PayoffCalcAutoSave").ToString());
                tgglAutoSave.IsOn = autoSave;
            }

            if (ConfigurationManager.CheckLocalSetting("PayoffCalcNextMo"))
            {
                cbxNextMonth.IsChecked = bool.Parse(ConfigurationManager.GetLocalSetting("PayoffCalcNextMo").ToString());
            }            
        }

        internal class PayoffAccount
        {
            public string AccountName { get; set; }
            public double PayAmmount { get; set; }
            public double TotalDue { get; set; }
            public double APR { get; set; }
            public List<PayoffItem> PayoffList { get; set; }
            public string _DispPayAmmnt { get; set; }
            public string _DispTotalDue { get; set; }
            public string _Payments { get; set; }
            public string _Interest { get; set; }
            public string _Payoff { get; set; }

            internal static PayoffAccount calculateAccount(PayoffAccount acct, bool nextMonth)
            {
                double totalInterest = 0;

                if (acct.PayAmmount != 0 && acct.TotalDue != 0)
                {
                    acct.PayoffList = PayoffItem.CalculatePayments(acct.TotalDue, acct.PayAmmount, nextMonth, acct.APR);
                    foreach (var item in acct.PayoffList)
                    {
                      totalInterest += item._Interest;
                    }
                } else
                {
                    acct.PayoffList = null;
                }

                acct._Payments = "Total Payments:  " + ((acct.PayoffList != null) ? (acct.PayoffList.Count - 1).ToString() : "0");
                acct._Payoff = "Payoff Date:  " + ((acct.PayoffList != null) ? acct.PayoffList.Last().Month : "UNK");

                acct._Interest = "Est. Interest Paid:  " + totalInterest.ToCurrencyString();

                return acct;
            }

            
        }

        internal class PayoffItem
        {
            public string Month { get; set; }
            public string Ammnt { get; set; }
            public string Remain { get; set; }
            public string _BGColor { get; set; }
            public string _FontColor { get; set; }
            public double _Interest { get; set; }
            public string Tooltip { get; set; }

            internal static List<PayoffItem> CalculatePayments(double Total, double Monthly, bool nextMonth, double apr)
            {
                List<PayoffItem> items = new List<PayoffItem>();
                int Month = 0;
                DateTime cMonth = DateTime.Now.Date;

                Month = nextMonth == true ? 1 : 0;

                items.Add(new PayoffItem() { Month = "Start", Ammnt = "$0", Remain = Total.ToCurrencyString(), _FontColor = "Black", _BGColor = "LightBlue" });

                while (Total > 0)
                {
                    PayoffItem pitem = new PayoffItem();
                    pitem.Ammnt = Monthly.ToCurrencyString();
                    pitem._Interest = calculateInterest(Total, apr);
                    pitem.Remain = (Total - (Monthly - pitem._Interest)).ToCurrencyString();
                    pitem.Month = cMonth.AddMonths(Month).ToString("MMM yyyy");

                    Total -= Monthly;
                    if (Total > 0)
                    {
                        pitem._BGColor = "LightGreen";
                        pitem._FontColor = "Black";
                    }
                    else
                    {
                        pitem._BGColor = "LightGray";
                        pitem._FontColor = "Red";
                    }
                    pitem.Tooltip = string.Format("Principal: {0} \nInterest: {1}", (Monthly - pitem._Interest).ToCurrencyString(), pitem._Interest.ToCurrencyString());

                    items.Add(pitem);

                    Month++;
                }            

                return items;
            }

            internal static double calculateInterest(double ammount, double apr)
            {
                double monthlyAPR = (apr / 100) / 12;
                double monthlyInt = monthlyAPR * ammount;
                return Math.Round(monthlyInt, 2);
            }
        }


        private void addAccount_Click(object sender, RoutedEventArgs e)
        {
            PayoffAccount acct = new PayoffAccount() { AccountName = "Enter Account Name", PayAmmount = 0, _DispPayAmmnt = "Enter Monthly Payment", TotalDue = 0, _DispTotalDue = "Enter Total Due" };
            acctList.Add(acct);

            lvAcct.ItemsSource = null;
            lvAcct.ItemsSource = acctList;
        }
        
        private void tbxAcctName_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            changeValues(sender, e);
        }

        private void tbxStart_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            changeValues(sender, e);
        }

        private void tbxMonthly_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            changeValues(sender, e);
        }

        private void tbxAcctName_LostFocus(object sender, RoutedEventArgs e)
        {
            lostFocusReset(sender, e);
        }
        private void tbxStart_LostFocus(object sender, RoutedEventArgs e)
        {
            lostFocusReset(sender, e);
        }

        private void tbxMonthly_LostFocus(object sender, RoutedEventArgs e)
        {
            lostFocusReset(sender, e);
        }

        internal void changeValues(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Tab)
            {
                PayoffAccount acct = ((sender as TextBox).DataContext as PayoffAccount);
                string boxText = (sender as TextBox).Text;

                if (boxText.Length != 0)
                {
                    switch ((sender as TextBox).Name)
                    {
                        case "tbxStart":
                            if (boxText.ToCurrency() >= 25)
                            {
                                acct.TotalDue = boxText.ToCurrency();
                                acct._DispTotalDue = acct.TotalDue.ToCurrencyString();
                                acct = PayoffAccount.calculateAccount(acct, cbxNextMonth.IsChecked.Value);
                            }
                            break;

                        case "tbxMonthly":
                            if (boxText.ToCurrency() >= 25)
                            {
                                acct.PayAmmount = boxText.ToCurrency();
                                acct._DispPayAmmnt = acct.PayAmmount.ToCurrencyString();
                                acct = PayoffAccount.calculateAccount(acct, cbxNextMonth.IsChecked.Value);
                                //acct.PayoffList = acct.TotalDue == 0 ? null : CalculatePayments(acct.TotalDue, acct.PayAmmount, cbxNextMonth.IsChecked.Value);
                            }
                            break;

                        case "tbxAcctName":
                            if ((sender as TextBox).Text.Length > 0)
                            {
                                acct.AccountName = boxText;
                            }
                            break;

                        case "tbxAPR":
                            if (boxText.ToCurrency() >= 0)
                            {
                                acct.APR = boxText.ToCurrency();
                                acct = PayoffAccount.calculateAccount(acct, cbxNextMonth.IsChecked.Value);
                            }
                            break;
                    }   

                    acctList[acctList.FindIndex(a => a.AccountName == acct.AccountName && a.TotalDue == acct.TotalDue)] = acct;
                    lvAcct.ItemsSource = null;
                    lvAcct.ItemsSource = acctList;
                    if (autoSave == true)
                    {
                        SavePayOff();
                    }
                }
                
            }
        }

        internal void updateList()
        {
            if (acctList.Count != 0)
            {
                foreach (var item in acctList)
                {
                    PayoffAccount.calculateAccount(item, cbxNextMonth.IsChecked.Value);
                    //item.PayoffList = CalculatePayments(item.TotalDue, item.PayAmmount, cbxNextMonth.IsChecked.Value);
                }

                lvAcct.ItemsSource = null;
                lvAcct.ItemsSource = acctList;
                SavePayOff();
            }
        }

        internal void lostFocusReset(object sender, RoutedEventArgs e)
        {

            PayoffAccount acct = ((sender as TextBox).DataContext as PayoffAccount);
            if (acct != null)
            {
                switch ((sender as TextBox).Name)
                {
                    case "tbxStart":
                        (sender as TextBox).Text = acct._DispTotalDue;
                        break;

                    case "tbxMonthly":
                        (sender as TextBox).Text = acct._DispPayAmmnt;
                        break;

                    case "tbxAcctName":
                        (sender as TextBox).Text = acct.AccountName;
                        break;
                }

            }

        }

        private void tgglAutoSave_Toggled(object sender, RoutedEventArgs e)
        {
            autoSave = tgglAutoSave.IsOn;
            ConfigurationManager.SetLocalSetting("PayoffCalcAutoSave", tgglAutoSave.IsOn.ToString());
        }

        private void cbxNextMonth_Checked(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.SetLocalSetting("PayoffCalcNextMo", cbxNextMonth.IsChecked.Value.ToString());
            updateList();
        }

        private async void SavePayOff()
        {
            try
            {
                string JS = JsonConvert.SerializeObject(acctList, Formatting.Indented);
                StorageFile expfile = await ApplicationData.Current.LocalFolder.CreateFileAsync("debtList.JSON", CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(expfile, JS);
            }
            catch (Exception)
            {

            }

        }

        private async void getSavedPayoff()
        {
            try
            {
                StorageFile impfile = await ApplicationData.Current.LocalFolder.GetFileAsync("debtList.JSON");
                string impJSON = await Windows.Storage.FileIO.ReadTextAsync(impfile);
                List<PayoffAccount> impList = JsonConvert.DeserializeObject<List<PayoffAccount>>(impJSON);
                acctList = impList;
                lvAcct.ItemsSource = acctList;
            }
            catch (Exception)
            {
                lvAcct.ItemsSource = null;
            }
        }

        private void bttnDel_Click(object sender, RoutedEventArgs e)
        {
            PayoffAccount delAcct = (PayoffAccount)(sender as AppBarButton).DataContext;
            acctList.RemoveAt(acctList.FindIndex(a => a.AccountName == delAcct.AccountName && a.TotalDue == delAcct.TotalDue));
            updateList();

        }
    }
}
