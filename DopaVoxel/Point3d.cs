using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaVoxel
{
    class Point3d
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public Point3d()
        {

        }

        public Point3d(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3d Copy()
        {
            return new Point3d(X, Y, Z);
        }

        public static Point3d operator +(Point3d p1, Point3d p2)
        {
            return new Point3d(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        public static Point3d operator -(Point3d p1, Point3d p2)
        {
            return new Point3d(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public static bool operator ==(Point3d p1, Point3d p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
        }

        public static bool operator !=(Point3d p1, Point3d p2)
        {
            return !(p1 == p2);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
