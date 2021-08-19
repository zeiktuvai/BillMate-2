using SQLite.Net.Attributes;
using System;

namespace BillMate.Services.ViewModel.Models
{
    public class Account
    {
        [PrimaryKey, AutoIncrement]
        public int accountID { get; set; }
        public string acctName { get; set; }
        public double acctBal { get; set; }
        public double startBal { get; set; }
        public double creditLimit { get; set; }
        public DateTime startDate { get; set; }
        public double mnthPayAmnt { get; set; }
        public double APR { get; set; }
        public AcctType acctType { get; set; }
        public int billID { get; set; }
        public int closed { get; set; }
        [Ignore]
        public string estPayoff { get; set; }
        [Ignore]
        public string dispStartDate { get; set; }
        [Ignore]
        public string dispBal { get; set; }
        [Ignore]
        public string dispMonthly { get; set; }
        [Ignore]
        public string dispColor { get; set; }
        [Ignore]
        public int _currPercentage { get; set; }
        [Ignore]
        public string _Limit { get; set; }
        [Ignore]
        public string _radialColor { get; set; }
    }
}
