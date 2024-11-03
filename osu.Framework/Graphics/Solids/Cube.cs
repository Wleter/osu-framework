// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics;

namespace osu.Framework.Graphics
{
    public partial class Cube : Solid
    {
        private IShader? cubeShader { get; set; }

        [Resolved]
        private IRenderer? renderer { get; set; }

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaders)
        {
            cubeShader = shaders.Load(VertexShaderDescriptor.TEXTURE_3, FragmentShaderDescriptor.TEXTURE);
        }

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
        protected override DrawNode CreateDrawNode() => new CubeDrawNode(this);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            texture?.Dispose();
            texture = null;
        }
    }
}
