using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gem.Rendering;

public class Model3D
{
   // public Vector3 Position =  new Vector3(0,0,55);
    //public Vector3 Rotation = Vector3.Zero;
    
    private readonly Texture2D _albedo;
    private readonly Texture2D _normal;
    private readonly Texture2D _specularGloss;
    private readonly Texture2D _glow;
    private Matrix _transform;
    private Matrix _lastWorldViewProjection;
    private readonly SamplerState _samplerState;
    private readonly Model _model;

    public Model3D(Texture2D albedo, Texture2D normal, Texture2D specularGloss, Texture2D glow, Model model)
    {
        _transform = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
        _albedo = albedo;
        _normal = normal;
        _specularGloss = specularGloss;
        _glow = glow;
        _model = model;

        _samplerState = new SamplerState();
        
        _samplerState.AddressU = TextureAddressMode.Wrap;
        _samplerState.AddressV = TextureAddressMode.Wrap;

        _samplerState.Filter = TextureFilter.Anisotropic;
        _samplerState.FilterMode = TextureFilterMode.Default;
    }

    private Matrix GetWorldMatrix()
    {
        return _transform;
    }

    public void Translate(Vector3 moveAmount)
    {
        _transform *= Matrix.CreateTranslation(moveAmount);
    }
    
    public void Rotate(Vector3 rotateAxis, float rotation)
    {
        _transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(rotateAxis, rotation));
    }
    
    private Matrix GetWorldViewProjectionMatrix(Camera3D camera)
    {
        var world = GetWorldMatrix();
        return world * camera.ViewMatrix * camera.ProjectionMatrix;
    }
    
    public void Draw(Effect effect, Camera3D camera, GraphicsDevice graphics)
    {
        var worldViewProjection = GetWorldViewProjectionMatrix(camera);
        graphics.SamplerStates[0] =  _samplerState;
        // TODO: Move camera position to global buffer
        effect.Parameters["CameraPosition"].SetValue(camera.Transform.Translation);
        effect.Parameters["ModelViewProjection"].SetValue(worldViewProjection);
        effect.Parameters["LastModelViewProjection"].SetValue(_lastWorldViewProjection);
        effect.Parameters["ModelToWorld"].SetValue(_transform);
        effect.Parameters["AlbedoTexture"].SetValue(_albedo);
        effect.Parameters["NormalsTexture"].SetValue(_normal);
        effect.Parameters["SpecularGlossTexture"].SetValue(_specularGloss);
        effect.Parameters["GlowTexture"].SetValue(_glow);
        
        foreach (var mesh in _model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                graphics.SetVertexBuffer(meshPart.VertexBuffer);
                graphics.Indices = meshPart.IndexBuffer;
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, meshPart.StartIndex, meshPart.PrimitiveCount);                    
                }
            }
        }

        _lastWorldViewProjection = worldViewProjection;
    }

    public void DrawShadow(Effect effect, Camera3D camera, GraphicsDevice graphics)
    {
        var worldViewProjection = GetWorldViewProjectionMatrix(camera);
        effect.Parameters["CameraPosition"].SetValue(camera.Transform.Translation);
        effect.Parameters["ModelViewProjection"].SetValue(worldViewProjection);
        effect.Parameters["ModelToWorld"].SetValue(_transform);
        
        foreach (var mesh in _model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                graphics.SetVertexBuffer(meshPart.VertexBuffer);
                graphics.Indices = meshPart.IndexBuffer;
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, meshPart.StartIndex, meshPart.PrimitiveCount);                    
                }
            }
        }
    }
}

