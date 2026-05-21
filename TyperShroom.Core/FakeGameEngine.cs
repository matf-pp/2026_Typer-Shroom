using System;
using System.Collections.Generic;

namespace TyperShroom.Core {

    public class FakeGameEngine : IGameEngine {
        public GameState CurrentState => new GameState {
            Lives = 2,
            Score = 120,
            Wave = 2,
            CurrentInput = "spi",
            ActiveBugs = new List<Bug>
            {
                new Bug { Word = "spider", PositionX = 10, PositionY = 5 },
                new Bug { Word = "beetle", PositionX = 40, PositionY = 3 },
                new Bug { Word = "ant",    PositionX = 25, PositionY = 8 },
            }
        };

        public void StartGame() { }
        public void ProcessKeystroke(char key) { }
        public GameResult EndGame() => new GameResult { FinalScore = 450, WPM = 67 };

        public void Update(double deltaTime) { }

        public event Action<Bug>? OnBugSpawned;
        public event Action<Bug>? OnBugKilled;
        public event Action<Bug>? OnBugReached;
        public event Action? OnGameOver;
    }

}