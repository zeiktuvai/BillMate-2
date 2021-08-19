using Microsoft.Toolkit.Services.OneDrive;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Toolkit.Services.Services.MicrosoftGraph;
using System.IO;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BillMate.Services.Data.Repository
{
    public class OneDriveServiceOperation
    {
        //TODO: Update to new service.
        public OneDriveServiceOperation()
        {
            var scopes = new string[] { MicrosoftGraphScope.FilesReadWriteAppFolder, MicrosoftGraphScope.UserReadAll };
            string appClientId = "bdb9dc24-4eff-4c9f-bc58-4c036768c106";
            OneDriveService.Instance.Initialize(appClientId, scopes, null, null);
        }

        public async Task LogIn()
        {
            await OneDriveService.Instance.LoginAsync();
        }
        public async Task LogOut()
        {
            await OneDriveService.Instance.LogoutAsync();
        }
        public async Task<Microsoft.Graph.User> GetUser()
        {
            if (OneDriveService.Instance.Provider.IsAuthenticated == true)
            {
                var userFields = new Microsoft.Toolkit.Services.MicrosoftGraph.MicrosoftGraphUserFields[] { Microsoft.Toolkit.Services.MicrosoftGraph.MicrosoftGraphUserFields.DisplayName };
                Microsoft.Graph.User user = await OneDriveService.Instance.Provider.User.GetProfileAsync(userFields);
                return user;
            }
            else
            {
                return new Microsoft.Graph.User();
            }
        }
        public async Task UploadToAppFolder(StorageFile UpFile)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var folder = await OneDriveService.Instance.AppRootFolderAsync();

                try
                {
                    using (var localStream = await UpFile.OpenReadAsync())
                    {
                        var fileCreated = await folder.StorageFolderPlatformService.CreateFileAsync(UpFile.Name, CreationCollisionOption.ReplaceExisting, localStream);
                    }

                }
                catch (Exception e)
                {
                    throw new IOException(e.Message);
                }
            }
            else
            {
                throw new IOException("Network connection not available.");
            }

        }
        public async Task<StorageFile> GetFromAppFolder()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var folder = await OneDriveService.Instance.AppRootFolderAsync();
                var ODSItems = await folder.GetFileAsync("bills.db3");

                if (ODSItems != null)
                {
                    OneDriveStorageFile odsFile = ODSItems;
                    var localFolder = ApplicationData.Current.LocalFolder;
                    var mylocalfile = await localFolder.CreateFileAsync("ods.db3", CreationCollisionOption.ReplaceExisting);

                    using (var remoteStream = (await odsFile.StorageFilePlatformService.OpenAsync()) as IRandomAccessStream)
                    {
                        byte[] buffer = new byte[remoteStream.Size];
                        var localBuffer = await remoteStream.ReadAsync(buffer.AsBuffer(), (uint)remoteStream.Size, InputStreamOptions.ReadAhead);

                        using (var localStream = await mylocalfile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await localStream.WriteAsync(localBuffer);
                            await localStream.FlushAsync();
                        }
                    }
                    return mylocalfile;
                }                
            }
            return null;
        }
            public async Task<string> GetUploadedFileDate()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var folder = await OneDriveService.Instance.AppRootFolderAsync();
                var ODSItems = await folder.GetFileAsync("bills.db3");
                if (ODSItems != null)
                {
                    var bdate = ODSItems.DateModified.ToString();
                    return bdate.Substring(0, bdate.IndexOf(' '));
                }
                else
                {
                    return "Last Backup: None";
                }

            }
            else
            {
                return "Last Backup: Unable to retrieve - Offline";
            }
        }

    }
}
