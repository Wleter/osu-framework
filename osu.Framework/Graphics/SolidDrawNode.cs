// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Rendering;

namespace osu.Framework.Graphics
{
    public class SolidDrawNode : DrawNode
    {
        protected new Solid Source => (Solid)base.Source;

        public SolidDrawNode(Solid source)
            : base(source)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();
        }

        public override void Draw(IRenderer renderer)
        {
            base.Draw(renderer);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
        }
    }
}
