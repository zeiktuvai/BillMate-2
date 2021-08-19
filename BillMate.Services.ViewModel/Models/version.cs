using SQLite.Net.Attributes;

namespace BillMate.Services.ViewModel.Models
{
    public class version
    {
        [PrimaryKey, AutoIncrement]
        public int verID { get; set; }
        public string ver { get; set; }

    }
}
