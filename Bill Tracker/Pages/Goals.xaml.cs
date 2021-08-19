using Bill_Tracker.Dialog;
using BillMate.Services.ViewModel.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Bill_Tracker.UI;
using BillMate.Services.ViewModel.Models;

namespace Bill_Tracker.Pages
{
    public sealed partial class Goals : Page
    {
        internal GoalDataProvider GP { get; set; }
        public Goals()
        {
            this.InitializeComponent();
            RefreshGoalData();
        }

        internal void RefreshGoalData()
        {
            GP = new GoalDataProvider();
            List<Goal> gList = GP.GetGoalTable();
            List<Goal> goalList = gList.OrderBy(g => g.isComplete).ThenBy(o => DateTime.Parse(o.goalDate)).ToList();
            lvGoal.ItemsSource = null;
            lvGoal.ItemsSource = goalList;

            double gTotal = goalList.Sum(a => a.goalAmount);
            double gProg = goalList.Sum(p => p.currentAmount);

            lblGoalCnt.Text = goalList.Count.ToString();
            lblGTotal.Text = gTotal.ToCurrencyString();
            lblGProg.Text = gProg.ToCurrencyString();

            if (goalList.Count == 0)
            {
                stkNoItems.Visibility = Visibility.Visible;
                lvGoal.Visibility = Visibility.Collapsed;
            }
            else
            {
                stkNoItems.Visibility = Visibility.Collapsed;
                lvGoal.Visibility = Visibility.Visible;
            }
        }
        
        private async void editGoal(Goal goal)
        {
            addGoal egd = new addGoal(true, goal as Goal);
            await egd.ShowAsync();
            RefreshGoalData();
        }
        private async void subtractGoal(Goal goal)
        {
            if ((goal as Goal).currentAmount != 0)
            {
                goalAction sbg = new goalAction(goal as Goal, true);
                await sbg.ShowAsync();
                RefreshGoalData();
            }
        }
        private async void addGoal(Goal goal)
        {
            if ((goal as Goal).currentAmount != (goal as Goal).goalAmount)
            {
                goalAction addg = new goalAction(goal as Goal, false);
                await addg.ShowAsync();
                RefreshGoalData();
            }
        }
        private async void deleteGoal(Goal goal)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(string.Format("Are you sure you wish to delete {0}?", goal.goalName), "Delete Goal?");
            dialog.Options = Windows.UI.Popups.MessageDialogOptions.AcceptUserInputAfterDelay;
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 1 });            
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var action = await dialog.ShowAsync();

            if (action.Id.ToString() == "1")
            {
                GP.DeleteGoal(goal);
            }

            RefreshGoalData();
        }

        private void goalEdit_Click(object sender, RoutedEventArgs e)
        {
            editGoal((Goal)(sender as Button).DataContext);
        }

        private void goalSubt_Click(object sender, RoutedEventArgs e)
        {
            subtractGoal((Goal)(sender as Button).DataContext);            
        }

        private void goalAdd_Click(object sender, RoutedEventArgs e)
        {
            addGoal((Goal)(sender as Button).DataContext);
        }


        private void goalDel_Click(object sender, RoutedEventArgs e)
        {
            deleteGoal((Goal)(sender as Button).DataContext as Goal);
        }

        private void SwipeDel_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            deleteGoal((Goal)args.SwipeControl.DataContext);
        }
        private void SwipeEdit_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            editGoal((Goal)args.SwipeControl.DataContext);
        }
        private void SwipeSub_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            subtractGoal((Goal)args.SwipeControl.DataContext);
        }
        private void SwipeAdd_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            addGoal((Goal)args.SwipeControl.DataContext);
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UIFunctions.pointerEnter(sender, e);
        }

      
        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UIFunctions.pointerExit(sender, e);
        }

        private async void bttnAdd_Click(object sender, RoutedEventArgs e)
        {
            addGoal aGoal = new addGoal();
            await aGoal.ShowAsync();
            this.Frame.Navigate(typeof(Goals));
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UIFunctions.swipe(sender, e);
        }

        private void bttnHlp_Click(object sender, RoutedEventArgs e)
        {
            UIFunctions.showHelp(this.Frame);
        }
    }
}
