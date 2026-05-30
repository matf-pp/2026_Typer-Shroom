using System.Collections.Generic;

namespace TyperShroom.Core {
    public class GameState {
        public List<Bug> ActiveBugs { get; set; } = new List<Bug>();
        public int Lives            { get; set; }
        public int Score            { get; set; }
        public int Wave             { get; set; }
        public string CurrentInput  { get; set; } = string.Empty;
        public Bug? CurrentTarget   { get; set; } 
        public bool IsGameOver      { get; set; }
        public bool IsWaveClearing  { get; set; }
    }
}