using SQLite.Net.Attributes;

namespace BillMate.Services.ViewModel.Models
{
    public class OneTime
    {
        [PrimaryKey, AutoIncrement]
        public int otpID { get; set; }
        public string Name { get; set; }
        public double PayAmount { get; set; }
        public string DueDate { get; set; }
        public int Paid { get; set; }
        public string Type { get; set; }
        [Ignore]
        public bool _isPastDue { get; set; }
        [Ignore]
        public string _rectColor { get; set; }
        [Ignore]
        public string _rectVis { get; set; }
        [Ignore]
        public string _pastVis { get; set; }
        [Ignore]
        public string _dueDate { get; set; }
        [Ignore]
        public string _formattedPayAmount { get; set; }
    }
}
