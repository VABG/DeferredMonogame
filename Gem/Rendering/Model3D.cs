using System.Drawing;
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
    private SamplerState _samplerState;
    public readonly Model Model;

    public Model3D(Texture2D albedo, Texture2D normal, Texture2D specularGloss, Texture2D glow, Model model)
    {
        _transform = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
        _albedo = albedo;
        _normal = normal;
        _specularGloss = specularGloss;
        _glow = glow;
        Model = model;

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
        graphics.SamplerStates[0] =  _samplerState;
        effect.Parameters["CameraPosition"].SetValue(camera.Transform.Translation);
        effect.Parameters["ModelViewProjection"].SetValue(GetWorldViewProjectionMatrix(camera));
        effect.Parameters["ModelToWorld"].SetValue(Matrix.Transpose(GetWorldMatrix()));
        effect.Parameters["AlbedoTexture"].SetValue(_albedo);
        effect.Parameters["NormalsTexture"].SetValue(_normal);
        effect.Parameters["SpecularGlossTexture"].SetValue(_specularGloss);
        effect.Parameters["GlowTexture"].SetValue(_glow);
        foreach (var mesh in Model.Meshes)
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

