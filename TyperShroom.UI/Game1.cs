using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TyperShroom.Core;
using TyperShroom.Data;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TyperShroom.UI.Screens;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;
    private IGameEngine _engine;

    // put null! to remove warning, all fields are used and assigned later
    private Texture2D _background = null!, _mainMenuBackground = null!, _mushroom = null!, _spiderSpritesheet = null!, _antSpritesheet = null!, _wormSpritesheet = null!, _mosquitoSpritesheet = null!, _flySpritesheet = null!, _butterflySpritesheet = null!, _splashSpritesheet = null!, _pixel = null!, _heart = null!, _title = null!;
    private Texture2D _mushroom2 = null!, _mushroom3 = null!, _mushroom4 = null!;
    private SoundEffect _squashSound = null!, _eatSound = null!, _mistypeSound = null!;
    private Song _backgroundMusic = null!;
    private double _mushroomFlashTimer = 0;
    private int frame = 0;
    private double frameTime = 0;

    private struct SplashEffect { public float PosX, PosY; public double Timer; }
    private List<SplashEffect> _splashEffects = new();

    private MainMenu? _mainMenu;
    private SpriteFont _font = null!;

    private enum Screen { MainMenu, Game, NameInput, Result, HighScores }

    private Screen _currentScreen = Screen.MainMenu;
    private ResultScreen? _resultScreen;
    private NameInputScreen? _nameInputScreen;
    private HighScoreScreen? _highScoreScreen;
    private GameResult? _lastResult;
    private double _waveClearedTimer = 3.0;
    private bool _showWaveCleared = false;
    private KeyboardState _previousKeyboard;

    private ScoreRepository _scoreRepository = new ScoreRepository();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _engine = new GameEngine();

        _engine.OnBugReached += (bug) =>
        {
            _mushroomFlashTimer = 0.4;
            _eatSound.Play();
        };

        _engine.OnBugKilled += (bug) =>
        {
            _splashEffects.Add(new SplashEffect { PosX = (float)bug.PositionX, PosY = bug.PositionY, Timer = 0 });
            _squashSound.Play();
        };

        _engine.OnBugMistyped += (bug) => _mistypeSound.Play();
        _engine.OnKeyNotMatched += () => _mistypeSound.Play();

        _engine.OnWaveCleared += () => {
            _showWaveCleared = true;
            _waveClearedTimer = 3.0;
            _engine.CurrentState.IsWaveClearing = true;
        };
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _background           = Content.Load<Texture2D>("images/background");
        _mainMenuBackground   = Content.Load<Texture2D>("images/main-menu-background");
        _mushroom             = Content.Load<Texture2D>("images/mushroom");
        _mushroom2            = Content.Load<Texture2D>("images/mushroom_2");
        _mushroom3            = Content.Load<Texture2D>("images/mushroom_3");
        _mushroom4            = Content.Load<Texture2D>("images/mushroom-4");
        _heart                = Content.Load<Texture2D>("images/heart");
        _title                = Content.Load<Texture2D>("images/title");
        _spiderSpritesheet    = Content.Load<Texture2D>("images/spider-spritesheet");
        _butterflySpritesheet = Content.Load<Texture2D>("images/butterfly-spritesheet");
        _antSpritesheet       = Content.Load<Texture2D>("images/ant-spritesheet");
        _flySpritesheet       = Content.Load<Texture2D>("images/fly-spritesheet");
        _mosquitoSpritesheet  = Content.Load<Texture2D>("images/mosquito-spritesheet");
        _wormSpritesheet      = Content.Load<Texture2D>("images/worm-spritesheet");
        _splashSpritesheet    = Content.Load<Texture2D>("images/splash-spritesheet");
        _squashSound          = Content.Load<SoundEffect>("sounds/squash");
        _eatSound             = Content.Load<SoundEffect>("sounds/eat");
        _mistypeSound         = Content.Load<SoundEffect>("sounds/mistype");
        _backgroundMusic      = Song.FromUri("background-music", new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/sounds/background-music.ogg")));

        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_backgroundMusic);

        _font = Content.Load<SpriteFont>("DefaultFont");

        _mainMenu        = new MainMenu(_font, _title, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _resultScreen    = new ResultScreen(_font, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _nameInputScreen = new NameInputScreen(_font, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _highScoreScreen = new HighScoreScreen(_font, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape) &&
            (_currentScreen == Screen.Game || _currentScreen == Screen.MainMenu))
            Exit();

        switch (_currentScreen)
        {
            case Screen.MainMenu:   UpdateMainMenu(keyboard); break;
            case Screen.HighScores: UpdateHighScores(keyboard); break;
            case Screen.Game:       UpdateGame(gameTime, keyboard); break;
            case Screen.NameInput:  UpdateNameInput(keyboard); break;
            case Screen.Result:     UpdateResult(keyboard); break;
            default: break;    
        }

        _previousKeyboard = keyboard;
        base.Update(gameTime);
    }

    private void UpdateMainMenu(KeyboardState keyboard)
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
        else if (_mainMenu?.OpenHighScores == true)
        {
            _highScoreScreen?.Reset();
            _currentScreen = Screen.HighScores;
        }
    }

    private void UpdateHighScores(KeyboardState keyboard)
    {
        _highScoreScreen?.Update(keyboard, _previousKeyboard);
        if (_highScoreScreen?.ReturnToMenu == true)
        {
            _mainMenu?.Reset();
            _currentScreen = Screen.MainMenu;
        }
    }

    private void UpdateGame(GameTime gameTime, KeyboardState keyboard)
    {
        ProcessKeyInput(keyboard);
        UpdateAnimation(gameTime);
        _engine.Update(gameTime.ElapsedGameTime.TotalSeconds);
        CheckGameOver();
        UpdateWaveCleared(gameTime);
        UpdateTimers(gameTime);
    }

    private void ProcessKeyInput(KeyboardState keyboard)
    {
        if (_showWaveCleared) return;
        foreach (Keys key in keyboard.GetPressedKeys())
        {
            if (key >= Keys.A && key <= Keys.Z && !_previousKeyboard.IsKeyDown(key))
                _engine.ProcessKeystroke((char)('a' + (key - Keys.A)));
        }
    }

    private void UpdateAnimation(GameTime gameTime)
    {
        frameTime += gameTime.ElapsedGameTime.TotalSeconds;
        if (frameTime >= 0.1)
        {
            frame = (frame + 1) % 4;
            frameTime = 0;
        }
    }

    private void CheckGameOver()
    {
        if (_engine.CurrentState.IsGameOver)
        {
            _lastResult = _engine.EndGame();
            _nameInputScreen?.Reset();
            _currentScreen = Screen.NameInput;
        }
    }

    private void UpdateWaveCleared(GameTime gameTime)
    {
        if (!_showWaveCleared) return;
        _waveClearedTimer -= gameTime.ElapsedGameTime.TotalSeconds;
        if (_waveClearedTimer <= 0)
        {
            _showWaveCleared = false;
            _engine.CurrentState.IsWaveClearing = false;
        }
    }

    private void UpdateTimers(GameTime gameTime)
    {
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

    private void UpdateNameInput(KeyboardState keyboard)
    {
        _nameInputScreen?.Update(keyboard, _previousKeyboard);
        if (_nameInputScreen?.Confirmed == true && _lastResult != null)
        {
            _lastResult.PlayerName = _nameInputScreen.PlayerName;
            _scoreRepository.Save(_lastResult);
            _resultScreen?.Reset();
            _currentScreen = Screen.Result;
        }
    }

    private void UpdateResult(KeyboardState keyboard)
    {
        _resultScreen?.Update(keyboard, _previousKeyboard);
        if (_resultScreen?.ReturnToMenu == true)
        {
            _currentScreen = Screen.MainMenu;
            _mainMenu?.Reset();
            _resultScreen.Reset();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

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

        if (_currentScreen == Screen.HighScores)
        {
            _spriteBatch?.Begin();
            _highScoreScreen?.Draw(_spriteBatch!, _scoreRepository.LoadTop10(), _mainMenuBackground);
            _spriteBatch?.End();
            return;
        }

        if (_currentScreen == Screen.NameInput)
        {
            _spriteBatch?.Begin();
            _nameInputScreen?.Draw(_spriteBatch!, _mainMenuBackground);
            _spriteBatch?.End();
            return;
        }

        if (_currentScreen == Screen.Result)
        {
            _spriteBatch?.Begin();
            _resultScreen?.Draw(_spriteBatch!, _lastResult!, _scoreRepository.LoadTop5(), _mainMenuBackground);
            _spriteBatch?.End();
            return;
        }

        var state = _engine.CurrentState;
        int width = GraphicsDevice.Viewport.Width;
        int height = GraphicsDevice.Viewport.Height;

        _spriteBatch?.Begin();

        float bgScale = Math.Max((float)width / _background.Width, (float)height / _background.Height);
        _spriteBatch?.Draw(_background, new Vector2(width, height) * 0.5f, null, Color.White, 0.0f,
            new Vector2(_background.Width, _background.Height) * 0.5f, bgScale, SpriteEffects.None, 0.0f);

        float scale = 0.44f;
        Texture2D mushroomTex = state.Lives >= 3 ? _mushroom : state.Lives == 2 ? _mushroom2 : state.Lives == 1 ? _mushroom3 : _mushroom4;
        Color mushroomColor = _mushroomFlashTimer > 0 ? Color.Red : Color.White;
        _spriteBatch?.Draw(mushroomTex, new Vector2(width * 0.19f, height * 0.52f), null, mushroomColor, 0f,
            new Vector2(mushroomTex.Width / 2f, mushroomTex.Height / 2f), scale, SpriteEffects.None, 0f);

        string livesLabel = "Lives: ";
        Vector2 livesLabelSize = _font.MeasureString(livesLabel);
        _spriteBatch?.DrawString(_font, livesLabel, new Vector2(10, 10), Color.White);

        float heartSize = 100f;
        float heartScale = heartSize / _heart.Height;
        float heartY = 10 + livesLabelSize.Y / 2f - heartSize / 2f + 5f;
        for (int i = 0; i < state.Lives; i++)
            _spriteBatch?.Draw(_heart, new Vector2(10 + livesLabelSize.X - 35 + i * (heartSize - 40), heartY), null, Color.White, 0f,
                Vector2.Zero, heartScale, SpriteEffects.None, 0f);

        _spriteBatch?.DrawString(_font, $"Score: {state.Score}   Wave: {state.Wave}",
            new Vector2(10, 10 + livesLabelSize.Y + 4), Color.White);

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

            float targetHeight = height * 0.6f;
            float bugScale = targetHeight / bugTexture.Height;

            Rectangle? sourceRect = null;
            if (bug.BugType == "fly") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "butterfly") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "worm") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "mosquito") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "spider") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "ant") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);

            _spriteBatch?.Draw(bugTexture, new Vector2(pos.X, pos.Y), sourceRect, Color.White, 0f,
                new Vector2(sourceRect.HasValue ? sourceRect.Value.Width / 2f : bugTexture.Width / 2f, bugTexture.Height / 2f),
                bugScale, SpriteEffects.None, 0f);

            Vector2 wordSize = _font.MeasureString(bug.Word);
            float wordOffsetX = bug.BugType == "fly" ? -125f + 192f * bugScale : bug.BugType == "ant" ? -35f : bug.BugType == "mosquito" ? -40f : -30f;
            float wordStartX = pos.X - wordSize.X / 2f + wordOffsetX;
            _spriteBatch?.Draw(_pixel, new Rectangle((int)wordStartX - 2, (int)pos.Y + 15, (int)wordSize.X + 4, (int)wordSize.Y + 4), Color.Black * 0.5f);
            _spriteBatch?.DrawString(_font, typed, new Vector2(wordStartX, pos.Y + 15), Color.Gray);
            _spriteBatch?.DrawString(_font, remaining, new Vector2(wordStartX + typedSize.X, pos.Y + 15), Color.Red);
        }

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

            float targetHeight = height * 0.6f;
            float bugScale = targetHeight / bugTexture.Height;

            Rectangle? sourceRect = null;
            if (bug.BugType == "fly") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "butterfly") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "worm") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "mosquito") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "spider") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);
            else if (bug.BugType == "ant") sourceRect = new Rectangle(frame * 384, 0, 384, 1024);

            _spriteBatch?.Draw(bugTexture, new Vector2(pos.X, pos.Y), sourceRect, Color.White, 0f,
                new Vector2(sourceRect.HasValue ? sourceRect.Value.Width / 2f : bugTexture.Width / 2f, bugTexture.Height / 2f),
                bugScale, SpriteEffects.None, 0f);

            Vector2 wordSize = _font.MeasureString(bug.Word);
            float wordOffsetX = bug.BugType == "fly" ? -125f + 192f * bugScale : bug.BugType == "ant" ? -35f : bug.BugType == "mosquito" ? -40f : -30f;
            float wordStartX = pos.X - wordSize.X / 2f + wordOffsetX;
            _spriteBatch?.Draw(_pixel, new Rectangle((int)wordStartX - 2, (int)pos.Y + 15, (int)wordSize.X + 4, (int)wordSize.Y + 4), Color.Black * 0.5f);
            _spriteBatch?.DrawString(_font, typed, new Vector2(wordStartX, pos.Y + 15), Color.Gray);
            _spriteBatch?.DrawString(_font, remaining, new Vector2(wordStartX + typedSize.X, pos.Y + 15), Color.Red);
        }

        foreach (var splash in _splashEffects)
        {
            int splashFrame = Math.Min((int)(splash.Timer / 0.05), 7);
            Vector2 splashPos = new Vector2(splash.PosX / 100f * width, splash.PosY / 100f * height);
            float splashScale = (height * 0.6f) / _splashSpritesheet.Height;
            _spriteBatch?.Draw(_splashSpritesheet, splashPos,
                new Rectangle(splashFrame * 384, 0, 384, 1024), Color.White, 0f,
                new Vector2(270f, _splashSpritesheet.Height / 2f), splashScale, SpriteEffects.None, 0f);
        }

        if (_showWaveCleared)
        {
            string waveText = $"Wave {_engine.CurrentState.Wave - 1} cleared!";
            Vector2 waveTextSize = _font.MeasureString(waveText);
            _spriteBatch?.DrawString(_font, waveText,
                new Vector2(width / 2f, height / 2f),
                new Color(255, 210, 80), 0f,
                new Vector2(waveTextSize.X / 2f, waveTextSize.Y / 2f),
                1.8f, SpriteEffects.None, 0f);
        }

        _spriteBatch?.DrawString(_font, "Press ESC to quit",
            new Vector2(10, height - _font.LineSpacing - 10), Color.Gray * 0.7f);

        _spriteBatch?.End();
        base.Draw(gameTime);
    }
}
