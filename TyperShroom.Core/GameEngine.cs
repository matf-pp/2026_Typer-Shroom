using System;

namespace TyperShroom.Core {
    public class GameEngine : IGameEngine {
        
        private GameState _state = new GameState(); 
        private WordManager _wordManager = new WordManager();
        private DateTime _startTime;
        private Random _random = new Random();
        private int _bugsKilledThisWave = 0;
        private int _totalBugsKilled = 0;
        private int _totalKeystrokes = 0;
        private int _correctKeystrokes = 0;

        public GameState CurrentState => _state;

        public void StartGame() {
            _state.Lives = 3;
            _state.Wave = 1;
            _state.Score = 0;
            _startTime = DateTime.Now;
            for (int i = 0; i < 3; i++) {
                SpawnBug();
            }
        }

        private void SpawnBug() {
            Bug Bug = new Bug();

            // ensure no two bugs on screen share the same starting letter
            List<char> firstLetters = new List<char>();
            foreach (Bug bug in _state.ActiveBugs) 
                firstLetters.Add(bug.Word[0]);

            Bug.Word = _wordManager.GetWord(_state.Wave, firstLetters, _state.ActiveBugs);
            Bug.RemainingWord = Bug.Word;
            Bug.PositionX = 100;
            Bug.PositionY = _random.Next(5, 95);
            Bug.Speed = 2.0 + _state.Wave * 0.5;

            List<string> bugTypes = ["ant", "spider", "beetle"];
            Bug.BugType = bugTypes[_random.Next(0, 3)];

            Bug.IsDead = false;
            Bug.ReachedMushroom = false;

            OnBugSpawned?.Invoke(Bug);
            _state.ActiveBugs.Add(Bug);
        }

        public void ProcessKeystroke(char key) {
            // no target locked — check if typed key matches any bugs first letter to acquire target
            if (_state.CurrentTarget == null) {
                bool targetFlag = false;
                foreach (Bug bug in _state.ActiveBugs) {
                    if (key == bug.Word[0]) {
                        _state.CurrentTarget = bug;
                        bug.RemainingWord = bug.RemainingWord.Substring(1);   // shorten the word when key was typed
                        _correctKeystrokes++;
                        _totalKeystrokes++;
                        targetFlag = true;
                    }
                }    

                if (!targetFlag) {
                    _totalKeystrokes++; 
                    _state.Score -= 10;
                    OnKeyNotMatched?.Invoke();
                }
            }
            // if a target is already locked -> directly update remainingWord and states
            else {
                if (key == _state.CurrentTarget.RemainingWord[0]) {
                    _state.CurrentTarget.RemainingWord = _state.CurrentTarget.RemainingWord.Substring(1);
                    _correctKeystrokes++;   
                    _totalKeystrokes++;

                    // if its the last letter in bug.Word
                    if (_state.CurrentTarget.RemainingWord.Length == 0) {
                        OnBugKilled?.Invoke(_state.CurrentTarget);
                        _state.Score += 30;
                        _bugsKilledThisWave++;
                        _totalBugsKilled++;
                        _state.CurrentTarget.IsDead = true;
                        _state.CurrentTarget = null;

                        // if wave is cleared
                        if (_bugsKilledThisWave >= 10) {
                            _state.Wave++;
                            OnWaveCleared?.Invoke();
                            _bugsKilledThisWave = 0;
                        }
                    }

                }
                // if user mystyped
                else {
                    _totalKeystrokes++; 
                    _state.Score -= 15;
                    OnBugMistyped?.Invoke(_state.CurrentTarget);
                }
            }    
        }

        public void Update(double deltaTime) {
            List<Bug> toRemove = new List<Bug>();

            foreach (Bug bug in _state.ActiveBugs) {
                bug.PositionX -= (int)(bug.Speed * deltaTime);

                // if bug reached the mushroom update, but dont spawn new bug -> too much pressure
                if (bug.PositionX <= 15) {
                    _state.Lives -= 1;
                    OnBugReached?.Invoke(bug);
                    toRemove.Add(bug);
                    bug.ReachedMushroom = true;
                }

                if (bug.IsDead) {
                    toRemove.Add(bug);
                }
            }

            if (_state.Lives <= 0) {
                    OnGameOver?.Invoke();
                    _state.IsGameOver = true;
            }

            foreach(Bug bug in toRemove) 
                _state.ActiveBugs.Remove(bug);

            // for every killed bug -> spawn new one
            foreach(Bug bug in toRemove) {
                if (bug.IsDead)
                    SpawnBug();
            }
        }

        public GameResult EndGame() => new GameResult {
            FinalScore = _state.Score,
            WavesReached = _state.Wave,
            BugsKilled = _totalBugsKilled,
            LivesLost = 3 - _state.Lives,
            TotalKeystrokes = _totalKeystrokes,
            CorrectKeystrokes = _correctKeystrokes,
            Accuracy = _totalKeystrokes != 0 ? (double)_correctKeystrokes / _totalKeystrokes : 0,
            WPM = (int)(_totalBugsKilled / (DateTime.Now - _startTime).TotalMinutes),
            GameDuration = DateTime.Now - _startTime,
            PlayedAt = _startTime,
        };

        public event Action<Bug>? OnBugSpawned;
        public event Action<Bug>? OnBugKilled;
        public event Action<Bug>? OnBugReached;
        public event Action<Bug>? OnBugMistyped;
        public event Action? OnWaveCleared;
        public event Action? OnKeyNotMatched;
        public event Action? OnGameOver;
    }

}