using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Calmo.Core;
using Calmo.Data.Configuration;
using Calmo.Core.Properties;

namespace Calmo.Data
{
    public static class CustomConfigurationDataExtensions
    {
        private static readonly object _lockObjectData = new object();
        private static DataSection _dataSection;

        public static DataSection Data(this CustomConfiguration config)
        {
            lock (_lockObjectData)
            {
                if (_dataSection == null)
                {
                    _dataSection = ConfigurationManager.GetSection(DataSection.SectionName) as DataSection;

                    if (_dataSection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, DataSection.SectionName));
                }
            }

            return _dataSection;
        }
    }
}