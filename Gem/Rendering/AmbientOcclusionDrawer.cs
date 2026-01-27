using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gem.Rendering;

public class AmbientOcclusionDrawer
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Effect _effect;
    
    private readonly FullScreenRectangle _rect;
    public RenderTarget2D Ao { get; private set; }

    private Vector3[] _hemisphereSamples;
    private Vector3[] _noise;
    
    public AmbientOcclusionDrawer(GraphicsDevice graphicsDevice, Effect aoEffect)
    {
        Ao = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        _graphicsDevice = graphicsDevice;
        _effect = aoEffect;
        _rect = new FullScreenRectangle();
        GenerateSampleHemisphere();
        GenerateNoise();
        _effect.Parameters["HemisphereSamples"].SetValue(_hemisphereSamples);
        _effect.Parameters["Noise"].SetValue(_noise);
    }

    public void Draw(Texture2D normalsGloss,
        Texture2D worldDepth, Camera3D camera)
    {
        _graphicsDevice.SetRenderTarget(Ao);
        var viewProjection = camera.ViewMatrix * camera.ProjectionMatrix;
        _effect.Parameters["NormalsGloss"].SetValue(normalsGloss);
        _effect.Parameters["WorldDepth"].SetValue(worldDepth);
        _effect.Parameters["ViewProjectionMatrix"].SetValue(viewProjection);
        _effect.Parameters["Resolution"].SetValue(new Vector2(Ao.Width, Ao.Height));
        _effect.Parameters["Scale"].SetValue(0.2f);
        var pass = _effect.CurrentTechnique.Passes[0];
        pass.Apply();
        _rect.DrawIndexed(_graphicsDevice);
    }

    private void GenerateSampleHemisphere()
    {
        uint samples = 64;
        var random = new Random(0);
        _hemisphereSamples = new Vector3[samples];
        for (int i = 0; i < samples; i++)
        {
            _hemisphereSamples[i] = new Vector3(
                random.NextSingle() * 2 - 1.0f,
                random.NextSingle() * 2 - 1.0f,
                random.NextSingle());
            _hemisphereSamples[i].Normalize();
            

            var scale = i / 1.0f;
            scale = MathHelper.Lerp(0.1f, 1.0f, scale * scale);
            _hemisphereSamples[i] *= scale;
        }
    }

    private void GenerateNoise()
    {
        uint samples = 100;
        _noise = new Vector3[samples];
        var random = new Random(1);
        for (int i = 0; i < samples; ++i) {
            _noise[i] = new Vector3(
                random.NextSingle() * 2 - 1.0f,
                random.NextSingle() * 2 - 1.0f,
               0);
            _noise[i].Normalize();
        }
    }
}