using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaVoxel
{
    partial class Chunk
    {
        public void computeLight()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            computeSunLight();
            computeArtificialLight();

            if(LightFirstPassComputed)
            {
                LightComputed = true;
                VertexBufferCreated = false;
            }
            else
            {
                LightFirstPassComputed = true;
            }
            LightComputing = false;

            var elapsedMs = watch.ElapsedMilliseconds;
            //Console.WriteLine("Light computing : " + elapsedMs);
        }

        void computeArtificialLight()
        {
            int blocCount = chunkHeight * chunkSize * chunkSize;
            for (int iteration = 15; iteration >= 1; iteration--)
            {
                int blocIndex2 = BlocDataSize * -1;
                for (int blocIndex = 0; blocIndex < blocCount; blocIndex++)
                {
                    blocIndex2 += BlocDataSize;
                    if (getArtificialLuminosity(blocIndex2) != iteration)
                    {
                        continue;
                    }

                    for (int side = 0; side < 6; side++)
                    {
                        int blocNeighborChunk = BlocNeighborChunks[blocIndex, side];
                        int blocNeighborIndex = BlocNeighborIndex[blocIndex, side];
                        if (blocNeighborChunk == 255)
                        {
                            if (getArtificialLuminosity(blocNeighborIndex) < iteration - 1)
                            {
                                int luminosity = iteration - 1;
                                if (IsWater(Blocs[blocNeighborIndex]))
                                {
                                    luminosity = Math.Max(luminosity - 3, 0);
                                }
                                setArtificialLuminosity(blocNeighborIndex, (byte)luminosity);
                            }
                        }
                        else
                        {
                            if (blocNeighborChunk == (byte)BlocSide.FrontY || blocNeighborChunk == (byte)BlocSide.BackY)
                            {
                                continue;
                            }
                            if (Neighbors[blocNeighborChunk] != null && Neighbors[blocNeighborChunk].getSunLuminosity(blocNeighborIndex) < iteration - 1)
                            {
                                int luminosity = iteration - 1;
                                if (Chunk.IsWater(Blocs[blocNeighborIndex]))
                                {
                                    luminosity = Math.Max(luminosity - 3, 0);
                                }
                                Neighbors[blocNeighborChunk].setArtificialLuminosity(blocNeighborIndex, (byte)luminosity);
                                break;
                            }
                        }
                    }
                }
            }
        }

        void computeSunLight()
        {
            int blocCount = chunkHeight * chunkSize * chunkSize;
            for (int iteration = 15; iteration >= 1; iteration--)
            {
                int blocIndex2 = BlocDataSize * -1;
                for (int blocIndex = 0; blocIndex < blocCount; blocIndex++)
                {
                    blocIndex2 += BlocDataSize;
                    if (Blocs[blocIndex2] == 1 || getSunLuminosity(blocIndex2) >= iteration)
                    {
                        continue;
                    }
                    for (int side = 0; side < 6; side++)
                    {
                        int blocNeighborChunk = BlocNeighborChunks[blocIndex, side];
                        int blocNeighborIndex = BlocNeighborIndex[blocIndex, side];
                        if (blocNeighborChunk == 255)
                        {
                            if (getSunLuminosity(blocNeighborIndex) == iteration)
                            {
                                int luminosity = iteration - 1;
                                if (IsWater(Blocs[blocIndex2]))
                                {
                                    luminosity = Math.Max(luminosity - 3, 0);
                                }
                                setSunLuminosity(blocIndex2, (byte)luminosity);
                                break;
                            }
                        }
                        else
                        {
                            if (blocNeighborChunk == (byte)BlocSide.FrontY || blocNeighborChunk == (byte)BlocSide.BackY)
                            {
                                continue;
                            }
                            if (Neighbors[blocNeighborChunk] != null && Neighbors[blocNeighborChunk].getSunLuminosity(blocNeighborIndex) == iteration)
                            {
                                int luminosity = iteration - 1;
                                if (IsWater(Blocs[blocIndex2]))
                                {
                                    luminosity = Math.Max(luminosity - 3, 0);
                                }
                                setSunLuminosity(blocIndex2, (byte)luminosity);
                                break;
                            }
                        }
                    }
                }
            }
        }

        void updateLight()
        {
             if(WaitForLightUpdate || LightComputing || LightComputed)
            {
                return;
            }

            if (!LightFirstPassComputed)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i == (int)BlocSide.FrontY || i == (int)BlocSide.BackY)
                    {
                        continue;
                    }
                    if (Neighbors[i] == null)
                    {
                        return;
                    }
                }
                WaitForLightUpdate = true;
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i == (int)BlocSide.FrontY || i == (int)BlocSide.BackY)
                    {
                        continue;
                    }
                    if (Neighbors[i] == null || !Neighbors[i].LightFirstPassComputed)
                    {
                        return;
                    }
                }
                WaitForLightUpdate = true;
            }
        }
    }
}
