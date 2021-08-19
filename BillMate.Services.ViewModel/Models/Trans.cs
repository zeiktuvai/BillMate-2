using SQLite.Net.Attributes;
using System;

namespace BillMate.Services.ViewModel.Models
{
    public class Trans
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public double entryAmount { get; set; }
        public double intrestAmount { get; set; }
        public DateTime entryDate { get; set; }
        public string Desc { get; set; }
        public bool isInterest { get; set; }
        public int accountID { get; set; }
        public int paymentID { get; set; }
        [Ignore]
        public string _entDate { get; set; }
    }
}
