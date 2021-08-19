using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.ViewModel.Models;
using BillMate.Services.ViewModel.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Bill_Tracker.Pages.Tools
{
    public sealed partial class PayPlanner : Page
    {
        List<PayItem> payitems = new List<PayItem>();
        List<Bill> billList = new List<Bill>();
        double startingAmount = 0;
        bool autoSave = false;
        public PayPlanner()
        {
            this.InitializeComponent();
            getStartAmnt();
            GetSavedPayList();
            GetAutoSaveSet();
            BillDataProvider BP = new BillDataProvider();
            billList = BP.GetBillData("srtByDte", 0);
            BP = null;
            List<IGrouping<string, Bill>> gBills = billList.OrderBy(o => o.dueDate).GroupBy(g => g._stringDispDate).ToList();
            cvsBills.Source = gBills;
        }

        private void addPay_Click(object sender, RoutedEventArgs e)
        {
            tbxPayAmmnt.Text = "";
            tbxPayName.Text = "";
            addItemGrid.Visibility = Visibility.Visible;
        }

        private void refreshPaySet_Click(object sender, RoutedEventArgs e)
        {
            GetSavedPayList();
        }

        private void saveAddPay_Click(object sender, RoutedEventArgs e)
        {
            if (tbxPayAmmnt.Text.Length != 0 && tbxPayName.Text != null)
            {
                payitems.Add(new PayItem() { name = tbxPayName.Text, amount = double.Parse(tbxPayAmmnt.Text) });
                refreshListView();
                addItemGrid.Visibility = Visibility.Collapsed;
            }
            if (autoSave == true)
            {
                SavePaySet_Click(sender, e);
            }
        }

        private void cancelAddPay_Click(object sender, RoutedEventArgs e)
        {
            tbxPayAmmnt.Text = "";
            tbxPayName.Text = "";
            addItemGrid.Visibility = Visibility.Collapsed;
        }

        private async void ClearPaySet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await (await ApplicationData.Current.LocalFolder.GetFileAsync("payList.JSON")).DeleteAsync();
            }
            catch (Exception)
            {            }
            payitems.Clear();
            refreshListView();
        }

        private async void SavePaySet_Click(object sender, RoutedEventArgs e)
        {
                      
            try
            {
                string JS = JsonConvert.SerializeObject(payitems, Formatting.Indented);
                StorageFile expfile = await ApplicationData.Current.LocalFolder.CreateFileAsync("payList.JSON", CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(expfile, JS);
                if (tbxStartAmm.Text.Length != 0)
                {
                    ConfigurationManager.SetLocalSetting("PayPlanStartAM", tbxStartAmm.Text);
                }
            }
            catch (Exception)
            {
                
            }

        }
        private void bttnDel_Click(object sender, RoutedEventArgs e)
        {
            PayItem clickedItem = (PayItem)(sender as Button).DataContext;
            payitems.Remove(payitems.Find(p => p.name == clickedItem.name && p.amount == clickedItem.amount));
            refreshListView();
            if (autoSave == true)
            {
                SavePaySet_Click(sender, e);
            }
        }
        private void getStartAmnt()
        {
            var settingStartAm = ConfigurationManager.GetLocalSetting("PayPlanStartAM") ?? 0;
            tbxStartAmm.Text = settingStartAm.ToString();

            if (tbxStartAmm.Text.Length == 0)
            {
                startingAmount = double.Parse(tbxStartAmm.PlaceholderText);
            } else
            {
                startingAmount = double.Parse(tbxStartAmm.Text);
            }
        }

        private void refreshListView()
        {
            getStartAmnt();
            payList.ItemsSource = null;
            payList.ItemsSource = PayItem.ProcessPayItems(payitems, startingAmount);
        }

        private async void GetSavedPayList()
        {
            try
            {
                StorageFile impfile = await ApplicationData.Current.LocalFolder.GetFileAsync("payList.JSON");
                string impJSON = await Windows.Storage.FileIO.ReadTextAsync(impfile);
                List<PayItem> impList = JsonConvert.DeserializeObject<List<PayItem>>(impJSON);
                payitems = impList;
                payList.ItemsSource = PayItem.ProcessPayItems(payitems, startingAmount);
            }
            catch (Exception)
            {
                payList.ItemsSource = null;
            }
        }

        private void GetAutoSaveSet()
        {
            autoSave = (ConfigurationManager.GetLocalSetting("PaycheckCalcAutoSave") == null) ? tgglAutoSave.IsOn : 
                        bool.Parse(ConfigurationManager.GetLocalSetting("PaycheckCalcAutoSave").ToString());
        }

        private void tbxPayName_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var auto = (AutoSuggestBox)sender;
            var suggestion = billList.Where(b => b.Name.StartsWith(auto.Text, StringComparison.OrdinalIgnoreCase) && b.Reminder != "Archived").OrderBy(b => b.dispDueDate).Select(b => b.Name).ToArray();
            if (suggestion.Length == 0)
            {
                suggestion = new string[] { "Nothing Found" };
            }
            auto.ItemsSource = suggestion;
        }

        private void tbxPayName_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var auto = (AutoSuggestBox)sender;
            var suggestion = billList.Where(b => b.Name.StartsWith(auto.Text, StringComparison.OrdinalIgnoreCase) && b.Reminder != "Archived").OrderBy(b => b.dispDueDate).Select(b => b.Name).ToArray();
            if (suggestion.Length == 0)
            {
                suggestion = new string[] { "Nothing Found" };
            }
            auto.ItemsSource = suggestion;
        }

        private void tbxPayName_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var chosen = billList.Find(b => b.Name == args.SelectedItem.ToString());
            tbxPayAmmnt.Text = (chosen as Bill).baseAmount.ToString();
        }

        private void tbxStartAmm_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.SetLocalSetting("PayPlanStartAM", tbxStartAmm.Text);
            refreshListView();
        }

        private void tgglAutoSave_Toggled(object sender, RoutedEventArgs e)
        {
            autoSave = tgglAutoSave.IsOn;
            ConfigurationManager.SetLocalSetting("PaycheckCalcAutoSave", tgglAutoSave.IsOn.ToString());
        }

        private void bttnDone_Click(object sender, RoutedEventArgs e)
        {
            PayItem paidItem = (PayItem)(sender as Button).DataContext;
            PayItem found = payitems.Find(p => p.name == paidItem.name && p.amount == paidItem.amount);

            if (found != null)
            {
                found._paid = !found._paid;
            }

            refreshListView();
            if (autoSave == true)
            {
                SavePaySet_Click(sender, e);
            }
        }

        private void openBillPane_Click(object sender, RoutedEventArgs e)
        {
            if (scrlBills.Visibility == Visibility.Collapsed)
            {
                scrlBills.Visibility = Visibility.Visible;
                openBillPane.Content = "\xE0E2";
            } else
            {
                scrlBills.Visibility = Visibility.Collapsed;
                openBillPane.Content = "\xE0E3";
            }
        }


    }
}
