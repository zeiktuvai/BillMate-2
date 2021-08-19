using System;
using System.Net.NetworkInformation;
using Windows.Storage;
using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.Data.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Services.OneDrive;
using System.IO;

namespace BillMate.Services.ViewModel.Providers
{
    public class OneDriveProvider
    {
        public static async Task<bool> connectODS()
        {
            if (ConfigurationManager.CheckLocalSetting("MSLogin"))
            {
                if (ConfigurationManager.GetLocalSetting("MSLogin").ToString() == "True")
                {
                    try
                    {
                        OneDriveServiceOperation ODO = new OneDriveServiceOperation();
                        await ODO.LogIn();
                        ConfigurationManager.SetLocalSetting("MSLogin", "True");
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static async Task<bool> disconnectODS()
        {
            if (ConfigurationManager.GetLocalSetting("MSLogin").ToString() == "True")
            {
                try
                {
                    OneDriveServiceOperation ODO = new OneDriveServiceOperation();
                    await ODO.LogOut();
                    ConfigurationManager.SetLocalSetting("MSLogin", "False");
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<Microsoft.Graph.User> getODSuser()
        {
            OneDriveServiceOperation ODO = new OneDriveServiceOperation();
            var user = await ODO.GetUser();

            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                ConfigurationManager.SetLocalSetting("UserName", user.DisplayName);
                ConfigurationManager.SetLocalSetting("UserEmail", user.UserPrincipalName);
            }
            return user;
        }
        public async Task<string> getDBdate()
        {
            OneDriveServiceOperation ODO = new OneDriveServiceOperation();
            return await ODO.GetUploadedFileDate();           
        }

        public async Task<bool> UploadToOneDrive()
        {
            StorageFile expfile = await ApplicationData.Current.LocalFolder.GetFileAsync("bills.db3");
            OneDriveServiceOperation ODO = new OneDriveServiceOperation();
            try
            {
                await ODO.UploadToAppFolder(expfile);
                return true;
            }
            catch (IOException ex)
            {                
                throw new IOException(ex.Message);
            }
        }

        public async Task<StorageFile> GetFromOneDrive()
        {
            OneDriveServiceOperation ODO = new OneDriveServiceOperation();
            return await ODO.GetFromAppFolder();
        }       
    }
}


    

