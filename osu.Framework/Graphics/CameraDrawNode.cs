// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Graphics
{
    public partial class Camera : Container<Drawable>
    {
        private class CameraDrawNode : DrawNode, ICompositeDrawNode
        {
            protected new Camera Source => (Camera)base.Source;

            /// <summary>
            /// Contains the colour and blending information of this <see cref="DrawNode"/>.
            /// </summary>
            protected new DrawColourInfo DrawColourInfo { get; private set; }
            private Color4 backgroundColour;
            private Matrix4 projectionMatrix;
            private IFrameBuffer mainBuffer;
            private RectangleF screenSpaceDrawRectangle;
            private Vector2 frameBufferSize;
            protected CompositeDrawableDrawNode Child { get; private set; }
            public List<DrawNode> Children { get => Child.Children; set => Child.Children = value; }

            public bool AddChildDrawNodes => true;

            public CameraDrawNode(Drawable source, CompositeDrawableDrawNode child) : base(source)
            {
                Child = child;
            }

            public override void ApplyState()
            {
                base.ApplyState();

                backgroundColour = Source.BackgroundColour;
                float fov = Source.Fov;
                screenSpaceDrawRectangle = Source.ScreenSpaceDrawQuad.AABBFloat;
                frameBufferSize = new Vector2(screenSpaceDrawRectangle.Width, screenSpaceDrawRectangle.Height);
                DrawColourInfo = Source.FrameBufferDrawColour ?? new DrawColourInfo(Color4.White, base.DrawColourInfo.Blending);

                projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fov, screenSpaceDrawRectangle.Width / screenSpaceDrawRectangle.Height, 0.1f, 1000f);

                Child.ApplyState();
            }

            public sealed override void Draw(IRenderer renderer)
            {
                base.Draw(renderer);

                renderer.PushProjectionMatrix(projectionMatrix);
                renderer.Clear(new ClearInfo(backgroundColour));

                Child.Draw(renderer);

                renderer.PopProjectionMatrix();
                // using (establishFrameBufferViewport(renderer))
                // {
                //     mainBuffer ??= renderer.CreateFrameBuffer();
                //     // Fill the frame buffer with drawn children
                //     using (BindFrameBuffer(mainBuffer))
                //     {
                //         renderer.PushProjectionMatrix(projectionMatrix);
                //         renderer.Clear(new ClearInfo(backgroundColour));

                //         Child.Draw(renderer);

                //         renderer.PopProjectionMatrix();
                //     }
                // }
            }

            /// <summary>
            /// Binds and initialises an <see cref="IFrameBuffer"/> if required.
            /// </summary>
            /// <param name="frameBuffer">The <see cref="IFrameBuffer"/> to bind.</param>
            /// <returns>A token that must be disposed upon finishing use of <paramref name="frameBuffer"/>.</returns>
            protected IDisposable BindFrameBuffer(IFrameBuffer frameBuffer)
            {
                // This setter will also take care of allocating a texture of appropriate size within the frame buffer.
                frameBuffer.Size = frameBufferSize;

                frameBuffer.Bind();

                return new ValueInvokeOnDisposal<IFrameBuffer>(frameBuffer, b => b.Unbind());
            }

            private IDisposable establishFrameBufferViewport(IRenderer renderer)
            {
                // Disable masking for generating the frame buffer since masking will be re-applied
                // when actually drawing later on anyways. This allows more information to be captured
                // in the frame buffer and helps with cached buffers being re-used.
                RectangleI screenSpaceMaskingRect = new RectangleI((int)Math.Floor(screenSpaceDrawRectangle.X), (int)Math.Floor(screenSpaceDrawRectangle.Y), (int)frameBufferSize.X,
                    (int)frameBufferSize.Y);

                renderer.PushMaskingInfo(new MaskingInfo
                {
                    ScreenSpaceAABB = screenSpaceMaskingRect,
                    MaskingRect = screenSpaceDrawRectangle,
                    ToMaskingSpace = Matrix3.Identity,
                    BlendRange = 1,
                    AlphaExponent = 1,
                }, true);

                // Match viewport to FrameBuffer such that we don't draw unnecessary pixels.
                renderer.PushViewport(new RectangleI(0, 0, (int)frameBufferSize.X, (int)frameBufferSize.Y));
                renderer.PushScissor(new RectangleI(0, 0, (int)frameBufferSize.X, (int)frameBufferSize.Y));
                renderer.PushScissorOffset(screenSpaceMaskingRect.Location);

                return new ValueInvokeOnDisposal<(CameraDrawNode node, IRenderer renderer)>((this, renderer), tup => tup.node.returnViewport(tup.renderer));
            }

            private void returnViewport(IRenderer renderer)
            {
                renderer.PopScissorOffset();
                renderer.PopViewport();
                renderer.PopScissor();
                renderer.PopMaskingInfo();
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
            }
        }
    }
}
