using System.Collections.Generic;
using Gem.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gem.Structure;

public class DeferredRenderPass
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly SpriteBatch _spriteBatch;
    private readonly ContentManager _content;
    private RenderTarget2D _albedo;
    private RenderTarget2D _normalsGloss;
    private RenderTarget2D _specularGlow;
    private RenderTarget2D _worldSpace;
    private RenderTarget2D _motionVectors;
    
    private TextureCube _cubeMap;
    private RenderTarget2D _gather;

    private Effect _deferredPass;
    private Effect _directionalLight;
    public DirectionalLightDrawer DirLight {get; private set;}
    private AmbientOcclusionDrawer _ambientOcclusionDrawer;
    private readonly BlendState _multiplyBlendState = new() { ColorSourceBlend = Blend.DestinationColor, ColorDestinationBlend = Blend.Zero };
    public MotionBlurDrawer MotionBlur {get; private set;}
    private SamplerState _samplerState;
    
    private readonly GraphicsDevice _graphicsDevice;
    public bool DebugDrawRenderPasses;

    public DeferredRenderPass(GraphicsDeviceManager graphicsDeviceManager, SpriteBatch spriteBatch,
        ContentManager content)
    {
        _graphics = graphicsDeviceManager;
        _spriteBatch = spriteBatch;
        _graphicsDevice = graphicsDeviceManager.GraphicsDevice;
        _content = content;
        Initalize();
        Load();
    }

    private void Load()
    {
        _cubeMap = _content.Load<TextureCube>("Textures/cubemap_test");
        _deferredPass = _content.Load<Effect>("Shaders/Deferred");
        var directionalLight = _content.Load<Effect>("Shaders/DirectionalLight");
        var dirLight = new DirLight
        {
            Direction = new Vector3(-0.590393543f,  -0.801861346f, 0.0919487178f),
            Color = new Vector4(1, 0.96f, 0.9f, 1)
        };
        dirLight.Direction.Normalize();
        DirLight = new DirectionalLightDrawer(_graphics.GraphicsDevice, dirLight, directionalLight);
        var blurEffect = _content.Load<Effect>("Shaders/MotionBlur");

        MotionBlur = new MotionBlurDrawer(_graphics.GraphicsDevice, blurEffect);
        
        var aoShader = _content.Load<Effect>("Shaders/SSAO");
        _ambientOcclusionDrawer = new AmbientOcclusionDrawer(_graphicsDevice, aoShader);

    }

    private void Initalize()
    {
        DepthFormat depthFormat = DepthFormat.Depth24Stencil8;

        _albedo = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, depthFormat);
        _normalsGloss = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.HalfVector4, DepthFormat.None);
        _specularGlow = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
        _worldSpace = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Vector4, DepthFormat.None);
        _gather = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.HdrBlendable, DepthFormat.None);
        _motionVectors = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight, false, SurfaceFormat.HalfVector2, DepthFormat.None);

        _samplerState = new SamplerState()
        {
            AddressU = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            Filter = TextureFilter.Linear,
        };
    }

    public void Draw(Camera3D camera, List<Model3D> models)
    {
        // TODO: Get latest camera info and ensure the camera buffer is up to date
        DrawDeferredPass(camera, models);
        
        _ambientOcclusionDrawer.Draw(_normalsGloss, _worldSpace, camera);
        if (DebugDrawRenderPasses)
            DrawDebugPasses();
        else
        {
            DrawLighting(camera);
            DrawPostProcessing();
        }
    }

    private void DrawLighting(Camera3D camera)
    {
        _graphicsDevice.SetRenderTarget(_gather);
        _graphicsDevice.Clear(Color.Black);
        _graphicsDevice.BlendState = BlendState.Additive;
        DirLight.Draw(_albedo, _normalsGloss, _specularGlow, _worldSpace, _ambientOcclusionDrawer.Ao, _cubeMap, camera);
        _graphicsDevice.SetRenderTarget(null);
        //_spriteBatch.Begin();
        //_spriteBatch.Draw(_gather,
        //    Vector2.Zero, Color.White);
        //_spriteBatch.End();
    }

    private void DrawPostProcessing()
    {
        _graphicsDevice.SamplerStates[0] = _samplerState;
        _graphicsDevice.BlendState = BlendState.Opaque;
        MotionBlur.Draw(_motionVectors, _gather);
    }
    
    private void DrawDeferredPass(Camera3D camera, List<Model3D> models)
    {
        _graphicsDevice.SetRenderTargets(_albedo, _normalsGloss, _specularGlow, _worldSpace, _motionVectors);
        _graphicsDevice.Clear(Color.Transparent);
        _graphicsDevice.BlendState = BlendState.Opaque;
        _graphicsDevice.DepthStencilState = DepthStencilState.Default;

        foreach (var model in models)
            model.Draw(_deferredPass, camera, _graphicsDevice);

        _graphicsDevice.SetRenderTarget(null);
    }

    private void DrawDebugPasses()
    {
        _graphicsDevice.SetRenderTarget(null);
        _graphicsDevice.Clear(Color.DarkGray);
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
        _spriteBatch.Draw(_worldSpace, new Vector2(0, _graphics.PreferredBackBufferHeight / 2.0f),
            null, Color.White, 0, Vector2.Zero,
            new Vector2(0.5f), SpriteEffects.None, 0);
        _spriteBatch.Draw(_ambientOcclusionDrawer.Ao,
            new Vector2(_graphics.PreferredBackBufferWidth / 2.0f, _graphics.PreferredBackBufferHeight / 2.0f),
            null, Color.White, 0, Vector2.Zero,
            new Vector2(0.5f), SpriteEffects.None, 0);
        _spriteBatch.End();
    }
}