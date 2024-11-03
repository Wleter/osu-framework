// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using osuTK;

namespace osu.Framework.Graphics.Primitives
{
    /// <summary>Stores a set of four floating-point numbers that represent the location and size of a cube.</summary>
    /// <filterpriority>1</filterpriority>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct CubeF : IEquatable<CubeF>
    {
        /// <summary>Represents an instance of the <see cref="CubeF"/> class with its members uninitialized.</summary>
        /// <filterpriority>1</filterpriority>
        public static CubeF Empty { get; } = new CubeF();

        public float X;
        public float Y;
        public float Z;

        public float Width;
        public float Height;
        public float Depth;

        /// <summary>Initializes a new instance of the <see cref="CubeF"/> class with the specified location and size.</summary>
        /// <param name="x">The x-coordinate of the upper-left-front edge of the cube. </param>
        /// <param name="y">The y-coordinate of the upper-left-front edge of the cube. </param>
        /// <param name="z">The z-coordinate of the upper-left-front edge of the cube. </param>
        /// <param name="width">The width of the cube. </param>
        /// <param name="height">The height of the cube. </param>
        /// <param name="depth">The depth of the cube. </param>
        public CubeF(float x, float y, float z, float width, float height, float depth)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
            Depth = depth;
        }

        /// <summary>Initializes a new instance of the <see cref="CubeF"/> class with the specified location and size.</summary>
        /// <param name="location">A <see cref="Vector2"/> that represents the upper-left-front edge of the cube region. </param>
        /// <param name="size">A <see cref="Vector3"/> that represents the width and height and depth of the cube region. </param>
        public CubeF(Vector3 location, Vector3 size)
        {
            X = location.X;
            Y = location.Y;
            Z = location.Z;
            Width = size.X;
            Height = size.Y;
            Depth = size.Z;
        }

        /// <summary>Gets or sets the coordinates of the upper-left-front corner of this <see cref="CubeF"/> structure.</summary>
        /// <returns>A <see cref="Vector3"/> that represents the upper-left-front edge of this <see cref="CubeF"/> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public Vector3 Location
        {
            get => new Vector3(X, Y, Z);
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        /// <summary>Gets or sets the size of this <see cref="CubeF"/>.</summary>
        /// <returns>A <see cref="Vector3"/> that represents the width and height of this <see cref="CubeF"/> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public Vector3 Size
        {
            get => new Vector3(Width, Height, Depth);
            set
            {
                Width = value.X;
                Height = value.Y;
                Depth = value.Z;
            }
        }

        /// <summary>Gets the center of the cube.</summary>
        [Browsable(false)]
        public Vector3 Centre => new Vector3(X + Width / 2, Y + Height / 2, Z + Depth / 2);

        /// <summary>Tests whether the <see cref="Width"/>, <see cref="Height"/> or <see cref="Depth"> property of this <see cref="CubeF"/> has a value of zero.</summary>
        /// <returns>This property returns true if the <see cref="Width"/>, <see cref="Height"/> or <see cref="Depth"> property of this <see cref="CubeF"/> has a value of zero; otherwise, false.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public bool IsEmpty => Width <= 0 || Height <= 0 || Depth <= 0;

        /// <summary>Tests whether obj is a <see cref="CubeF"/> with the same location and size of this <see cref="CubeF"/>.</summary>
        /// <returns>This method returns true if obj is a <see cref="CubeF"/> and its X, Y, Z, Width, Height and Depth properties are equal to the corresponding properties of this <see cref="CubeF"/>; otherwise, false.</returns>
        /// <param name="obj">The <see cref="object"/> to test.</param>
        /// <filterpriority>1</filterpriority>
        public override bool Equals(object obj) => obj is CubeF rec && Equals(rec);

        /// <summary>Tests whether two <see cref="CubeF"/> structures have equal location and size.</summary>
        /// <returns>This operator returns true if the two specified <see cref="CubeF"/> structures have equal <see cref="X"/>, <see cref="Y"/>, <see cref="Z"/>, <see cref="Width"/>, <see cref="Height"/>, and <see cref="Depth"/> properties.</returns>
        /// <param name="right">The <see cref="CubeF"/> structure that is to the right of the equality operator. </param>
        /// <param name="left">The <see cref="CubeF"/> structure that is to the left of the equality operator. </param>
        /// <filterpriority>3</filterpriority>
        public static bool operator ==(CubeF left, CubeF right) => left.Equals(right);

        /// <summary>Tests whether two <see cref="CubeF"/> structures differ in location or size.</summary>
        /// <returns>This operator returns true if any of the <see cref="X"/>, <see cref="Y"/>, <see cref="Z"/>, <see cref="Width"/>,
        /// <see cref="Height"/>, and <see cref="Depth"/> properties of the two <see cref="CubeF"/>structures are unequal; otherwise false.</returns>
        /// <param name="right">The <see cref="CubeF"/> structure that is to the right of the inequality operator.</param>
        /// <param name="left">The <see cref="CubeF"/> structure that is to the left of the inequality operator.</param>
        /// <filterpriority>3</filterpriority>
        public static bool operator !=(CubeF left, CubeF right) => !(left == right);

        public static CubeF operator *(CubeF cube, float scale) => new CubeF(cube.X * scale, cube.Y * scale, cube.Z * scale, cube.Width * scale, cube.Height * scale, cube.Depth * scale);

        public static CubeF operator /(CubeF cube, float scale) => new CubeF(cube.X / scale, cube.Y / scale, cube.Z / scale, cube.Width / scale, cube.Height / scale, cube.Depth / scale);

        public static CubeF operator *(CubeF cube, Vector3 scale) => new CubeF(cube.X * scale.X, cube.Y * scale.Y, cube.Z * scale.Z, cube.Width * scale.X, cube.Height * scale.Y, cube.Depth * scale.Z);

        public static CubeF operator /(CubeF cube, Vector3 scale) => new CubeF(cube.X / scale.X, cube.Y / scale.Y, cube.Z / scale.Z, cube.Width / scale.X, cube.Height / scale.Y, cube.Depth / scale.Z);

        /// <summary>Determines if the specified point is contained within this <see cref="CubeF"/> structure.</summary>
        /// <returns>This method returns true if the point defined by x and y is contained within this <see cref="CubeF"/> structure; otherwise false.</returns>
        /// <param name="y">The y-coordinate of the point to test.</param>
        /// <param name="x">The x-coordinate of the point to test.</param>
        /// <param name="z">The z-coordinate of the point to test.</param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(float x, float y, float z) => X <= x && x < X + Width && Y <= y && y < Y + Height && Z <= z && Z < Z + Depth;

        /// <summary>Determines if the specified point is contained within this <see cref="CubeF"/> structure.</summary>
        /// <returns>This method returns true if the point defined by x, y and z is contained within this <see cref="CubeF"/> structure; otherwise false.</returns>
        /// <param name="pt">The point to test against this <see cref="CubeF"/>.</param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(Vector3 pt) => Contains(pt.X, pt.Y, pt.Z);

        /// <summary>Determines if the cube region represented by cube is entirely contained within this <see cref="CubeF"/> structure.</summary>
        /// <returns>This method returns true if the cube region represented by cube is entirely contained within the cube region represented by this <see cref="CubeF"/>; otherwise false.</returns>
        /// <param name="cube">The <see cref="CubeF"/> to test.</param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(CubeF cube) =>
            X <= cube.X && cube.X + cube.Width <= X + Width && Y <= cube.Y &&
            cube.Y + cube.Height <= Y + Height && Z <= cube.Z &&
            cube.Z + cube.Depth <= Z + Depth;

        /// <summary>Gets the Volume of this <see cref="CubeF"/>.</summary>
        public float Volume => Width * Height * Depth;

        /// <summary>
        /// Gets this <see cref="CubeF"/> with positive width, height and depth.
        /// This is useful if you have a <see cref="CubeF"/> with negative <see cref="Width"/>, <see cref="Height"/> or <see cref="Depth">.
        /// </summary>
        /// <example>
        /// var cube = new <see cref="CubeF"/> { <see cref="Width"/> = -200, <see cref="Height"/> = -300, <see cref="Depth"/> = -400 };
        ///
        /// cube.<see cref="WithPositiveExtent"/> will result in
        /// Width = 200
        /// Height = 300
        /// Depth = 400
        /// X = -200
        /// Y = -300
        /// Z = -400
        /// </example>
        public CubeF WithPositiveExtent
        {
            get
            {
                CubeF result = this;

                if (result.Width < 0)
                {
                    result.Width = -result.Width;
                    result.X -= result.Width;
                }

                if (Height < 0)
                {
                    result.Height = -result.Height;
                    result.Y -= result.Height;
                }

                if (Depth < 0)
                {
                    result.Depth = -result.Depth;
                    result.Z -= result.Depth;
                }

                return result;
            }
        }

        /// <summary>Replaces this <see cref="CubeF"/> structure with the intersection of itself and the specified <see cref="CubeF"/> structure.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="cube">The cube to intersect.</param>
        /// <filterpriority>1</filterpriority>
        public void Intersect(CubeF cube)
        {
            CubeF ef = Intersect(cube, this);
            X = ef.X;
            Y = ef.Y;
            Z = ef.Z;
            Width = ef.Width;
            Height = ef.Height;
            Depth = ef.Depth;
        }

        /// <summary>Returns a <see cref="CubeF"/> structure that represents the intersection of two cubes. If there is no intersection, and empty <see cref="CubeF"/> is returned.</summary>
        /// <returns>A third <see cref="CubeF"/> structure the size of which represents the overlapped volume of the two specified cubes.</returns>
        /// <param name="a">A cube to intersect.</param>
        /// <param name="b">A cube to intersect.</param>
        /// <filterpriority>1</filterpriority>
        public static CubeF Intersect(CubeF a, CubeF b)
        {
            float x = Math.Max(a.X, b.X);
            float num2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y = Math.Max(a.Y, b.Y);
            float num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            float z = Math.Max(a.Z, b.Z);
            float num6 = Math.Min(a.Z + a.Depth, b.Z + b.Depth);
            if (num2 >= x && num4 >= y && num6 >= z)
                return new CubeF(x, y, z, num2 - x, num4 - y, num6 - z);

            return Empty;
        }

        /// <summary>Determines if this cube intersects with cube.</summary>
        /// <returns>This method returns true if there is any intersection.</returns>
        /// <param name="cube">The cube to test.</param>
        /// <filterpriority>1</filterpriority>
        public bool IntersectsWith(CubeF cube) =>
            cube.X <= X + Width && X <= cube.X + cube.Width && cube.Y <= Y + Height && Y <= cube.Y + cube.Height && cube.Z <= Z + Depth && Z <= cube.Z + cube.Depth;

        /// <summary>Creates the smallest possible third cube that can contain both of two cubes that form a union.</summary>
        /// <returns>A third <see cref="CubeF"/> structure that contains both of the two cubes that form the union.</returns>
        /// <param name="a">The first cube to union.</param>
        /// <param name="b">The second cube to union.</param>
        /// <filterpriority>1</filterpriority>
        public static CubeF Union(CubeF a, CubeF b)
        {
            float x = Math.Min(a.X, b.X);
            float num2 = Math.Max(a.X + a.Width, b.X + b.Width);
            float y = Math.Min(a.Y, b.Y);
            float num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            float z = Math.Min(a.Z, b.Z);
            float num6 = Math.Max(a.Z + a.Depth, b.Z + b.Depth);
            return new CubeF(x, y, z, num2 - x, num4 - y, num6 - z);
        }

        /// <summary>Adjusts the location of this cube by the specified amount.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="pos">The amount to offset the location.</param>
        /// <filterpriority>1</filterpriority>
        public CubeF Offset(Vector3 pos) => Offset(pos.X, pos.Y, pos.Z);

        /// <summary>Adjusts the location of this cube by the specified amount.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="y">The amount to offset the location vertically.</param>
        /// <param name="x">The amount to offset the location horizontally.</param>
        /// <filterpriority>1</filterpriority>
        public CubeF Offset(float x, float y, float z) => new CubeF(X + x, Y + y, Z + z, Width, Height, Depth);

        /// <summary>Converts the Location and <see cref="Size"/> of this <see cref="CubeF"/> to a human-readable string.</summary>
        /// <returns>A string that contains the position, width, and height of this <see cref="CubeF"/> structure¾for example, "{X=20, Y=20, Width=100, Height=50}".</returns>
        /// <filterpriority>1</filterpriority>
        public override string ToString() => $"X={Math.Round(X, 3).ToString(CultureInfo.CurrentCulture)}, "
                                             + $"Y={Math.Round(Y, 3).ToString(CultureInfo.CurrentCulture)}, "
                                             + $"Z={Math.Round(Z, 3).ToString(CultureInfo.CurrentCulture)}, "
                                             + $"Width={Math.Round(Width, 3).ToString(CultureInfo.CurrentCulture)}, "
                                             + $"Height={Math.Round(Height, 3).ToString(CultureInfo.CurrentCulture)}, "
                                             + $"Depth={Math.Round(Depth, 3).ToString(CultureInfo.CurrentCulture)}";

        public bool Equals(CubeF other) => X == other.X && Y == other.Y && Z == other.Z && Width == other.Width && Height == other.Height && Depth == other.Depth;
    }
}
