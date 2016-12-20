using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace VoxelTest
{
    class HeightMap
    {
        public int Seed { get; set; }
        List<byte[]> _heightMaps;
        Dictionary<Tuple<int, int>, int> _sections;
        Mutex _mutex;
        Random _random;

        const int margin = 64;
        const int mapSize = 512;


        public void init()
        {
            _random = new Random(Seed);
            _mutex = new Mutex();
            _sections = new Dictionary<Tuple<int, int>, int>();
            _heightMaps = new List<byte[]>();
            var repMaps = new DirectoryInfo("maps");
            foreach(var file in repMaps.GetFiles("*.height"))
            {
                _heightMaps.Add(loadFile(file.FullName));
            }
        }

        byte[] loadFile(string fileName)
        {
            byte[] result;
            byte[] data = File.ReadAllBytes(fileName);
            using (var inStream = new MemoryStream(data))
            using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (var bigStreamOut = new MemoryStream())
            {
                bigStream.CopyTo(bigStreamOut);
                result = bigStreamOut.ToArray();
            }
            return result;
        }

        private byte[] getHeightMap(int imageX, int imageZ)
        {
            var imageKey = new Tuple<int, int>(imageX, imageZ);
            byte[] heigtMap;
            if (_sections.ContainsKey(imageKey))
            {
                int heightMapIndex = _sections[imageKey];
                heigtMap = _heightMaps[heightMapIndex];
            }
            else
            {
                int heightMapIndex = _random.Next() % _heightMaps.Count;
                _sections.Add(imageKey, heightMapIndex);
                heigtMap = _heightMaps[heightMapIndex];
            }
            return heigtMap;
        }

        private void getPixelPosition(int imageX, int imageZ, int absolutX, int absolutZ, out int pixelX, out int pixelZ)
        {
            pixelX = absolutX - ((mapSize - margin) * imageX);
            if (absolutX < 0)
            {
                pixelX--;
            }

            pixelZ = absolutZ - ((mapSize - margin) * imageZ);
            if (absolutZ < 0)
            {
                pixelZ--;
            }
        }

        private int getHeightOnMap(int imageX, int imageZ, int x, int z)
        {
            byte[] heigtMap = getHeightMap(imageX, imageZ);
            return heigtMap[x * mapSize + z];
        }

        private int getMixedHeightOnMap(int imageX, int imageZ, int absolutX, int absolutZ)
        {
            int pixelX;
            int pixelZ;
            getPixelPosition(imageX, imageZ, absolutX, absolutZ, out pixelX, out pixelZ);
            if (pixelX < 0 || pixelX >= 512 || pixelZ < 0 || pixelZ >= 512)
            {
                return 0;
            }

            int height = getHeightOnMap(imageX, imageZ, pixelX, pixelZ);

            int mixCount = 0;

            float ratioX = 0.0f;
            if(pixelX < margin)
            {
                ratioX = (float)pixelX / margin;
                mixCount++;
            }
            else if (pixelX > mapSize - margin)
            {
                ratioX = ((float)mapSize - pixelX) / margin;
                mixCount++;
            }

            float ratioZ = 0.0f;
            if (pixelZ < margin)
            {
                ratioZ = (float)pixelZ / margin;
                mixCount++;
            }
            else if (pixelZ > mapSize - margin)
            {
                ratioZ = ((float)mapSize - pixelZ) / margin;
                mixCount++;
            }

            if(mixCount == 0)
            {
                return height;
            }
            else if(mixCount == 1)
            {
                return (int)((float)height * (ratioX + ratioZ));
            }
            else if(mixCount == 2)
            {
                return (int)((float)height * (ratioX * ratioZ));
            }
            return 0;
        }

        public int getHeight(int x, int z)
        {
            _mutex.WaitOne();
            int imageX = x / (mapSize - margin);
            if (x < 0)
            {
                imageX = (x / (mapSize - margin)) - 1;
            }

            int imageZ = z / (mapSize - margin);
            if (z < 0)
            {
                imageZ = (z / (mapSize - margin)) - 1;
            }

            int height = getMixedHeightOnMap(imageX, imageZ, x, z);
            height += getMixedHeightOnMap(imageX - 1, imageZ, x, z);
            height += getMixedHeightOnMap(imageX, imageZ - 1, x, z);
            height += getMixedHeightOnMap(imageX - 1, imageZ - 1, x, z);

            int result = height / 10 + 118;

            _mutex.ReleaseMutex();
            return result;
        }
    }
}
