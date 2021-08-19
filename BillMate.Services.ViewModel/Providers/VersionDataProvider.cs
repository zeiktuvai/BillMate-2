using BillMate.Services.Data.Interfaces;
using BillMate.Services.Data.Repository;
using BillMate.Services.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillMate.Services.ViewModel.Providers
{
    public class VersionDataProvider
    {
        internal SQLiteDatabaseOperation<version> VersionProvider { get; set; }
        public VersionDataProvider()
        {
            VersionProvider = new SQLiteDatabaseOperation<version>();
        }
        public IEnumerable<version> GetTable()
        {
            return VersionProvider.GetTable(new version());
        }
        public void UpdateItem(version ItemToUpdate)
        {
            VersionProvider.UpdateItem(ItemToUpdate);
        }
        public void CreateItem(version ItemToCreate)
        {
            throw new NotImplementedException();
        }
        public void DeleteItem(version ItemToDelete)
        {
            throw new NotImplementedException();
        }
        public string GetVersion()
        {
            List<version> version = GetTable().ToList();
            return version[0].ver;
        }

        public void checkTable()
        {

            try
            {
                var temp = VersionProvider.GetTable(new version());
            }
            catch (Exception)
            {
                string cCmd = "CREATE TABLE 'version' ('verID' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'ver' TEXT NOT NULL);";
                VersionProvider.ExecuteCmd(cCmd);
                VersionProvider.ExecuteCmd("INSERT INTO 'version' ('ver') VALUES ('1.2');");

            }

            List<version> vercheck = VersionProvider.GetTable(new version()).ToList();

            if (vercheck[0].ver == "1.2" || vercheck[0].ver == "1.1")
            {
                string cCmd = "CREATE TABLE 'Goal' ('ID' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'goalName' TEXT NOT NULL, 'goalAmount' REAL NOT NULL, 'currentAmount' REAL NOT NULL, 'goalDate' TEXT NOT NULL, 'isComplete' TEXT NOT NULL);";
                VersionProvider.ExecuteCmd(cCmd);
                VersionProvider.ExecuteCmd("UPDATE 'version' SET 'ver' = '1.5';");
                vercheck = VersionProvider.GetTable(new version()).ToList();
            }
            if (vercheck[0].ver == "1.4")
            {
                string cCmd = "CREATE TABLE 'oneTime' ('otpID' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'Name' TEXT NOT NULL, 'PayAmount' REAL NOT NULL, 'DueDate' TEXT NOT NULL, 'Paid' INTEGER NOT NULL, 'Type' TEXT NOT NULL);";
                VersionProvider.ExecuteCmd(cCmd);
                VersionProvider.ExecuteCmd("UPDATE 'version' SET 'ver' = '1.5';");
            }
        }
    }
}
