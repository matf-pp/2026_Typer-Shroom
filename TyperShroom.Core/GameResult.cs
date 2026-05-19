using System;

namespace TyperShroom.Core {
    public class GameResult {
        public string PlayerName     { get; set; } = string.Empty;
        public int FinalScore        { get; set; }
        public int WavesReached      { get; set; }
        public int BugsKilled        { get; set; }
        public int LivesLost         { get; set; }
        public int TotalKeystrokes   { get; set; }
        public int CorrectKeystrokes { get; set; }
        public double Accuracy       { get; set; }
        public int WPM               { get; set; }
        public TimeSpan GameDuration { get; set; }
        public DateTime PlayedAt     { get; set; }
    }
}