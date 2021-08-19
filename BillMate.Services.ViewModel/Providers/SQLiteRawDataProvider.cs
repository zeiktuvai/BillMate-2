using BillMate.Services.Data.Factory;
using BillMate.Services.Data.Repository;
using BillMate.Services.ViewModel.Models;
using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace BillMate.Services.ViewModel.Providers
{
    public class SQLiteRawDataProvider
    {
        public async Task CreateSQLiteDB()
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = (StorageFile) await folder.TryGetItemAsync("bills.db3");

            if (file != null)
            {
                //GC.Collect();
                //GC.WaitForPendingFinalizers();                

                if (file != null)
                {
                    try
                    {
                        await file.DeleteAsync();
                    }
                    catch (Exception)
                    {
                        throw new FileLoadException("An error occurred removing the old Database File.");
                    }   
                }
            }

            file = await folder.CreateFileAsync("bills.db3", CreationCollisionOption.ReplaceExisting);
                        
            string VersionSQL = "CREATE TABLE [version] ([verID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [ver] text NOT NULL); " +
                                "CREATE UNIQUE INDEX[version_sqlite_autoindex_version_1] ON[version]([verID] ASC);";
            string BillSQL = "CREATE TABLE [Bill] ([BillID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [Name] text NOT NULL, [dispDueDate] text NOT NULL, " +
                             "[Frequency] text NOT NULL, [baseAmount] real NOT NULL, [Category] text NULL, [Reminder] text NULL); " +
                             "CREATE UNIQUE INDEX[Bill_sqlite_autoindex_Bill_1] ON[Bill]([BillID] ASC);";
            string PaymentSQL = "CREATE TABLE [Payment] ([paymentID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [dispPayDate] text NOT NULL, [amount] real NOT NULL" +
                                ", [posted] text NOT NULL, [BillID] bigint NOT NULL, CONSTRAINT[FK_Payment_0_0] FOREIGN KEY([BillID]) REFERENCES[Bill]([BillID]) ON DELETE NO ACTION ON UPDATE NO ACTION); " +
                                "CREATE UNIQUE INDEX[Payment_sqlite_autoindex_Payment_1] ON[Payment]([paymentID] ASC);";
            string GoalSQL = "CREATE TABLE [Goal] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [goalName] text NOT NULL, [goalAmount] real NOT NULL" +
                          ", [currentAmount] real NOT NULL, [goalDate] text NOT NULL, [isComplete] text NOT NULL); " +
                          "CREATE UNIQUE INDEX[Goal_sqlite_autoindex_Goal_1] ON[Goal]([ID] ASC);";
            string OTPSQL = "CREATE TABLE [oneTime] ([otpID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [Name] text NOT NULL, [PayAmount] real NOT NULL" +
                            ", [DueDate] text NOT NULL, [Paid] bigint NOT NULL, [Type] text NOT NULL); " +
                            "CREATE UNIQUE INDEX[oneTime_sqlite_autoindex_oneTime_1] ON[oneTime]([otpID] ASC);";

            List<string> Commands = new List<string>();
            Commands.Add(VersionSQL);
            Commands.Add("INSERT INTO 'version' ('ver') VALUES ('1.5');");
            Commands.Add(BillSQL);
            Commands.Add(PaymentSQL);
            Commands.Add(GoalSQL);
            Commands.Add(OTPSQL);            

            SQLiteDatabaseOperation<object> DB = new SQLiteDatabaseOperation<object>();

            DB.ExecuteCmd(Commands);
            
        }

        public async Task ImportSQLiteDB(StorageFile file)
        {
            try
            {   
                //open connection to file                       
                await file.CopyAsync(ApplicationData.Current.LocalFolder, "tempdb.db3", NameCollisionOption.ReplaceExisting);
                string TempDBPath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "tempdb.db3");

                //get db version
                List<version> tempVer = new SQLiteDatabaseOperation<version>(TempDBPath).GetTable(new version()).ToList();               

                //import each table as list
                List<Bill> tempBills = new SQLiteDatabaseOperation<Bill>(TempDBPath).GetTable(new Bill()).ToList();
                List<Payment> tempPay = new SQLiteDatabaseOperation<Payment>(TempDBPath).GetTable(new Payment()).ToList();
                List<Goal> tempGoal = new SQLiteDatabaseOperation<Goal>(TempDBPath).GetTable(new Goal()).ToList();
                List<OneTime> tempOTP = null;

                if (tempVer[0].ver == "1.5")
                {
                    tempOTP = new SQLiteDatabaseOperation<OneTime>(TempDBPath).GetTable(new OneTime()).ToList();
                }

                try
                {
                    await CreateSQLiteDB();
                }
                catch (FileLoadException e)
                {
                    throw new FileLoadException(e.InnerException.Message);                    
                }



                //add each item to new db
                SQLiteDatabaseOperation<object> DB = new SQLiteDatabaseOperation<object>();
                List<string> ImportCmd = new List<string>();

                foreach (Bill bill in tempBills)
                    {
                        ImportCmd.Add(string.Format("INSERT INTO 'Bill' VALUES ({0},'{1}','{2}','{3}',{4},'{5}','{6}');", bill.BillID, bill.Name, bill.dispDueDate, bill.Frequency, bill.baseAmount, bill.Category, bill.Reminder));                        
                    }

                    foreach (Payment pymnt in tempPay)
                    {
                        ImportCmd.Add(string.Format("INSERT INTO 'Payment' VALUES ({0},'{1}',{2},'{3}',{4});", pymnt.paymentID, pymnt.dispPayDate, pymnt.amount, pymnt.posted, pymnt.BillID));
                    }

                    foreach (Goal goal in tempGoal)
                    {
                        ImportCmd.Add(string.Format("INSERT INTO 'Goal' VALUES ({0},'{1}',{2},{3},'{4}','{5}');", goal.ID, goal.goalName, goal.goalAmount, goal.currentAmount, goal.goalDate, goal.isComplete));
                    }

                    if (tempOTP != null)
                    {
                        foreach (OneTime otp in tempOTP)
                        {
                            ImportCmd.Add(string.Format("INSERT INTO 'oneTime' VALUES ({0}, '{1}', {2}, '{3}', {4}, '{5}');", otp.otpID, otp.Name, otp.PayAmount, otp.DueDate, otp.Paid, otp.Type));
                        }                    
                    }
                try
                {
                    DB.ExecuteCmd(ImportCmd);
                }
                catch (Exception)
                {
                    throw new DataException("Error importing data into database");
                }
                

                //cleanup 
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                StorageFile tempdb = await ApplicationData.Current.LocalFolder.GetFileAsync("tempdb.db3");

                try
                {
                    await tempdb.DeleteAsync();
                }
                catch (Exception e)
                {
                    throw new FileLoadException("Failed to delete imported database file");
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
