using BillMate.Services.Data.Repository;
using BillMate.Services.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillMate.Services.ViewModel.Providers
{
    public class GoalDataProvider
    {
        internal SQLiteDatabaseOperation<Goal> GoalProvider { get; set; }

        public GoalDataProvider()
        {
            GoalProvider = new SQLiteDatabaseOperation<Goal>();
        }
        public List<Goal> GetGoalTable()
        {
            return FormatGoalData(GoalProvider.GetTable(new Goal()).ToList());
        }
        public void CreateGoal(Goal goal)
        {
            GoalProvider.CreateItem(goal);
        }
        public void UpdateGoal(Goal goal)
        {
            GoalProvider.UpdateItem(goal);
        }
        public void DeleteGoal(Goal goal)
        {
            GoalProvider.DeleteItem(goal);
        }

        internal List<Goal> FormatGoalData(List<Goal> FormatList)
        {
            foreach (Goal item in FormatList)
            {
                item.progress = (item.currentAmount / item.goalAmount) * 100;
                item._dispCurAmnt = item.currentAmount.ToCurrencyString();
                item._dispGoalAmnt = item.goalAmount.ToCurrencyString();
                item._dispGoalDate = DateTime.Parse(item.goalDate).ToFormattedString();
                item._goalDate = DateTime.Parse(item.goalDate);

                if (item.currentAmount == item.goalAmount)
                {
                    item.isComplete = true;
                }

                if (item.isComplete == true)
                {
                    item._background = "Lightgreen";
                    item._completionText = "Complete";
                    item._barColor = "Green";
                }
                else
                {
                    if (item._goalDate < DateTime.Now)
                    {
                        item._textFore = "Red";
                        item._pastSuspense = "Visible";
                        item._barColor = "Red";
                    }
                    else
                    {
                        item._textFore = "Black";
                        item._pastSuspense = "Collapsed";
                        item._barColor = "Black";
                    }
                    item._background = "CornflowerBlue";
                    item._completionText = "In Progress";
                }

                item.minPay = CalcMinimumPayment(item);
            }
            return FormatList;
        }

        internal string CalcMinimumPayment(Goal goal)
        {
            var dueDate = goal._goalDate;
            var ammnt = goal.goalAmount;
            string minPay = "";
            int dateDiff = ((dueDate.Year - DateTime.Now.Year) * 12) + dueDate.Month - DateTime.Now.Month;


            switch (dateDiff)
            {
                case var expression when dateDiff <= 0:
                    minPay = "Per Mo: " + (ammnt - goal.currentAmount).ToString("C0");
                    break;
                case var expression when dateDiff > 0:
                    minPay = "Per Mo: " + ((ammnt - goal.currentAmount) / dateDiff).ToString("C0");
                    break;
            }
            return minPay;
        }
    }
}
