using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Gem.Rendering;

public class Camera3D
{
    private GraphicsDevice graphicsDevice;

    //public Vector3 Rotation;
    //public Vector3 Position;
    public float FOV;
    public float OrthoFOV = 100;
    public bool Orthographic = false;
    private float nearClipPlane = 0.1f;
    private float farClipPlane = 200;
    private Vector3 Rotation;
    private Vector3 Position;
    public Matrix ViewMatrix { get; private set; }
    public Matrix ProjectionMatrix { get; private set; }
    public Matrix Transform { get; private set; }
    
    public Camera3D(Vector3 position, Vector3 rotation, float fov, GraphicsDevice graphicsDevice)
    {
        Position = position;
        FOV = fov;
        Rotation = rotation;
        this.graphicsDevice = graphicsDevice;
        Update();
    }

    private Matrix GetTransform()
    {
        return Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) * 
               Matrix.CreateTranslation(Position);
    }

    public void Update()
    {
        Transform = GetTransform();
        ViewMatrix = GetViewMatrix;
        ProjectionMatrix = GetProjectionMatrix;
    }
    
    public void Translate(Vector3 moveAmount)
    {
        Position += moveAmount;
    }

    public void RotateY(float degrees)
    {
        Rotation.Y += degrees;
    }

    public void RotateX(float degrees)
    {
        Rotation.X += degrees;
    }
    
    private Matrix GetViewMatrix
    {
        
        get
        {
            var upVector = Vector3.UnitY;
            return Matrix.CreateLookAt(Transform.Translation, Transform.Translation + Transform.Forward, upVector);
        }
    }

    private Matrix GetProjectionMatrix
    {
        get
        {
            float fieldOfView = MathHelper.ToRadians(FOV);
            float aspectRatio = (float)graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;
            if (Orthographic)
                return Matrix.CreateOrthographic(OrthoFOV, OrthoFOV / aspectRatio, nearClipPlane, farClipPlane);
            
            return Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
        }
    }
}