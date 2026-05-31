using System;

namespace TyperShroom.Core {
    public class GameEngine : IGameEngine {
        
        private GameState _state = new GameState(); 
        private WordManager _wordManager = new WordManager();
        private DateTime _startTime;
        private Random _random = new Random();
        private int _bugsKilledThisWave = 0;
        private int _bugsSpawnedThisWave = 0;
        private int _bugsPerWave = 8;
        private int _totalBugsKilled = 0;
        private int _totalKeystrokes = 0;
        private int _correctKeystrokes = 0;
        private double _spawnInterval = 2;
        private double _timer = 0.0;

        public GameState CurrentState => _state;

        public void StartGame() {
            _wordManager = new WordManager();
            _state.Lives = 3;
            _state.Wave = 1;
            _state.Score = 0;
            _state.ActiveBugs = new List<Bug>();
            _state.CurrentTarget = null;
            _state.IsGameOver = false;
            _state.IsWaveClearing = false;
            _bugsKilledThisWave = 0;
            _bugsSpawnedThisWave = 0;
            _bugsPerWave = 8;
            _totalBugsKilled = 0;
            _totalKeystrokes = 0;
            _correctKeystrokes = 0;
            _startTime = DateTime.Now;
            for (int i = 0; i < 3; i++) {
                SpawnBug();
                _bugsSpawnedThisWave++;
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
            Bug.Speed = 2.0 + _state.Wave * 0.5;

            // choosing the bug type, randomness in chossing the air or ground type
            // when updating the ground and air has to have same number of elements for this to work
            List<string> groundBugTypes = ["ant", "spider", "worm"];
            List<string> airBugTypes = ["fly", "mosquito", "butterfly"];

            List<string>[] bugTypes = new List<string>[2];
            bugTypes[0] = groundBugTypes;
            bugTypes[1] = airBugTypes;
            int bugType = _random.Next(0, 3);
            int groundType = _random.Next(2);

            Bug.BugType = bugTypes[groundType][bugType];

            Bug.PositionX = 108;
            Bug.PositionY = groundType == 0 ? _random.Next(65, 90) : _random.Next(10, 40); 
            Bug.IsDead = false;
            Bug.ReachedMushroom = false;

            OnBugSpawned?.Invoke(Bug);
            _state.ActiveBugs.Add(Bug);
        }

        private void CheckWaveCleared() {
            if (_bugsKilledThisWave >= _bugsPerWave) {
                _state.Wave++;
                _spawnInterval *= 0.92;
                OnWaveCleared?.Invoke();
                _bugsKilledThisWave = 0;
                _bugsSpawnedThisWave = 0;
                _bugsPerWave++;
            }
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

                        CheckWaveCleared();
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
            if (_state.IsGameOver) return;
            List<Bug> toRemove = new List<Bug>();

            // if time from the last bug spawn is bigger then spawnInterval AND 
            // there is less then 15 bugs on Screen -> SpawnBug()
            // only do this when there is no wave clearing happening
            if (!_state.IsWaveClearing) {
                _timer += deltaTime;
                if (_timer >= _spawnInterval) {
                    if (_bugsSpawnedThisWave < _bugsPerWave && _state.ActiveBugs.Count < 15) {
                        SpawnBug();
                        _bugsSpawnedThisWave++;
                    }
                    _timer = 0;
                }

                foreach (Bug bug in _state.ActiveBugs) {
                    bug.PositionX -= bug.Speed * deltaTime;

                    if (bug.PositionX <= 30) {
                        _state.Lives -= 1;
                        OnBugReached?.Invoke(bug);
                        toRemove.Add(bug);
                        bug.ReachedMushroom = true;
                        _bugsKilledThisWave++;
                        CheckWaveCleared();
                        if (bug == _state.CurrentTarget) {
                            _state.CurrentTarget = null;
                        }
                    }
                }
            }

            foreach (Bug bug in _state.ActiveBugs) {
                if (bug.IsDead) toRemove.Add(bug);
            }

            foreach(Bug bug in toRemove) {
                _state.ActiveBugs.Remove(bug);
            }

            if (_state.Lives <= 0) {
                OnGameOver?.Invoke();
                _state.IsGameOver = true;
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
            WPM = (int)(_correctKeystrokes / 5.0 / (DateTime.Now - _startTime).TotalMinutes),
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