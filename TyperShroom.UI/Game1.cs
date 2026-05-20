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

        var state = _engine.CurrentState;

        // Start render
        _spriteBatch?.Begin();

        /*
         *
         * RENDER
         *
        */

        // End render
        _spriteBatch?.End();


    }
}