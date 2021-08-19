using Bill_Tracker.Pages;
using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.ViewModel.Models;
using BillMate.Services.ViewModel.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        internal List<Payment> PaymentList { get; set; }
        internal List<Bill> BillList { get; set; }
        public Home()
        {
            this.InitializeComponent();

            BillDataProvider BP = new BillDataProvider();
            PaymentDataProvider PP = new PaymentDataProvider();
            GoalDataProvider GP = new GoalDataProvider();
            BillList = BP.GetBillData();
            PaymentList = PP.GetPaymentData();            
            double totPaid = BillList.FindAll(b => b.isPaid == true).Sum(s => s.Amount);
            double totRem = BillList.FindAll(b => b.isPaid == false).Sum(s => s.baseAmount);
            double total = BillList.Sum(t => t.baseAmount);           
            int paidRadial = (int)((totPaid / total) * 100);
            
            List<Payment> billPaymens = PaymentList.TakeLast(6).ToList();
            List<Payment> paidBills = new List<Payment>();
            
            radBillStat.Value = paidRadial;

            var pastDue = BillList.FindAll(b => b.isPastDue == true).Count;
            var totBill = BillList.FindAll(d => d.dueDate.Month == DateTime.Now.Month && d.dueDate.Year == DateTime.Now.Year).Count;
            tbxTotBill.Text = totBill.ToString();
            tbxPaidBill.Text = BillList.FindAll(b => b.isPaid == true).Count.ToString();
            tbxPastDue.Text = BillList.FindAll(b => b.isPastDue == true).Count.ToString();
            tbxPostBill.Text = PaymentList.FindAll(p => (DateTime.Parse(p.dispPayDate).Month == DateTime.Now.Month)
                               && (DateTime.Parse(p.dispPayDate).Year == DateTime.Now.Year)).Where(po => po.posted == true).Count().ToString();
            tbxTotPost.Text = PaymentList.FindAll(p => (DateTime.Parse(p.dispPayDate).Month == DateTime.Now.Month)
                               && (DateTime.Parse(p.dispPayDate).Year == DateTime.Now.Year)).Where(po => po.posted == true)
                               .Sum(s => s.amount).ToCurrencyString();
            tbxTotPaid.Text = BillList.FindAll(b => b.isPaid == true).Sum(s => s.Amount).ToCurrencyString();
            tbxTotRem.Text = BillList.FindAll(b => b.isPaid == false).Sum(s => s.baseAmount).ToCurrencyString();
            tbxTotRemNum.Text = BillList.FindAll(b => b.isPaid == false).Count.ToString();

            //code for menu notification of late bills
            MainPage mainPage = (Window.Current.Content as Frame).Content as MainPage;
            mainPage.menNumLateBill = tbxPastDue.Text;
          
            goalGv.ItemsSource =  GP.GetGoalTable().Where(G => G.isComplete == false).OrderByDescending(O => O.goalDate).ToList();

            int PaymentCount = 0;
            foreach (Payment item in billPaymens)
            {
                if (PaymentCount <=2)
                {
                    if (item.payDate.Month == DateTime.Now.Month)
                    {
                        var bill = BillList.Find(b => b.BillID == item.BillID);
                        if (bill != null)
                        {
                            Payment nPay = new Payment();
                            nPay.billName = bill.Name;
                            nPay.dispPayDate = item.dispPayDate.ToCurrencyString();
                            nPay.amount = item.amount;
                            paidBills.Add(nPay);
                            PaymentCount++;
                        }
                    }
                }
            }
            pastBill.ItemsSource = paidBills;
            calculateHigh3();
            lblDate.Text = "Today is " + DateTime.Now.ToString("dd MMMM yyyy");
            getUser();            
        }

        private void calculateHigh3()
        {
            double highest;
            DateTime m1 = DateTime.Now;
            DateTime m2 = DateTime.Now.AddMonths(-1);
            DateTime m3 = DateTime.Now.AddMonths(-2);
            double month1;
            double month2;
            double month3;
            var m1totalpay = PaymentList.FindAll(p => (DateTime.Parse(p.dispPayDate).Month == m1.Month) && (DateTime.Parse(p.dispPayDate).Year == m1.Year)).ToList();
            var m2totalpay = PaymentList.FindAll(p2 => (DateTime.Parse(p2.dispPayDate).Month == m2.Month) && (DateTime.Parse(p2.dispPayDate).Year == m2.Year)).ToList();
            var m3totalpay = PaymentList.FindAll(p3 => (DateTime.Parse(p3.dispPayDate).Month == m3.Month) && (DateTime.Parse(p3.dispPayDate).Year == m3.Year)).ToList();
            month1 = m1totalpay.Sum(t => t.amount);
            month2 = m2totalpay.Sum(t2 => t2.amount);
            month3 = m3totalpay.Sum(t3 => t3.amount);

            if (month1 > month2 && month1 > month3)
            {
                highest = month1;
            } else if (month2 > month1 && month2 > month3)
            {
                highest = month2;
            }else
            {
                highest = month3;
            }

            progM1.Maximum = highest + 400;
            progM2.Maximum = highest + 400;
            progM3.Maximum = highest + 400;
            progM1.Value = month1;
            progM2.Value = month2;
            progM3.Value = month3;
            m3Name.Text = m3.ToString("MMM");
            m2Name.Text = m2.ToString("MMM");
            m1Name.Text = m1.ToString("MMM");
            m3Amm.Text = month3.ToCurrencyString();
            m2Amm.Text = month2.ToCurrencyString();
            m1Amm.Text = month1.ToCurrencyString();
        }


        private void gvi_bills_tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Frame;
            frame.Navigate(typeof(billList));
        }

        private void goalGv_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Frame;
            frame.Navigate(typeof(Goals));
        }

        private async void getUser()
        {
            if (ConfigurationManager.GetLocalSetting("MSLogin") != null)
            {
                OneDriveProvider ODP = new OneDriveProvider();
                var user = await ODP.getODSuser();
                profName.Text = "Welcome " + ConfigurationManager.GetLocalSetting("UserName");

                IReadOnlyList<User> users = await User.FindAllAsync();
                IRandomAccessStreamReference streamRef = await users[0].GetPictureAsync(UserPictureSize.Size424x424);
                BitmapImage bmp = new BitmapImage();

                if (streamRef != null)
                {
                    IRandomAccessStream stream = await streamRef.OpenReadAsync();
                    bmp.SetSource(stream);                    
                    canvProf.ProfilePicture = bmp;                    
                }

            }
        }

        private void DashList_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            
        }
    }
}
