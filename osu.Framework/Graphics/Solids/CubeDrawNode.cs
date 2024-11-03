// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Vertices;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace osu.Framework.Graphics
{
    public partial class Cube
    {
        private class CubeDrawNode : SolidDrawNode
        {
            public CubeDrawNode(Cube source)
                : base(source)
            {
            }
            protected new Cube Source => (Cube)base.Source;
            private Texture? texture;
            protected new SpaceDrawInfo DrawInfo;
            private IVertexBatch<TexturedVertex3D>? batch;
            private IShader? shader;

            public override void ApplyState()
            {
                base.ApplyState();

                texture = Source.Texture;
                DrawInfo = Source.DrawInfo;
                shader = Source.cubeShader;
            }

            public override void Draw(IRenderer renderer)
            {
                base.Draw(renderer);

                if (texture?.Available != true || shader == null)
                    return;

                batch ??= renderer.CreateQuadBatch<TexturedVertex3D>(6 * 4, 6 * 4);

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
                Debug.Assert(texture != null);

                RectangleF texRect = texture.GetTextureRect(new RectangleF(0.5f, 0.5f, texture.Width - 1, texture.Height - 1));
                addWall(new Vector3(0, 0, 0), new Vector3(0, -10f, 0), new Vector3(0, 0, -10f), new Vector3(-10f, 0, 0), texRect);
                addWall(new Vector3(0, 0, 0), new Vector3(0, 10f, 0), new Vector3(0, 0, -10f), new Vector3(10f, 0, 0), texRect);
                addWall(new Vector3(0, 0, 0), new Vector3(0, 0, -10f), new Vector3(-10f, 0, 0), new Vector3(0, -10f, 0), texRect);
                addWall(new Vector3(0, 0, 0), new Vector3(0, 0, 10f), new Vector3(10f, 0, 0), new Vector3(0, -10f, 0), texRect);
                addWall(new Vector3(0, 0, 0), new Vector3(-10f, 0, 0), new Vector3(0, 0, -10f), new Vector3(0, 10f, 0), texRect);
                addWall(new Vector3(0, 0, 0), new Vector3(10f, 0, 0), new Vector3(0, 0, -10f), new Vector3(0, -10f, 0), texRect);
            }

            private void addWall(Vector3 centre, Vector3 toWall, Vector3 down, Vector3 left, RectangleF texRect)
            {
                Debug.Assert(batch != null);
                centre += toWall;

                batch.Add(new TexturedVertex3D
                {
                    Position = centre + (down + left),
                    Colour = Colour4.White,
                    TexturePosition = new Vector2(texRect.Left, texRect.Bottom),
                });
                batch.Add(new TexturedVertex3D
                {
                    Position = centre + down - left,
                    Colour = Colour4.White,
                    TexturePosition = new Vector2(texRect.Right, texRect.Bottom),
                });
                batch.Add(new TexturedVertex3D
                {
                    Position = centre - down - left,
                    Colour = Colour4.White,
                    TexturePosition = new Vector2(texRect.Right, texRect.Top),
                });
                batch.Add(new TexturedVertex3D
                {
                    Position = centre - down + left,
                    Colour = Colour4.White,
                    TexturePosition = new Vector2(texRect.Left, texRect.Top),
                });
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
            }
        }
    }
}
