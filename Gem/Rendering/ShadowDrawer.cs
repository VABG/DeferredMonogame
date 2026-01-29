using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gem.Rendering;

public class ShadowDrawer
{
    private readonly GraphicsDevice _graphicsDevice;
    public RenderTarget2D ShadowMap { get; private set; }
    public Camera3D Camera { get; private set; }
    private readonly Effect _effect;

    public ShadowDrawer(int resolution,Camera3D camera, GraphicsDevice graphicsDevice, ContentManager content)
    {
        _effect = content.Load<Effect>("Shaders/ShadowPass");
        Camera = camera;
        _graphicsDevice = graphicsDevice;
        ShadowMap = new RenderTarget2D(graphicsDevice, resolution, resolution, false, SurfaceFormat.Single, DepthFormat.Depth24);
    }
    
    public void Draw(Model3D[] models)
    {
        _graphicsDevice.SetRenderTarget(ShadowMap);
        foreach (var m in models)
            m.DrawShadow(_effect, Camera, _graphicsDevice);
        _graphicsDevice.SetRenderTarget(null);
    }
}