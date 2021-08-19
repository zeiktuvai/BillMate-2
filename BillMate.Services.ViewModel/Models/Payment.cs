using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace BillMate.Services.ViewModel.Models
{
    public class Payment
    {
        [PrimaryKey, AutoIncrement]
        public int paymentID { get; set; }
        public int BillID { get; set; }
        public string dispPayDate { get; set; }
        public double amount { get; set; }
        public bool posted { get; set; }
        [Ignore]
        public string billName { get; set; }
        [Ignore]
        public DateTime payDate { get; set; }
        [Ignore]
        public string _ledgerBalanceColor { get; set; }
        [Ignore]
        public double ledgerBalance { get; set; }
        [Ignore]
        public string _ledgerBalance { get; set; }
        [Ignore]
        public string _displayDate { get; set; }
        [Ignore]
        public string _ledgerLinePosted { get; set; }
        [Ignore]
        public string _displayAmount { get; set; }
        [Ignore]
        public string _bgColor { get; set; }

        //
        // Summary: Take a list of payment objects and check if the date is longer than
        // 10 characters and if so take a substring of anything before a space.
        //
        // Reason: Due to my old bad code, there was a time zone introduced that was recorded
        // into the database and would mess with dates and times.  This stripps that out.
        public static List<Payment> DateConversion(List<Payment> payments)
        {
            foreach (Payment paym in payments)
            {
                if (paym.dispPayDate.Length > 10)
                {
                    paym.dispPayDate = paym.dispPayDate.Substring(0, paym.dispPayDate.IndexOf(' '));
                }
            }
            return payments;
        }
    }
}
