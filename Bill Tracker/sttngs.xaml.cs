using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.ViewModel.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class sttngs : Page
    {
        internal VersionDataProvider VP { get; set; }
        internal OneDriveProvider ODP { get; set; }
        public sttngs()
        {
            this.InitializeComponent();
            VP = new VersionDataProvider();
            ODP = new OneDriveProvider();
            Package pack = Package.Current;
            PackageId packId = pack.Id;
            PackageVersion pver = packId.Version;

            lblVer.Text = "Version " + string.Format("{0}.{1}.{2}.{3}", pver.Major, pver.Minor, pver.Build, pver.Revision);
            if (ConfigurationManager.GetLocalSetting("loginEnbld").ToString().ToLower() == "true")
            {
                tgglLogin.IsOn = true;
            }
            if (ConfigurationManager.GetLocalSetting("AutoBkup") != null && ConfigurationManager.GetLocalSetting("AutoBkup").ToString() == "True")
            {
                tgglAutoDB.IsOn = true;
            }
            tgglLogin.Toggled += tgglLogin_Toggled;
            lblDBVer.Text = "DB Version " + VP.GetVersion();

                        
            var loginInfo = ConfigurationManager.GetLocalSetting("MSLogin");

            if (loginInfo != null  && loginInfo.ToString()  == "True")
            {                
                bttnLogin.Content = "Sign out";
                bttnSttngExportODS.IsEnabled = true;
                bttnSttngImportODS.IsEnabled = true;
                tgglAutoDB.IsEnabled = true;
                getUser();
                getDBDate();              
            } else
            {
                bttnSttngExportODS.IsEnabled = false;
                bttnSttngImportODS.IsEnabled = false;
                tgglAutoDB.IsEnabled = false;
                bttnLogin.Content = "Log in";
            }
        }

        private async void getDBDate()
        {
            lblLbkup.Text = "Last Backup: " + await ODP.getDBdate();
        }

        internal static string hash(string input)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            return CryptographicBuffer.EncodeToHexString(hashed);

        }

        private async void bttnSttngImprt_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(string.Format("This action will over write the existing data with data from the selected database.  "), "Replace Database?");
            dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var action = await dialog.ShowAsync();
           
            if (action.Id.ToString() == "1")
            {
                StorageFile file = await SelectFile(false, PickerLocationId.Downloads, Tuple.Create("SQLite Database", ".db3"));
                if (file != null)
                {
                    Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 0);
                    SQLiteRawDataProvider RDP = new SQLiteRawDataProvider();
                    try
                    {
                        await RDP.ImportSQLiteDB(file).ConfigureAwait(true);
                        var sdialog = new Windows.UI.Popups.MessageDialog("Import operation completed successfully.", "Import Complete");
                        await sdialog.ShowAsync();
                        Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
                        Frame.Navigate(typeof(billList));
                    }
                    catch (Exception err)
                    {
                        Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
                        var errdialog = new Windows.UI.Popups.MessageDialog("An error occured importing the file. Data may have been lost." + System.Environment.NewLine +
                                                                            err.Message, "Error");
                        await errdialog.ShowAsync();
                    }
                }                
            }
            
        }

        private async void bttnSttngExprt_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await SelectFile(true, PickerLocationId.DocumentsLibrary, Tuple.Create("SQLite Database", ".db3"), "bills.db3");

            if (file != null)
            {
                FileSystemProvider FSP = new FileSystemProvider();
                FSP.SQLiteSaveToFile(file);
            }
        }

        private void tgglLogin_Toggled(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.SetLocalSetting("loginEnbld", tgglLogin.IsOn.ToString());
            if (tgglLogin.IsOn == true)
            {
                setPassword();
            }
        }

        private async void setPassword()
        {
            Dialog.Password dialog = new Dialog.Password();
            await dialog.ShowAsync();
            string pass = dialog.result;
            if (pass != null)
            {
                ConfigurationManager.SetLocalSetting("usrPasswrd", hash(pass));
            }
            else
            {
                tgglLogin.IsOn = false;
            }
        }

        private async void BttnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = ConfigurationManager.GetLocalSetting("MSLogin");
            

            if (login == null || login.ToString() == "False")
            {

                try
                {
                    await OneDriveProvider.connectODS();
                    ConfigurationManager.SetLocalSetting("MSLogin", "True");
                    bttnLogin.Content = "Sign out";
                    this.Frame.Navigate(typeof(sttngs));
                }
                catch (Exception)
                {
                    var dialog = new Windows.UI.Popups.MessageDialog(string.Format("Failed to log in with this account."), "Login Failed");
                    var action = await dialog.ShowAsync();
                }

                
            }
            else if (login.ToString() == "True")
            {
                var dialog = new Windows.UI.Popups.MessageDialog(string.Format("Log out of one drive?  "), "Logout?");
                dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 0;

                var action = await dialog.ShowAsync();

                if (action.Id.ToString() == "1")
                {
                    if (OneDriveProvider.disconnectODS().Result == true)
                    {
                        bttnLogin.Content = "Log in";
                    } else
                    {
                        var dialog2 = new Windows.UI.Popups.MessageDialog(string.Format("Failed to log out of this account."), "Logout Failed?");
                        var action2 = await dialog2.ShowAsync();
                    }
                }
            }
        }

        private async void getUser()
        {
            OneDriveProvider ODP = new OneDriveProvider();
            var userInfo = await ODP.getODSuser();
            lblUser.Text = "Logged in as: " + ConfigurationManager.GetLocalSetting("UserEmail");
            lblUser.Visibility = Visibility.Visible;
        }

        private async void BttnSttngExportODS_Click(object sender, RoutedEventArgs e)
        {
            var okdialog = new Windows.UI.Popups.MessageDialog(string.Format("This action will overwrite the existing data stored in OneDrive."), "Upload to OneDrive?");
            okdialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
            okdialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
            okdialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
            okdialog.DefaultCommandIndex = 0;
            okdialog.CancelCommandIndex = 0;

            var action = await okdialog.ShowAsync();

            if (action.Id.ToString() == "1")
            {

                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 1);
                OneDriveProvider ODP = new OneDriveProvider();

                try
                {
                    ODP.UploadToOneDrive();
                    var dialog = new Windows.UI.Popups.MessageDialog("Upload to OneDrive successfully completed.", "Complete");
                    dialog.Options = Windows.UI.Popups.MessageDialogOptions.None;
                    await dialog.ShowAsync();
                }
                catch(Exception ex)
                {
                    var dialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Error");
                    dialog.Options = Windows.UI.Popups.MessageDialogOptions.None;
                    await dialog.ShowAsync();
                }
                finally
                {
                    Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                    this.Frame.Navigate(typeof(sttngs));
                }

            }
            else
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
            }

        }

        private async void BttnSttngImportODS_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new Windows.UI.Popups.MessageDialog(string.Format("This action will over write the existing data with data stored in OneDrive."), "Replace Database?");
            dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var action = await dialog.ShowAsync();

            if (action.Id.ToString() == "1")
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 1);                
                StorageFile file = await ODP.GetFromOneDrive();

                if (file != null)
                {
                    SQLiteRawDataProvider RDP = new SQLiteRawDataProvider();
                    try
                    {
                        await RDP.ImportSQLiteDB(file).ConfigureAwait(true);
                        var sdialog = new Windows.UI.Popups.MessageDialog("Import operation completed successfully.", "Import Complete");
                        await sdialog.ShowAsync();
                        Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
                        Frame.Navigate(typeof(billList));
                    }
                    catch (Exception err)
                    {
                        Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
                        var errdialog = new Windows.UI.Popups.MessageDialog("An error occured importing the file. Data may have been lost." + System.Environment.NewLine +
                                                                            err.Message, "Error");
                        await errdialog.ShowAsync();
                    }
                }
            }     
        }

        private void TgglAutoDB_Toggled(object sender, RoutedEventArgs e)
        {
            if (tgglAutoDB.IsOn)
            {
                ConfigurationManager.SetLocalSetting("AutoBkup", "True");

            } else
            {
                ConfigurationManager.SetLocalSetting("AutoBkup", "False");
            }
        }

        internal async Task<StorageFile> SelectFile(bool isSave, PickerLocationId pickerLocationId, Tuple<string, string> FileTypeChoice, string SuggestedFileName = "")
        {
            if (isSave == false)
            {
                var picker = new FileOpenPicker();
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = pickerLocationId;
                picker.FileTypeFilter.Add(".db3");
                return await picker.PickSingleFileAsync();
            }
            else
            {
                var picker = new FileSavePicker();
                picker.SuggestedFileName = SuggestedFileName;
                picker.SuggestedStartLocation = pickerLocationId;
                picker.FileTypeChoices.Add(FileTypeChoice.Item1, new List<string>() { FileTypeChoice.Item2 });
                return await picker.PickSaveFileAsync();
            }
        }
    }
}