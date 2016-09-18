﻿using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Drawables;
using osu.Framework.Graphics.OpenGL.Textures;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Timing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using RectangleF = osu.Framework.Graphics.Primitives.RectangleF;
using System.Collections.Concurrent;
using osu.Framework.Input;
using OpenTK.Input;
using osu.Framework.Graphics.Transformations;
using osu.Framework.Statistics;

namespace osu.Framework.Graphics.Performance
{
    class FrameTimeDisplay : Container
    {
        static Vector2 padding = new Vector2(0, 0);

        const int WIDTH = 800;
        const int HEIGHT = 100;

        const float visible_range = 20;
        const float scale = HEIGHT / visible_range;

        private Sprite[] timeBars = new Sprite[2];

        private AutoSizeContainer timeBarContainer;

        private byte[] textureData = new byte[HEIGHT * 4];

        private static Color4[] garbageCollectColors = new Color4[] { Color4.Green, Color4.Yellow, Color4.Red };
        private PerformanceMonitor monitor;

        private int currentX = 0;

        private int TimeBarIndex => currentX / WIDTH;
        private int TimeBarX => currentX % WIDTH;

        private bool processFrames = true;

        FlowContainer legendSprites;
        List<Drawable> eventSprites = new List<Drawable>();

        public FrameTimeDisplay(string name, PerformanceMonitor monitor)
        {
            Size = new Vector2(WIDTH, HEIGHT);
            this.monitor = monitor;
        }

        public override void Load()
        {
            base.Load();

            Add(new MaskingContainer
            {
                Children = new Drawable[] {
                    timeBarContainer = new AutoSizeContainer(),
                    legendSprites = new FlowContainer {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Padding = new Vector2(5, 1),
                        Children = new [] {
                            new Box {
                                SizeMode = InheritMode.XY,
                                Colour = Color4.Gray,
                                Alpha = 0.2f,
                            }
                        }
                    },
                    new SpriteText
                    {
                        Text = $@"{visible_range}ms",
                    },
                    new SpriteText
                    {
                        Text = @"0ms",
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft
                    },
                }
            });

            for (int i = 0; i < timeBars.Length; ++i)
            {
                timeBars[i] = new Sprite(new Texture(WIDTH, HEIGHT));
                timeBarContainer.Add(timeBars[i]);
            }

            foreach (FrameTimeType t in Enum.GetValues(typeof(FrameTimeType)))
            {
                if (t >= FrameTimeType.Empty) continue;

                legendSprites.Add(new SpriteText()
                {
                    Colour = getColour(t),
                    Text = t.ToString(),
                });

                legendSprites.FadeOut(2000, EasingTypes.InExpo);
            }

            // Initialize background
            for (int i = 0; i < WIDTH * timeBars.Length; ++i)
            {
                currentX = i;
                Sprite timeBar = timeBars[TimeBarIndex];

                addArea(null, FrameTimeType.Empty, HEIGHT);
                timeBar.Texture.SetData(new TextureUpload(textureData)
                {
                    Bounds = new Rectangle(TimeBarX, 0, 1, HEIGHT)
                });
            }
        }

        public void AddEvent(int type)
        {
            Box b = new Box()
            {
                Position =  new Vector2(currentX, 0),
                Colour = garbageCollectColors[type],
                Size = new Vector2(3, 3),
            };

            eventSprites.Add(b);
            timeBarContainer.Add(b);
        }

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            if (args.Key == Key.ControlLeft)
            {
                legendSprites.FadeIn(100);
                processFrames = false;
            }
            return base.OnKeyDown(state, args);
        }

        protected override bool OnKeyUp(InputState state, KeyUpEventArgs args)
        {
            if (args.Key == Key.ControlLeft)
            {
                legendSprites.FadeOut(100);
                processFrames = true;
            }
            return base.OnKeyUp(state, args);
        }
        protected override void Update()
        {
            base.Update();

            FrameStatistics frame;
            while (processFrames && monitor.PendingFrames.TryDequeue(out frame))
            {
                foreach (int gcLevel in frame.GarbageCollections)
                    AddEvent(gcLevel);

                Sprite timeBar = timeBars[TimeBarIndex];

                int currentHeight = HEIGHT;

                for (int i = 0; i <= (int)FrameTimeType.Empty; i++)
                    currentHeight = addArea(frame, (FrameTimeType)i, currentHeight);

                timeBar.Texture.SetData(new TextureUpload(textureData)
                {
                    Bounds = new Rectangle(TimeBarX, 0, 1, HEIGHT)
                });

                if (processFrames)
                {
                    timeBars[TimeBarIndex].MoveToX((WIDTH - TimeBarX), 0);
                    timeBars[(TimeBarIndex + 1) % timeBars.Length].MoveToX(-TimeBarX, 0);
                }

                currentX = (currentX + 1) % (timeBars.Length * WIDTH);

                foreach (Drawable e in timeBarContainer.Children)
                    if (e.Position.X == TimeBarX)
                        e.Expire();
            }
        }

        private Color4 getColour(FrameTimeType type)
        {
            Color4 col = default(Color4);

            switch (type)
            {
                default:
                case FrameTimeType.Update:
                    col = Color4.YellowGreen;
                    break;
                case FrameTimeType.Draw:
                    col = Color4.BlueViolet;
                    break;
                case FrameTimeType.SwapBuffer:
                    col = Color4.Red;
                    break;
#if DEBUG
                case FrameTimeType.Debug:
                    col = Color4.Yellow;
                    break;
#endif
                case FrameTimeType.Sleep:
                    col = Color4.DarkBlue;
                    break;
                case FrameTimeType.Scheduler:
                    col = Color4.HotPink;
                    break;
                case FrameTimeType.BetweenFrames:
                    col = Color4.GhostWhite;
                    break;
                case FrameTimeType.Empty:
                    col = new Color4(50, 40, 40, 180);
                    break;
            }

            return col;
        }

        private int addArea(FrameStatistics frame, FrameTimeType frameTimeType, int currentHeight)
        {
            double elapsedMilliseconds = 0;
            int drawHeight = 0;

            if (frameTimeType == FrameTimeType.Empty)
                drawHeight = currentHeight;
            else if (frame.CollectedTimes.TryGetValue(frameTimeType, out elapsedMilliseconds))
            {
                drawHeight = (int)(elapsedMilliseconds * scale);
            }
            else
                return currentHeight;

            Color4 col = getColour(frameTimeType);

            for (int i = currentHeight - 1; i >= 0; --i)
            {
                if (drawHeight-- == 0) break;

                int index = i * 4;
                textureData[index] = (byte)(255 * col.R);
                textureData[index + 1] = (byte)(255 * col.G);
                textureData[index + 2] = (byte)(255 * col.B);
                textureData[index + 3] = (byte)(255 * (frameTimeType == FrameTimeType.Empty ? (col.A * (1 - (int)((i * 4) / HEIGHT) / 8f)) : col.A));
                currentHeight--;
            }

            return currentHeight;
        }
    }
}
