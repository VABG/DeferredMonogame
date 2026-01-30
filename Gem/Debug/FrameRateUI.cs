using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gem.Debug;

public class FrameRateUI
{
    private readonly SpriteBatch _spriteBatch;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteFont _font;
    private int _samples = 0;
    private double _accumulated = 0;
    private double _fpsTimer = 0;
    private double _currentFps = 0;
    
    public FrameRateUI(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont font)
    {
        _spriteBatch = spriteBatch;
        _graphicsDevice = graphicsDevice;
        _font = font;
    }

    public void Render(GameTime gameTime)
    {
        _fpsTimer += gameTime.ElapsedGameTime.TotalSeconds;
        _accumulated += gameTime.ElapsedGameTime.TotalSeconds;
        _samples++;
        if (_fpsTimer >= 0.5)
        {
            _currentFps = 1.0 / (_accumulated / _samples);
            _fpsTimer -= 0.5;
            _accumulated = 0;
            _samples = 0;
        }
        _graphicsDevice.SetRenderTarget(null);
        _graphicsDevice.BlendState = BlendState.Opaque;
        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, $"{(int)_currentFps}",  new Vector2(10, 2), Color.White);
        _spriteBatch.End();
    }
}