using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TyperShroom.UI.Screens
{
    public class MainMenu
    {
        private SpriteFont _font;
        private int _width;
        private int _height;

        public bool StartGame { get; private set; } = false;

        public MainMenu(SpriteFont font, int width, int height)
        {
            _font = font;
            _width = width;
            _height = height;
        }

        public void Reset()
        {
            StartGame = false;
        }

        public void Update(KeyboardState keyboard, KeyboardState previous)
        {
            if (keyboard.IsKeyDown(Keys.Enter) && !previous.IsKeyDown(Keys.Enter))
                StartGame = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string title = "TYPER SHROOM";
            Vector2 titleSize = _font.MeasureString(title);
            spriteBatch.DrawString(
                _font,
                title,
                new Vector2(_width / 2f - titleSize.X / 2, _height * 0.3f),
                Color.Green
            );

            string start = "Press ENTER to start";
            Vector2 startSize = _font.MeasureString(start);
            spriteBatch.DrawString(
                _font,
                start,
                new Vector2(_width / 2f - startSize.X / 2, _height * 0.5f),
                Color.White
            );
        }


    }
}