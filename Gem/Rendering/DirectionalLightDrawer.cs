using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gem.Rendering;

public class DirectionalLightDrawer
{
    public DirLight DirLight { get; }
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Effect _effect;
    
    private readonly FullScreenRectangle _rect;
    public DirectionalLightDrawer(GraphicsDevice graphicsDevice, DirLight dirLight, Effect dirLightEffect)
    {
        _graphicsDevice = graphicsDevice;
        _effect = dirLightEffect;
        DirLight = dirLight;
        _rect = new FullScreenRectangle();
    }
    
    public void Draw(Texture2D albedo,
        Texture2D normalsGloss,
        Texture2D specularGlow,
        Texture2D worldSpace, 
        TextureCube cubeMap,
        Camera3D camera)
    {
        _effect.Parameters["CubeMap"].SetValue(cubeMap);
        _effect.Parameters["CubeMapLevelCount"].SetValue(cubeMap.LevelCount);
        _effect.Parameters["Albedo"].SetValue(albedo);
        _effect.Parameters["NormalsGloss"].SetValue(normalsGloss);
        _effect.Parameters["SpecularGlow"].SetValue(specularGlow);
        _effect.Parameters["WorldSpace"].SetValue(worldSpace);
        _effect.Parameters["LightDirection"].SetValue(DirLight.Direction);
        _effect.Parameters["LightColorStrength"].SetValue(DirLight.Color);
        _effect.Parameters["CameraPosition"].SetValue(camera.Transform.Translation);

        var pass = _effect.CurrentTechnique.Passes[0];
        pass.Apply();
        _rect.DrawIndexed(_graphicsDevice);
    }

    public void Rotate(Vector3 rotation)
    {
        Rotate(ref DirLight.Direction, rotation);
    }

    private void Rotate(ref Vector3 vector, Vector3 rotation)
    {
        if (rotation.X != 0)
            vector = Vector3.Transform(vector, Matrix.CreateRotationX(rotation.X));
        if (rotation.Y != 0)
            vector = Vector3.Transform(vector, Matrix.CreateRotationY(rotation.Y));
        if (rotation.Z != 0)
            vector = Vector3.Transform(vector, Matrix.CreateRotationZ(rotation.Z));
    }
}