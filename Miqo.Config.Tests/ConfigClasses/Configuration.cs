using System;

namespace Miqo.Config.Tests.ConfigClasses
{
    public class Configuration
    {
        public string Title { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public DateTime? AiredOn { get; set; }
    }
}
