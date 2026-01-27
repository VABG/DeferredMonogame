using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gem.Rendering;

public class ShadowDrawer
{
    private readonly GraphicsDevice _graphicsDevice;
    public RenderTarget2D ShadowMap { get; private set; }

    public ShadowDrawer(int resolution, GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        ShadowMap = new RenderTarget2D(graphicsDevice, resolution, resolution, false, SurfaceFormat.Single, DepthFormat.Depth24);
    }
    
    public void DrawShadowMap(Model3D[] models, Camera3D shadowCamera, RenderTarget2D targetMap)
    {
        _graphicsDevice.SetRenderTarget(ShadowMap);
        
        
    }
}