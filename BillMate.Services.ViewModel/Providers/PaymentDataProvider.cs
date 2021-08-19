using BillMate.Services.Data.Repository;
using BillMate.Services.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillMate.Services.ViewModel.Providers
{
    public class PaymentDataProvider
    {
        internal SQLiteDatabaseOperation<Payment> PaymentProvider { get; set; }
        internal List<Bill> bills { get; set; }
        public PaymentDataProvider()
        {
            PaymentProvider = new SQLiteDatabaseOperation<Payment>();
            SQLiteDatabaseOperation<Bill> BillSQL = new SQLiteDatabaseOperation<Bill>();
            bills = BillSQL.GetTable(new Bill()).ToList();
            BillSQL = null;
        }

        public List<Payment> GetPaymentData()
        {
            List<Payment> formatList = new List<Payment>();


            foreach (Payment paym in PaymentProvider.GetTable(new Payment()).ToList())
            {
                var formatPay = new Payment();

                formatPay.paymentID = paym.paymentID;
                formatPay.BillID = paym.BillID;
                if (paym.dispPayDate.Length <= 10)
                {
                    formatPay.dispPayDate = paym.dispPayDate;
                }
                else
                {
                    formatPay.dispPayDate = paym.dispPayDate.Substring(0, paym.dispPayDate.IndexOf(' '));
                }
                formatPay.amount = paym.amount;
                formatPay.posted = paym.posted;
                formatPay._bgColor = paym.posted ? "DarkGreen" : "LightGray";

                try
                {
                    formatPay.billName = bills.Find(b => b.BillID == formatPay.BillID).Name;
                    formatPay.payDate = DateTime.Parse(formatPay.dispPayDate);
                    formatPay._ledgerBalanceColor = "";
                    formatPay._ledgerLinePosted = "";
                    formatPay.ledgerBalance = 0;
                    formatPay._ledgerBalance = "";
                    formatPay._displayDate = DateTime.Parse(paym.dispPayDate).ToString("MM/dd/yyyy");
                    formatPay._displayAmount = paym.amount.ToCurrencyString();

                    formatList.Add(formatPay);
                }
                catch (Exception)
                {

                }
            }

            return formatList;
        }
        public void CreatePayment(Payment payment)
        {
            PaymentProvider.CreateItem(payment);
        }
        public void DeletePayment(Payment payment)
        {
            PaymentProvider.DeleteItem(payment);
        }
        public void UpdatePayment(Payment payment)
        {
            PaymentProvider.UpdateItem(payment);
        }
    }
}
