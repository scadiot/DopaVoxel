using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaVoxel
{
    partial class Chunk
    {
        Random _random;

        public void LoadData()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _random = new Random(ChunkPosition.X * (chunkSize * chunkSize) + ChunkPosition.Y * chunkSize + ChunkPosition.Z);
            _highestBloc = 128;
            _lowestBloc = 110;
            for (int z = 0; z < chunkSize; z++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    var height = HeightMap.getHeight(x + (ChunkPosition.X * chunkSize), z + (ChunkPosition.Z * chunkSize));
                    int luminosity = 15;
                   
                    for (int y = chunkHeight - 1; y >= 0; y--)
                    {
                        int artificialLight = 0;
                        int index = positionToIndex(new Point3d(x, y, z));
                        Blocs[index] = 0;
                        if (y < height)
                        {
                            BlocMaterial material = BlocMaterial.Air;

                            if (height - 1 == y)
                            {
                                if(y < 128)
                                {
                                    material = BlocMaterial.Sand;
                                }
                                else
                                {
                                    material = BlocMaterial.Earth;
                                }
                                
                            }
                            else if (height > y)
                            {
                                material = BlocMaterial.Stone;
                            }
                            else
                            {
                                material = BlocMaterial.Wood;
                            }


                            luminosity = 0;

                            setOpacity(index, true);
                            setWater(index, false);
                            setMaterial(index, material);

                            _highestBloc = Math.Max(_highestBloc, y);
                        }
                        else
                        {
                            if (y < 128)
                            {
                                setWater(index, true);
                                luminosity -= 3;
                            }
                            else
                            {
                                setWater(index, false);
                            }
                            setOpacity(index, false);
                        }
                        luminosity = (byte)Math.Max((int)luminosity, 0);

                        setSunLuminosity(index, (byte)luminosity);
                        setArtificialLuminosity(index, (byte)artificialLight);
                    }
                }
            }

            createBox(new Point3d(10,128,10), new Point3d(30, 134, 30));
            int treeCount = _random.Next() % 3;
            for (int i =0;i < treeCount; i++)
            {
                createTree(new Point3d(_random.Next() % 11 + 2, 0, _random.Next() % 11 + 2));
            }

            addArtificialLights();
            addPlants();

            var elapsedMs = watch.ElapsedMilliseconds;
            //Console.WriteLine("Loading computing : " + elapsedMs);
            IsLoading = false;
        }

        private void addArtificialLights()
        {
            int lightCount = _random.Next() % 3;
            for (int i = 0; i < lightCount; i++)
            {
                var position = new Point3d(_random.Next() % 13 + 1, 0, _random.Next() % 13 + 1);
                Point3d absolutPosition = position + FirstBlocAbsolutPosition;
                while (isOpaque(absolutPosition))
                {
                    absolutPosition.Y++;
                    position.Y++;
                }
                int index = positionToIndex(position);
                setArtificialLuminosity(index, (byte)(_random.Next() % 10 + 5));
            }
        }

        private void addPlants()
        {
            int plantCount = _random.Next() % 10;
            for (int i = 0; i < plantCount; i++)
            {
                var position = new Point3d(_random.Next() % 13 + 1, 0, _random.Next() % 13 + 1);
                Point3d absolutPosition = position + FirstBlocAbsolutPosition;
                while (isOpaque(absolutPosition))
                {
                    absolutPosition.Y++;
                    position.Y++;
                }
                int index = positionToIndex(position);
                setPlant(index, true);

                int materialType = (_random.Next() % 5) + (int)BlocMaterial.TreeSprout;
                if (position.Y < 128)
                {
                    materialType = (int)BlocMaterial.Reed;
                }

                setMaterial(index, (BlocMaterial)materialType);
            }
        }

        private void createTree(Point3d position)
        {
            Point3d currentPosition = new Point3d(position.X, 0, position.Z);
            Point3d absolutPosition = currentPosition + FirstBlocAbsolutPosition;
            while (isOpaque(absolutPosition))
            {
                currentPosition.Y++;
                absolutPosition.Y++;
            }

            if(currentPosition.Y < 128)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                int index = positionToIndex(currentPosition);
                setSunLuminosity(index, (byte)0);
                setArtificialLuminosity(index, (byte)0);
                setOpacity(index, true);
                setMaterial(index, BlocMaterial.Wood);

                currentPosition.Y++;
                _highestBloc = Math.Max(_highestBloc, currentPosition.Y);
            }

            currentPosition.Y += 0;

            for (int z = currentPosition.Z - 3; z < currentPosition.Z + 3; z++)
            {
                for (int y = currentPosition.Y - 3; y < currentPosition.Y + 3; y++)
                {
                    for (int x = currentPosition.X - 3; x < currentPosition.X + 3; x++)
                    {
                        var blocPosition = new Point3d(x, y, z);
                        if((blocPosition.ToVector3() - currentPosition.ToVector3()).Length() < 2.3f)
                        {
                            int index = positionToIndex(blocPosition);
                            setSunLuminosity(index, (byte)0);
                            setArtificialLuminosity(index, (byte)0);
                            setOpacity(index, true);
                            setMaterial(index, BlocMaterial.Foliage);
                            _highestBloc = Math.Max(_highestBloc, y);
                        }
                    }
                }
            }
        }

        private void createBox(Point3d position1, Point3d position2)
        {
            for(int z = position1.Z; z < position2.Z + 1;z++)
            {
                for (int y = position1.Y; y < position2.Y + 1; y++)
                {
                    for (int x = position1.X; x < position2.X + 1; x++)
                    {
                        var position = new Point3d(x, y, z);
                        var relativePosition = position - FirstBlocAbsolutPosition;
                        if(relativePosition.X < 0 || relativePosition.X > chunkSize - 1 ||
                           relativePosition.Z < 0 || relativePosition.Z > chunkSize - 1)
                        {
                            continue;
                        }

                        int index = positionToIndex(relativePosition);

                        if (x == position1.X || x == position2.X ||
                            y == position1.Y || y == position2.Y ||
                            z == position1.Z || z == position2.Z)
                        {
                            setMaterial(index, BlocMaterial.Plank);
                            setOpacity(index, true);
                            _highestBloc = Math.Max(_highestBloc, y);
                        }
                        else
                        {
                            setMaterial(index, BlocMaterial.Air);
                            setOpacity(index, false);
                        }
                        setSunLuminosity(index, 0);
                        setArtificialLuminosity(index, 0);
                        setWater(index, false);
                    }
                }
            }
        }
    }
}
