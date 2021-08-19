using BillMate.DataAccess.Interfaces;
using SQLite.Net;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillMate.DataAccess.Models
{
    public class DatabaseOperations : IDatabaseOperation
    {
        public string dbPath { get; } = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "bills.db3");
        public SQLiteConnection con { get; set; }

        public void openCon()
        {
            con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), dbPath);
        }
        public void closeCon()
        {
            con.Close();
        }

        public void createItem()
        {
            throw new NotImplementedException();
        }

        public void deleteItem()
        {
            throw new NotImplementedException();
        }


        public void updateItem()
        {
            throw new NotImplementedException();
        }
    }
}
