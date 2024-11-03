// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Extensions.MatrixExtensions;
using osuTK;
using osu.Framework.Extensions.TypeExtensions;
using osu.Framework.Utils;

namespace osu.Framework.Graphics
{
    public struct SpaceDrawInfo : IEquatable<SpaceDrawInfo>
    {
        public Matrix4 Matrix;
        public Matrix4 MatrixInverse;

        public SpaceDrawInfo(Matrix4? matrix = null, Matrix4? matrixInverse = null)
        {
            Matrix = matrix ?? Matrix4.Identity;
            MatrixInverse = matrixInverse ?? Matrix4.Identity;
        }

        /// <summary>
        /// Applies a transformation to the current DrawInfo.
        /// </summary>
        /// <param name="translation">The amount by which to translate the current position.</param>
        /// <param name="scale">The amount by which to scale.</param>
        /// <param name="rotation">The amount by which to rotate.</param>
        /// <param name="origin">The center of rotation and scale.</param>
        public void ApplyTransform(Vector3 translation, Vector3 scale, Quaternion rotation, Vector3 origin)
        {
            if (translation != Vector3.Zero)
            {
                Matrix *= Matrix4.CreateTranslation(translation);
                MatrixInverse = Matrix4.CreateTranslation(-translation) * MatrixInverse;
            }

            if (rotation != Quaternion.Identity)
            {
                Matrix *= Matrix4.CreateFromQuaternion(rotation);
                MatrixInverse = Matrix4.CreateFromQuaternion(Quaternion.Invert(rotation)) * MatrixInverse;
            }

            if (scale != Vector3.One)
            {
                // Zero scale leads to unexpected input and autosize calculations, so it's clamped to a sane value.
                if (scale.X == 0) scale.X = Precision.FLOAT_EPSILON;
                if (scale.Y == 0) scale.Y = Precision.FLOAT_EPSILON;
                if (scale.Z == 0) scale.Z = Precision.FLOAT_EPSILON;

                Matrix *= Matrix4.CreateScale(scale);
                MatrixInverse = Matrix4.CreateScale(Vector3.Divide(Vector3.One, scale)) * MatrixInverse;
            }

            if (origin != Vector3.Zero)
            {
                Matrix *= Matrix4.CreateTranslation(-origin);
                MatrixInverse = Matrix4.CreateTranslation(origin) * MatrixInverse;
            }

            //========================================================================================
            //== Uncomment the following 2 lines to use a ground-truth matrix inverse for debugging ==
            //========================================================================================
            //target.MatrixInverse = target.Matrix;
            //MatrixExtensions.FastInvert(ref target.MatrixInverse);
        }

        public readonly bool Equals(SpaceDrawInfo other) => Matrix.Equals(other.Matrix);

        public override string ToString() => $@"{GetType().ReadableName().Replace(@"DrawInfo", string.Empty)} DrawInfo";
    }
}
