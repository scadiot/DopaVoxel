using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelTest
{
    public struct VertexPositionNormalTextureColor : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public float SunLight;
        public float ArtificialLight;
        public float OcclusionLight;
        public static readonly VertexDeclaration VertexDeclaration;

        public VertexPositionNormalTextureColor(Vector3 position, Vector3 normal, Vector2 textureCoordinate, float SunLight, float ArtificialLight, float OcclusionLight)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoordinate = textureCoordinate;
            this.SunLight = SunLight;
            this.ArtificialLight = ArtificialLight;
            this.OcclusionLight = OcclusionLight;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }
        public override int GetHashCode()
        {
            // TODO: FIc gethashcode
            return 0;
        }

        public override string ToString()
        {
            return string.Format("{{Position:{0} Normal:{1} TextureCoordinate:{2}}}", new object[] { this.Position, this.Normal, this.TextureCoordinate });
        }

        public static bool operator ==(VertexPositionNormalTextureColor left, VertexPositionNormalTextureColor right)
        {
            return (((left.Position == right.Position) && (left.Normal == right.Normal)) && (left.TextureCoordinate == right.TextureCoordinate));
        }

        public static bool operator !=(VertexPositionNormalTextureColor left, VertexPositionNormalTextureColor right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != base.GetType())
            {
                return false;
            }
            return (this == ((VertexPositionNormalTextureColor)obj));
        }

        static VertexPositionNormalTextureColor()
        {
            VertexElement[] elements = new VertexElement[] 
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.Color, 0),
                new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.Color, 1),
                new VertexElement(40, VertexElementFormat.Single, VertexElementUsage.Color, 2)
            };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }
    }
}
