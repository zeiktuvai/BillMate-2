using SQLite.Net;
using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillMate.Services.Data.Factory
{
    public class DataConnectionFactory : IDisposable
    {
        //public static SQLiteConnection SQLiteCon { get; set; }
        //public static void InitiateSQLiteConncetion(string DBPath)
        //{
        //    SQLiteCon = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DBPath);            
        //}
        //public static SQLiteConnection InitiateSpecialSQLiteConnection(string DBPath)
        //{
        //    return new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DBPath);            
        //}

        public SQLiteConnection SQLiteConn { get; set; }

        public DataConnectionFactory(string ConType, string DataPath)
        {
            switch (ConType)
            {
                case "SQLite":
                    SQLiteConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DataPath);
                    break;                
            }
        }
        public void Dispose()
        {            
            SQLiteConn.Close();
            SQLiteConn.Dispose();
        }
    }
}
