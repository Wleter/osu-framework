// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace osu.Framework.Graphics
{
    public partial class Camera : Container<Drawable>
    {
        protected override DrawNode CreateDrawNode() => new CameraDrawNode(this, new CompositeDrawableDrawNode(this));
        public Color4 BackgroundColour { get; private set; } = Color4.Black;
        public DrawColourInfo? FrameBufferDrawColour { get; private set; }
        public float Fov { get; private set; } = 1f;

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
        }
    }
}
