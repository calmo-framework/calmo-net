using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Calmo.Core;
using Calmo.Data.SAP.Configuration;
using Calmo.Core.Properties;

namespace Calmo.Data
{
    public static class CustomConfigurationSAPDataExtensions
    {
        private static readonly object _lockObjectSAPData = new object();
        private static SAPDataSection _activeDirectoryDataSection;

        public static SAPDataSection SAPData(this CustomConfiguration config)
        {
            lock (_lockObjectSAPData)
            {
                if (_activeDirectoryDataSection == null)
                {
                    _activeDirectoryDataSection = ConfigurationManager.GetSection(SAPDataSection.SectionName) as SAPDataSection;

                    if (_activeDirectoryDataSection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, SAPDataSection.SectionName));
                }
            }

            return _activeDirectoryDataSection;
        }
    }
}