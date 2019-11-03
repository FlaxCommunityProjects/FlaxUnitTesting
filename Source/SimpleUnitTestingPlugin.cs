using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace FlaxCommunity.UnitTesting
{
    public class SimpleUnitTestingPlugin : GamePlugin
    {
        public override PluginDescription Description => new PluginDescription
        {
            Author = "Lukáš Jech",
            AuthorUrl = "https://lukas.jech.me",
            Category = "Unit Testing",
            Description = "Simple unit testing framework",
            IsAlpha = false,
            IsBeta = false,
            Name = "Simple Unit Testing",
            SupportedPlatforms = new PlatformType[] { PlatformType.Windows },
            Version = new Version(1, 1),
            RepositoryUrl = "https://github.com/FlaxCommunityProjects/FlaxUnitTesting"
        };
    }
}
