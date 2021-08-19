using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using System.Diagnostics;
using Windows.Security.Authentication.Web.Provider;

namespace BillMate.Services.ViewModel.Models
{
    public class Bill
    {
        [PrimaryKey, AutoIncrement]
        public int BillID { get; set; }
        public string Name { get; set; }
        public string dispDueDate { get; set; }
        public string Frequency { get; set; }
        public double baseAmount { get; set; }
        public string Category { get; set; }
        public string Reminder { get; set; }
        [Ignore]
        public DateTime dueDate { get; set; }
        [Ignore]
        public double Amount { get; set; }
        [Ignore]
        public bool isPaid { get; set; }
        [Ignore]
        public bool isPastDue { get; set; }
        [Ignore]
        public bool isPay { get; set; }
        [Ignore]
        public string catDue { get; set; }
        [Ignore]
        public string _stringDispDate { get; set; }
        [Ignore]
        public string _pastDueColor { get; set; }
        [Ignore]
        public string _pastDueThickness { get; set; }
        [Ignore]
        public string _RectColor { get; set; }
        [Ignore]
        public string _RectVisi { get; set; }
        [Ignore]
        public string _PastDVisi { get; set; }
        [Ignore]
        public int _pOfBudget { get; set; }
        [Ignore]
        public bool _isPosted { get; set; }
        [Ignore]
        public string _DispAmount { get; set; }
    }
}
