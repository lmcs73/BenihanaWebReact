using System;
using System.Collections.Generic;

namespace BenihanaWebReact
{
    public class ResultModel
    {
        #region Bar
        public double BarDrinksSold { get; set; }
        public double BarAvgCust { get; set; }
        public double BarMaxCust { get; set; }
        public double BarAvgWait { get; set; }
        public double BarMaxWait { get; set; }
        public double BarLostCust { get; set; }
        public double BarAvgDrinksPerCust { get; set; }
        #endregion
        #region Dining Room
        public double DiningRoomDinnersServed { get; set; }
        public double DiningRoomTablesServed { get; set; }
        public double DiningRoomAvgTablesInUse { get; set; }
        public double DiningRoomAvgDiningTime { get; set; }
        public double DiningRoomAvgCust { get; set; }
        public double DiningRoomMaxCust { get; set; }
        public double DiningRoomAvgUtilization { get; set; }
        #endregion
        #region Financial
        public double FinancialRevenueBar { get; set; }
        public double FinancialRevenueDinner { get; set; }
        public double FinancialRevenueTotal { get; set; }
        public double FinancialCostOfDrink { get; set; }
        public double FinancialCostOfDinner { get; set; }
        public double FinancialAdvertisingCost { get; set; }
        public double FinancialLaborCost { get; set; }
        public double FinancialOverheadCost { get; set; }
        public double FinancialCostTotal { get; set; }
        public double FinancialNightProfit { get; set; }
        #endregion
        #region Customers
        public List<DateTime> Times { get; set; } = new List<DateTime>();
        public List<double> CustomersInBar { get; set; } = new List<double>();
        public List<double> CustomersInDinningRoom { get; set; } = new List<double>();
        public List<double> CustomersLost { get; set; } = new List<double>();
        #endregion
        #region Total Served
        public List<double> TotalServedDrinks { get; set; } = new List<double>();
        public List<double> TotalServedDinners { get; set; } = new List<double>();
        #endregion
    }
}
