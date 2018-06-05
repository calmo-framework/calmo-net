using System;
using System.Configuration;
using Calmo.Core;
using Calmo.Data.Sharepoint.Configuration;
using Calmo.Core.Properties;

namespace Calmo.Data.Sharepoint
{
    public static class CustomConfigurationDataExtensions
    {
        private static readonly object _lockObjectSharepoint = new object();
        private static SharepointSection _sharepointSection;

        public static SharepointSection Sharepoint(this CustomConfiguration config)
        {
            lock (_lockObjectSharepoint)
            {
                if (_sharepointSection == null)
                {
                    _sharepointSection = ConfigurationManager.GetSection(SharepointSection.SectionName) as SharepointSection;

                    if (_sharepointSection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, SharepointSection.SectionName));
                }
            }

            return _sharepointSection;
        }
    }
}