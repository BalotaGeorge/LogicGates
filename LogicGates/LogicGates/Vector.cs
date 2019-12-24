using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicGates
{
    class Vector
    {
        public float x;
        public float y;
        public float z;
        public Vector(float _x = 0f, float _y = 0f, float _z = 0f)
        {
            x = _x;
            y = _y;
            z = _z;
        }
        public Vector(PointF p)
        {
            x = p.X;
            y = p.Y;
            z = 0f;
        }
        public float Dot(Vector v)
        {
            return (x * v.x + y * v.y + z * v.z);
        }
        public Vector Cross(Vector v)
        {
            return new Vector(y * v.z - z * v.y, z * v.x - x * v.z, x * v.y - y * v.x);
        }
        public float Magnitude()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }
        public Vector Normalize()
        {
            float d = (float)Math.Sqrt(x * x + y * y + z * z);
            x /= d;
            y /= d;
            z /= d;
            return new Vector(x, y, z);
        }
        public Vector VectorFromAngle(float angle)
        {
            return new Vector((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        public float AngleFromVector()
        {
            return (float)Math.Atan2(y, x);
        }
        public bool OutsideBounds(float x1, float y1, float x2, float y2)
        {
            if(x1 > x2)
            {
                float t = x1;
                x1 = x2;
                x2 = t;
            }
            if(y1 > y2)
            {
                float t = y1;
                y1 = y2;
                y2 = t;
            }
            if (x >= x1 && y >= y1 && x <= x2 && y <= y2) return false;
            return true;
        }
        public PointF AsPoint()
        {
            return new PointF(x, y);
        }
        public override string ToString()
        {
            return $"x: {x}, y: {y}, z: {z}";
        }
        public static Vector operator *(Vector v1, float scalar)
        {
            return new Vector(v1.x * scalar, v1.y * scalar, v1.z * scalar);
        }
        public static Vector operator *(float scalar, Vector v1)
        {
            return new Vector(v1.x * scalar, v1.y * scalar, v1.z * scalar);
        }
        public static Vector operator /(Vector v1, float scalar)
        {
            return new Vector(v1.x / scalar, v1.y / scalar, v1.z / scalar);
        }
        public static Vector operator /(float scalar, Vector v1)
        {
            return new Vector(v1.x / scalar, v1.y / scalar, v1.z / scalar);
        }
        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
    }
}
