﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Lottery.Logging.Configuration
{
    public class LoggingSection : ConfigurationSection
    {
        [ConfigurationProperty("logging", IsRequired = true)]
        public LoggingElement Logging
        {
            get { return (LoggingElement)base["logging"]; }
        }

        protected override void PostDeserialize()
        {
            base.PostDeserialize();
        }
    }
}
