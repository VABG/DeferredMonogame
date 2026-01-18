using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gem.Rendering;


public class FullScreenRectangle
{
    private readonly VertexPositionTexture[] _rectangleVertices;
    private readonly VertexDeclaration _vertexDeclaration;
    private readonly short[] _indices;
    
    public FullScreenRectangle()
    {
        var pos = new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0);
        var texCoords = new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1);
        _vertexDeclaration =
            new VertexDeclaration(pos, texCoords);
        
        _rectangleVertices = new VertexPositionTexture[4];
        _rectangleVertices[0] = new VertexPositionTexture(new Vector3(-1,1,0), Vector2.Zero);
        _rectangleVertices[1] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1,0));
        _rectangleVertices[2] = new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0,1));
        _rectangleVertices[3] = new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1,1));
        
        _indices = new short[6];
        _indices[0] = 0;
        _indices[1] = 1;
        _indices[2] = 2;
        _indices[3] = 1;
        _indices[4] = 3;
        _indices[5] = 2;
    }

    public void DrawIndexed(GraphicsDevice graphics)
    {
        graphics.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, 
            _rectangleVertices, 
            0, 
            4, 
            _indices, 
            0, 
            2);
    }
}