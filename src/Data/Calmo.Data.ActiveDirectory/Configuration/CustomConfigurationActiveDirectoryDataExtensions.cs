using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Calmo.Core;
using Calmo.Data.ActiveDirectory.Configuration;
using Calmo.Core.Properties;

namespace Calmo.Data
{
    public static class CustomConfigurationActiveDirectoryDataExtensions
    {
        private static readonly object _lockObjectActiveDirectoryData = new object();
        private static ActiveDirectoryDataSection _activeDirectoryDataSection;

        public static ActiveDirectoryDataSection ActiveDirectoryData(this CustomConfiguration config)
        {
            lock (_lockObjectActiveDirectoryData)
            {
                if (_activeDirectoryDataSection == null)
                {
                    _activeDirectoryDataSection = ConfigurationManager.GetSection(ActiveDirectoryDataSection.SectionName) as ActiveDirectoryDataSection;

                    if (_activeDirectoryDataSection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, ActiveDirectoryDataSection.SectionName));
                }
            }

            return _activeDirectoryDataSection;
        }
    }
}