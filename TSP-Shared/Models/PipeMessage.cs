using System;
using System.Collections.Generic;
using System.Text;

namespace TSP_Shared.Models
{
    [Serializable]
    public enum MessageType
    {
        START,
        PAUSE,
        UNPAUSE,
        STOP,
        PROGRESS,
        BEST_SOLUTION,
    }

    [Serializable]
    public class PipeMessage
    {
        public List<City> CityList { get; set; }
        public MessageType Type { get; set; }

        public int HowMany { get; set; }

        //public double Progress { get; set; }

        //public int Concurrency { get; set; }
    }
}
