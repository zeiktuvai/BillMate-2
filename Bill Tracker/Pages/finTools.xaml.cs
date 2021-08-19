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
using Bill_Tracker.Pages.Tools;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class finTools : Page
    {
        public finTools()
        {
            this.InitializeComponent();
        }

        private void tools_pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((tools_pivot.SelectedItem as PivotItem).Name)
            {
                case "PayPlanner":
                    toolsFrame.Navigate(typeof(PayPlanner));
                    break;

                case "PayOff":
                    toolsFrame.Navigate(typeof(PayoffCalc));
                    break;

                default:
                    break;
            }
        }
    }
}
