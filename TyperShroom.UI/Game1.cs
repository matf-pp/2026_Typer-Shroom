using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TyperShroom.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TyperShroom.UI.Screens;
public class Game1 : Game
{
    // Window creation, resolution, fullscreen
    private GraphicsDeviceManager _graphics;

    // Sprites, text, images
    private SpriteBatch? _spriteBatch;

    private IGameEngine _engine;

    private Texture2D _background, _mushroom, _spider, _beetle, _ant;

    private MainMenu? _mainMenu;
    private bool _gameStarted = false;

    private SpriteFont? _font;
    public Game1()
    {
        // Initialize GPU and window
        _graphics = new GraphicsDeviceManager(this);

        // Images and fonts location
        Content.RootDirectory = "Content";

        IsMouseVisible = true;

        _engine = new FakeGameEngine();
    }

    protected override void Initialize()
    {   
        // 
        base.Initialize();
    }

    protected override void LoadContent()
    {
        //
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _background = Content.Load<Texture2D>("images/background");
        _mushroom   = Content.Load<Texture2D>("images/mushroom");
        _spider     = Content.Load<Texture2D>("images/spider");
        _beetle     = Content.Load<Texture2D>("images/beetle");
        _ant        = Content.Load<Texture2D>("images/ant");

        
        _font = Content.Load<SpriteFont>("DefaultFont");

        _mainMenu = new MainMenu(
            _font,
            _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight
        );
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();

        foreach (Keys key in keyboard.GetPressedKeys())
        {
            if (key >= Keys.A && key <= Keys.Z)
            {
                _engine.ProcessKeystroke((char)('a' + (key - Keys.A)));
            }
        }

        _engine.Update(gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);

        if(!_gameStarted)
        {
            _mainMenu?.Update(keyboard);
            if(_mainMenu?.StartGame == true)
                _gameStarted = true;
            return;
        } 
    }

    protected override void Draw(GameTime gameTime)
    {
        // Clears last frame
        GraphicsDevice.Clear(Color.Black);

        if(!_gameStarted)
        {
            _spriteBatch?.Begin();
            _mainMenu?.Draw(_spriteBatch!);
            _spriteBatch?.End();
            return;
        }

        var state = _engine.CurrentState;
        int width  = GraphicsDevice.Viewport.Width;
        int height = GraphicsDevice.Viewport.Height;


        // Begin the sprite batch to prepare for rendering
        _spriteBatch?.Begin();

        // Draw the texture at the center of the window
        _spriteBatch?.Draw(
            _background,                // texture
            new Vector2(                // position
                width,
                height) * 0.5f,
            null,                       // sourceRectangle
            Color.White,                // color (tint, .White = no tint)
            0.0f,                       // rotation
            new Vector2(                // origin
                _background.Width,
                _background.Height) * 0.5f,
            1.0f,                       // scale
            SpriteEffects.None,         // effects
            0.0f                        // layerDepth
        );

        // Draw a mushroom
        float scale = 0.08f;
        _spriteBatch?.Draw(
            _mushroom,
            new Vector2(width * 0.15f, height * 0.55f),
            null,
            Color.White,
            0f,
            new Vector2(_mushroom.Width / 2f, _mushroom.Height / 2f),
            scale,
            SpriteEffects.None,
            0f
        );


        // HUD
        _spriteBatch?.DrawString(
                                 _font,                     // font
                                 $"Lives: {state.Lives}   Score: {state.Score}   Wave:  {state.Wave}",
                                  new Vector2(10, 10),      // pos
                                  Color.White               // color
                                );


        // bugs
        foreach (var bug in state.ActiveBugs)
        {
            string typed = bug.Word.Substring(0, bug.Word.Length - bug.RemainingWord.Length);
            string remaining = bug.RemainingWord;

            Vector2 pos = new Vector2(bug.PositionX / 100f * width, bug.PositionY / 100f * height);
            Vector2 typedSize = _font.MeasureString(typed);
            
            Texture2D? bugTexture = bug.BugType switch
            {
                "spider" => _spider,
                "beetle" => _beetle,
                "ant"    => _ant,
                _        => _mushroom
            };

            // Texture normalization
            float targetHeight = height * 0.1f;
            float bugScale = targetHeight / bugTexture.Height;

            // Bug texture
            _spriteBatch?.Draw(
                bugTexture,
                new Vector2(pos.X + 50, pos.Y),
                null,
                Color.White,
                0f,
                new Vector2(bugTexture.Width / 2f, bugTexture.Height / 2f),
                bugScale,
                SpriteEffects.None,
                0f
            );

            // Typed letters are gray
            _spriteBatch?.DrawString(_font, typed, pos, Color.Black);

            // Remaining letters are red
            _spriteBatch?.DrawString(_font, remaining, new Vector2(pos.X + typedSize.X, pos.Y), Color.Red);
        }

        // input
        string input = $"Input: {state.CurrentInput}";
        Vector2 size = _font.MeasureString(input);
        _spriteBatch?.DrawString(
                                 _font,
                                 input,
                                 new Vector2(0.5f * width - size.X / 2,
                                             0.9f * height),
                                 Color.Yellow
                                );

        _spriteBatch?.End();

        base.Draw(gameTime);
    }
}