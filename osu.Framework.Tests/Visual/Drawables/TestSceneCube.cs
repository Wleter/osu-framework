// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osuTK.Graphics;
using osuTK;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using System;

namespace osu.Framework.Tests.Visual.Drawables
{
    public partial class TestSceneCube : TestScene
    {
        private readonly Cube cube = new Cube
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.TopLeft,
            Size = new Vector3(0.5f, 0.5f, 0.5f),
            Colour = Color4.Red,
            Position = new Vector3(50, 50, 100)
        };

        private readonly Path path = new Path { PathRadius = 12 };
        private readonly SpriteText innerText = createLabel();
        private readonly SpriteText outerText = createLabel();

        private readonly Vector2 center = new Vector2(500f, 350f);

        public TestSceneCube()
        {
            AddRange(new Drawable[]
            {
                new FillFlowContainer
                {
                    Direction = FillDirection.Vertical,
                    Children = new[]
                    {
                        innerText,
                        outerText,
                    },
                },
                new Camera
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        cube,
                        path
                    },
                },
            });
        }

        private int timeStep = 0;

        protected override void Update()
        {
            base.Update();
            float angle = timeStep * 0.001f;
            cube.Rotation = Quaternion.FromEulerAngles(0f, angle, angle);

            innerText.Text = "second euler angle: " + MathHelper.RadiansToDegrees(angle).ToString("000.000");
            outerText.Text = "third euler angle: " + MathHelper.RadiansToDegrees(angle).ToString("000.000");

            Vector2 inner = center + 40f * new Vector2(MathF.Cos(angle), MathF.Sin(angle));

            path.Vertices = new[] { center, inner };

            timeStep = (timeStep + 1) % 3000;
        }

        private static SpriteText createLabel() => new SpriteText
        {
            Font = new FontUsage(size: 20),
            Colour = Color4.White,
        };
    }
}
