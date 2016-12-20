using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaVoxel
{
    partial class Chunk
    {
        public bool isOpaque(Point3d position)
        {
            Chunk chunk;
            int index;
            getChunkAndIndex(position, out chunk, out index);
            if (chunk == null)
            {
                return true;
            }
            return IsOpaque(chunk.Blocs[index]);
        }

        public bool isWater(Point3d position)
        {
            Chunk chunk;
            int index;
            getChunkAndIndex(position, out chunk, out index);
            if (chunk == null)
            {
                return false;
            }
            return IsWater(chunk.Blocs[index]);
        }

        public byte getSunLuminosity(Point3d position)
        {
            Chunk chunk;
            int index;
            getChunkAndIndex(position, out chunk, out index);
            if (chunk == null)
            {
                return 0;
            }
            return chunk.Blocs[index + 3];
        }

        private void setSunLuminosity(int index, byte value)
        {
            Blocs[index + 3] = value;
        }

        private byte getSunLuminosity(int index)
        {
            return Blocs[index + 3];
        }

        public byte getArtificialLuminosity(Point3d position)
        {
            Chunk chunk;
            int index;
            getChunkAndIndex(position, out chunk, out index);
            if (chunk == null)
            {
                return 0;
            }
            return chunk.Blocs[index + 4];
        }

        private void setArtificialLuminosity(int index, byte value)
        {
            Blocs[index + 4] = value;
        }

        private byte getArtificialLuminosity(int index)
        {
            return Blocs[index + 4];
        }

        private void setOpacity(int index, bool value)
        {
            if (value)
            {
                Blocs[index] |= 1;
            }
            else
            {
                if (IsBitSet(Blocs[index], 0))
                {
                    Blocs[index] -= 1;
                }
            }
        }

        private void setWater(int index, bool value)
        {
            if (value)
            {
                Blocs[index] |= (1 << 1);
            }
            else
            {
                if (IsBitSet(Blocs[index], 1))
                {
                    Blocs[index] -= (1 << 1);
                }
            }
        }

        private void setPlant(int index, bool value)
        {
            if (value)
            {
                Blocs[index] |= (1 << 2);
            }
            else
            {
                if (IsBitSet(Blocs[index], 2))
                {
                    Blocs[index] -= (1 << 2);
                }
            }
        }

        private void setMaterial(int index, BlocMaterial material)
        {
            byte[] data = BitConverter.GetBytes((ushort)material);
            Blocs[index + 1] = data[0];
            Blocs[index + 2] = data[1];
        }

        private BlocMaterial getMaterial(int index)
        {
            ushort value = BitConverter.ToUInt16(Blocs, index + 1);
            return (BlocMaterial)value;
        }

        private int GetTextureNumber(int blocIndex, BlocSide blockSize)
        {
            switch(getMaterial(blocIndex))
            {
                case BlocMaterial.Stone:
                    return 1;
                case BlocMaterial.Sand:
                    return 18;
                case BlocMaterial.Foliage:
                    return 145;
                case BlocMaterial.Plank:
                    return 4;
                case BlocMaterial.FlowerRed:
                    return 12;
                case BlocMaterial.Reed:
                    return 73;
                case BlocMaterial.FlowerYellow:
                    return 13;
                case BlocMaterial.Mushroom:
                    return 29;
                case BlocMaterial.TreeSprout:
                    return 15;
                case BlocMaterial.Grass:
                    return 91;
                case BlocMaterial.Wood:
                    if (blockSize == BlocSide.BackY)
                    {
                        return 21;
                    }
                    else
                    {
                        return 20;
                    }
                case BlocMaterial.Earth:
                    if (blockSize == BlocSide.BackY)
                    {
                        return 0;
                    }
                    else
                    {
                        return 3;
                    }
                default:
                    return 0;
            }
        }

        public static int positionToIndex(Point3d position)
        {
            return ((position.Z * (chunkSize * chunkHeight)) + (position.Y * chunkSize) + position.X) * BlocDataSize;
        }

        public static int positionToIndex(int x, int y, int z)
        {
            return ((z * (chunkSize * chunkHeight)) + (y * chunkSize) + x) * BlocDataSize;
        }

        private Point3d indexToPosition(int index)
        {
            return new Point3d()
            {
                Z = index / (chunkSize * chunkHeight),
                Y = (index / chunkHeight) % chunkSize,
                X = index % chunkSize
            };
        }

        public static bool IsOpaque(byte blocInfo)
        {
            return IsBitSet(blocInfo, 0);
        }

        public static bool IsWater(byte blocInfo)
        {
            return IsBitSet(blocInfo, 1);
        }

        public static bool IsPlant(byte blocInfo)
        {
            return IsBitSet(blocInfo, 2);
        }

        public static bool IsBitSet(byte value, int bitNumber)
        {
            return ((value & (1 << bitNumber)) != 0);
        }

        public void getChunkAndIndex(Point3d position, out Chunk chunk, out int index)
        {
            index = -1;
            chunk = null;
            Point3d relativePosition = position - FirstBlocAbsolutPosition;
            if (relativePosition.X < 0)
            {
                if (Neighbors[(byte)BlocSide.FrontX] != null)
                {
                    Neighbors[(byte)BlocSide.FrontX].getChunkAndIndex(position, out chunk, out index);
                }
                return;
            }
            else if (relativePosition.X >= chunkSize)
            {
                if (Neighbors[(byte)BlocSide.BackX] != null)
                {
                    Neighbors[(byte)BlocSide.BackX].getChunkAndIndex(position, out chunk, out index);
                }
                return;
            }
            if (relativePosition.Y < 0 || relativePosition.Y >= chunkHeight)
            {
                return;
            }
            if (relativePosition.Z < 0)
            {
                if (Neighbors[(byte)BlocSide.FrontZ] != null)
                {
                    Neighbors[(byte)BlocSide.FrontZ].getChunkAndIndex(position, out chunk, out index);
                }
                return;
            }
            else if (relativePosition.Z >= chunkSize)
            {
                if (Neighbors[(byte)BlocSide.BackZ] != null)
                {
                    Neighbors[(byte)BlocSide.BackZ].getChunkAndIndex(position, out chunk, out index);
                }
                return;
            }
            chunk = this;
            index = positionToIndex(relativePosition);
        }

        static byte[,] BlocNeighborChunks;
        static int[,] BlocNeighborIndex;

        public static void initNeighborsRef()
        {
            int blocCount = chunkSize * chunkHeight * chunkSize;
            BlocNeighborChunks = new byte[blocCount, 6];
            BlocNeighborIndex = new int[blocCount, 6];
            for (int z = 0; z < chunkSize; z++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    for (int x = 0; x < chunkSize; x++)
                    {
                        int index = positionToIndex(x, y, z) / BlocDataSize;
                        for (byte side = 0; side < 6; side++)
                        {
                            byte neighborChunks = (byte)255;
                            int neighborX = x + SideDecal[side].X;
                            if (neighborX < 0)
                            {
                                neighborX = chunkSize - 1;
                                neighborChunks = (byte)BlocSide.FrontX;
                            }
                            else if (neighborX > chunkSize - 1)
                            {
                                neighborX = 0;
                                neighborChunks = (byte)BlocSide.BackX;
                            }

                            int neighborY = y + SideDecal[side].Y;
                            if (neighborY < 0)
                            {
                                neighborY = chunkHeight - 1;
                                neighborChunks = (byte)BlocSide.FrontY;
                            }
                            else if (neighborY > chunkHeight - 1)
                            {
                                neighborY = 0;
                                neighborChunks = (byte)BlocSide.BackY;
                            }

                            int neighborZ = z + SideDecal[side].Z;
                            if (neighborZ < 0)
                            {
                                neighborZ = chunkSize - 1;
                                neighborChunks = (byte)BlocSide.FrontZ;
                            }
                            else if (neighborZ > chunkSize - 1)
                            {
                                neighborZ = 0;
                                neighborChunks = (byte)BlocSide.BackZ;
                            }
                            int neighborIndex = positionToIndex(neighborX, neighborY, neighborZ);
                            BlocNeighborIndex[index, side] = neighborIndex;
                            BlocNeighborChunks[index, side] = neighborChunks;
                        }
                    }
                }
            }
        }
    }
}
