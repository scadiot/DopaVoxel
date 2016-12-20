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


    class ChunkManager
    {
        const int _chunkLoadDistance = 20;
        public GraphicsDevice GraphicsDevice { get; set; }

        public List<Chunk> Chunks { get; set; }
        Dictionary<Tuple<int, int>, Chunk> _chunksByPosition;
        HeightMap _heightMap;

        List<Chunk> _chunksToLoad;
        Mutex _chunksToLoadMutex;

        List<Chunk> _chunksToLightUpdate;
        Mutex _chunksToLightUpdateMutex;

        List<Chunk> _chunksToLightPriorityUpdate;
        Mutex _chunksToLightPriorityUpdateMutex;

        List<Chunk> _chunksFacesComputing;
        Mutex _chunksFacesComputingMutex;

        List<Chunk> _unusedChunksList;

        public void Initialize()
        {
            Chunk.initNeighborsRef();

            _heightMap = new HeightMap();
            _heightMap.init();

            Chunks = new List<Chunk>();
            _chunksByPosition = new Dictionary<Tuple<int, int>, Chunk>();

            _unusedChunksList = new List<Chunk>();

            _chunksToLoad = new List<Chunk>();
            _chunksToLoadMutex = new Mutex();
            Thread t = new Thread(WorkerChunkLoader);
            t.IsBackground = true;
            t.Start();

            _chunksFacesComputing = new List<Chunk>();
            _chunksFacesComputingMutex = new Mutex();
            Thread t2 = new Thread(WorkerFacesComputing);
            t2.IsBackground = true;
            t2.Start();

            _chunksToLightUpdate = new List<Chunk>();
            _chunksToLightUpdateMutex = new Mutex();
            _chunksToLightPriorityUpdate = new List<Chunk>();
            _chunksToLightPriorityUpdateMutex = new Mutex();
            Thread t3 = new Thread(WorkerLightComputing);
            t3.IsBackground = true;
            t3.Start();
            //addMiltiplesLight(20);
        }

        void AddChunkLightPriorityComputing(Chunk[] chunks)
        {
            _chunksToLightPriorityUpdateMutex.WaitOne();
            _chunksToLightPriorityUpdate.AddRange(chunks);
            foreach(var chunk in chunks)
            {
                chunk.LightComputed = false;
                chunk.LightFirstPassComputed = false;
                chunk.LightComputing = true;
            }
            _chunksToLightPriorityUpdateMutex.ReleaseMutex();
        }

        void AddChunkLightComputing(Chunk chunk)
        {
            _chunksToLightPriorityUpdateMutex.WaitOne();
            _chunksToLightUpdate.Add(chunk);
            chunk.WaitForLightUpdate = false;
            chunk.LightComputing = true;
            _chunksToLightPriorityUpdateMutex.ReleaseMutex();
        }

        void WorkerLightComputing()
        {
            while (true)
            {
                _chunksToLightUpdateMutex.WaitOne();
                _chunksToLightPriorityUpdateMutex.WaitOne();
                Chunk chunksToLightUpdate = null;
                if (_chunksToLightPriorityUpdate.Count > 0)
                {
                    chunksToLightUpdate = _chunksToLightPriorityUpdate.First();
                    _chunksToLightPriorityUpdate.RemoveAt(0);
                }
                else
                {
                    foreach (Chunk chunk in _chunksToLightUpdate)
                    {
                        if (chunksToLightUpdate == null ||
                            chunksToLightUpdate.DistanceFromCamera > chunk.DistanceFromCamera)
                        {
                            chunksToLightUpdate = chunk;
                        }
                    }

                    if (chunksToLightUpdate != null)
                    {
                        _chunksToLightUpdate.Remove(chunksToLightUpdate);
                    }
                }

                _chunksToLightPriorityUpdateMutex.ReleaseMutex();
                _chunksToLightUpdateMutex.ReleaseMutex();

                if (chunksToLightUpdate != null)
                {
                    chunksToLightUpdate.computeLight();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        void AddChunkFacesComputing(Chunk chunk)
        {
            _chunksFacesComputingMutex.WaitOne();
            _chunksFacesComputing.Add(chunk);
            chunk.WaitForFacesComputing = false;
            chunk.FacesComputing = true;
            _chunksFacesComputingMutex.ReleaseMutex();
        }

        void WorkerFacesComputing()
        {
            while (true)
            {
                _chunksFacesComputingMutex.WaitOne();
                Chunk chunksFacesComputing = null;
                foreach(Chunk chunk in _chunksFacesComputing)
                {
                    if(chunksFacesComputing == null ||
                       chunksFacesComputing.DistanceFromCamera > chunk.DistanceFromCamera)
                    {
                        chunksFacesComputing = chunk;
                    }
                }

                if (chunksFacesComputing != null)
                {
                    _chunksFacesComputing.Remove(chunksFacesComputing);
                }
                _chunksFacesComputingMutex.ReleaseMutex();

                if (chunksFacesComputing != null)
                {
                    chunksFacesComputing.ComputeFaces();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        void AddChunkToLoad(Chunk chunk)
        {
            _chunksToLoadMutex.WaitOne();
            _chunksToLoad.Add(chunk);
            _chunksToLoadMutex.ReleaseMutex();
        }

        void WorkerChunkLoader()
        {
            
            while(true)
            {
                _chunksToLoadMutex.WaitOne();
                Chunk chunkToLoad = null;
                if(_chunksToLoad.Count > 0)
                {
                    chunkToLoad = _chunksToLoad.First();
                    _chunksToLoad.RemoveAt(0);
                }
                _chunksToLoadMutex.ReleaseMutex();

                if(chunkToLoad != null)
                {
                    chunkToLoad.LoadData();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private bool IsInScope(Point3d chunkPosition, Vector3 cameraPosition, float bias)
        {
            Vector2 cameraPosition2D = new Vector2(cameraPosition.X, cameraPosition.Z);
            Vector2 chunkPosition2D = new Vector2(chunkPosition.X, chunkPosition.Z);

            if ((chunkPosition2D - (cameraPosition2D / Chunk.chunkSize)).Length() > _chunkLoadDistance + bias)
            {
                return false;
            }
            return true;
        }

        private Chunk GetChunk(Point3d position, Vector3 cameraPosition)
        {
            if (_chunksByPosition.ContainsKey(new Tuple<int, int>(position.X, position.Z)))
            {
                Chunk chunk = _chunksByPosition[new Tuple<int, int>(position.X, position.Z)];
                if(chunk.IsLoading)
                {
                    return null;
                }
                else
                {
                    return chunk;
                }
            }
            else
            {
                Chunk chunk = null;
                if (_unusedChunksList.Count > 0)
                {
                    chunk = _unusedChunksList.First();
                    _unusedChunksList.RemoveAt(0);
                    chunk.ChunkPosition = position.Copy();
                }
                else
                {
                    chunk = new Chunk()
                    {
                        ChunkPosition = position.Copy(),
                        GraphicsDevice = GraphicsDevice,
                        HeightMap = _heightMap,
                        ChunkManager = this
                    };
                }
                
                chunk.Initialize();
                chunk.UpdateCameraPosition(cameraPosition);

                _chunksByPosition.Add(new Tuple<int, int>(position.X, position.Z), chunk);

                AddChunkToLoad(chunk);
            }
            return null;
        }

        private void UnloadChunks(Vector3 cameraPosition)
        {
            List<Chunk> chunksToRemove = new List<Chunk>();
            foreach(Chunk chunk in Chunks)
            {
                if(!IsInScope(chunk.ChunkPosition, cameraPosition, 1.0f))
                {
                    chunksToRemove.Add(chunk);
                }
            }
            foreach (Chunk chunkToRemove in chunksToRemove)
            {
                unloadChunk(chunkToRemove);
                Chunks.Remove(chunkToRemove);
                int n = _chunksByPosition.Count;
                _chunksByPosition.Remove(new Tuple<int, int>(chunkToRemove.ChunkPosition.X, chunkToRemove.ChunkPosition.Z));
            }
        }

        private void unloadChunk(Chunk chunk)
        {
            if(chunk.Neighbors[(byte)BlocSide.FrontX] != null)
            {
                chunk.Neighbors[(byte)BlocSide.FrontX].Neighbors[(byte)BlocSide.BackX] = null;
            }
            if (chunk.Neighbors[(byte)BlocSide.BackX] != null)
            {
                chunk.Neighbors[(byte)BlocSide.BackX].Neighbors[(byte)BlocSide.FrontX] = null;
            }
            if (chunk.Neighbors[(byte)BlocSide.FrontZ] != null)
            {
                chunk.Neighbors[(byte)BlocSide.FrontZ].Neighbors[(byte)BlocSide.BackZ] = null;
            }
            if (chunk.Neighbors[(byte)BlocSide.BackZ] != null)
            {
                chunk.Neighbors[(byte)BlocSide.BackZ].Neighbors[(byte)BlocSide.FrontZ] = null;
            }

            chunk.uninit();
            _unusedChunksList.Add(chunk);
        }

        private void LoadChunk(Chunk chunk, Vector3 cameraPosition)
        {
            foreach (BlocSide side in Enum.GetValues(typeof(BlocSide)))
            {
                if(side == BlocSide.FrontY || side == BlocSide.BackY)
                {
                    continue;
                }
                var decal = Chunk.SideDecal[(int)side];
                if (chunk.Neighbors[(int)side] == null && IsInScope(chunk.ChunkPosition + decal, cameraPosition, 0))
                {
                    var newChunk = GetChunk(chunk.ChunkPosition + decal, cameraPosition);
                    if (newChunk != null)
                    {
                        chunk.Neighbors[(int)side] = newChunk;
                        if(!Chunks.Contains(newChunk))
                        {
                            Chunks.Add(newChunk);
                        }
                    }
                }
            }
        }

        public Chunk GetChunkAtPosition(Vector3 position)
        {
            Point3d chunkPosition = new Point3d()
            {
                X = (int)(position.X / Chunk.chunkSize),
                Y = 0,
                Z = (int)(position.Z / Chunk.chunkSize)
            };

            if (_chunksByPosition.ContainsKey(new Tuple<int, int>((int)chunkPosition.X, chunkPosition.Z)))
            {
                Chunk chunk = _chunksByPosition[new Tuple<int, int>(chunkPosition.X, chunkPosition.Z)];
                if (chunk.IsLoading)
                {
                    return null;
                }
                else
                {
                    return chunk;
                }
            }
            else
            {
                return null;
            }
        }

        public void Update(Vector3 cameraPosition)
        {
            UnloadChunks(cameraPosition);

            Point3d chunkPosition = new Point3d()
            {
                X = (int)(cameraPosition.X / Chunk.chunkSize),
                Y = 0,
                Z = (int)(cameraPosition.Z / Chunk.chunkSize)
            };

            Chunk cameraChunk = GetChunk(chunkPosition, cameraPosition);
            if(cameraChunk != null)
            {
                if (!Chunks.Contains(cameraChunk))
                {
                    Chunks.Add(cameraChunk);
                }
            }

            for (int i = 0;i < Chunks.Count;i++)
            {
                LoadChunk(Chunks[i], cameraPosition);
            }

            foreach (Chunk chunk in Chunks)
            {
                chunk.UpdateCameraPosition(cameraPosition);
            }

            foreach (Chunk chunk in Chunks)
            {
                chunk.update();
                if (chunk.WaitForLightUpdate)
                {
                    AddChunkLightComputing(chunk);
                }
                if (chunk.WaitForFacesComputing)
                {
                    AddChunkFacesComputing(chunk);
                }
            }
        }

        private void UpdateLight(Point3d position)
        {
            Chunk chunk = GetChunkAtPosition(position.ToVector3());
            if(chunk == null)
            {
                return;
            }
            List<Chunk> chunksToUpdateLight = new List<Chunk>();

            for (int neighborIndex = 0; neighborIndex < 6; neighborIndex++)
            {
                var neighborChunk = chunk.Neighbors[neighborIndex];
                if (neighborChunk == null)
                {
                    continue;
                }
                chunksToUpdateLight.Insert(0, neighborChunk);

                for (int neighborIndex2 = 0; neighborIndex2 < 6; neighborIndex2++)
                {
                    var neighborChunk2 = neighborChunk.Neighbors[neighborIndex2];
                    if (neighborChunk2 == null || neighborIndex == neighborIndex2 || chunksToUpdateLight.Contains(neighborChunk2))
                    {
                        continue;
                    }
                    chunksToUpdateLight.Add(neighborChunk2);
                }
            }
            chunksToUpdateLight.Insert(0, chunk);

            foreach(var chunkToUpdateLight in chunksToUpdateLight)
            {
                for (int index = 0; index < chunkToUpdateLight.Blocs.Length; index += Chunk.BlocDataSize)
                {
                    if(chunkToUpdateLight.Blocs[index + 3] != 15)
                    {
                        chunkToUpdateLight.Blocs[index + 3] = 0;
                    }
                }
            }

            AddChunkLightPriorityComputing(chunksToUpdateLight.ToArray());
        }

        public void RemoveBloc(Point3d position)
        {
            Chunk chunk = GetChunkAtPosition(position.ToVector3());
            if (chunk != null)
            {
                Point3d relativePosition = position - chunk.FirstBlocAbsolutPosition;
                int index = Chunk.positionToIndex(relativePosition);
                chunk.Blocs[index] = 0;
                chunk.ComputeFaces();
                chunk.update();
                if (chunk.getSunLuminosity(new Point3d(position.X, position.Y + 1, position.Z)) == 15)
                {
                    for (int y = position.Y; y >= 0; --y)
                    {
                        Point3d p = new Point3d(position.X, y, position.Z);
                        if (chunk.isOpaque(p))
                        {
                            break;
                        }
                        chunk.Blocs[Chunk.positionToIndex(p) + 3] = 15;
                    }
                }
                
                UpdateLight(position);
            }
        }

        public void AddBloc(Point3d position)
        {
            Chunk chunk = GetChunkAtPosition(position.ToVector3());
            if (chunk != null)
            {
                Point3d relativePosition = position - chunk.FirstBlocAbsolutPosition;
                int index = Chunk.positionToIndex(relativePosition);
                chunk.Blocs[index] = 1;
                chunk.Blocs[index + 3] = 0;
                chunk.ComputeFaces();
                chunk.update();

                if (chunk.getSunLuminosity(new Point3d(position.X, position.Y + 1, position.Z)) == 15)
                {
                    for (int y = position.Y - 1; y >= 0; --y)
                    {
                        Point3d p = new Point3d(relativePosition.X, y, relativePosition.Z);
                        if (chunk.isOpaque(p))
                        {
                            break;
                        }
                        chunk.Blocs[Chunk.positionToIndex(p) + 3] = 0;
                    }
                }
                chunk.VertexBufferCreated = false;
                
                UpdateLight(position);
            }
        }
    }
}
