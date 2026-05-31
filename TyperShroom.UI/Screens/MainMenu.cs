using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TyperShroom.UI.Screens
{
    public class MainMenu
    {
        private SpriteFont _font;
        private Texture2D _title;
        private int _width;
        private int _height;

        public bool StartGame { get; private set; } = false;
        public bool OpenHighScores { get; private set; } = false;

        public MainMenu(SpriteFont font, Texture2D title, int width, int height)
        {
            _font = font;
            _title = title;
            _width = width;
            _height = height;
        }

        public void Reset()
        {
            StartGame = false;
            OpenHighScores = false;
        }

        public void Update(KeyboardState keyboard, KeyboardState previous)
        {
            if (keyboard.IsKeyDown(Keys.Enter) && !previous.IsKeyDown(Keys.Enter))
                StartGame = true;

            if (keyboard.IsKeyDown(Keys.Space) && !previous.IsKeyDown(Keys.Space))
                OpenHighScores = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float titleScale = (float)_width * 0.5f / _title.Width;
            spriteBatch.Draw(_title,
                new Vector2(_width / 2f + 20f, _height * 0.32f), null, Color.White, 0f,
                new Vector2(_title.Width / 2f, _title.Height / 2f),
                titleScale, SpriteEffects.None, 0f);

            string start = "Press ENTER to start";
            Vector2 startSize = _font.MeasureString(start);
            spriteBatch.DrawString(_font, start,
                new Vector2(_width / 2f - startSize.X / 2, _height * 0.5f), Color.White);

            string scores = "Press SPACE for High Scores";
            Vector2 scoresSize = _font.MeasureString(scores);
            spriteBatch.DrawString(_font, scores,
                new Vector2(_width / 2f - scoresSize.X / 2, _height * 0.6f), Color.Yellow);

            string exit = "Press ESC to exit";
            Vector2 exitSize = _font.MeasureString(exit);
            spriteBatch.DrawString(_font, exit,
                new Vector2(_width / 2f - exitSize.X / 2, _height * 0.7f), Color.Yellow);
        }
    }
}