namespace TyperShroom.Core {
    public class Bug {
        public string BugType       { get; set; } = string.Empty;
        public string Word          { get; set; } = string.Empty;
        public string RemainingWord { get; set; } = string.Empty;
        public int PositionX        { get; set; }
        public int PositionY        { get; set; }
        public double Speed         { get; set; }
        public bool IsDead          { get; set; }
        public bool ReachedMushroom { get; set; }
    }
}