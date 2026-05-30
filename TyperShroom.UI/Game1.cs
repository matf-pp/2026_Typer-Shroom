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

    private Texture2D _background, _mushroom, _spider, _ant, _worm, _mosquito, _fly, _butterfly, _pixel;

    private MainMenu? _mainMenu;
    private bool _gameStarted = false;

    private SpriteFont? _font;

    private enum Screen { MainMenu, Game, Result }

    private Screen _currentScreen = Screen.MainMenu;
    private ResultScreen? _resultScreen;
    private GameResult? _lastResult;
    private double _waveClearedTimer = 3.0;
    private bool _showWaveCleared = false;

    private KeyboardState _previousKeyboard;
    public Game1()
    {
        // Initialize GPU and window
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;

        // Images and fonts location
        Content.RootDirectory = "Content";

        IsMouseVisible = true;

        _engine = new GameEngine();

        // code that runs when wave is cleared
        _engine.OnWaveCleared += () => {
            _showWaveCleared = true;
            _waveClearedTimer = 3.0;
            _engine.CurrentState.IsWaveClearing = true;
        };
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

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _background = Content.Load<Texture2D>("images/background");
        _mushroom   = Content.Load<Texture2D>("images/mushroom");
        _spider     = Content.Load<Texture2D>("images/spider");
        _butterfly  = Content.Load<Texture2D>("images/butterfly");
        _ant        = Content.Load<Texture2D>("images/ant");
        _fly        = Content.Load<Texture2D>("images/fly");
        _mosquito   = Content.Load<Texture2D>("images/mosquito");
        _worm       = Content.Load<Texture2D>("images/worm");

        
        _font = Content.Load<SpriteFont>("DefaultFont");

        _mainMenu = new MainMenu(
            _font,
            _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight
        );

        _resultScreen = new ResultScreen(
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

        if (!_showWaveCleared)
        {
            foreach (Keys key in keyboard.GetPressedKeys())
            {
                // if the key is down now AND was already down last frame, skip it. Only
                // Only process it if it's new.
                if (key >= Keys.A && key <= Keys.Z && !_previousKeyboard.IsKeyDown(key))
                {
                    _engine.ProcessKeystroke((char)('a' + (key - Keys.A)));
                }
            }
        }
        
        // if(keyboard.IsKeyDown(Keys.G))
        // {
        //     _lastResult = _engine.EndGame();
        //     _currentScreen = Screen.Result;
        // }

        _engine.Update(gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);

        // if(!_gameStarted)
        // {
        //     _mainMenu?.Update(keyboard);
        //     if(_mainMenu?.StartGame == true)
        //         _gameStarted = true;
        //     return;
        // } 

        if (_currentScreen == Screen.MainMenu)
        {
            _mainMenu?.Update(keyboard, _previousKeyboard);
            if (_mainMenu?.StartGame == true) 
            {
                _currentScreen = Screen.Game;
                _engine.StartGame();
            }
            _previousKeyboard = keyboard;
            return;
        }

        if (_currentScreen == Screen.Game)
        {
            
            if (_engine.CurrentState.IsGameOver)
            {
                _lastResult = _engine.EndGame();
                _currentScreen = Screen.Result;   
            }

            if (_showWaveCleared)
            {
                _waveClearedTimer -= gameTime.ElapsedGameTime.TotalSeconds; // timer -= deltaTime
                if (_waveClearedTimer <= 0)
                {
                    _showWaveCleared = false;
                    _engine.CurrentState.IsWaveClearing = false;
                }
            }
        }

        if (_currentScreen == Screen.Result)
        {
            _resultScreen?.Update(keyboard, _previousKeyboard);
            if (_resultScreen?.ReturnToMenu == true)
            {
                _currentScreen = Screen.MainMenu;
                _mainMenu?.Reset();
                _resultScreen.Reset();
            }
            _previousKeyboard = keyboard;
            return;
        }

        _previousKeyboard = keyboard;
    }

    protected override void Draw(GameTime gameTime)
    {
        // Clears last frame
        GraphicsDevice.Clear(Color.Black);

        // if(!_gameStarted)
        // {
        //     _spriteBatch?.Begin();
        //     _mainMenu?.Draw(_spriteBatch!);
        //     _spriteBatch?.End();
        //     return;
        // }

        if (_currentScreen == Screen.MainMenu)
        {
            _spriteBatch?.Begin();
            _mainMenu?.Draw(_spriteBatch!);
            _spriteBatch?.End();
            return;
        }

        if (_currentScreen == Screen.Result)
        {
            _spriteBatch?.Begin();
            _resultScreen?.Draw(_spriteBatch!, _lastResult!);
            _spriteBatch?.End();
            return;
        }

        var state = _engine.CurrentState;
        int width  = GraphicsDevice.Viewport.Width;
        int height = GraphicsDevice.Viewport.Height;


        // Begin the sprite batch to prepare for rendering
        _spriteBatch?.Begin();

        // Draw the texture at the center of the window
        float bgScale = Math.Max((float)width / _background.Width, (float)height / _background.Height);
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
            bgScale,                    // scale
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
        // first draw non targeted bug first
        foreach (var bug in state.ActiveBugs)
        {
            if (bug == state.CurrentTarget) continue;

            string typed = bug.Word.Substring(0, bug.Word.Length - bug.RemainingWord.Length);
            string remaining = bug.RemainingWord;

            Vector2 pos = new Vector2((float)bug.PositionX / 100f * width, bug.PositionY / 100f * height);
            Vector2 typedSize = _font.MeasureString(typed);
            
            Texture2D? bugTexture = bug.BugType switch
            {
                "spider"    => _spider,
                "butterfly" => _butterfly,
                "ant"       => _ant,
                "fly"       => _fly,
                "mosquito"  => _mosquito,
                "worm"      => _worm,
                _           => _mushroom
            };

            // Texture normalization
            float targetHeight = height * 0.15f;
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

            // Background behind word
            Vector2 wordSize = _font.MeasureString(bug.Word);
            _spriteBatch?.Draw(_pixel, new Rectangle((int)pos.X - 2, (int)pos.Y + 15, (int)wordSize.X + 4, (int)wordSize.Y + 4), Color.Black * 0.5f);

            // Typed letters are gray
            _spriteBatch?.DrawString(_font, typed, new Vector2(pos.X, pos.Y + 15), Color.Gray);

            // Remaining letters are red
            _spriteBatch?.DrawString(_font, remaining, new Vector2(pos.X + typedSize.X, pos.Y + 15), Color.Red);
        }

        // then draw targeted bug
        foreach (var bug in state.ActiveBugs)
        {
            if (bug != state.CurrentTarget) continue;

            string typed = bug.Word.Substring(0, bug.Word.Length - bug.RemainingWord.Length);
            string remaining = bug.RemainingWord;

            Vector2 pos = new Vector2((float)bug.PositionX / 100f * width, bug.PositionY / 100f * height);
            Vector2 typedSize = _font.MeasureString(typed);
            
            Texture2D? bugTexture = bug.BugType switch
            {
                "spider"    => _spider,
                "butterfly" => _butterfly,
                "ant"       => _ant,
                "fly"       => _fly,
                "mosquito"  => _mosquito,
                "worm"      => _worm,
                _           => _mushroom
            };

            // Texture normalization
            float targetHeight = height * 0.15f;
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

            // Background behind word
            Vector2 wordSize = _font.MeasureString(bug.Word);
            _spriteBatch?.Draw(_pixel, new Rectangle((int)pos.X - 2, (int)pos.Y + 15, (int)wordSize.X + 4, (int)wordSize.Y + 4), Color.Black * 0.5f);

            // Typed letters are gray
            _spriteBatch?.DrawString(_font, typed, new Vector2(pos.X, pos.Y + 15), Color.Gray);

            // Remaining letters are red
            _spriteBatch?.DrawString(_font, remaining, new Vector2(pos.X + typedSize.X, pos.Y + 15), Color.Red);
        }

        // input
        string input = "Target: None";
        if (state.CurrentTarget != null)
            input = $"Target: {state.CurrentTarget.Word}";

        Vector2 size = _font.MeasureString(input);
        _spriteBatch?.DrawString(
                                 _font,
                                 input,
                                 new Vector2(0.5f * width - size.X / 2,
                                             0.9f * height),
                                 Color.Yellow
                                );


        // wave
        if (_showWaveCleared)
        {
            string waveText = $"Wave {_engine.CurrentState.Wave - 1} cleared!";
            Vector2 waveTextSize = _font.MeasureString(waveText);
            _spriteBatch?.DrawString(
                _font,
                waveText,
                new Vector2(width / 2f - waveTextSize.X / 2f, height / 2f - waveTextSize.Y / 2f),
                Color.Black
            );
        }

        _spriteBatch?.End();

        base.Draw(gameTime);
    }
}