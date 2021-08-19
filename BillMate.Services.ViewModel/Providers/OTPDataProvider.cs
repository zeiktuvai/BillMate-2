using BillMate.Services.Data.Repository;
using BillMate.Services.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillMate.Services.ViewModel.Providers
{
    public class OTPDataProvider
    {
        public SQLiteDatabaseOperation<OneTime> OTPProvider { get; set; }

        public OTPDataProvider()
        {
            OTPProvider = new SQLiteDatabaseOperation<OneTime>();
        }
        public List<OneTime> GetOTPTable()
        {
            return FormatOTPList(OTPProvider.GetTable(new OneTime()).ToList());
        }
        public void CreateOTP(OneTime oneTime)
        {
            OTPProvider.CreateItem(oneTime);
        }
        public void UpdateOTP(OneTime oneTime)
        {
            OTPProvider.UpdateItem(oneTime);
        }
        public void DeleteOTP(OneTime oneTime)
        {
            OTPProvider.DeleteItem(oneTime);
        }

        internal List<OneTime> FormatOTPList(List<OneTime> otpList)
        {
            var nowMo = DateTime.Today.Month;
            var nowYr = DateTime.Today.Year;

            foreach (OneTime item in otpList)
            {
                DateTime itemDate = DateTime.Parse(item.DueDate);

                if (nowMo > itemDate.Month && nowYr >= itemDate.Year)
                {
                    item._isPastDue = true;
                    item._rectColor = "White";
                    item._pastVis = "Visible";
                }
                else
                {
                    item._isPastDue = false;
                    item._rectColor = "White";
                    item._pastVis = "Collapsed";
                }

                if (item.Paid == 1)
                {
                    item._isPastDue = false;
                    item._rectColor = "LightGreen";
                    item._pastVis = "Collapsed";
                }

                item._dueDate = itemDate.ToFormattedString();

                item._formattedPayAmount = item.PayAmount.ToCurrencyString();
            }
            return otpList;
        }
    }
}
