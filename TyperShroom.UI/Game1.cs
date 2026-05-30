using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TyperShroom.Core;
using TyperShroom.Data;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TyperShroom.UI.Screens;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;
    private IGameEngine _engine;

    private Texture2D _background, _mushroom, _spider, _ant, _worm, _mosquito, _fly, _butterfly, _pixel;

    private MainMenu? _mainMenu;
    private SpriteFont? _font;

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

        _engine.OnWaveCleared += () => {
            _showWaveCleared = true;
            _waveClearedTimer = 3.0;
            _engine.CurrentState.IsWaveClearing = true;
        };
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _background = Content.Load<Texture2D>("images/background");
        _mushroom = Content.Load<Texture2D>("images/mushroom");
        _spider = Content.Load<Texture2D>("images/spider");
        _butterfly = Content.Load<Texture2D>("images/butterfly");
        _ant = Content.Load<Texture2D>("images/ant");
        _fly = Content.Load<Texture2D>("images/fly");
        _mosquito = Content.Load<Texture2D>("images/mosquito");
        _worm = Content.Load<Texture2D>("images/worm");

        _font = Content.Load<SpriteFont>("DefaultFont");

        _mainMenu = new MainMenu(_font, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _resultScreen = new ResultScreen(_font, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _nameInputScreen = new NameInputScreen(_font, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _highScoreScreen = new HighScoreScreen(_font, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape) && _currentScreen == Screen.Game)
            Exit();

        if (_currentScreen == Screen.MainMenu)
        {
            _mainMenu?.Update(keyboard, _previousKeyboard);
            if (_mainMenu?.StartGame == true)
            {
                _currentScreen = Screen.Game;
                _engine.StartGame();
            }
            else if (_mainMenu?.OpenHighScores == true)
            {
                _highScoreScreen?.Reset();
                _currentScreen = Screen.HighScores;
            }
            _previousKeyboard = keyboard;
            base.Update(gameTime);
            return;
        }

        if (_currentScreen == Screen.HighScores)
        {
            _highScoreScreen?.Update(keyboard, _previousKeyboard);
            if (_highScoreScreen?.ReturnToMenu == true)
            {
                _mainMenu?.Reset();
                _currentScreen = Screen.MainMenu;
            }
            _previousKeyboard = keyboard;
            base.Update(gameTime);
            return;
        }

        if (_currentScreen == Screen.Game)
        {
            if (!_showWaveCleared)
            {
                foreach (Keys key in keyboard.GetPressedKeys())
                {
                    if (key >= Keys.A && key <= Keys.Z && !_previousKeyboard.IsKeyDown(key))
                        _engine.ProcessKeystroke((char)('a' + (key - Keys.A)));
                }
            }

            _engine.Update(gameTime.ElapsedGameTime.TotalSeconds);

            if (_engine.CurrentState.IsGameOver)
            {
                _lastResult = _engine.EndGame();
                _nameInputScreen?.Reset();
                _currentScreen = Screen.NameInput;
            }

            if (_showWaveCleared)
            {
                _waveClearedTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_waveClearedTimer <= 0)
                {
                    _showWaveCleared = false;
                    _engine.CurrentState.IsWaveClearing = false;
                }
            }

            _previousKeyboard = keyboard;
            base.Update(gameTime);
            return;
        }

        if (_currentScreen == Screen.NameInput)
        {
            _nameInputScreen?.Update(keyboard, _previousKeyboard);
            if (_nameInputScreen?.Confirmed == true && _lastResult != null)
            {
                _lastResult.PlayerName = _nameInputScreen.PlayerName;
                _scoreRepository.Save(_lastResult);
                _resultScreen?.Reset();
                _currentScreen = Screen.Result;
            }
            _previousKeyboard = keyboard;
            base.Update(gameTime);
            return;
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
            base.Update(gameTime);
            return;
        }

        _previousKeyboard = keyboard;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        if (_currentScreen == Screen.MainMenu)
        {
            _spriteBatch?.Begin();
            _mainMenu?.Draw(_spriteBatch!);
            _spriteBatch?.End();
            return;
        }

        if (_currentScreen == Screen.HighScores)
        {
            _spriteBatch?.Begin();
            _highScoreScreen?.Draw(_spriteBatch!, _scoreRepository.LoadTop10());
            _spriteBatch?.End();
            return;
        }

        if (_currentScreen == Screen.NameInput)
        {
            _spriteBatch?.Begin();
            _nameInputScreen?.Draw(_spriteBatch!);
            _spriteBatch?.End();
            return;
        }

        if (_currentScreen == Screen.Result)
        {
            _spriteBatch?.Begin();
            _resultScreen?.Draw(_spriteBatch!, _lastResult!, _scoreRepository.LoadTop5());
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

        float scale = 0.08f;
        _spriteBatch?.Draw(_mushroom, new Vector2(width * 0.15f, height * 0.55f), null, Color.White, 0f,
            new Vector2(_mushroom.Width / 2f, _mushroom.Height / 2f), scale, SpriteEffects.None, 0f);

        _spriteBatch?.DrawString(_font, $"Lives: {state.Lives}   Score: {state.Score}   Wave:  {state.Wave}",
            new Vector2(10, 10), Color.White);

        foreach (var bug in state.ActiveBugs)
        {
            if (bug == state.CurrentTarget) continue;
            DrawBug(bug, width, height);
        }

        foreach (var bug in state.ActiveBugs)
        {
            if (bug != state.CurrentTarget) continue;
            DrawBug(bug, width, height);
        }

        string input = state.CurrentTarget != null ? $"Target: {state.CurrentTarget.Word}" : "Target: None";
        Vector2 size = _font.MeasureString(input);
        _spriteBatch?.DrawString(_font, input, new Vector2(0.5f * width - size.X / 2, 0.9f * height), Color.Yellow);

        if (_showWaveCleared)
        {
            string waveText = $"Wave {_engine.CurrentState.Wave - 1} cleared!";
            Vector2 waveTextSize = _font.MeasureString(waveText);
            _spriteBatch?.DrawString(_font, waveText,
                new Vector2(width / 2f - waveTextSize.X / 2f, height / 2f - waveTextSize.Y / 2f), Color.Black);
        }

        _spriteBatch?.End();
        base.Draw(gameTime);
    }

    private void DrawBug(TyperShroom.Core.Bug bug, int width, int height)
    {
        string typed = bug.Word.Substring(0, bug.Word.Length - bug.RemainingWord.Length);
        string remaining = bug.RemainingWord;

        Vector2 pos = new Vector2((float)bug.PositionX / 100f * width, bug.PositionY / 100f * height);
        Vector2 typedSize = _font.MeasureString(typed);

        Texture2D? bugTexture = bug.BugType switch
        {
            "spider" => _spider,
            "butterfly" => _butterfly,
            "ant" => _ant,
            "fly" => _fly,
            "mosquito" => _mosquito,
            "worm" => _worm,
            _ => _mushroom
        };

        float targetHeight = height * 0.15f;
        float bugScale = targetHeight / bugTexture.Height;

        _spriteBatch?.Draw(bugTexture, new Vector2(pos.X + 50, pos.Y), null, Color.White, 0f,
            new Vector2(bugTexture.Width / 2f, bugTexture.Height / 2f), bugScale, SpriteEffects.None, 0f);

        Vector2 wordSize = _font.MeasureString(bug.Word);
        _spriteBatch?.Draw(_pixel, new Rectangle((int)pos.X - 2, (int)pos.Y + 15, (int)wordSize.X + 4, (int)wordSize.Y + 4), Color.Black * 0.5f);
        _spriteBatch?.DrawString(_font, typed, new Vector2(pos.X, pos.Y + 15), Color.Gray);
        _spriteBatch?.DrawString(_font, remaining, new Vector2(pos.X + typedSize.X, pos.Y + 15), Color.Red);
    }
}