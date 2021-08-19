using Bill_Tracker.Dialog;
using BillMate.Services.ViewModel.Configuration;
using BillMate.Services.ViewModel.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Bill_Tracker.UI
{
    public static class UIFunctions
    {
        static int taps { get; set; }
        internal static void pointerEnter(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {

                Grid item = sender as Grid;
                if (item.DataContext is Bill)
                {
                    if ((item.DataContext as Bill).Frequency == "OneTime")
                    {

                    }
                    else
                    {
                        foreach (var child in item.Children)
                        {
                            if (child is RelativePanel)
                            {
                                if ((child as RelativePanel).Name == "bttnPanel")
                                {
                                    child.Visibility = Visibility.Visible;
                                }
                            }
                        }
                    }

                }
                else
                {
                    foreach (var child in item.Children)
                    {
                        if (child is RelativePanel)
                        {
                            if ((child as RelativePanel).Name == "bttnPanel")
                            {
                                child.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }
        }

        internal static void pointerExit(object sender, PointerRoutedEventArgs e)
        {
            Grid item = sender as Grid;
            foreach (var child in item.Children)
            {
                if (child is RelativePanel)
                {
                    if ((child as RelativePanel).Name == "bttnPanel")
                    {
                        child.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        internal static async void swipe(object sender, TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                taps++;
                if (taps == 3)
                {
                    if (ConfigurationManager.GetLocalSetting("swipeHelp") == null)
                    {
                        help_swipe help = new help_swipe();
                        await help.ShowAsync();
                        ConfigurationManager.SetLocalSetting("swipeHelp", "true");
                    }
                }
            }
        }

        internal static void showHelp(Frame mainframe)
        {
            mainframe.Navigate(typeof(Pages.Help));
        }
    }
}
