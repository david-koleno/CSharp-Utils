using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LocalDataGeneration
{
    public class SettingsZ
    {
        #region Default Settings

        private static readonly string DefaultLogPath = "";

        #endregion


        public static Dictionary<string,bool> loggerSettings;
        public static Dictionary<string,string> SQLSettings;
        public static Dictionary<string,string> ODBCSettings;

        public static void GetSettings (OdbcConnection connection)
        {
            GetSettingsFromDatabase(connection);
        }

        public static void GetSettings (string filepath = "settings.xml")
        {
            if(File.Exists(filepath))
            {
                GetSettingsFromXML(filepath);
            }
            else
            {
                GenerateSettingsFile(filepath);
            }
                    
            
        }

        private static void GenerateSettingsFile (string filepath)
        {
            #region Start Document
            XmlWriter xmlWriter = XmlWriter.Create(filepath);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("settings");
            #endregion

            #region Logger Settings
            xmlWriter.WriteStartElement("Logger");
            xmlWriter.WriteElementString("LogFilePath",DefaultLogPath);

            xmlWriter.WriteStartElement("File");
            xmlWriter.WriteElementString("Info","true");
            xmlWriter.WriteElementString("Debug","true");
            xmlWriter.WriteElementString("Warn","true");
            xmlWriter.WriteElementString("Error","true");
            xmlWriter.WriteElementString("Fatal","true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Console");
            xmlWriter.WriteElementString("Info","true");
            xmlWriter.WriteElementString("Debug","true");
            xmlWriter.WriteElementString("Warn","true");
            xmlWriter.WriteElementString("Error","true");
            xmlWriter.WriteElementString("Fatal","true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Database");
            xmlWriter.WriteElementString("Info","true");
            xmlWriter.WriteElementString("Debug","true");
            xmlWriter.WriteElementString("Warn","true");
            xmlWriter.WriteElementString("Error","true");
            xmlWriter.WriteElementString("Fatal","true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("EventLog");
            xmlWriter.WriteElementString("Info","true");
            xmlWriter.WriteElementString("Debug","true");
            xmlWriter.WriteElementString("Warn","true");
            xmlWriter.WriteElementString("Error","true");
            xmlWriter.WriteElementString("Fatal","true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            #endregion

            #region SQL Settings
            xmlWriter.WriteStartElement("SQL");

            xmlWriter.WriteEndElement();
            #endregion

            #region ODBC Settings
            xmlWriter.WriteStartElement("ODBC");

            xmlWriter.WriteEndElement();
            #endregion

            #region Discord Settings
            xmlWriter.WriteStartElement("Discord");

            xmlWriter.WriteEndElement();
            #endregion

            #region template
            xmlWriter.WriteStartElement("");

            xmlWriter.WriteEndElement();
            #endregion

            #region End Document
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            #endregion
        }

        private static void GetSettingsFromDatabase (OdbcConnection connection)
        {
            throw new NotImplementedException();
        }

        private static void GetSettingsFromXML (string filepath)
        {
            try
            {
                XElement settingsXml = XElement.Parse(String.Join("\n",File.ReadAllLines(filepath)));
                IEnumerable<XElement> settingsX = settingsXml.Elements();
                foreach(XElement setting in settingsX)
                {
                    switch(setting.Name.ToString())
                    {
                        #region Logger Settings
                        case "Logger":
                            foreach(XElement element in setting.Elements())
                            {
                                if(element.Name == "LogFilePath")
                                {

                                }
                                else
                                {
                                    foreach(XElement level in element.Elements())
                                    {
                                        loggerSettings.Add(element.Name.ToString() + "." + level.Name.ToString(),( level.Value == "true" ));
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region SQL Settings
                        case "SQL":
                            foreach(XElement element in setting.Elements())
                            {
                                if(element.Name == "BulkInsertCount")
                                {
                                    //bulkInsertCount = Int32.Parse(element.Value.ToString());
                                }
                            }
                            
                            break;
                        #endregion

                        #region ODBC Settings
                        case "ODBC":
                            OdbcZ.defaultConnection = new ODBCHandler(setting.Elements());
                            break;
                        #endregion

                        #region Discord Settings
                        case "Discord":
                            break;
                        #endregion
                    }


                    if(setting.Name == "Connection")
                    {

                    }
                    else if(setting.Name == "Logging")
                    {
                        
                    }
                    else if(setting.Name == "General")
                    {
                        foreach(XElement element in setting.Elements())
                        {

                            

                        }
                    }
                    else if(setting.Name == "Discord")
                    {

                    }
                }
            }
            catch(Exception e)
            {
                LoggerZ.FatalError(String.Format("Error: could not read lines in file \"{0}\"\n{1}",filepath,e.Message));
            }
        }
    }
}
