using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TyperShroom.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TyperShroom.UI.Screens
{
    public class HighScoreScreen
    {
        private SpriteFont _font;
        private int _width;
        private int _height;

        public bool ReturnToMenu { get; private set; } = false;

        public HighScoreScreen(SpriteFont font, int width, int height)
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

        public void Draw(SpriteBatch spriteBatch, List<GameResult> top10, Texture2D background)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, _width, _height), Color.White);

            int centerX = _width / 2;

            // Title
            string title = "HIGH SCORES";
            Vector2 titleSize = _font.MeasureString(title);
            spriteBatch.DrawString(_font, title,
                new Vector2(centerX - titleSize.X / 2, _height * 0.08f), Color.Yellow);

            // Column headers
            float colRank = centerX - 320f;
            float colName = centerX - 250f;
            float colScore = centerX - 20f;
            float colWave = centerX + 130f;
            float colAcc = centerX + 230f;
            float rowHeight = _height * 0.057f;
            float startY = _height * 0.38f - 2 * rowHeight;

            spriteBatch.DrawString(_font, "#", new Vector2(colRank, startY - 40), Color.Gray);
            spriteBatch.DrawString(_font, "NAME", new Vector2(colName, startY - 40), Color.Gray);
            spriteBatch.DrawString(_font, "SCORE", new Vector2(colScore, startY - 40), Color.Gray);
            spriteBatch.DrawString(_font, "WAVE", new Vector2(colWave, startY - 40), Color.Gray);
            spriteBatch.DrawString(_font, "ACCURACY", new Vector2(colAcc, startY - 40), Color.Gray);

            // Separator line
            string line = "----------------------------------------------------";
            Vector2 lineSize = _font.MeasureString(line);
            spriteBatch.DrawString(_font, line,
                new Vector2(centerX - lineSize.X / 2, startY - 15), Color.DarkGray);

            // Top 10 rows
            Color[] rankColors = { Color.Gold, Color.Silver, new Color(205, 127, 50), Color.White, Color.White,
                                   Color.White, Color.White, Color.White, Color.White, Color.White };

            for (int i = 0; i < top10.Count; i++)
            {
                var r = top10[i];
                float y = startY + 30f + i * rowHeight;
                Color rowColor = rankColors[i];

                spriteBatch.DrawString(_font, $"{i + 1}.", new Vector2(colRank, y), rowColor);
                spriteBatch.DrawString(_font, r.PlayerName, new Vector2(colName, y), rowColor);
                spriteBatch.DrawString(_font, $"{r.FinalScore}", new Vector2(colScore, y), rowColor);
                spriteBatch.DrawString(_font, $"{r.WavesReached}", new Vector2(colWave, y), rowColor);
                spriteBatch.DrawString(_font, $"{r.Accuracy:P0}", new Vector2(colAcc, y), rowColor);
            }

            // Back prompt
            string back = "Press ENTER to return";
            Vector2 backSize = _font.MeasureString(back);
            spriteBatch.DrawString(_font, back,
                new Vector2(centerX - backSize.X / 2, _height * 0.93f), Color.Gray);
        }
    }
}