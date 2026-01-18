using Gem.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Gem;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _albedo;
    private RenderTarget2D _normalsGloss;
    private RenderTarget2D _specularGlow;
    private RenderTarget2D _worldSpace;

    private RenderTarget2D _gather;

    private Camera3D _camera;
    private Effect _deferredPass;
    private Effect _directionalLight;
    private Model3D _model;
    private DirectionalLightDrawer _dirLight;
    private bool _debugDrawRenderPasses;
    
    private KeyboardState _oldState;
    private KeyboardState _newState;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            GraphicsProfile = GraphicsProfile.HiDef,
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720,
            PreferMultiSampling = true,
            SynchronizeWithVerticalRetrace = true,
            IsFullScreen = false,
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        DepthFormat depthFormat = DepthFormat.Depth24;

        _albedo = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, depthFormat);
        _normalsGloss = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.HalfVector4, depthFormat);
        _specularGlow = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, depthFormat);
        _worldSpace = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Vector4, depthFormat);
        _gather = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, depthFormat);

        _camera = new Camera3D(new Vector3(0, 0, -10), new Vector3(0,3.14f,0), 60, _graphics.GraphicsDevice);


        base.Initialize();
    } 

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _deferredPass = Content.Load<Effect>("Shaders/Deferred");
        _directionalLight = Content.Load<Effect>("Shaders/DirectionalLight");

        var model = Content.Load<Model>("Models/Knight/Kniggt_LP");
        var alb = Content.Load<Texture2D>("Models/Knight/Knight_d");
        var specGloss = Content.Load<Texture2D>("Models/Knight/Knight_mr");
        var norm = Content.Load<Texture2D>("Models/Knight/Knight_n");
        var glow = Content.Load<Texture2D>("Models/Knight/Knight_g");


        var dirLight = new DirLight
        {
            Direction = Vector3.Down,
            Color = new Vector4(1, 1, 1, 1)
        };
        _dirLight = new DirectionalLightDrawer(GraphicsDevice, dirLight, _directionalLight);
        _model = new Model3D(alb, norm, specGloss, glow, model);
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = gameTime.ElapsedGameTime.Seconds;
        _newState = Keyboard.GetState();

        _model.Rotate(Vector3.Up, dt * 5.0f);
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (_newState.IsKeyDown(Keys.L) && !_oldState.IsKeyDown(Keys.L))
            _debugDrawRenderPasses = !_debugDrawRenderPasses;

        float moveAmount = 0.1f;
        if (_newState.IsKeyDown(Keys.D))
            _camera.Translate(Vector3.Right * moveAmount);
        if (_newState.IsKeyDown(Keys.A))
            _camera.Translate(Vector3.Left * moveAmount);
        if (_newState.IsKeyDown(Keys.W))
            _camera.Translate(Vector3.Forward * moveAmount);
        if (_newState.IsKeyDown(Keys.S))
            _camera.Translate(Vector3.Backward * moveAmount);

        if (_newState.IsKeyDown(Keys.Q))
            _camera.RotateY(moveAmount / 10f);
        if (_newState.IsKeyDown(Keys.Z))
            _camera.RotateY(-moveAmount / 10f);
        _camera.Update();

        if (_newState.IsKeyDown(Keys.Left))
            _dirLight.Rotate(Vector3.Left *.2f);
        if (_newState.IsKeyDown(Keys.Right))
            _dirLight.Rotate(Vector3.Right*.2f);
        if (_newState.IsKeyDown(Keys.Up))
            _dirLight.Rotate(Vector3.Forward *.2f);
        if (_newState.IsKeyDown(Keys.Down))
            _dirLight.Rotate(Vector3.Backward *.2f);
        _oldState = _newState;
        
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // TODO: Get latest camera info and ensure the camera buffer is up to date
        DrawDeferredPass();

        if (_debugDrawRenderPasses)
            DrawDebugPasses();
        else
            DrawLighting();

        base.Draw(gameTime);
    }

    private void DrawLighting()
    {
        GraphicsDevice.SetRenderTarget(_gather);
        GraphicsDevice.Clear(Color.DarkRed);
        GraphicsDevice.BlendState = BlendState.NonPremultiplied;
        GraphicsDevice.DepthStencilState = DepthStencilState.None;
        _dirLight.Draw(_albedo, _normalsGloss, _specularGlow, _worldSpace, _camera);

        GraphicsDevice.SetRenderTarget(null);
        _spriteBatch.Begin();
        _spriteBatch.Draw(_gather,
            Vector2.Zero, Color.White);
        _spriteBatch.End();
    }

    private void DrawDeferredPass()
    {
        GraphicsDevice.SetRenderTargets(_albedo, _normalsGloss, _specularGlow, _worldSpace);
        GraphicsDevice.BlendState = BlendState.Opaque;
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        _model.Draw(_deferredPass, _camera, GraphicsDevice);
        GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawDebugPasses()
    {
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.DarkGray);
        _spriteBatch.Begin();
        _spriteBatch.Draw(_albedo, Vector2.Zero,
            new Rectangle(0, 0, _graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2),
            Color.White);
        _spriteBatch.Draw(_albedo,
            Vector2.Zero, null, Color.White, 0, Vector2.Zero,
            new Vector2(0.5f), SpriteEffects.None, 0);
        _spriteBatch.Draw(_normalsGloss,
            new Vector2(_graphics.PreferredBackBufferWidth / 2.0f, 0),
            null, Color.White, 0, Vector2.Zero,
            new Vector2(0.5f), SpriteEffects.None, 0);
        _spriteBatch.Draw(_specularGlow, new Vector2(0, _graphics.PreferredBackBufferHeight / 2.0f),
            null, Color.White, 0, Vector2.Zero,
            new Vector2(0.5f), SpriteEffects.None, 0);
        _spriteBatch.Draw(_worldSpace,
            new Vector2(_graphics.PreferredBackBufferWidth / 2.0f, _graphics.PreferredBackBufferHeight / 2.0f),
            null, Color.White, 0, Vector2.Zero,
            new Vector2(0.5f), SpriteEffects.None, 0);
        _spriteBatch.End();
    }
}