using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
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

    private Texture2D _background, _mainMenuBackground, _mushroom, _spider, _spiderSpritesheet, _ant, _antSpritesheet, _worm, _wormSpritesheet, _mosquito, _mosquitoSpritesheet, _fly, _flySpritesheet, _butterfly, _butterflySpritesheet, _splashSpritesheet, _pixel;
    private Texture2D _mushroom2, _mushroom3, _mushroom4;
    private SoundEffect _squashSound, _eatSound;
    private double _mushroomFlashTimer = 0;
    private int frame = 0;
    private double frameTime = 0;

    private struct SplashEffect { public float PosX, PosY; public double Timer; }
    private List<SplashEffect> _splashEffects = new();

    private MainMenu? _mainMenu;
    private bool _gameStarted = false;

    private SpriteFont? _font;

    private enum Screen { MainMenu, Game, GameOver, Result }

    private Screen _currentScreen = Screen.MainMenu;
    private GameResult? _lastResult;
    private double _waveClearedTimer = 3.0;
    private double _gameOverTimer = 0;
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

        _engine.OnBugReached += (bug) => {
            _mushroomFlashTimer = 0.4;
            _eatSound.Play();
        };

        _engine.OnBugKilled += (bug) => {
            _splashEffects.Add(new SplashEffect { PosX = (float)bug.PositionX, PosY = bug.PositionY, Timer = 0 });
            _squashSound.Play();
        };

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
        _mainMenuBackground = Content.Load<Texture2D>("images/main-menu-background");
        _mushroom   = Content.Load<Texture2D>("images/mushroom");
        _mushroom2  = Content.Load<Texture2D>("images/mushroom_2");
        _mushroom3  = Content.Load<Texture2D>("images/mushroom_3");
        _mushroom4  = Content.Load<Texture2D>("images/mushroom-4");
        _spider     = Content.Load<Texture2D>("images/spider");
        _spiderSpritesheet = Content.Load<Texture2D>("images/spider-spritesheet");
        _butterfly  = Content.Load<Texture2D>("images/butterfly");
        _butterflySpritesheet = Content.Load<Texture2D>("images/butterfly-spritesheet");
        _ant        = Content.Load<Texture2D>("images/ant");
        _antSpritesheet = Content.Load<Texture2D>("images/ant-spritesheet");
        _fly        = Content.Load<Texture2D>("images/fly");
        _flySpritesheet = Content.Load<Texture2D>("images/fly-spritesheet");
        _mosquito   = Content.Load<Texture2D>("images/mosquito");
        _mosquitoSpritesheet = Content.Load<Texture2D>("images/mosquito-spritesheet");
        _worm       = Content.Load<Texture2D>("images/worm");
        _wormSpritesheet = Content.Load<Texture2D>("images/worm-spritesheet");
        _splashSpritesheet = Content.Load<Texture2D>("images/splash-spritesheet");
        _squashSound = Content.Load<SoundEffect>("sounds/squash");
        _eatSound = Content.Load<SoundEffect>("sounds/eat");

        
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
        
        // animation handler
        // change frame every 0.15 seconds (there are 4 differnet frames for each bug animation)
        frameTime += gameTime.ElapsedGameTime.TotalSeconds;
        if (frameTime >= 0.1)
        {
            frame = (frame + 1) % 4;
            frameTime = 0;
        }

        _engine.Update(gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);

        if (_currentScreen == Screen.MainMenu)
        {
            _mainMenu?.Update(keyboard, _previousKeyboard);
            if (_mainMenu?.StartGame == true) 
            {
                _currentScreen = Screen.Game;
                _engine.StartGame();
                _mushroomFlashTimer = 0;
                _showWaveCleared = false;
                _splashEffects.Clear();
            }
            _previousKeyboard = keyboard;
            return;
        }

        if (_currentScreen == Screen.Game)
        {
            
            if (_engine.CurrentState.IsGameOver)
            {
                _lastResult = _engine.EndGame();
                _gameOverTimer = 0;
                _currentScreen = Screen.GameOver;
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

            double dt = gameTime.ElapsedGameTime.TotalSeconds;
            if (_mushroomFlashTimer > 0) _mushroomFlashTimer -= dt;
            for (int i = _splashEffects.Count - 1; i >= 0; i--)
            {
                var e = _splashEffects[i];
                e.Timer += dt;
                if (e.Timer >= 0.4) _splashEffects.RemoveAt(i);
                else _splashEffects[i] = e;
            }
        }

        if (_currentScreen == Screen.GameOver)
        {
            if (_gameOverTimer > 0)
                _gameOverTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            else if (keyboard.IsKeyDown(Keys.Enter) && !_previousKeyboard.IsKeyDown(Keys.Enter))
            {
                _currentScreen = Screen.MainMenu;
                _mainMenu?.Reset();
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
            int mw = GraphicsDevice.Viewport.Width;
            int mh = GraphicsDevice.Viewport.Height;
            _spriteBatch?.Begin();
            float mmBgScale = Math.Max((float)mw / _mainMenuBackground.Width, (float)mh / _mainMenuBackground.Height);
            _spriteBatch?.Draw(_mainMenuBackground, new Vector2(mw, mh) * 0.5f, null, Color.White, 0f, new Vector2(_mainMenuBackground.Width, _mainMenuBackground.Height) * 0.5f, mmBgScale, SpriteEffects.None, 0f);
            _mainMenu?.Draw(_spriteBatch!);
            _spriteBatch?.End();
            return;
        }

        if (_currentScreen == Screen.GameOver)
        {
            int w = GraphicsDevice.Viewport.Width;
            int h = GraphicsDevice.Viewport.Height;
            _spriteBatch?.Begin();
            float goBgScale = Math.Max((float)w / _background.Width, (float)h / _background.Height);
            _spriteBatch?.Draw(_background, new Vector2(w, h) * 0.5f, null, Color.White, 0f, new Vector2(_background.Width, _background.Height) * 0.5f, goBgScale, SpriteEffects.None, 0f);
            float mScale = 0.44f;
            _spriteBatch?.Draw(_mushroom4, new Vector2(w * 0.19f, h * 0.60f), null, Color.White, 0f, new Vector2(_mushroom4.Width / 2f, _mushroom4.Height / 2f), mScale, SpriteEffects.None, 0f);
            string gameOverText = "GAME OVER";
            Vector2 goSize = _font!.MeasureString(gameOverText);
            _spriteBatch?.DrawString(_font, gameOverText, new Vector2(w / 2f - goSize.X / 2f, h * 0.2f), Color.Red);
            if (_gameOverTimer <= 0 && _lastResult != null)
            {
                _spriteBatch?.DrawString(_font, $"Score: {_lastResult.FinalScore}", new Vector2(w / 2f - 100, h * 0.38f), Color.White);
                _spriteBatch?.DrawString(_font, $"Wave reached: {_lastResult.WavesReached}", new Vector2(w / 2f - 100, h * 0.46f), Color.White);
                _spriteBatch?.DrawString(_font, $"Accuracy: {_lastResult.Accuracy:P0}", new Vector2(w / 2f - 100, h * 0.54f), Color.White);
                string back = "Press ENTER to return to menu";
                Vector2 backSize = _font.MeasureString(back);
                _spriteBatch?.DrawString(_font, back, new Vector2(w / 2f - backSize.X / 2f, h * 0.8f), Color.Gray);
            }
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
        float scale = 0.44f;
        Texture2D mushroomTex = state.Lives >= 3 ? _mushroom : state.Lives == 2 ? _mushroom2 : state.Lives == 1 ? _mushroom3 : _mushroom4;
        Color mushroomColor = _mushroomFlashTimer > 0 ? Color.Red : Color.White;
        _spriteBatch?.Draw(
            mushroomTex,
            new Vector2(width * 0.19f, height * 0.52f),
            null,
            mushroomColor,
            0f,
            new Vector2(mushroomTex.Width / 2f, mushroomTex.Height / 2f),
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
                "spider"    => _spiderSpritesheet,
                "butterfly" => _butterflySpritesheet,
                "ant"       => _antSpritesheet,
                "fly"       => _flySpritesheet,
                "mosquito"  => _mosquitoSpritesheet,
                "worm"      => _wormSpritesheet,
                _           => _mushroom
            };

            // Texture normalization
            float targetHeight = height * 0.6f;
            float bugScale = targetHeight / bugTexture.Height;

            Rectangle? sourceRect = null;
            if (bug.BugType == "fly") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "butterfly") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "worm") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "mosquito") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "spider") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "ant") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);

            // Bug texture
            _spriteBatch?.Draw(
                bugTexture,
                new Vector2(pos.X, pos.Y),
                sourceRect,
                Color.White,
                0f,
                new Vector2(sourceRect.HasValue ? sourceRect.Value.Width / 2f : bugTexture.Width / 2f, bugTexture.Height / 2f),
                bugScale,
                SpriteEffects.None,
                0f
            );

            // Background behind word
            Vector2 wordSize = _font.MeasureString(bug.Word);
            float wordOffsetX = bug.BugType == "fly" ? -125f + 192f * bugScale : bug.BugType == "ant" ? -35f : bug.BugType == "mosquito" ? -40f : -30f;
            float wordStartX = pos.X - wordSize.X / 2f + wordOffsetX;
            _spriteBatch?.Draw(_pixel, new Rectangle((int)wordStartX - 2, (int)pos.Y + 15, (int)wordSize.X + 4, (int)wordSize.Y + 4), Color.Black * 0.5f);

            // Typed letters are gray
            _spriteBatch?.DrawString(_font, typed, new Vector2(wordStartX, pos.Y + 15), Color.Gray);

            // Remaining letters are red
            _spriteBatch?.DrawString(_font, remaining, new Vector2(wordStartX + typedSize.X, pos.Y + 15), Color.Red);
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
                "spider"    => _spiderSpritesheet,
                "butterfly" => _butterflySpritesheet,
                "ant"       => _antSpritesheet,
                "fly"       => _flySpritesheet,
                "mosquito"  => _mosquitoSpritesheet,
                "worm"      => _wormSpritesheet,
                _           => _mushroom
            };

            // Texture normalization
            float targetHeight = height * 0.6f;
            float bugScale = targetHeight / bugTexture.Height;

            Rectangle? sourceRect = null;
            if (bug.BugType == "fly") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "butterfly") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "worm") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "mosquito") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "spider") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "ant") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);

            // Bug texture
            _spriteBatch?.Draw(
                bugTexture,
                new Vector2(pos.X, pos.Y),
                sourceRect,
                Color.White,
                0f,
                new Vector2(sourceRect.HasValue ? sourceRect.Value.Width / 2f : bugTexture.Width / 2f, bugTexture.Height / 2f),
                bugScale,
                SpriteEffects.None,
                0f
            );

            // Background behind word
            Vector2 wordSize = _font.MeasureString(bug.Word);
            float wordOffsetX = bug.BugType == "fly" ? -125f + 192f * bugScale : bug.BugType == "ant" ? -35f : bug.BugType == "mosquito" ? -40f : -30f;
            float wordStartX = pos.X - wordSize.X / 2f + wordOffsetX;
            _spriteBatch?.Draw(_pixel, new Rectangle((int)wordStartX - 2, (int)pos.Y + 15, (int)wordSize.X + 4, (int)wordSize.Y + 4), Color.Black * 0.5f);

            // Typed letters are gray
            _spriteBatch?.DrawString(_font, typed, new Vector2(wordStartX, pos.Y + 15), Color.Gray);

            // Remaining letters are red
            _spriteBatch?.DrawString(_font, remaining, new Vector2(wordStartX + typedSize.X, pos.Y + 15), Color.Red);
        }

        // splash death effects
        foreach (var splash in _splashEffects)
        {
            int splashFrame = Math.Min((int)(splash.Timer / 0.05), 7);
            Vector2 splashPos = new Vector2(splash.PosX / 100f * width, splash.PosY / 100f * height);
            float splashScale = (height * 0.6f) / _splashSpritesheet.Height;
            _spriteBatch?.Draw(
                _splashSpritesheet,
                splashPos,
                new Rectangle(splashFrame * 384, 0, 384, 1024),
                Color.White,
                0f,
                new Vector2(270f, _splashSpritesheet.Height / 2f),
                splashScale,
                SpriteEffects.None,
                0f
            );
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
            Vector2 waveTextSize = _font!.MeasureString(waveText);
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