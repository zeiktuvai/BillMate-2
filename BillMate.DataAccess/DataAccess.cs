using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace BillMate.DataAccess
{
    public static class DataAccess
    {
        internal static string dbLoc = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "bills.db3").ToString();

       


    }


}
