using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TyperShroom.Core;

public class Game1 : Game
{
    // Window creation, resolution, fullscreen
    private GraphicsDeviceManager _graphics;

    // Sprites, text, images
    private SpriteBatch? _spriteBatch;

    private IGameEngine _engine;

    private Texture2D _background;

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

        _font = Content.Load<SpriteFont>("DefaultFont");
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.A))
            _engine.ProcessKeystroke('a');

        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();

        _engine.Update(gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);    
    }

    protected override void Draw(GameTime gameTime)
    {
        // Clears last frame
        GraphicsDevice.Clear(Color.Black);

        // var state = _engine.CurrentState;

        // Begin the sprite batch to prepare for rendering
        _spriteBatch?.Begin();

        // Draw the texture at the center of the window
        _spriteBatch?.Draw(
            _background,                // texture
            new Vector2(                // position
                Window.ClientBounds.Width,
                Window.ClientBounds.Height) * 0.5f,
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

        var state = _engine.CurrentState;

        _spriteBatch?.DrawString(
                                 _font,                     // font
                                 $"Lives: {state.Lives}   Score: {state.Score}   Wave:  {state.Wave}",
                                  new Vector2(10, 10),      // pos
                                  Color.White               // color
                                );

        foreach (var bug in state.ActiveBugs)
        {
            _spriteBatch?.DrawString(
                                     _font,
                                     bug.Word,
                                     new Vector2(bug.PositionX * 5,
                                                 bug.PositionY * 25),
                                                 Color.Red
                                    );
        }

        _spriteBatch?.End();

        base.Draw(gameTime);
    }
}