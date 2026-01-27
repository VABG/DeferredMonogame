using System.Collections.Generic;
using Gem.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gem.Structure;

public class Scene
{
    DeferredRenderPass _render;
    private readonly ContentManager _content;
    private List<Model3D> _models = [];

    private Model3D _knight;
    private Model3D _knight2;
    private KeyboardState _oldState;
    private KeyboardState _newState;
    public Camera3D Camera { get; private set; }

    public Scene(DeferredRenderPass render, ContentManager content, GraphicsDevice graphicsDevice)
    {
        _render = render;
        _content = content;
        Camera = new Camera3D(new Vector3(0, 0, -1.5f), new Vector3(0,3.14f,0), 60, graphicsDevice);
        _knight = LoadKnight();
        _knight2 = LoadKnight();
        _knight2.Rotate(Vector3.Up, 3.14f);
        _knight2.Translate(Vector3.Right* 0.75f);
        _models.Add(_knight);
        _models.Add(_knight2);
    }
    
    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _newState = Keyboard.GetState();

        //_knight.Rotate(Vector3.Up, dt * 1.0f);
        _knight2.Rotate(Vector3.Up, dt * -2.5f);

        if (_newState.IsKeyDown(Keys.L) && !_oldState.IsKeyDown(Keys.L))
           _render.DebugDrawRenderPasses = !_render.DebugDrawRenderPasses;

        float moveAmount = 2.0f * dt;
        if (_newState.IsKeyDown(Keys.D))
            Camera.Translate(Vector3.Right * moveAmount);
        if (_newState.IsKeyDown(Keys.A))
            Camera.Translate(Vector3.Left * moveAmount);
        if (_newState.IsKeyDown(Keys.W))
            Camera.Translate(Vector3.Forward * moveAmount);
        if (_newState.IsKeyDown(Keys.S))
            Camera.Translate(Vector3.Backward * moveAmount);

        if (_newState.IsKeyDown(Keys.Q))
            Camera.RotateY(moveAmount / 10f);
        if (_newState.IsKeyDown(Keys.Z))
            Camera.RotateY(-moveAmount / 10f);
        Camera.Update();

         float rotationSpeed = 2.5f * dt;
        if (_newState.IsKeyDown(Keys.Left))
            _render.DirLight.Rotate(Vector3.Left * rotationSpeed);
        if (_newState.IsKeyDown(Keys.Right))
            _render.DirLight.Rotate(Vector3.Right* rotationSpeed);
        if (_newState.IsKeyDown(Keys.Up))
            _render.DirLight.Rotate(Vector3.Forward * rotationSpeed);
        if (_newState.IsKeyDown(Keys.Down))
            _render.DirLight.Rotate(Vector3.Backward * rotationSpeed);
        _oldState = _newState;
    }

    private Model3D LoadKnight()
    {
        var model = _content.Load<Model>("Models/Knight/Kniggt_LP");
        var alb = _content.Load<Texture2D>("Models/Knight/Knight_d");
        var specGloss = _content.Load<Texture2D>("Models/Knight/Knight_mr");
        var norm = _content.Load<Texture2D>("Models/Knight/Knight_n");
        var glow = _content.Load<Texture2D>("Models/Knight/Knight_g");
        return new Model3D(alb, norm, specGloss, glow, model);
    }

    public void Render()
    {
        _render.Draw(Camera, _models);
    }
}