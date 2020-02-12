using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2DESNet;
using O2DESNet.Distributions;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using O2DESNet.Standard;

namespace Benihana
{
    public partial class Model : Sandbox
    {
        #region Statics
        public class Statics
        {
            #region Environment
            /// <summary>
            /// The arrival rate for groups of customers during peak hours
            /// </summary>
            public double ArrivalHourlyRatePeak { get; set; } = 23;// modified from 90 to 23
            /// <summary>
            /// The arrival rate for groups of customers during non-peak hours
            /// </summary>
            public double ArrivalHourlyRateNonPeak { get; set; } = 14; // modified from 60 to 14
            /// <summary>
            /// Probabilities for the group size of customer arrival.
            /// </summary>
            public double[] ArrivalGroupSizeProbs { get; set; } = new double[] { 0, 1, 1, 1, 1, 1, 1, 1, 1 };
            /// <summary>
            /// The ratio of demand increment for unit level of corresponding ads option.
            /// </summary>
            public double[] AdsEffectRatio { get; set; } = new double[] { 0.05, 0.10, 0.15 };
            /// <summary>
            /// The cost of ads at unit level.
            /// </summary>
            public double[] AdsUnitCost { get; set; } = new double[] { 447, 411.3, 394.75 };
            /// <summary>
            /// The overhead cost of ads.
            /// </summary>
            public double[] AdsOverheadCost { get; set; } = new double[] { 894, 822.6, 789.5 };
            /// <summary>
            /// The starting hour of the peak in a day.
            /// </summary>
            public int PeakStartingHour { get; set; } = 19;
            /// <summary>
            /// The ending hour of the peak in a day.
            /// </summary>
            public int PeakEndingHour { get; set; } = 20;
            /// <summary>
            /// The closing hour in a day.
            /// </summary>
            public int ClosingHour { get; set; } = 21;
            /// <summary>
            /// Total capacity in unit of number of seats
            /// </summary>
            public int TotalCapacity { get; set; } = 167;
            /// <summary>
            /// Revenue per customer for drink in the bar area
            /// </summary>
            public double RevenuePerDrink { get; set; } = 2;
            /// <summary>
            /// Revenue per customer for drink in the bar area, before peak
            /// </summary>
            public double RevenuePerDrinkHappyHour { get; set; } = 1.5;
            /// <summary>
            /// Revenue per customer for dinner at the dining area
            /// </summary>
            public double RevenuePerDinner { get; set; } = 10;
            /// <summary>
            /// Revenue per customer for dinner at the dining area with discount
            /// </summary>
            public double RevenuePerDinnerDiscounted { get; set; } = 8.5;
            public double ExpectedNDrinksPerHour { get; set; } = 5; // changed from 2 to 5       
            /// <summary>
            /// For total 5.5 hours if open at 17:00.
            /// </summary>
            public double LaborCostPerHour { get; set; } = 99.33;
            public double CostOfDrink { get; set; } = 1.4;
            public double CostOfDinner { get; set; } = 3.5;
            #endregion

            #region Policy
            /** STRATEGY #1 & #5 **/
            public enum BatchingRules { None, Table8, Table4, Table4to8 };
            public BatchingRules BatchingRule { get; set; } = BatchingRules.Table4to8;

            /** STRATEGY #2 **/
            /// <summary>
            /// Number of dining tables with 8 seats each, ranges from 19 to 10.
            /// </summary>
            public int NDiningTables { get; set; } = 15;
            /// <summary>
            /// Number of seats at bar area, as a function of number of dining tables because the total capacity is constrained.
            /// </summary>
            public int NBarSeats { get { return TotalCapacity - NDiningTables * 8; } }

            /** STRATEGY #3 **/
            /// <summary>
            /// Expected dining time in unit of minutes, before peark hours
            /// </summary>
            public int ExpectedDiningMinutesbeforePeak { get; set; } = 60; // 45-75
            /// <summary>
            /// Expected dining time in unit of minutes, during peark hours
            /// </summary>
            public int ExpectedDiningMinutesduringPeak { get; set; } = 60;  // 45-75
            /// <summary>
            /// Expected dining time in unit of minutes, after peark hours
            /// </summary>
            public int ExpectedDiningMinutesafterPeak { get; set; } = 60; // 45-75

            /** STRATEGY #4 **/
            public enum AdsOptions { AwarenessBuilding, DiscountPromotion, HappyHour };
            public AdsOptions AdsOption { get; set; }
            public double AdsLevel { get; set; } = 0;
            /// <summary>
            /// Openning hour of in a day, ranges among 17, 18, and 19.
            /// </summary>
            public int OpeningHour { get; set; } = 17;
            #endregion

            public void ToXML(string name)
            {
                using (var sw = new StreamWriter(string.Format("{0}.xml", name)))
                    new XmlSerializer(typeof(Statics)).Serialize(XmlWriter.Create(sw), this);
            }
            public static Statics FromXML(string name)
            {
                return (Statics)new XmlSerializer(typeof(Statics)).Deserialize(new StreamReader(string.Format("{0}.xml", name)));
            }
        }

        public Statics Config { get; private set; }
        #endregion

        #region Dynamics
        public Generator ArrivalBeforePeak { get; private set; }
        public Generator ArrivalDuringPeak { get; private set; }
        public Generator ArrivalAfterPeak { get; private set; }
        public int CustomersCount { get; private set; }
        /// <summary>
        /// Customers in groups waiting at the bar seats.
        /// </summary>
        public List<List<Customer>> Waiting { get; private set; } = new List<List<Customer>>();
        /// <summary>
        /// Customers waiting at the bar seats.
        /// </summary>
        public Customer[] BarSeats { get; private set; }
        /// <summary>
        /// Customers dining at each table.
        /// </summary>
        public List<Customer>[] DiningTables { get; private set; }
        //public List<Customer> Departed { get; private set; } = new List<Customer>();
        public HourCounter HourCounterCustomersInBar { get; private set; }
        public HourCounter HourCounterDinnersServing { get; private set; }
        public HourCounter HourCounterTablesServing { get; private set; }
        public HourCounter HourCounterRejected { get; private set; }
        public int TotalNDrinksServed { get; private set; } = 0;
        public int TotalNDrinksServedInHappyHours { get; private set; } = 0;
        public List<Customer> AllCustomers { get; private set; } = new List<Customer>();
        public List<Customer> StartedCustomers { get { return AllCustomers.Where(c => c.DiningTime < DateTime.MaxValue).ToList(); } }
        public List<Customer> DepartedCustomers { get { return AllCustomers.Where(c => c.DepartureTime < DateTime.MaxValue).ToList(); } }
        public List<Customer> LostCustomers { get { return AllCustomers.Where(c => c.DiningTime == DateTime.MaxValue && c.BarSeatIndex < 0).ToList(); } }
        public Result Result { get; private set; } = new Result();

        public class Customer
        {
            public int Id { get; internal set; }
            public DateTime ArrivalTime { get; internal set; } = DateTime.MaxValue;
            public DateTime DiningTime { get; internal set; } = DateTime.MaxValue;
            public DateTime DepartureTime { get; internal set; } = DateTime.MaxValue;
            public int BarSeatIndex { get; internal set; } = -1;
            public int TableIndex { get; internal set; } = -1;
        }
        #endregion

        #region Events
        void Arrive()

        {
            var customers = Enumerable.Range(0, Empirical.Sample(DefaultRS, Config.ArrivalGroupSizeProbs)).Select(i => new Customer()).ToList();

            foreach (var c in customers)
            {
                c.Id = ++CustomersCount;
                c.ArrivalTime = ClockTime;
            }
            AllCustomers.AddRange(customers);
            Waiting.Add(customers);
            Start();
            Log("{0} arrived.", customers.Count);
        }

        void Start()
        {
            var tableIndices = Enumerable.Range(0, Config.NDiningTables).Where(i => DiningTables[i].Count == 0).ToList();
            if (tableIndices.Count > 0 && Waiting.Count > 0)
            {
                var tblIdx = tableIndices.First(); /// get the table

                List<int> indices = null;
                var sizes = Waiting.Select(g => g.Count).ToList();
                switch (Config.BatchingRule)
                {
                    case Statics.BatchingRules.None:
                        indices = new List<int> { 0 };
                        break;
                    case Statics.BatchingRules.Table8:
                        indices = Batch(sizes, 8, 8);
                        break;
                    case Statics.BatchingRules.Table4:
                        indices = Batch(sizes, 8, 8);
                        if (indices == null) indices = Batch(sizes, 4, 4);
                        break;
                    case Statics.BatchingRules.Table4to8:
                        indices = Batch(sizes, 4, 8);
                        break;
                }

                /// Start dining
                if (indices != null)
                {
                    /// assign from bar seat to dining table
                    DiningTables[tblIdx] = indices.SelectMany(i => Waiting[i]).ToList();
                    HourCounterDinnersServing.ObserveChange(DiningTables[tblIdx].Count, ClockTime);
                    HourCounterTablesServing.ObserveChange(1, ClockTime);
                    foreach (var c in DiningTables[tblIdx])
                    {
                        c.DiningTime = ClockTime;
                        c.TableIndex = tblIdx;
                        if (c.BarSeatIndex > -1)
                        {
                            BarSeats[c.BarSeatIndex] = null;
                            HourCounterCustomersInBar.ObserveChange(-1, ClockTime);
                        }
                        // count drinks
                        var waitingHours = (c.DiningTime - c.ArrivalTime).TotalHours;
                        if (waitingHours > 0)
                        {
                            var nDrinks = MathNet.Numerics.Distributions.Poisson.Sample(DefaultRS, Config.ExpectedNDrinksPerHour * waitingHours);
                            if (c.ArrivalTime.Hour < Config.PeakStartingHour && Config.AdsOption == Statics.AdsOptions.HappyHour)
                                TotalNDrinksServedInHappyHours += nDrinks;
                            TotalNDrinksServed += nDrinks;
                        }
                    }
                    Waiting = Enumerable.Range(0, Waiting.Count).Where(i => !indices.Contains(i)).Select(i => Waiting[i]).ToList();
                    /// dining time
                    var mean = Config.ExpectedDiningMinutesduringPeak;
                    if (ClockTime.Hour < Config.PeakStartingHour) mean = Config.ExpectedDiningMinutesbeforePeak;
                    if (ClockTime.Hour >= Config.PeakEndingHour) mean = Config.ExpectedDiningMinutesafterPeak;
                    Schedule(() => Depart(tblIdx), TimeSpan.FromMinutes(Exponential.Sample(DefaultRS, mean)));
                }
            }

            /// Reject non-seated customers, if capacity limit is exceeded
            if (Waiting.Sum(g => g.Count) > Config.NBarSeats)
            {
                Reject(Waiting.Last());
                Waiting.RemoveAt(Waiting.Count - 1);
            }

            /// Seat newly arrived customers
            int barSeatIdx = 0;
            foreach (var c in Waiting.SelectMany(g => g).Where(c => c.BarSeatIndex < 0).ToList())
            {
                while (BarSeats[barSeatIdx] != null) barSeatIdx++;
                c.BarSeatIndex = barSeatIdx;
                BarSeats[barSeatIdx] = c;
                HourCounterCustomersInBar.ObserveChange(1, ClockTime);
            }

            Result.Update(this, ClockTime);
        }

        /// <summary>
        /// Find the indices of batched groups based on the range of batched group size.
        /// </summary>
        /// <param name="groupSizes">List of sizes of all groups.</param>
        /// <param name="lbSize">Lowerbound for the total size of the batched groups.</param>
        /// <param name="ubSize">Upperbound for the total size of the batched groups.</param>
        /// <returns>Indices of the groups to be batched.</returns>
        static List<int> Batch(List<int> groupSizes, int lbSize, int ubSize)
        {
            if (lbSize < 0) return null;
            for (int i = 0; i < groupSizes.Count; i++)
            {
                if (groupSizes[i] >= lbSize && groupSizes[i] <= ubSize) return new List<int> { i };
                var subBatch = Batch(groupSizes.GetRange(i + 1, groupSizes.Count - i - 1), lbSize - groupSizes[i], ubSize - groupSizes[i]);
                if (subBatch != null) return new List<int> { i }.Concat(subBatch.Select(j => j + i + 1)).ToList();
            }
            return null;
        }

        void Depart(int TableIndex)
        {
            foreach (var c in DiningTables[TableIndex]) c.DepartureTime = ClockTime;
            //This.Departed.AddRange(This.DiningTables[TableIndex]);
            HourCounterDinnersServing.ObserveChange(-DiningTables[TableIndex].Count, ClockTime);
            HourCounterTablesServing.ObserveChange(-1, ClockTime);
            DiningTables[TableIndex] = new List<Customer>();
            Schedule(Start, TimeSpan.FromSeconds(0));
        }

        void Reject(List<Customer> Customers)
        {
            foreach (var c in Customers)
                if (c.BarSeatIndex > -1 || c.TableIndex > -1)
                    throw new Exception("Cannot reject seated customers.");
            HourCounterRejected.ObserveChange(Customers.Count, ClockTime);
        }
        #endregion

        #region Input Events - Getters
        #endregion

        #region Output Events - Reference to Getters
        #endregion

        public Model(Statics config, int seed, string tag = null)
            : base(seed, "Benihana")
        {
            Config = config;
            BarSeats = new Customer[Config.NBarSeats];
            DiningTables = Enumerable.Range(0, Config.NDiningTables).Select(i => new List<Customer>()).ToArray();
            ArrivalBeforePeak = AddChild(new Generator(new Generator.Statics
            {
                InterArrivalTime = rs => TimeSpan.FromHours(Exponential.Sample(rs, 1 / Config.ArrivalHourlyRateNonPeak *
                    (1 + Config.AdsEffectRatio[(int)Config.AdsOption] * Config.AdsLevel))),
            }, DefaultRS.Next()));
            ArrivalDuringPeak = AddChild(new Generator(new Generator.Statics
            {
                InterArrivalTime = rs => TimeSpan.FromHours(Exponential.Sample(rs, 1 / Config.ArrivalHourlyRatePeak *
                    (1 + Config.AdsEffectRatio[(int)Config.AdsOption] * Config.AdsLevel))),
            }, DefaultRS.Next()));
            ArrivalAfterPeak = AddChild(new Generator(new Generator.Statics
            {
                InterArrivalTime = rs => TimeSpan.FromHours(Exponential.Sample(rs, 1 / Config.ArrivalHourlyRateNonPeak *
                    (1 + Config.AdsEffectRatio[(int)Config.AdsOption] * Config.AdsLevel))),
            }, DefaultRS.Next()));

            // Connect generators to internal events
            ArrivalBeforePeak.OnArrive += Arrive;
            ArrivalDuringPeak.OnArrive += Arrive;
            ArrivalAfterPeak.OnArrive += Arrive;

            // HourCounter
            HourCounterCustomersInBar = AddHourCounter();
            HourCounterDinnersServing = AddHourCounter();
            HourCounterTablesServing = AddHourCounter();
            HourCounterRejected = AddHourCounter();

            // Add initial events
            Schedule(ArrivalBeforePeak.Start, TimeSpan.FromHours(Config.OpeningHour));
            Schedule(ArrivalBeforePeak.End, TimeSpan.FromHours(Config.PeakEndingHour));
            Schedule(ArrivalDuringPeak.Start, TimeSpan.FromHours(Config.PeakStartingHour));
            Schedule(ArrivalDuringPeak.End, TimeSpan.FromHours(Config.PeakEndingHour));
            Schedule(ArrivalAfterPeak.Start, TimeSpan.FromHours(Config.PeakEndingHour));
            Schedule(ArrivalAfterPeak.End, TimeSpan.FromHours(Config.ClosingHour));
        }

        public void WriteToConsole()
        {
            Console.Write("Bar Seats:\t\t");
            foreach (var c in BarSeats)
            {
                if (c == null) Console.Write("O ");
                else Console.Write("X ", c.Id);
            }
            Console.WriteLine();

            Console.WriteLine("Dining at tables:\t");
            foreach (var g in DiningTables)
            {
                foreach (var c in g) Console.Write("X");
                Console.WriteLine();
            }
            //Console.WriteLine("#Served:\t{0}", NCustomersServed);
            Console.WriteLine("#Rejected:\t{0}", HourCounterRejected.LastCount);
            //Console.WriteLine("#Drinks:\t{0}", NDrinksSold);
        }

    }
}