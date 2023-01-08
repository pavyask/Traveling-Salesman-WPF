using TSP_Shared.Models;

namespace TSP_Shared.Pipes
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
        public MessageType Type { get; set; }
        public string ThreadId { get; set; }
        public CitiesSolution CitiesSolution { get; set; }
        public long SolutionCount { get; set; }
        public int HowMany { get; set; }
        public double Progress { get; set; }
        public double MaxDurationInMilisecs { get; set; }
    }
}