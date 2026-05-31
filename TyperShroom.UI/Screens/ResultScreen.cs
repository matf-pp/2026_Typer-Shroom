using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TyperShroom.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TyperShroom.UI.Screens
{
    public class ResultScreen
    {
        private SpriteFont _font;
        private int _width;
        private int _height;

        public bool ReturnToMenu { get; private set; } = false;

        public ResultScreen(SpriteFont font, int width, int height)
        {
            _font = font;
            _width = width;
            _height = height;
        }

        public void Reset()
        {
            ReturnToMenu = false;
        }

        public void Update(KeyboardState keyboard, KeyboardState previous)
        {
            if (keyboard.IsKeyDown(Keys.Enter) && !previous.IsKeyDown(Keys.Enter))
                ReturnToMenu = true;
        }

        public void Draw(SpriteBatch spriteBatch, GameResult result, List<GameResult> top5, Texture2D background)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, _width, _height), Color.White);

            int centerX = _width / 2;

            // Title
            string title = "GAME OVER";
            Vector2 titleSize = _font.MeasureString(title);
            spriteBatch.DrawString(_font, title,
                new Vector2(centerX - titleSize.X / 2, _height * 0.05f), Color.Red);

            // Current game result
            string yourScore = $"Your score:  {result.FinalScore} pts   |   Wave: {result.WavesReached}   |   Accuracy: {result.Accuracy:P0}";
            Vector2 yourScoreSize = _font.MeasureString(yourScore);
            spriteBatch.DrawString(_font, yourScore,
                new Vector2(centerX - yourScoreSize.X / 2, _height * 0.18f), Color.LightBlue);

            // Separator line
            string line = "----------------------------------------";
            Vector2 lineSize = _font.MeasureString(line);
            spriteBatch.DrawString(_font, line,
                new Vector2(centerX - lineSize.X / 2, _height * 0.27f), Color.DarkGray);

            // Top 5 title
            string topTitle = "TOP 5 PLAYERS";
            Vector2 topTitleSize = _font.MeasureString(topTitle);
            spriteBatch.DrawString(_font, topTitle,
                new Vector2(centerX - topTitleSize.X / 2, _height * 0.33f), Color.Yellow);

            // Column headers
            float colName = centerX - 280f;
            float colScore = centerX - 80f;
            float colWave = centerX + 60f;
            float colAcc = centerX + 160f;
            float startY = _height * 0.46f;
            float rowHeight = _height * 0.09f;

            spriteBatch.DrawString(_font, "NAME", new Vector2(colName, startY - 28), Color.Gray);
            spriteBatch.DrawString(_font, "SCORE", new Vector2(colScore, startY - 28), Color.Gray);
            spriteBatch.DrawString(_font, "WAVE", new Vector2(colWave, startY - 28), Color.Gray);
            spriteBatch.DrawString(_font, "ACCURACY", new Vector2(colAcc, startY - 28), Color.Gray);

            // Top 5 rows
            for (int i = 0; i < top5.Count; i++)
            {
                var r = top5[i];
                float y = startY + 15f + i * rowHeight;

                bool isCurrentPlayer = r.PlayerName == result.PlayerName && r.FinalScore == result.FinalScore;
                Color rowColor = isCurrentPlayer ? Color.LightGreen : Color.White;

                string rank = $"{i + 1}.";
                spriteBatch.DrawString(_font, rank, new Vector2(colName - 30, y), rowColor);
                spriteBatch.DrawString(_font, r.PlayerName, new Vector2(colName, y), rowColor);
                spriteBatch.DrawString(_font, $"{r.FinalScore}", new Vector2(colScore, y), rowColor);
                spriteBatch.DrawString(_font, $"{r.WavesReached}", new Vector2(colWave, y), rowColor);
                spriteBatch.DrawString(_font, $"{r.Accuracy:P0}", new Vector2(colAcc, y), rowColor);
            }

            // Back prompt
            string back = "Press ENTER to return to menu";
            Vector2 backSize = _font.MeasureString(back);
            spriteBatch.DrawString(_font, back,
                new Vector2(centerX - backSize.X / 2, _height * 0.92f), Color.Gray);
        }
    }
}