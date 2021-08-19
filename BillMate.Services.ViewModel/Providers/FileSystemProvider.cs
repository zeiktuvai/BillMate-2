using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;


namespace BillMate.Services.ViewModel.Providers
{
    public class FileSystemProvider
    {
        public async void SQLiteSaveToFile(StorageFile file)
        {            
            if (file != null)
            {
                try
                {
                    StorageFile expfile = await ApplicationData.Current.LocalFolder.GetFileAsync("bills.db3");
                    await expfile.CopyAndReplaceAsync(file);                    
                }
                catch (Exception)
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("An error occured exporting the file.", "Error");
                    dialog.Options = Windows.UI.Popups.MessageDialogOptions.None;
                    await dialog.ShowAsync();
                }
            }

        }
        public async Task<StorageFile> TryGetSQLiteDatabase()
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("bills.db3");
            return (StorageFile)item;
        }
    }
}
