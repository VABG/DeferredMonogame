using Microsoft.Xna.Framework.Graphics;

namespace Gem.Rendering;

public class MotionBlurDrawer
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Effect _effect;
    
    private readonly FullScreenRectangle _rect;
    
    public MotionBlurDrawer(GraphicsDevice graphicsDevice, Effect motionBlurEffect)
    {
        _graphicsDevice = graphicsDevice;
        _effect = motionBlurEffect;
        _rect = new FullScreenRectangle();
    }

    public void Draw(Texture2D motionVectors,
        Texture2D prePostProcessImage)
    {
        //TODO: make framerate independent (need current framerate divided by target framerate or similar)
        const float velocityScale = 0.5f;
        //TODO: Add depth check for better looking blur sampling
        _effect.Parameters["MotionVectors"].SetValue(motionVectors);
        _effect.Parameters["GatherImage"].SetValue(prePostProcessImage);
        //_effect.Parameters["MaxSamples"].SetValue(16);
         //_effect.Parameters["Resolution"].SetValue(new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height));
       // _effect.Parameters["VelocityScale"].SetValue(velocityScale);
        var pass = _effect.CurrentTechnique.Passes[0];
        pass.Apply();
        _rect.DrawIndexed(_graphicsDevice);
    }
}