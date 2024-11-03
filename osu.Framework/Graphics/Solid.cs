// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osuTK;

namespace osu.Framework.Graphics
{
    public abstract partial class Solid : Drawable
    {
        private Vector3 position;
        public new Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                base.Position = value.Xy;
                Depth = value.Z;
            }
        }

        private Vector3 size;
        public new Vector3 Size
        {
            get => size;
            set
            {
                size = value;
                base.Size = value.Xy;
                Depth = value.Z;
            }
        }

        private Camera parent;

        /// <summary>
        /// The parent of this solid in the scene graph.
        /// </summary>
        public new Camera Parent
        {
            get => parent;
            internal set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(ToString(), "Disposed Drawables may never get a parent and return to the scene graph.");

                if (value == null)
                    ChildID = 0;

                if (parent == value) return;

                if (value != null && parent != null)
                    throw new InvalidOperationException("May not add a drawable to multiple containers.");

                parent = value;
                Invalidate(InvalidationFromParentSize | Invalidation.Colour | Invalidation.Presence | Invalidation.Parent);

                if (parent != null)
                {
                    //we should already have a clock at this point (from our LoadRequested invocation)
                    //this just ensures we have the most recent parent clock.
                    //we may want to consider enforcing that parent.Clock == clock here.
                    UpdateClock(parent.Clock);
                }
            }
        }
        private Vector3 originPosition;
        public new Vector3 OriginPosition
        {
            get => originPosition;

            set
            {
                originPosition = value;
                Invalidate(Invalidation.MiscGeometry);
            }
        }

        private Quaternion rotation;

        /// <summary>
        /// Rotation represented as quaternion around <see cref="OriginPosition"/>.
        /// </summary>
        public new Quaternion Rotation
        {
            get => rotation;
            set
            {
                if (value == rotation) return;

                rotation = value;

                Invalidate(Invalidation.MiscGeometry);
            }
        }

        private Vector3 scale = Vector3.One;
        public new Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                Invalidate(Invalidation.MiscGeometry);
            }
        }

        public new SpaceDrawInfo DrawInfo => computeDrawInfo();
        private SpaceDrawInfo computeDrawInfo()
        {
            SpaceDrawInfo di = new SpaceDrawInfo(null);
            di.ApplyTransform(Position, Scale, Rotation, OriginPosition);

            return di;
        }

        protected override DrawNode CreateDrawNode() => new SolidDrawNode(this);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
        }
    }
}
