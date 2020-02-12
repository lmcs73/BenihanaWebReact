using System;
namespace BenihanaWebReact
{
    public class SimulationConfig
    {
        public int Batching { get; set; }
        public int LayoutTable { get; set; }
        public int DiningTimeBeforePeak { get; set; }
        public int DiningTimeDuringPeak { get; set; }
        public int DiningTimeAfterPeak { get; set; }
        public int Advertisement { get; set; }
        public int AdsLevel { get; set; }
        public int OpeningHour { get; set; }
    }
}
