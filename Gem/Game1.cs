using Gem.Debug;
using Gem.Structure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gem;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Scene? _scene;
    private FrameRateUI _frameRate;
    private bool _running = true;
    private KeyboardState _oldState;
    private KeyboardState _newState;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            GraphicsProfile = GraphicsProfile.HiDef,
            PreferredBackBufferWidth = 1024,
            PreferredBackBufferHeight = 1024,
            PreferMultiSampling = false,
            SynchronizeWithVerticalRetrace = true,
            IsFullScreen = false,
        };
        IsFixedTimeStep = true;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        base.Initialize();
    } 

    protected override void LoadContent()
    {
        var font = Content.Load<SpriteFont>("Fonts/Arial");
        _frameRate = new FrameRateUI(_spriteBatch, GraphicsDevice, font);
        var render=  new DeferredRenderPass(_graphics, _spriteBatch, Content);
        _scene = new Scene(render, Content, GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        _newState = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (_newState.IsKeyDown(Keys.Space) && _oldState.IsKeyUp(Keys.Space))
            _running = !_running;

        if (_running)
        {
            _scene?.Update(gameTime);
        }
        base.Update(gameTime);
        _oldState = _newState;
    }

    protected override void Draw(GameTime gameTime)
    {
        if (_running)
        {
            _scene?.Render();
            _frameRate.Render(gameTime);            
        }
        base.Draw(gameTime);
    }
}