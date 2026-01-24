using Gem.Structure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace Gem;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Scene? _scene;
    private SpriteFont _font;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            GraphicsProfile = GraphicsProfile.HiDef,
            PreferredBackBufferWidth = 1920,
            PreferredBackBufferHeight = 1080,
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
        _font = Content.Load<SpriteFont>("Fonts/Arial");
        var render=  new DeferredRenderPass(_graphics, _spriteBatch, Content);
        _scene = new Scene(render, Content, GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        _scene?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _scene?.Render();
        FrameRate(gameTime);
        base.Draw(gameTime);
    }

    private int samples = 0;
    private double accumulated = 0;
    private double fpsTimer = 0;
    private double currentFps = 0;
    private void FrameRate(GameTime gameTime)
    {
        fpsTimer += gameTime.ElapsedGameTime.TotalSeconds;
        accumulated += gameTime.ElapsedGameTime.TotalSeconds;
        samples++;
        if (fpsTimer >= 0.5)
        {
            currentFps = 1.0 / (accumulated / samples);
            fpsTimer -= 0.5;
            accumulated = 0;
            samples = 0;
        }
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.BlendState = BlendState.Opaque;
        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, $"{(int)currentFps}",  new Vector2(2, 2), Color.White);
        _spriteBatch.End();
    }
}