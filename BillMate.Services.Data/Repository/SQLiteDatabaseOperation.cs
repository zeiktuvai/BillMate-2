using BillMate.Services.Data.Factory;
using BillMate.Services.Data.Interfaces;
using SQLite.Net;
using SQLite.Net.Interop;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace BillMate.Services.Data.Repository
{
    public class SQLiteDatabaseOperation<T> : IDatabaseOperation<T> where T : class
    {
        // Constructor creates a new sqlite connection object using the db path
        public static readonly string Path = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "bills.db3");
        internal string DBPath { get; set; }
        //public DataConnectionFactory SpecialCon { get; set; }

        public SQLiteDatabaseOperation()
        {
            DBPath = Path;
            //DBPath = (string.IsNullOrEmpty(OpenPath)) ? DBPath = Path : OpenPath;
            //DataConnectionFactory.InitiateSQLiteConncetion(DBPath);
        }
        public SQLiteDatabaseOperation(string OpenPath)
        {
            DBPath = OpenPath;
        }

        //public SQLiteDatabaseOperation(bool SpecialInstance, string DBPath = "")
        //{
        //    DBPath = (string.IsNullOrEmpty(DBPath)) ? DBPath = Path : DBPath;
        //    SpecialCon = new DataConnectionFactory("SQLite", DBPath);
        //    //SpecialCon = DataConnectionFactory.InitiateSpecialSQLiteConnection(DBPath);            
        //}

        /// <summary>
        /// Get a table from the DB as a list of objects
        /// </summary>
        /// <param name="TableType"></param>
        /// <returns>An IEnumerable of T, or null if the table is not found.</returns>
        public IEnumerable<T> GetTable(T TableType)
        {            
            try
            {
                using (var db = new DataConnectionFactory("SQLite", DBPath))
                {
                    return (from p in db.SQLiteConn.Table<T>() select p).ToList();
                }
                //return (from p in DataConnectionFactory.SQLiteCon.Table<T>() select p).ToList();
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public void CreateItem(T ItemToCreate)
        {
            using(var db = new DataConnectionFactory("SQLite", DBPath))
            {
                db.SQLiteConn.Insert(ItemToCreate);
            }
            //DataConnectionFactory.SQLiteCon.Insert(ItemToCreate);
        }
        public void DeleteItem(T ItemToDelete)
        {
            using(var db = new DataConnectionFactory("SQLite", DBPath))
            {
                db.SQLiteConn.Delete(ItemToDelete);
            }
            //DataConnectionFactory.SQLiteCon.Delete(ItemToDelete);
        }
        public void UpdateItem(T ItemToUpdate)
        {
            using (var db = new DataConnectionFactory("SQLite", DBPath))
            {
                db.SQLiteConn.Update(ItemToUpdate);
            }
            //DataConnectionFactory.SQLiteCon.Update(ItemToUpdate);
        }
        public int ExecuteCmd(string cmd)
        {
            using (var db = new DataConnectionFactory("SQLite", DBPath))
            {
                return db.SQLiteConn.Execute(cmd);
            }
                //return DataConnectionFactory.SQLiteCon.Execute(cmd);
        }
        public void ExecuteCmd(List<string> cmds)
        {
            var ExCount = 0;
            var Ex = new System.Exception();

            using (var db = new DataConnectionFactory("SQLite", DBPath))
            {
                foreach (string cmd in cmds)
                {
                    try
                    {
                        db.SQLiteConn.Execute(cmd);
                    }
                    catch (System.Exception e)
                    {
                        ExCount++;
                        Ex = e;
                    }
                }
            }

            if (ExCount > 0)
            {
                throw new DataException(Ex.Message);
            }
        }
    }
}
