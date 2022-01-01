// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace osu.Framework.Graphics.Visualisation
{
    internal abstract class ToolWindow : OverlayContainer
    {
        public const float WIDTH = 500;
        public const float HEIGHT = 600;

        private const float button_width = 140;
        private const float button_height = 40;

        protected readonly FillFlowContainer ToolbarContent;

        protected readonly ScrollContainer<Drawable> ScrollContent;

        protected readonly FillFlowContainer MainHorizontalContent;

        private readonly FillFlowContainer header;
        private readonly Container mainContent;
        private readonly FillFlowContainer content;

        protected override Container<Drawable> Content => content;

        private Layout.LayoutValue layout = new Layout.LayoutValue(Invalidation.DrawSize, Layout.InvalidationSource.Child);

        protected ToolWindow(string title, string keyHelpText)
        {
            AutoSizeAxes = Axes.X;
            Height = HEIGHT;

            Masking = true; // for cursor masking

            AddRangeInternal(new Drawable[]
            {
                new Box
                {
                    Colour = FrameworkColour.GreenDark,
                    RelativeSizeAxes = Axes.Both,
                    Depth = 0
                },
                content = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical
                },
                new CursorContainer()
            });

            AddRange(new Drawable[]
            {
                header = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        new TitleBar(title, keyHelpText, this),
                        new Container //toolbar
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    Colour = FrameworkColour.BlueGreenDark,
                                    RelativeSizeAxes = Axes.Both,
                                },
                                ToolbarContent = new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Spacing = new Vector2(5),
                                    Padding = new MarginPadding(5),
                                },
                            },
                        }
                    }
                },
                mainContent = new TooltipContainer
                {
                    AutoSizeAxes = Axes.X,
                    Child = MainHorizontalContent = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Direction = FillDirection.Horizontal,
                        Children = new Drawable[]
                        {
                            ScrollContent = new BasicScrollContainer<Drawable>
                            {
                                RelativeSizeAxes = Axes.Y,
                                Width = WIDTH
                            }
                        }
                    }
                },
            });

            AddLayout(layout);
        }

        protected void AddButton(string text, Action action)
        {
            ToolbarContent.Add(new BasicButton
            {
                Size = new Vector2(button_width, button_height),
                Text = text,
                Action = action
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
        }

        protected override void PopIn() => this.FadeIn(100);

        protected override void PopOut() => this.FadeOut(100);

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            if (!layout.IsValid)
            {
                mainContent.Height = Height - header.Height;
                layout.Validate();
            }
        }
    }
}
