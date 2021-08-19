using BillMate.Services.Data.Repository;
using BillMate.Services.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillMate.Services.ViewModel.Providers
{
    public class TransactionDataProvider
    {
        internal SQLiteDatabaseOperation<Trans> TransProvider { get; set; }

        public TransactionDataProvider()
        {
            TransProvider = new SQLiteDatabaseOperation<Trans>();
        }

        public List<Trans> getTransTable()
        {
            List<Trans> TransList = TransProvider.GetTable(new Trans()).ToList();
            foreach (Trans item in TransList)
            {
                item._entDate = item.entryDate.ToFormattedString();
            }
            return TransList;
        }

        public void CreateTransaction(Trans trans)
        {
            TransProvider.CreateItem(trans);
        }
    }
}
