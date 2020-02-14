using O2DESNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Benihana
{
    public class Result
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

        public void Update(Model model, DateTime clockTime)
        {
            var config = model.Config;
            #region 
            BarDrinksSold = model.TotalNDrinksServed;
            BarAvgCust = model.HourCounterCustomersInBar.AverageCount;
            BarMaxCust = Math.Max(BarMaxCust, model.HourCounterCustomersInBar.LastCount);
            var barwaits = model.AllCustomers.Where(c => c.DiningTime < DateTime.MaxValue).Select(c => (c.DiningTime - c.ArrivalTime).TotalMinutes).ToList();
            if (barwaits.Count > 0)
            {
                BarAvgWait = barwaits.Average();
                BarMaxWait = barwaits.Max();
            }
            BarLostCust = model.LostCustomers.Count;
            if (model.StartedCustomers.Count > 0) BarAvgDrinksPerCust = BarDrinksSold / model.StartedCustomers.Count;

            DiningRoomDinnersServed = model.HourCounterDinnersServing.TotalDecrement;
            DiningRoomTablesServed = model.HourCounterTablesServing.TotalDecrement;
            DiningRoomAvgTablesInUse = model.HourCounterTablesServing.AverageCount;
            if (model.DepartedCustomers.Count > 0)
                DiningRoomAvgDiningTime = model.AllCustomers.Where(c => c.DepartureTime < DateTime.MaxValue)
                    .Average(c => (c.DepartureTime - c.DiningTime).TotalMinutes);
            DiningRoomAvgCust = model.HourCounterDinnersServing.AverageCount;
            DiningRoomMaxCust = Math.Max(DiningRoomMaxCust, model.HourCounterDinnersServing.LastCount);
            DiningRoomAvgUtilization = DiningRoomAvgCust / (model.Config.NDiningTables * 8);
            
            FinancialRevenueBar = (model.TotalNDrinksServed - model.TotalNDrinksServedInHappyHours) * config.RevenuePerDrink +
                model.TotalNDrinksServedInHappyHours * config.RevenuePerDrinkHappyHour;
            FinancialRevenueDinner = DiningRoomDinnersServed * (model.Config.AdsOption == Model.Statics.AdsOptions.DiscountPromotion ?
                config.RevenuePerDinnerDiscounted : config.RevenuePerDinner);
            FinancialRevenueTotal = FinancialRevenueBar + FinancialRevenueDinner;
            FinancialCostOfDrink = BarDrinksSold * config.CostOfDrink;
            FinancialCostOfDinner = DiningRoomDinnersServed * config.CostOfDinner;
            FinancialAdvertisingCost = config.AdsUnitCost[(int)config.AdsOption] * config.AdsLevel;
            FinancialLaborCost = config.LaborCostPerHour * (17 + 5.5 - config.OpeningHour);
            FinancialOverheadCost = config.AdsOverheadCost[(int)config.AdsOption];
            FinancialCostTotal = FinancialCostOfDrink + FinancialCostOfDinner + FinancialAdvertisingCost + FinancialLaborCost + FinancialOverheadCost;
            FinancialNightProfit = FinancialRevenueTotal - FinancialCostTotal;
            #endregion
            Times.Add(clockTime);
            CustomersInBar.Add(model.HourCounterCustomersInBar.LastCount);
            CustomersInDinningRoom.Add(model.HourCounterDinnersServing.LastCount);
            CustomersLost.Add(model.AllCustomers.Count(c => c.DiningTime == DateTime.MaxValue && c.BarSeatIndex < 0));
            TotalServedDinners.Add(DiningRoomDinnersServed);
            TotalServedDrinks.Add(BarDrinksSold);
        }

        public void ToXML(string name)
        {
            using (var sw = new StreamWriter(string.Format("{0}.xml", name)))
                new XmlSerializer(typeof(Result)).Serialize(XmlWriter.Create(sw), this);
        }
    }
}
