using System.Numerics;
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

        public void Draw(SpriteBatch spriteBatch, GameResult result)
        {
            string title = "GAME OVER";
            Vector2 titleSize = _font.MeasureString(title);
            spriteBatch.DrawString(
                _font,
                title,
                new Vector2(_width / 2f - titleSize.X / 2, _height * 0.2f),
                Color.Red
            );
            spriteBatch.DrawString(_font, $"Score: {result.FinalScore}", new Vector2(_width / 2f - 100, _height * 0.38f), Color.White);
            spriteBatch.DrawString(_font, $"Wave reached: {result.WavesReached}", new Vector2(_width / 2f - 100, _height * 0.46f), Color.White);
            spriteBatch.DrawString(_font, $"WPM: {result.WPM}", new Vector2(_width / 2f - 100, _height * 0.54f), Color.White);
            spriteBatch.DrawString(_font, $"Accuracy: {result.Accuracy:P0}", new Vector2(_width / 2f - 100, _height * 0.62f), Color.White);
        
            string back = "Press ENTER to return to menu";
            Vector2 backSize = _font.MeasureString(back);
            spriteBatch.DrawString(_font, back, new Vector2(_width / 2f - backSize.X / 2, _height * 0.8f), Color.Gray);
        }
    }
}