using System;

namespace TyperShroom.Core {
    public interface IGameEngine {
        GameState CurrentState { get; }

        void StartGame();
        void Update(double deltaTime);
        void ProcessKeystroke(char key);
        GameResult EndGame();

        event Action<Bug> OnBugSpawned;
        event Action<Bug> OnBugKilled;
        event Action<Bug> OnBugReached;
        event Action<Bug> OnBugMistyped;
        event Action      OnWaveCleared;
        event Action      OnKeyNotMatched;
        event Action      OnGameOver;
    }
}