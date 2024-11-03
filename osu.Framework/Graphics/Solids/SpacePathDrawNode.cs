// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Vertices;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace osu.Framework.Graphics
{
    public partial class SpacePath
    {
        private class SpacePathDrawNode : SolidDrawNode
        {
            private const int max_res = 24;

            protected new SpacePath Source => (SpacePath)base.Source;

            private Texture? texture;
            protected new SpaceDrawInfo DrawInfo;
            private IShader? shader;

            private Vector2 drawSize;
            private float radius;
            private readonly List<(Vector3, Vector3)> segments = new List<(Vector3, Vector3)>();

            private IVertexBatch<TexturedVertex3D>? triangleBatch;
            public SpacePathDrawNode(SpacePath source)
                : base(source)
            {
            }


            public override void ApplyState()
            {
                base.ApplyState();

                segments.Clear();
                segments.AddRange(Source.segments);

                texture = Source.Texture;
                DrawInfo = Source.DrawInfo;
                radius = Source.PathRadius;
                shader = Source.pathShader;
            }

            public override void Draw(IRenderer renderer)
            {
                base.Draw(renderer);

                if (texture?.Available != true || shader == null)
                    return;

                triangleBatch ??= renderer.CreateLinearBatch<TexturedVertex3D>(max_res * 200 * 3, 10, PrimitiveTopology.Triangles);

                renderer.PushLocalMatrix(DrawInfo.Matrix);
                renderer.PushDepthInfo(DepthInfo.Default);
                renderer.SetBlend(BlendingParameters.None);

                shader.Bind();

                texture.Bind();

                updateVertexBuffer();

                shader.Unbind();

                renderer.PopDepthInfo();
                renderer.PopLocalMatrix();
            }

            private void updateVertexBuffer()
            {

            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
            }
        }
    }
}
