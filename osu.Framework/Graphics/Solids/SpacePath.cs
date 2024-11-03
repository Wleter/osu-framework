// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Graphics
{
    public partial class SpacePath : Solid
    {
        private IShader? pathShader { get; set; }

        [Resolved]
        private IRenderer? renderer { get; set; }

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaders)
        {
            pathShader = shaders.Load(VertexShaderDescriptor.TEXTURE_3, FragmentShaderDescriptor.TEXTURE);
        }

        private readonly List<Vector3> vertices = new List<Vector3>();

        public IReadOnlyList<Vector3> Vertices
        {
            get => vertices;
            set
            {
                vertices.Clear();
                vertices.AddRange(value);

                Invalidate(Invalidation.DrawSize);
            }
        }

        private float pathRadius = 10f;

        /// <summary>
        /// How wide this path is on each side of the line.
        /// </summary>
        /// <remarks>
        /// The actual width of the path is twice the PathRadius.
        /// </remarks>
        public virtual float PathRadius
        {
            get => pathRadius;
            set
            {
                if (pathRadius == value) return;

                pathRadius = value;

                Invalidate(Invalidation.DrawSize);
            }
        }

        public void AddVertex(Vector3 pos)
        {
            vertices.Add(pos);

            Invalidate(Invalidation.DrawSize);
        }

        private readonly List<(Vector3, Vector3)> segmentsBacking = new List<(Vector3, Vector3)>();
        private List<(Vector3, Vector3)> generateSegments()
        {
            segmentsBacking.Clear();

            if (vertices.Count > 1)
            {
                for (int i = 0; i < vertices.Count - 1; ++i)
                    segmentsBacking.Add((vertices[i], vertices[i + 1]));
            }

            return segmentsBacking;
        }

        private List<(Vector3, Vector3)> segments => generateSegments();

        private Texture? texture;

        protected Texture Texture
        {
            get => texture ??  renderer?.WhitePixel;
            set
            {
                if (texture == value)
                    return;

                texture?.Dispose();
                texture = value;

                Invalidate(Invalidation.DrawNode);
            }
        }
        public override DrawColourInfo DrawColourInfo => new DrawColourInfo(Color4.White, base.DrawColourInfo.Blending);
        protected override DrawNode CreateDrawNode() => new SpacePathDrawNode(this);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            texture?.Dispose();
            texture = null;
        }
    }
}
