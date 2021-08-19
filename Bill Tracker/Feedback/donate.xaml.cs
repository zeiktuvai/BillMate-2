using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Services.Store;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Feedback
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class donate : Page
    {
        //private StoreContext context = null;
       // private List<StoreProduct> products = new List<StoreProduct>();
        public donate()
        {
            this.InitializeComponent();
            lblMsg.Text = "Thank you for considering supporting development of this app. " +
                "It started as a personal project to manage bills but has turned into something useful for many people. " +
                "\n If so, I ask that you support by helping provide life saving caffeine in the form of Coca-Cola " +
                "to fuel development.";
           // getStoreProd(); 
        }

        //private async void getStoreProd()
        //{
        //    context = StoreContext.GetDefault();
        //    rngProg.IsActive = true;
        //    StoreProductResult queryRes = await context.GetStoreProductForCurrentAppAsync();
        //    rngProg.IsActive = false;
        //    string[] productKinds = { "UnmanagedConsumable" };
        //    List<String> filterList = new List<string>(productKinds);
        //    StoreProductQueryResult queryResult = await context.GetAssociatedStoreProductsAsync(filterList);
        //    foreach (KeyValuePair<string, StoreProduct> item in queryResult.Products)
        //    {
        //        products.Add(item.Value);
        //        if (item.Value.InAppOfferToken == "donate_small")
        //        {
        //            lblDonSmall.Text = item.Value.Title;
        //            bttnDonSmall.Content = item.Value.Price.FormattedPrice;
        //        }
        //        if (item.Value.InAppOfferToken == "donate_large")
        //        {
        //            lblDonLrg.Text = item.Value.Title;
        //            bttnDonLrg.Content = item.Value.Price.FormattedPrice;
        //        }
        //    }
        //}        

        //private void bttnCancel_Click(object sender, RoutedEventArgs e)
        //{
        //    Frame theframe = this.Frame;
        //    theframe.Navigate(typeof(Feedback));
        //}

        //private async void bttnDonSmall_Click(object sender, RoutedEventArgs e)
        //{
        //    StoreProduct smallProd = products.Find(s => s.InAppOfferToken == "donate_small");
        //    StorePurchaseResult result = await smallProd.RequestPurchaseAsync();

        //    string extendedError = string.Empty;
        //    if (result.ExtendedError != null)
        //    {
        //        extendedError = result.ExtendedError.Message;
        //    }

        //    switch (result.Status)
        //    {
        //    //    case StorePurchaseStatus.AlreadyPurchased:
        //    //        textBlock.Text = "The user has already purchased the product.";
        //    //        break;

        //        case StorePurchaseStatus.Succeeded:
        //            Guid trackid = Guid.NewGuid();
        //            StoreConsumableResult represult = await context.ReportConsumableFulfillmentAsync(smallProd.StoreId, 1, trackid);
        //            if (represult.Status == StoreConsumableStatus.Succeeded)
        //            {
        //                lblStat.Text = "Purchase Successful";
        //            }
        //            break;

        //        case StorePurchaseStatus.NotPurchased:
        //            lblStat.Text = "The purchase did not complete. " +
        //                "The user may have cancelled the purchase. ExtendedError: " + extendedError;
        //            break;

        //        case StorePurchaseStatus.NetworkError:
        //            lblStat.Text = "The purchase was unsuccessful due to a network error. " +
        //                "ExtendedError: " + extendedError;
        //            break;

        //        case StorePurchaseStatus.ServerError:
        //            lblStat.Text = "The purchase was unsuccessful due to a server error. " +
        //                "ExtendedError: " + extendedError;
        //            break;

        //        default:
        //            lblStat.Text = "The purchase was unsuccessful due to an unknown error. " +
        //                "ExtendedError: " + extendedError;
        //            break;
        //    }
        //}

        //private async void bttnDonLrg_Click(object sender, RoutedEventArgs e)
        //{
        //    StoreProduct lrgProd = products.Find(s => s.InAppOfferToken == "donate_large");
        //    StorePurchaseResult result = await lrgProd.RequestPurchaseAsync();

        //    string extendedError = string.Empty;
        //    if (result.ExtendedError != null)
        //    {
        //        extendedError = result.ExtendedError.Message;
        //    }

        //    switch (result.Status)
        //    {
        //        //    case StorePurchaseStatus.AlreadyPurchased:
        //        //        textBlock.Text = "The user has already purchased the product.";
        //        //        break;

        //        case StorePurchaseStatus.Succeeded:
        //            Guid trackid = Guid.NewGuid();
        //            StoreConsumableResult represult = await context.ReportConsumableFulfillmentAsync(lrgProd.StoreId, 1, trackid);
        //            if (represult.Status == StoreConsumableStatus.Succeeded)
        //            {
        //                lblStat.Text = "Purchase Successful";
        //            }                    
        //            break;

        //        case StorePurchaseStatus.NotPurchased:
        //            lblStat.Text = "The purchase did not complete. " +
        //                "The user may have cancelled the purchase. ExtendedError: " + extendedError;
        //            break;

        //        case StorePurchaseStatus.NetworkError:
        //            lblStat.Text = "The purchase was unsuccessful due to a network error. " +
        //                "ExtendedError: " + extendedError;
        //            break;

        //        case StorePurchaseStatus.ServerError:
        //            lblStat.Text = "The purchase was unsuccessful due to a server error. " +
        //                "ExtendedError: " + extendedError;
        //            break;

        //        default:
        //            lblStat.Text = "The purchase was unsuccessful due to an unknown error. " +
        //                "ExtendedError: " + extendedError;
        //            break;
        //    }
        //}
    }
}
