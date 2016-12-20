using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoxelTest
{
    enum BlocMaterial { Air = 0, Earth, Stone, Wood, Sand, Foliage, Plank, TreeSprout, Grass, FlowerRed, FlowerYellow, Mushroom, Reed }
    enum BlocSide { FrontX = 0, BackX, FrontY, BackY, FrontZ, BackZ }

    partial class Chunk
    {
        public const int BlocDataSize = 1 + sizeof(ushort) + 2;
        public const int LightDataSize = sizeof(int) + 1;

        public const ushort chunkSize = 16;
        public const ushort chunkHeight = 256;
        public const ushort lightsMax = 1000;
        public const ushort lightsCount = 0;

        public GraphicsDevice GraphicsDevice { get; set; }
        public ChunkManager ChunkManager { get; set; }

        public HeightMap HeightMap { get; set; }
        public Point3d ChunkPosition;
        public Point3d FirstBlocAbsolutPosition;

        public Chunk[] Neighbors { get; set; }

        public int OpaqueFaceCount { get; set; }
        public VertexBuffer VertexBufferOpaque { get; set; }
        public IndexBuffer IndexBufferOpaque { get; set; }


        public int WaterFaceCount { get; set; }
        public VertexBuffer VertexBufferWater { get; set; }
        public IndexBuffer IndexBufferWater { get; set; }

        public Matrix Transform { get; set; }

        public byte[] Blocs { get; set; }
        public float DistanceFromCamera { get; set; }
        public bool IsLoading { get; set; }

        public bool WaitForLightUpdate { get; set; }
        public bool LightComputing { get; set; }
        public bool LightFirstPassComputed { get; set; }
        public bool LightComputed { get; set; }

        public bool VertexBufferCreated { get; set; }
        public bool FacesComputing { get; set; }
        public bool WaitForFacesComputing { get; set; }
        public bool FacesCreated { get; set; }

        public static Point3d[] SideDecal = new Point3d[] { new Point3d(-1, 0, 0), new Point3d(1, 0, 0), new Point3d(0, -1, 0), new Point3d(0, 1, 0), new Point3d(0, 0, -1), new Point3d(0, 0, 1) };
        public static Point3d[] SideDecalToNeighbors = new Point3d[] { new Point3d(chunkSize - 1, -1, -1), new Point3d(0, -1, -1), new Point3d(-1, chunkSize - 1, -1), new Point3d(-1, 0, -1), new Point3d(-1, -1, chunkSize - 1), new Point3d(-1, -1, 0) };

        public Microsoft.Xna.Framework.BoundingBox BoundingBox;

        private int _highestBloc;
        private int _lowestBloc;

        public Chunk()
        {
            Blocs = new byte[Chunk.chunkSize * Chunk.chunkSize * Chunk.chunkHeight * Chunk.BlocDataSize];
        }

        ~Chunk()
        {
            Console.WriteLine("delete");
        }

        public void Initialize()
        {
            FacesCreated = false;
            VertexBufferCreated = false;
            FacesComputing = false;
            WaitForFacesComputing = false;
            OpaqueFaceCount = 0;
            WaterFaceCount = 0;

            IsLoading = true;
            Neighbors = new Chunk[6];
            LightComputed = false;
            LightComputing = false;
            LightFirstPassComputed = false;

            FirstBlocAbsolutPosition = new Point3d(ChunkPosition.X * chunkSize, ChunkPosition.Y, ChunkPosition.Z * chunkSize);
            Transform = Matrix.CreateTranslation(FirstBlocAbsolutPosition.X, FirstBlocAbsolutPosition.Y, FirstBlocAbsolutPosition.Z);
            BoundingBox = new BoundingBox(FirstBlocAbsolutPosition.ToVector3(), FirstBlocAbsolutPosition.ToVector3() + new Vector3(16, 256, 16));
        }

        public void update()
        {
            UpdateFaces();
            updateLight();
        }

        public void uninit()
        {
            //while (FacesComputing)
            //{
            //    Thread.Sleep(1);
            //}
        }
    }
}
