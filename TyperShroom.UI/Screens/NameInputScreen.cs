using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TyperShroom.UI.Screens
{
    public class NameInputScreen
    {
        private SpriteFont _font;
        private int _width;
        private int _height;

        private string _name = "";
        public bool Confirmed { get; private set; } = false;
        public string PlayerName => _name;

        public NameInputScreen(SpriteFont font, int width, int height)
        {
            _font = font;
            _width = width;
            _height = height;
        }

        public void Reset()
        {
            _name = "";
            Confirmed = false;
        }

        public void Update(KeyboardState keyboard, KeyboardState previous)
        {
            // Enter confirms name (min 1 character)
            if (keyboard.IsKeyDown(Keys.Enter) && !previous.IsKeyDown(Keys.Enter))
            {
                if (_name.Length > 0)
                    Confirmed = true;
                return;
            }

            // Backspace deletes last character
            if (keyboard.IsKeyDown(Keys.Back) && !previous.IsKeyDown(Keys.Back))
            {
                if (_name.Length > 0)
                    _name = _name.Substring(0, _name.Length - 1);
                return;
            }

            // Letters A-Z (max 12 characters)
            if (_name.Length < 12)
            {
                foreach (Keys key in keyboard.GetPressedKeys())
                {
                    if (!previous.IsKeyDown(key) && key >= Keys.A && key <= Keys.Z)
                    {
                        _name += (char)('A' + (key - Keys.A));
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string title = "GAME OVER";
            Vector2 titleSize = _font.MeasureString(title);
            spriteBatch.DrawString(_font, title,
                new Vector2(_width / 2f - titleSize.X / 2, _height * 0.25f), Color.Red);

            string prompt = "Enter your name:";
            Vector2 promptSize = _font.MeasureString(prompt);
            spriteBatch.DrawString(_font, prompt,
                new Vector2(_width / 2f - promptSize.X / 2, _height * 0.42f), Color.White);

            // Name input display
            string display = _name + "_";
            Vector2 nameSize = _font.MeasureString(display);
            spriteBatch.DrawString(_font, display,
                new Vector2(_width / 2f - nameSize.X / 2, _height * 0.52f), Color.Yellow);

            string hint = "Press ENTER to confirm";
            Vector2 hintSize = _font.MeasureString(hint);
            spriteBatch.DrawString(_font, hint,
                new Vector2(_width / 2f - hintSize.X / 2, _height * 0.65f), Color.Gray);
        }
    }
}