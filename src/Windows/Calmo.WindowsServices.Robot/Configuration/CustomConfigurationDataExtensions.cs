using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Calmo.Core;
using Calmo.WindowsServices.Robot.Configuration;

namespace Calmo.WindowsServices.Robot
{
    public static class CustomConfigurationRobotExtensions
    {
        private static readonly object _lockObjectData = new object();
        private static RobotConfiguration _configuration;

        public static RobotConfiguration Robot(this CustomConfiguration config, Action<string> logMethod = null)
        {
            lock (_lockObjectData)
            {
                if (logMethod != null)
                    logMethod("Atualizando configurações do serviço");

                var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var xmlPath = Path.Combine(appPath, "RobotConfiguration.xml");

                if (File.Exists(xmlPath))
                {
                    var xmlConfigReader = new StreamReader(xmlPath);
                    var xml = xmlConfigReader.ReadToEnd();
                    xmlConfigReader.Close();

                    if (!String.IsNullOrEmpty(xml))
                    {
                        var reader = new StringReader(xml);
                        var ds = new DataSet();
                        ds.ReadXml(reader);

                        var robotConfig = ds.Tables["robotConfig"];
                        _configuration = new RobotConfiguration();

                        if (robotConfig != null)
                        {
                            if (robotConfig.Rows.Count > 0)
                            {
                                if (robotConfig.Columns.IndexOf("name") > -1)
                                    _configuration.ServiceName = robotConfig.Rows[0]["name"].ToString();

                                if (robotConfig.Columns.IndexOf("executionMilliseconds") > -1)
                                {
                                    int millisecconds;

                                    if (int.TryParse(robotConfig.Rows[0]["executionMilliseconds"].ToString(), out millisecconds))
                                        _configuration.ExecutionMilliseconds = Math.Max(millisecconds, 1000);
                                }

                                if (robotConfig.Columns.IndexOf("executionTimeoutMinutes") > -1)
                                {
                                    int minutes;

                                    if (int.TryParse(robotConfig.Rows[0]["executionTimeoutMinutes"].ToString(), out minutes))
                                        _configuration.ExecutionTimeout = minutes;
                                }

                                if (robotConfig.Columns.IndexOf("logSteps") > -1)
                                    _configuration.LogSteps = robotConfig.Rows[0]["logSteps"].ToString().ToLower() == "true" || robotConfig.Rows[0]["logSteps"].ToString().ToLower() == "1";

                                if (robotConfig.Columns.IndexOf("logTextFile") > -1)
                                    _configuration.LogTextFile = robotConfig.Rows[0]["logTextFile"].ToString().ToLower() == "true" || robotConfig.Rows[0]["logTextFile"].ToString().ToLower() == "1";

                                if (_configuration.LogTextFile)
                                {
                                    if (robotConfig.Columns.IndexOf("textFilePath") > -1)
                                        _configuration.TextFilePath = robotConfig.Rows[0]["textFilePath"].ToString();
                                }
                            }
                        }

                        var configClasses = new List<ServiceClassDefinition>();
                        var tbServicesClass = ds.Tables["serviceClass"];

                        if (tbServicesClass != null)
                        {
                            if (tbServicesClass.Rows.Count > 0)
                            {
                                configClasses.AddRange(from DataRow row in tbServicesClass.Rows
                                                       select new ServiceClassDefinition
                                                           {
                                                               AssemblyName = Convert.ToString(row["assemblyName"]),
                                                               TypeFullName = Convert.ToString(row["typeFullName"])
                                                           });
                            }
                        }

                        if (_configuration.ServiceClasses.Count != configClasses.Count)
                        {
                            _configuration.ServiceClasses = configClasses;
                        }
                        else
                        {
                            var isEqual = true;
                            var x = 0;
                            foreach (var def in _configuration.ServiceClasses)
                            {
                                if (def.AssemblyName != configClasses[x].AssemblyName || def.TypeFullName != configClasses[x].TypeFullName)
                                {
                                    isEqual = false;
                                    break;
                                }

                                x++;
                            }

                            if (!isEqual)
                                _configuration.ServiceClasses = configClasses;
                        }
                    }
                }
                else
                    throw new FileNotFoundException("The file 'RobotConfiguration.xml' was not fount in the same directory as the service .exe file.");

                if (String.IsNullOrWhiteSpace(_configuration.ServiceName))
                    _configuration.ServiceName = "Automatic service execution service";

                if (logMethod != null)
                    logMethod("Service configurations updated");

                return _configuration;
            }
        }
    }
}