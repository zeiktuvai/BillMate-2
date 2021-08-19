using BillMate.Services.Data.Repository;
using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.ViewModel.Interfaces;
using BillMate.Services.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Payments;

namespace BillMate.Services.ViewModel.Providers
{
    public class BillDataProvider
    {
        internal SQLiteDatabaseOperation<Bill> BillProvider { get; set; }
        internal List<Payment> Payments { get; set; }
        internal List<OneTime> OTPList { get; set; }

        public BillDataProvider()
        {
            BillProvider = new SQLiteDatabaseOperation<Bill>();
            UpdateDataSources();
        }

        public List<Bill> GetBillData(string sortOrder = "srtByDte", int dateShift = 0, bool distinct = false)
        {
            UpdateDataSources();
            //define date variable
            DateTime baseDate = DateTime.Now.AddMonths(dateShift);

            //get data from the databases.            
            List<Bill> bills = BillProvider.GetTable(new Bill()).ToList();

            //Get all payments, then filter them by the year for last and next month, then get the
            //ones for last, this and next month.            
            List<Payment> paylist = Payment.DateConversion(Payments
                     .Where(y => DateTime.Parse(y.dispPayDate).Year >= DateTime.Now.AddMonths(-1).Year &&
                     DateTime.Parse(y.dispPayDate).Year <= DateTime.Now.AddMonths(1).Year)
                     .Where(d => DateTime.Parse(d.dispPayDate).Month >= DateTime.Now.AddMonths(-1).Month &&
                     DateTime.Parse(d.dispPayDate).Month <= DateTime.Now.AddMonths(1).Month).ToList());

            //Get all OTP items and convert them to bills to add to the bills list.
            List<Bill> currentOTP = OTPList.FindAll(d => DateTime.Parse(d.DueDate).Month == baseDate.Date.Month
                        && DateTime.Parse(d.DueDate).Year == baseDate.Date.Year).ConvertOTPtoBill();

            if (currentOTP.Count > 0)
            {
                foreach (var item in currentOTP)
                {
                    bills.Add(item);
                }
            }

            bills = GenerateBillData(bills, baseDate);
            bills = ProcessBillList(bills, paylist, baseDate, dateShift, sortOrder);

            if (distinct == true)
            {
                bills = bills.Distinct(new BillIdComparer()).ToList();
            }
            return bills;
        }

        public List<IGrouping<string, Bill>> SortBillData(List<Bill> billLst, string SortOrder = "srtByDte")
        {
            // Create new list of grouped bills
            List<IGrouping<string, Bill>> gBills = new List<IGrouping<string, Bill>>();

            switch (SortOrder)
            {
                // Group bills by either date or category.
                case "srtByDte":
                    gBills = billLst.OrderBy(o => o.dueDate).GroupBy(g => g._stringDispDate).ToList();
                    break;
                case "srtByCat":
                    gBills = billLst.Where(b => b.Reminder != "Future").OrderBy(o => o.dueDate).GroupBy(g => g.Category).ToList();
                    break;
            }
            return gBills;
        }
        public void UpdateBill(Bill bill)
        {
            BillProvider.UpdateItem(bill);
        }
        public void CreateBill(Bill bill)
        {
            BillProvider.CreateItem(bill);
        }
        public void DeleteBill(Bill bill)
        {
            List<Payment> deletePay = Payments.Where(p => p.BillID == bill.BillID).ToList();
            BillProvider.DeleteItem(bill);
            PaymentDataProvider PP = new PaymentDataProvider();
            foreach (Payment item in deletePay)
            {
                PP.DeletePayment(item);
            }
        }
        internal List<Bill> GenerateBillData(List<Bill> billsList, DateTime baseDate)
        {
            List<Bill> procList = new List<Bill>();

            //generate bill items.
            foreach (Bill item in billsList)
            {
                if (!item.Reminder.Contains("Archived"))
                {
                    switch (item.Frequency)
                    {
                        case "Monthly":
                            Bill procBill = item;
                            DateTime dbDueDate = DateTime.Parse(item.dispDueDate);
                            if (dbDueDate.Day > 28)
                            {
                                if (DateTime.DaysInMonth(baseDate.Year, baseDate.Month) < 31)
                                {
                                    procBill.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, DateTime.DaysInMonth(baseDate.Year, baseDate.Month)).ToString();
                                }
                            }
                            else
                            {
                                procBill.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, dbDueDate.Day).ToString();
                            }
                            procList.Add(procBill);
                            break;

                        case "Bi-Monthly":
                            Bill procBill1 = new Bill()
                            {
                                baseAmount = item.baseAmount,
                                BillID = item.BillID,
                                Category = item.Category,
                                dispDueDate = item.dispDueDate,
                                Frequency = item.Frequency,
                                Name = item.Name,
                                Reminder = item.Reminder
                            };
                            Bill procBill2 = new Bill()
                            {
                                baseAmount = item.baseAmount,
                                BillID = item.BillID,
                                Category = item.Category,
                                dispDueDate = item.dispDueDate,
                                Frequency = item.Frequency,
                                Name = item.Name,
                                Reminder = item.Reminder
                            };
                            var date = item.dispDueDate.Split(',');
                            DateTime due1;
                            DateTime due2;
                            if (date.Count() <= 1)
                            {
                                due1 = DateTime.Parse(date[0]);
                                due2 = DateTime.Parse(date[0]);
                            }
                            else
                            {
                                due1 = DateTime.Parse(date[0]);
                                due2 = DateTime.Parse(date[1]);
                            }

                            if (due1.Day > 28 || due2.Day > 28)
                            {
                                if (DateTime.DaysInMonth(baseDate.Year, baseDate.Month) < 31)
                                {
                                    if (due1.Day > 28)
                                    {
                                        procBill1.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, DateTime.DaysInMonth(baseDate.Year, baseDate.Month)).ToString();
                                    }
                                    else
                                    {
                                        procBill1.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, due1.Day).ToString();
                                    }
                                    if (due2.Day > 28)
                                    {
                                        procBill2.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, DateTime.DaysInMonth(baseDate.Year, baseDate.Month)).ToString();
                                    }
                                    else
                                    {
                                        procBill2.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, due2.Day).ToString();
                                    }
                                }
                                else
                                {
                                    procBill1.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, due1.Day).ToString();
                                    procBill2.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, due2.Day).ToString();
                                }
                            }
                            else
                            {
                                procBill1.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, due1.Day).ToString();
                                procBill2.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, due2.Day).ToString();
                            }
                            procList.Add(procBill1);
                            procList.Add(procBill2);
                            break;

                        case "Weekly":
                            int daysInMont = DateTime.DaysInMonth(baseDate.Year, baseDate.Month);

                            for (int i = 1; i < daysInMont; i++)
                            {
                                DateTime currentDate = new DateTime(baseDate.Year, baseDate.Month, i);
                                if (currentDate.DayOfWeek.ToString() == item.dispDueDate)
                                {
                                    Bill weeklyBill = new Bill()
                                    {
                                        baseAmount = item.baseAmount,
                                        BillID = item.BillID,
                                        Category = item.Category,
                                        dispDueDate = currentDate.Date.ToString(),
                                        Frequency = item.Frequency,
                                        Name = item.Name,
                                        Reminder = item.Reminder
                                    };
                                    procList.Add(weeklyBill);
                                }
                            }
                            break;

                        case "Annual":
                            Bill procBilla = item;
                            var dueDate = DateTime.Parse(procBilla.dispDueDate);
                            procBilla.dispDueDate = new DateTime(DateTime.Now.Year, dueDate.Month, dueDate.Day).ToString();
                            dueDate = DateTime.Parse(procBilla.dispDueDate);

                            if (dueDate.Month == baseDate.Month && dueDate.Year == baseDate.Year)
                            {
                                procList.Add(procBilla);
                            }
                            break;

                        //case "Single Future Payment":
                        //    Bill procBillb = item;
                        //    procList.Add(procBillb);
                        //    break;

                        case "OneTime":
                            Bill procBillOtp = item;
                            DateTime dbDueDateOtp = DateTime.Parse(item.dispDueDate);
                            if (dbDueDateOtp.Day > 28)
                            {
                                if (DateTime.DaysInMonth(baseDate.Year, baseDate.Month) < 31)
                                {
                                    procBillOtp.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, DateTime.DaysInMonth(baseDate.Year, baseDate.Month)).ToString();
                                }
                            }
                            else
                            {
                                procBillOtp.dispDueDate = new DateTime(baseDate.Year, baseDate.Month, dbDueDateOtp.Day).ToString();
                            }
                            procList.Add(procBillOtp);
                            break;
                    }
                }
            }

            return procList;
        }
        internal List<Bill> ProcessBillList(List<Bill> procList, List<Payment> paylist, DateTime baseDate, int dateShift = 0, string sortOrder = "strByDte")
        {
            //control variables
            double budget = 0;

            budget = ConfigurationManager.CheckLocalSetting("billBdgtAmt") ? double.Parse(ConfigurationManager.GetLocalSetting("billBdgtAmt").ToString()) : 1000;

            //Find all payments for each bill
            foreach (Bill item in procList)
            {
                DateTime today = DateTime.Today.Date;

                DateTime dbDueDate = DateTime.Parse(item.dispDueDate);
                List<Payment> listOfPay = paylist.FindAll(p => p.BillID == item.BillID);
                List<Payment> paymentsForSelectedMonth = listOfPay.Where(d => DateTime.Parse(d.dispPayDate).Month == baseDate.Month && DateTime.Parse(d.dispPayDate).Year == baseDate.Year).ToList();

                // Set item pay due date as date time.
                if (item.Frequency == "Annual")
                {
                    item.dueDate = dbDueDate;
                    paymentsForSelectedMonth.AddRange(listOfPay.Where(f => DateTime.Parse(f.dispPayDate).Year == baseDate.Year).ToList());
                }
                else
                {
                    item.dueDate = new DateTime(baseDate.Year, baseDate.Month, dbDueDate.Day);
                }

                //Set formatted text of base amount                
                item._DispAmount = item.baseAmount.ToCurrencyString(); ;

                //set payment status for bill
                foreach (Payment pay in paymentsForSelectedMonth)
                {
                    switch (item.Frequency)
                    {
                        case "Monthly":
                            item.Amount = pay.amount;
                            item.isPaid = true;
                            if (dateShift == -1)
                            {
                                item.dueDate = DateTime.Parse(pay.dispPayDate);
                            }
                            break;

                        case "Bi-Monthly":
                            if (DateTime.Parse(pay.dispPayDate).Day == DateTime.Parse(item.dispDueDate).Day)
                            {
                                item.Amount = pay.amount;
                                item.isPaid = true;
                            }
                            break;

                        case "Weekly":
                            if (DateTime.Parse(pay.dispPayDate).Day == DateTime.Parse(item.dispDueDate).Day)
                            {
                                item.Amount = pay.amount;
                                item.isPaid = true;
                            }
                            break;

                        case "Annual":
                            item.Amount = pay.amount;
                            item.isPaid = true;
                            break;

                            //case "Single Future Payment":
                            //    item.Amount = pay.amount;
                            //    item.isPaid = true;
                            //    break;
                    }

                }

                //if no payment set defaults
                if (item.isPaid != true)
                {
                    if (item.Amount == 0 || item.isPaid == false)
                    {
                        item.isPaid = false;
                        item.Amount = item.baseAmount;
                    }
                }

                // Set the formatted text version of the due date
                item._stringDispDate = item.dueDate.ToString("dddd, dd MMMM yyyy");


                //Past due section
                // If item is paid it cannot be past due
                if (item.isPaid == true)
                {
                    item.isPastDue = false;
                }
                else
                {
                    //if today is greater than due date, its past due.
                    //if (nbill.Frequency != "OneTime")
                    //{
                    if (today > item.dueDate)
                    {
                        item.isPastDue = true;
                    }
                    else
                    {
                        // otherwise item is not past due.
                        item.isPastDue = false;
                    }
                    // }
                }

                // set due date if sorting by category.
                if (sortOrder == "srtByCat")
                {
                    item.catDue = item._stringDispDate;
                }
                else
                {
                    item.catDue = item.Category;
                }

                // Set UI colors for each item
                switch (item.Frequency)
                {
                    case "OneTime":
                        item._pastDueColor = "White";
                        item._RectColor = "Orchid";
                        item._RectVisi = "Visible";
                        item._PastDVisi = "Collapsed";
                        break;
                    case "Annual":
                        item._pastDueColor = "White";
                        item._RectColor = "Brown";
                        item._RectVisi = "Visible";
                        item._PastDVisi = "Collapsed";
                        break;
                    default:
                        item._pastDueColor = "White";
                        item._RectColor = "Blue";
                        item._RectVisi = "Visible";
                        item._PastDVisi = "Collapsed";
                        break;
                }

                // Set UI colors if past due
                if (item.isPastDue == true)
                {
                    if (item.Frequency == "OneTime")
                    {
                        item._pastDueColor = "White";
                        item._RectColor = "Orchid";
                        item._RectVisi = "Visible";
                        item._PastDVisi = "Visible";
                    }
                    else
                    {
                        item._pastDueColor = "White";
                        item._RectColor = "DarkRed";
                        item._RectVisi = "Collapsed";
                        item._PastDVisi = "Visible";
                    }
                }
                // Set UI colors if paid
                else if (item.isPaid == true)
                {
                    item._pastDueColor = "#61BF80";
                    item._RectColor = "DarkGreen";
                    item._RectVisi = "Visible";
                    item._PastDVisi = "Collapsed";
                }

                // Calculate budget usage for each item
                int PercentageOfBudget = (int)(item.baseAmount / budget * 100);
                if (PercentageOfBudget > 100)
                {
                    PercentageOfBudget = 100;
                }
                else if (PercentageOfBudget < 0)
                {
                    PercentageOfBudget = 0;
                }
                item._pOfBudget = PercentageOfBudget;

                // Search all payments and see if annual bill has been posted to set green checkmark
                if (item.isPaid == true)
                {
                    if (item.Frequency != "Annual")
                    {
                        var isanypay = paylist.Find(p => p.BillID == item.BillID && DateTime.Parse(p.dispPayDate).Month == DateTime.Now.AddMonths(dateShift).Month
                                       && DateTime.Parse(p.dispPayDate).Year == DateTime.Now.AddMonths(dateShift).Year);

                        if (isanypay != null)
                        {
                            item._isPosted = isanypay.posted;
                        }
                        else
                        {
                            item._isPosted = false;
                        }
                    }

                }
                else
                {
                    item._isPosted = false;
                }
            }

            return procList;
        }

        internal void UpdateDataSources()
        {
            SQLiteDatabaseOperation<Payment> PaySQL = new SQLiteDatabaseOperation<Payment>();
            Payments = PaySQL.GetTable(new Payment()).ToList();
            PaySQL = null;
            SQLiteDatabaseOperation<OneTime> OTPSQL = new SQLiteDatabaseOperation<OneTime>();
            OTPList = OTPSQL.GetTable(new OneTime()).ToList();
            OTPSQL = null;
        }        
    }
}
