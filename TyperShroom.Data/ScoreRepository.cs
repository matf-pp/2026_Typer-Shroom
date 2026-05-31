using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TyperShroom.Core;

namespace TyperShroom.Data
{
    public class ScoreRepository
    {
        private readonly string _filePath;
        private const int MaxScores = 10;

        public ScoreRepository(string filePath = "scores.json")
        {
            _filePath = filePath;
        }

        private List<GameResult> DefaultScores() => new List<GameResult>
        {
            new GameResult { PlayerName = "Ana",    FinalScore = 500, WavesReached = 5, Accuracy = 0.95, PlayedAt = DateTime.Now },
            new GameResult { PlayerName = "Marko",  FinalScore = 420, WavesReached = 4, Accuracy = 0.88, PlayedAt = DateTime.Now },
            new GameResult { PlayerName = "Jovana", FinalScore = 380, WavesReached = 3, Accuracy = 0.82, PlayedAt = DateTime.Now },
            new GameResult { PlayerName = "Nikola", FinalScore = 300, WavesReached = 3, Accuracy = 0.75, PlayedAt = DateTime.Now },
            new GameResult { PlayerName = "Maja",   FinalScore = 210, WavesReached = 2, Accuracy = 0.70, PlayedAt = DateTime.Now },
        };

        public List<GameResult> LoadAll()
        {
            if (!File.Exists(_filePath))
                return DefaultScores();

            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<GameResult>>(json) ?? DefaultScores();
        }

        public List<GameResult> LoadTop5() => LoadAll().Take(5).ToList();

        public List<GameResult> LoadTop10() => LoadAll().Take(10).ToList();

        public void Save(GameResult result)
        {
            List<GameResult> all = LoadAll();
            all.Add(result);

            all = all.OrderByDescending(r => r.FinalScore)
                     .Take(MaxScores)
                     .ToList();

            string json = JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}