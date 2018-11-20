// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System.Linq;
using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace osu.Framework.Testing.Drawables.Sections
{
    public class ToolbarAssemblySection : ToolbarSection
    {
        private AssemblyDropdown assemblyDropdown;

        public ToolbarAssemblySection()
        {
            AutoSizeAxes = Axes.X;
            Masking = false;
        }

        [BackgroundDependencyLoader]
        private void load(TestBrowser browser)
        {
            BasicCheckbox runAllStepsCheckbox;

            InternalChild = new FillFlowContainer
            {
                Spacing = new Vector2(5),
                Direction = FillDirection.Horizontal,
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Children = new Drawable[]
                {
                    new SpriteText
                    {
                        Padding = new MarginPadding(5),
                        Text = "Assembly:"
                    },
                    assemblyDropdown = new AssemblyDropdown
                    {
                        Width = 250,
                    },
                    runAllStepsCheckbox = new BasicCheckbox
                    {
                        LabelText = "Run all steps",
                        LabelPadding = new MarginPadding { Left = 5, Right = 10 },
                        AutoSizeAxes = Axes.Y,
                        Width = 140,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    },
                }
            };

            assemblyDropdown.Current.BindTo(browser.Assembly);
            runAllStepsCheckbox.Current.BindTo(browser.RunAllSteps);
        }

        public void AddAssembly(string name, Assembly assembly) => assemblyDropdown.AddAssembly(name, assembly);

        private class AssemblyDropdown : BasicDropdown<Assembly>
        {
            public void AddAssembly(string name, Assembly assembly)
            {
                if (assembly == null) return;

                foreach (var item in MenuItems.ToArray())
                {
                    if (item.Text.Value.Contains("dynamic"))
                        RemoveDropdownItem(item.Value);
                }

                AddDropdownItem(name, assembly);
            }
        }
    }
}
