using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Miqo.Config.Tests
{
    public class MiqoConfigTests
    {
        [Fact]
        public void Load_CanReadValidString()
        {
            const string json = "{ \"ConnectionString\": \"localhost\" }";

            var config = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationDb>(json);
            
            Assert.Equal("localhost", config.ConnectionString);
        }
    }
}
