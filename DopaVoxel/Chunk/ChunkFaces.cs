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
    partial class Chunk
    {
        Point3d[][] listFacesDecal = new Point3d[][]
        {
            new Point3d[] { new Point3d(0,0,0), new Point3d(0,1,1), new Point3d(0,0,1),new Point3d(0,0,0),new Point3d(0,1,0),new Point3d(0,1,1) },
            new Point3d[] { new Point3d(1,0,0), new Point3d(1,0,1), new Point3d(1,1,1),new Point3d(1,0,0),new Point3d(1,1,1),new Point3d(1,1,0) },
            new Point3d[] { new Point3d(0,0,0), new Point3d(1,0,1), new Point3d(1,0,0),new Point3d(0,0,0),new Point3d(0,0,1),new Point3d(1,0,1) },
            new Point3d[] { new Point3d(0,1,0), new Point3d(1,1,0), new Point3d(1,1,1),new Point3d(0,1,0),new Point3d(1,1,1),new Point3d(0,1,1) },
            new Point3d[] { new Point3d(0,0,0), new Point3d(1,0,0), new Point3d(1,1,0),new Point3d(0,0,0),new Point3d(1,1,0),new Point3d(0,1,0) },
            new Point3d[] { new Point3d(0,0,1), new Point3d(1,1,1), new Point3d(1,0,1),new Point3d(0,0,1),new Point3d(0,1,1),new Point3d(1,1,1) }
        };

        Vector3[] listNormals = new Vector3[]
        {
             new Vector3(-1, 0, 0),
             new Vector3(1, 0, 0),
             new Vector3(0,-1, 0),
             new Vector3(0, 1, 0),
             new Vector3(0,0,-1),
             new Vector3(0,0,1)
        };

        Vector2[][] listUvs = new Vector2[][]
        {
             new Vector2[] { new Vector2(0.01f,0.99f), new Vector2(0.99f, 0.01f), new Vector2(0.99f, 0.99f), new Vector2(0.01f, 0.99f), new Vector2(0.01f, 0.01f), new Vector2(0.99f, 0.01f) },
             new Vector2[] { new Vector2(0.01f, 0.99f), new Vector2(0.99f, 0.99f), new Vector2(0.99f, 0.01f), new Vector2(0.01f, 0.99f), new Vector2(0.99f, 0.01f), new Vector2(0.01f, 0.01f) },
             new Vector2[] { new Vector2(0.01f, 0.01f), new Vector2(0.99f, 0.99f), new Vector2(0.99f, 0.01f), new Vector2(0.01f, 0.01f), new Vector2(0.01f, 0.99f), new Vector2(0.99f, 0.99f) },
             new Vector2[] { new Vector2(0.01f, 0.01f), new Vector2(0.99f, 0.01f), new Vector2(0.99f, 0.99f), new Vector2(0.01f, 0.01f), new Vector2(0.99f, 0.99f), new Vector2(0.01f, 0.99f) },
             new Vector2[] { new Vector2(0.01f, 0.99f), new Vector2(0.99f, 0.99f), new Vector2(0.99f, 0.01f), new Vector2(0.01f, 0.99f), new Vector2(0.99f, 0.01f), new Vector2(0.01f, 0.01f) },
             new Vector2[] { new Vector2(0.01f, 0.99f), new Vector2(0.99f, 0.01f), new Vector2(0.99f, 0.99f), new Vector2(0.01f, 0.99f), new Vector2(0.01f, 0.01f), new Vector2(0.99f, 0.01f) }
        };

        Point3d[][,] listDecalForOcclusion = new Point3d[][,]
        {
            new Point3d[,] { { new Point3d(-1, -1, 0), new Point3d(-1, 0, -1), new Point3d(-1, -1, -1) }, { new Point3d(-1, 1, 0), new Point3d(-1, 0, 1), new Point3d(-1, 1, 1) }, { new Point3d(-1, -1, 0), new Point3d(-1, 0, 1), new Point3d(-1, -1, 1) }, { new Point3d(-1, -1, 0), new Point3d(-1, 0, -1), new Point3d(-1, -1, -1) }, { new Point3d(-1, 1, 0), new Point3d(-1, 0, -1), new Point3d(-1, 1, -1) }, { new Point3d(-1, 1, 0), new Point3d(-1, 0, 1), new Point3d(-1, 1, 1) } },
            new Point3d[,] { { new Point3d(1, -1, 0), new Point3d(1, 0, -1), new Point3d(1, -1, -1) }, { new Point3d(1, -1, 0), new Point3d(1, 0, 1), new Point3d(1, -1, 1) }, { new Point3d(1, 1, 0), new Point3d(1, 0, 1), new Point3d(1, 1, 1) }, { new Point3d(1, -1, 0), new Point3d(1, 0, -1), new Point3d(1, -1, -1) }, { new Point3d(1, 1, 0), new Point3d(1, 0, 1), new Point3d(1, 1, 1) }, { new Point3d(1, 1, 0), new Point3d(1, 0, -1), new Point3d(1, 1, -1) } },
            new Point3d[,] { { new Point3d(-1, -1, 0), new Point3d(0, -1, -1), new Point3d(-1, -1, -1) }, { new Point3d(1, -1, 0), new Point3d(0, -1, 1), new Point3d(1, -1, 1) }, { new Point3d(1, -1, 0), new Point3d(0, -1, -1), new Point3d(1, -1, -1) }, { new Point3d(-1, -1, 0), new Point3d(0, -1, -1), new Point3d(-1, -1, -1) }, { new Point3d(-1, -1, 0), new Point3d(0, -1, 1), new Point3d(-1, -1, 1) }, { new Point3d(1, -1, 0), new Point3d(0, -1, 1), new Point3d(1, -1, 1) } },
            new Point3d[,] { { new Point3d(-1, 1, 0), new Point3d(0, 1, -1), new Point3d(-1, 1, -1) }, { new Point3d(1, 1, 0), new Point3d(0, 1, -1), new Point3d(1, 1, -1) }, { new Point3d(1, 1, 0), new Point3d(0, 1, 1), new Point3d(1, 1, 1) }, { new Point3d(-1, 1, 0), new Point3d(0, 1, -1), new Point3d(-1, 1, -1) }, { new Point3d(1, 1, 0), new Point3d(0, 1, 1), new Point3d(1, 1, 1) }, { new Point3d(-1, 1, 0), new Point3d(0, 1, 1), new Point3d(-1, 1, 1) } },
            new Point3d[,] { { new Point3d(-1, 0, -1), new Point3d(0, -1, -1), new Point3d(-1, -1, -1) }, { new Point3d(1, 0, -1), new Point3d(0, -1, -1), new Point3d(1, -1, -1) }, { new Point3d(1, 0, -1), new Point3d(0, 1, -1), new Point3d(1, 1, -1) }, { new Point3d(-1, 0, -1), new Point3d(0, -1, -1), new Point3d(-1, -1, -1) }, { new Point3d(1, 0, -1), new Point3d(0, 1, -1), new Point3d(1, 1, -1) }, { new Point3d(-1, 0, -1), new Point3d(0, 1, -1), new Point3d(-1, 1, -1) } },
            new Point3d[,] { { new Point3d(-1, 0, 1), new Point3d(0, -1, 1), new Point3d(-1,-1, 1) }, { new Point3d(1, 0, 1), new Point3d(0, 1, 1), new Point3d(1, 1, 1) }, { new Point3d(1, 0, 1), new Point3d(0, -1, 1), new Point3d(1, -1, 1) }, { new Point3d(-1, 0, 1), new Point3d(0, -1, 1), new Point3d(-1, -1, 1) }, { new Point3d(-1, 0, 1), new Point3d(0, 1, 1), new Point3d(-1, 1, 1) }, { new Point3d(1, 0, 1), new Point3d(0, 1, 1), new Point3d(1, 1, 1) } }
        };

        Vector3[] plantVerticesPositions = new Vector3[24]
        {
                new Vector3(0,0,0),new Vector3(1,0,1),new Vector3(1,1,1),new Vector3(0,0,0),new Vector3(1,1,1),new Vector3(0,1,0),
                new Vector3(0,0,0),new Vector3(1,1,1),new Vector3(1,0,1),new Vector3(0,0,0),new Vector3(0,1,0),new Vector3(1,1,1),
                new Vector3(1,0,0),new Vector3(0,0,1),new Vector3(0,1,1),new Vector3(1,0,0),new Vector3(0,1,1),new Vector3(1,1,0),
                new Vector3(1,0,0),new Vector3(0,1,1),new Vector3(0,0,1),new Vector3(1,0,0),new Vector3(1,1,0),new Vector3(0,1,1)
        };

        Vector2[] plantVerticesUvs = new Vector2[24]
        {
                new Vector2(0,1),new Vector2(1,1),new Vector2(1,0),new Vector2(0,1),new Vector2(1,0),new Vector2(0,0),
                new Vector2(0,1),new Vector2(1,0),new Vector2(1,1),new Vector2(0,1),new Vector2(0,0),new Vector2(1,0),
                new Vector2(0,1),new Vector2(1,1),new Vector2(1,0),new Vector2(0,1),new Vector2(1,0),new Vector2(0,0),
                new Vector2(0,1),new Vector2(1,0),new Vector2(1,1),new Vector2(0,1),new Vector2(0,0),new Vector2(1,0)
        };

        private VertexPositionNormalTextureColor[] createFaces(BlocSide side, Point3d position, int textureNumber, Vector3 decal)
        {
            Point3d[] decals = listFacesDecal[(byte)side];
            Vector2[] uvs = listUvs[(byte)side];
            VertexPositionNormalTextureColor[] vertices = new VertexPositionNormalTextureColor[6];

            float sunLuminosity = (float)getSunLuminosity(position + FirstBlocAbsolutPosition + SideDecal[(byte)side]) / 15;
            float artificialLuminosity = (float)getArtificialLuminosity(position + FirstBlocAbsolutPosition + SideDecal[(byte)side]) / 15;
            if (!LightComputed)
            {
                sunLuminosity = 1.0f;
                artificialLuminosity = 1.0f;
            }

            for (int i = 0; i < 6; i++)
            {
                Vector3 p = new Vector3((float)position.X + decals[i].X + decal.X, (float)position.Y + decals[i].Y + decal.Y, (float)position.Z + decals[i].Z + decal.Z);

                float decalX = (float)(textureNumber % 16) * (1.0f / 16.0f);
                float decalY = (float)(textureNumber / 16) * (1.0f / 16.0f);
                var uv = new Vector2(((1.0f / 16.0f) * uvs[i].X) + decalX, ((1.0f / 16.0f) * uvs[i].Y) + decalY);

                float vertexSunLuminosity = sunLuminosity;
                float vertexArtificialLuminosity = artificialLuminosity;
                float occlusionLuminosity = 1.0f;
                if (true)
                {
                    Point3d[] blocPositionOcclusionList = new Point3d[]
                    {
                    FirstBlocAbsolutPosition + position + listDecalForOcclusion[(byte)side][i, 0],
                    FirstBlocAbsolutPosition + position + listDecalForOcclusion[(byte)side][i, 1],
                    FirstBlocAbsolutPosition + position + listDecalForOcclusion[(byte)side][i, 2]
                    };
                    int mixCount = 1;
                    for (int j = 0; j < 3; j++)
                    {
                        var blocPositionOcclusion = blocPositionOcclusionList[j];
                        if (isOpaque(blocPositionOcclusion))
                        {
                            occlusionLuminosity -= 0.2f;
                        }
                        else if (j < 2 || mixCount > 1)
                        {
                            vertexSunLuminosity += (float)getSunLuminosity(blocPositionOcclusion) / 15;
                            vertexArtificialLuminosity += (float)getArtificialLuminosity(blocPositionOcclusion) / 15;
                            mixCount++;
                        }
                    }
                    vertexSunLuminosity = Math.Max(vertexSunLuminosity, 0.0f) / mixCount;
                    vertexArtificialLuminosity = Math.Max(vertexArtificialLuminosity, 0.0f) / mixCount;
                }

                vertices[i] = new VertexPositionNormalTextureColor(p, listNormals[(byte)side], uv, vertexSunLuminosity, vertexArtificialLuminosity, occlusionLuminosity);
            }
            return vertices;
        }

        

        private bool UpdateFacesNeeded()
        {
            return LightComputed &&
               Neighbors[(byte)BlocSide.FrontX] != null && Neighbors[(byte)BlocSide.FrontX].LightComputed &&
               Neighbors[(byte)BlocSide.BackX] != null && Neighbors[(byte)BlocSide.BackX].LightComputed &&
               Neighbors[(byte)BlocSide.FrontZ] != null && Neighbors[(byte)BlocSide.FrontZ].LightComputed &&
               Neighbors[(byte)BlocSide.BackZ] != null && Neighbors[(byte)BlocSide.BackZ].LightComputed;
        }

        

        VertexPositionNormalTextureColor[] _opaqueVertices;
        int[] _opaqueIndices;
        int _opaqueFaceCount;

        VertexPositionNormalTextureColor[] _waterVertices;
        int[] _waterIndices;
        int _waterFaceCount;
        

        Point3d NeighborsPositionOnNeighborsChunk(BlocSide side, Point3d position)
        {
            var decal = SideDecalToNeighbors[(int)side];
            return new Point3d()
            {
                X = decal.X == -1 ? position.X : decal.X,
                Y = decal.Y == -1 ? position.Y : decal.Y,
                Z = decal.Z == -1 ? position.Z : decal.Z
            };
        }

        public VertexPositionNormalTextureColor[] createPlantFace(Point3d position, int textureNumber)
        {
            VertexPositionNormalTextureColor[] vertices = new VertexPositionNormalTextureColor[24];

            float artificialLuminosity = (float)getArtificialLuminosity(position + FirstBlocAbsolutPosition) / 15;
            float sunLuminosity = (float)getSunLuminosity(position + FirstBlocAbsolutPosition) / 15;

            for (int i = 0;i < plantVerticesPositions.Length;i++)
            {
                Vector3 vertexPosition = plantVerticesPositions[i] + position.ToVector3();

                float decalX = (float)(textureNumber % 16) * (1.0f / 16.0f);
                float decalY = (float)(textureNumber / 16) * (1.0f / 16.0f);
                var uv = new Vector2(((1.0f / 16.0f) * plantVerticesUvs[i].X) + decalX, ((1.0f / 16.0f) * plantVerticesUvs[i].Y) + decalY);

                vertices[i] = new VertexPositionNormalTextureColor(vertexPosition, new Vector3(0, 0, 0), uv, sunLuminosity, artificialLuminosity, 1);
            }
            return vertices;
        }

        public void ComputeFaces()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            _opaqueFaceCount = 0;
            _waterFaceCount = 0;
            List<VertexPositionNormalTextureColor> opaquesVertices = new List<VertexPositionNormalTextureColor>();
            List<VertexPositionNormalTextureColor> waterVertices = new List<VertexPositionNormalTextureColor>();

            for (int z = 0; z < chunkSize; z++)
            {
                for (int y = _lowestBloc; y < _highestBloc + 1; y++)
                {
                    for (int x = 0; x < chunkSize; x++)
                    {
                        int index = positionToIndex(x, y, z);
                        if(Blocs[index] == 0)
                        {
                            continue;
                        }
                        int blocIndex2 = index / BlocDataSize;

                        bool opaque = IsOpaque(Blocs[index]);
                        bool water = IsWater(Blocs[index]);
                        bool plant = IsPlant(Blocs[index]);

                        if(plant)
                        {
                            var positionInChunk = new Point3d(x, y, z);
                            opaquesVertices.AddRange(createPlantFace(positionInChunk, GetTextureNumber(index, BlocSide.FrontX)));
                            _opaqueFaceCount += 8;
                        }
                        

                        for (int side = 0; side < 6; side++)
                        {
                            int blocNeighborChunk = BlocNeighborChunks[blocIndex2, side];
                            int blocNeighborIndex = BlocNeighborIndex[blocIndex2, side];
                            byte blocNeighborInfo;
                            if (blocNeighborChunk == 255)
                            {
                                blocNeighborInfo = blocNeighborInfo = Blocs[blocNeighborIndex];
                            }
                            else
                            {
                                if (Neighbors[blocNeighborChunk] == null)
                                {
                                    continue;
                                }
                                blocNeighborInfo = Neighbors[blocNeighborChunk].Blocs[blocNeighborIndex];
                            }

                            if (opaque && !IsOpaque(blocNeighborInfo))
                            {
                                var positionInChunk = new Point3d(x, y, z);
                                opaquesVertices.AddRange(createFaces((BlocSide)side, positionInChunk, GetTextureNumber(index, (BlocSide)side), new Vector3(0, 0, 0)));
                                _opaqueFaceCount += 2;
                            }
                            else if (water && (!IsOpaque(blocNeighborInfo) || side == (int)BlocSide.BackY) && !IsWater(blocNeighborInfo))
                            {
                                var positionInChunk = new Point3d(x, y, z);
                                waterVertices.AddRange(createFaces((BlocSide)side, positionInChunk, 14, new Vector3(0, -0.2f, 0)));
                                _waterFaceCount += 2;
                            }
                        }
                    }
                }
            }
            _opaqueVertices = opaquesVertices.ToArray();
            _waterVertices = waterVertices.ToArray();

            _opaqueIndices = new int[_opaqueVertices.Length];
            for (int i = 0; i < _opaqueIndices.Length; i++)
            {
                _opaqueIndices[i] = (int)i;
            }

            _waterIndices = new int[_waterVertices.Length];
            for (int i = 0; i < _waterIndices.Length; i++)
            {
                _waterIndices[i] = (int)i;
            }


            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            //Console.WriteLine("Face computing : " + elapsedMs);

            FacesComputing = false;
            FacesCreated = true;
        }

        public void UpdateCameraPosition(Vector3 cameraPosition)
        {
            DistanceFromCamera = (cameraPosition - new Vector3(FirstBlocAbsolutPosition.X, 0, FirstBlocAbsolutPosition.Z)).Length();
        }

        public void UpdateFaces()
        {
            if (FacesCreated)
            {
                FacesCreated = false;
                OpaqueFaceCount = _opaqueFaceCount;
                if (OpaqueFaceCount > 0)
                {
                    if (VertexBufferOpaque == null || VertexBufferOpaque.VertexCount < _opaqueVertices.Length)
                    {
                        if(VertexBufferOpaque != null)
                        {
                            VertexBufferOpaque.Dispose();
                        }
                        VertexBufferOpaque = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTextureColor), _opaqueVertices.Length + 2000, BufferUsage.WriteOnly);
                    }
                    VertexBufferOpaque.SetData<VertexPositionNormalTextureColor>(_opaqueVertices);

                    if (IndexBufferOpaque == null || IndexBufferOpaque.IndexCount < _opaqueIndices.Length)
                    {
                        if (IndexBufferOpaque != null)
                        {
                            IndexBufferOpaque.Dispose();
                        }
                        IndexBufferOpaque = new IndexBuffer(GraphicsDevice, typeof(int), _opaqueIndices.Length + 2000, BufferUsage.WriteOnly);
                    }
                    IndexBufferOpaque.SetData(_opaqueIndices);
                }

                WaterFaceCount = _waterFaceCount;
                if(WaterFaceCount > 0)
                {
                    if (VertexBufferWater == null || VertexBufferWater.VertexCount < _waterVertices.Length)
                    {
                        if (VertexBufferWater != null)
                        {
                            VertexBufferWater.Dispose();
                        }
                        VertexBufferWater = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTextureColor), _waterVertices.Length + 2000, BufferUsage.WriteOnly);
                    }
                    VertexBufferWater.SetData<VertexPositionNormalTextureColor>(_waterVertices);

                    if (IndexBufferWater == null || IndexBufferWater.IndexCount < _waterIndices.Length)
                    {
                        if (IndexBufferWater != null)
                        {
                            IndexBufferWater.Dispose();
                        }
                        IndexBufferWater = new IndexBuffer(GraphicsDevice, typeof(int), _waterIndices.Length + 2000, BufferUsage.WriteOnly);
                    }
                    IndexBufferWater.SetData(_waterIndices);
                }
                VertexBufferCreated = true;
            }
            else if(!VertexBufferCreated && !FacesComputing && !WaitForFacesComputing && UpdateFacesNeeded())
            {
                WaitForFacesComputing = true;
            }
        }
    }
}
