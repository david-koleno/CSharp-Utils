using Colorful;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;
using System.Diagnostics;
using System.Reflection;


namespace LocalDataGeneration
{
    public enum LogTarget
    {
        File,Database,EventLog,Console
    }
    public enum Level
    {
        Info,Debug,Warn,Error,Fatal
    }
    public abstract class LogBase
    {
        protected readonly object lockObj = new object();
        public abstract void Log (string message,string name,Level level);
    }
    public class FileLogger : LogBase
    {
        public string infoFilePath = @"InfoLog.txt";
        public string debugFilePath = @"debugLog.txt";
        public string warnFilePath = @"WarnLog.txt";
        public string errorFilePath = @"ErrorLog.txt";
        public string fatalFilePath = @"FatalLog.txt";
        public override void Log (string message,string name,Level level)
        {
            lock(lockObj)
            {
                string filePath = "";
                switch(level)
                {
                    case Level.Info:
                        filePath = infoFilePath;
                        break;
                    case Level.Debug:
                        filePath = debugFilePath;
                        break;
                    case Level.Warn:
                        filePath = warnFilePath;
                        break;
                    case Level.Error:
                        filePath = errorFilePath;
                        break;
                    case Level.Fatal:
                        filePath = fatalFilePath;
                        break;
                }
                if(!File.Exists(filePath))
                {
                    using(StreamWriter streamWriter = File.CreateText(filePath))
                    {
                        streamWriter.WriteLine(String.Format("[{0}]:{1}",DateTime.Now,message));
                        streamWriter.Close();
                    }
                }
                else
                {
                    using(StreamWriter streamWriter = File.AppendText(filePath))
                    {
                        streamWriter.WriteLine(String.Format("[{0}]:{1}",DateTime.Now,message));
                        streamWriter.Close();
                    }
                }
            }
        }
    }
    public class EventLogger : LogBase
    {
        public override void Log (string message,string name,Level level)
        {
            lock(lockObj)
            {
                EventLog m_EventLog = new EventLog("")
                {
                    Source = "IDGEventLog"
                };
                m_EventLog.WriteEntry(message);
            }
        }
    }
    public class DBLogger : LogBase
    {
        public override void Log (string message,string name,Level level)
        {
            lock(lockObj)
            {
                //ODBCHandler logConnection = new ODBCHandler();
                //logConnection.Connect(Controller.CashReportServerLogs);
                //string insertString = "INSERT INTO DatabaseLogs (timestamp ,Level ,Username ,Event) VALUES ('" + DateTime.Now.ToString() + "','" + level.ToString() + "','" + "LocalUser1" + "','" + message + "');";
                //logConnection.RunQuery(insertString);
                //logConnection.Disconnect();
            }
        }
    }
    public class ConsoleLogger : LogBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="name"></param>
        /// <param name="level"></param>
        public override void Log (string message,string name,Level level)
        {
            lock(lockObj)
            {
                Color levelColor;
                switch(level)
                {
                    case Level.Info:
                        levelColor = Color.PapayaWhip;
                        break;
                    case Level.Debug:
                        levelColor = Color.DarkGray;
                        break;
                    case Level.Warn:
                        levelColor = Color.Yellow;
                        break;
                    case Level.Error:
                        levelColor = Color.Orange;
                        break;
                    case Level.Fatal:
                        levelColor = Color.Red;
                        break;
                    default:
                        levelColor = Color.LightGray;
                        break;
                }
                Formatter[] data = new Formatter[]
                {
                    new Formatter("[",Color.LightGray),
                    new Formatter(DateTime.Now,Color.DarkGray),
                    new Formatter("]",Color.LightGray),
                    new Formatter(String.Format("{0,-5}",level),levelColor),
                    null,
                    new Formatter(String.Format("{0,33}"," "),levelColor)
                };
                if(message.Length > Console.WindowWidth - 35)
                {
                    string[] splitMessage = WordWrap(message,Console.WindowWidth - 35).TrimEnd('\n').Split('\n');
                    data.Append(null);
                    bool firstLine = true;
                    foreach(string mes in splitMessage)
                    {
                        data[4] = new Formatter(mes.Trim(' '),Color.White);
                        if(firstLine)
                        {
                            Console.WriteLineFormatted("{0}{1}{2} | {3} | {4}",Color.LightGray,data);
                            firstLine = false;
                        }
                        else
                        {
                            Console.WriteLineFormatted("{5} | {4}",Color.LightGray,data);
                        }
                    }
                }
                else
                {
                    data[4] = new Formatter(message,Color.White);
                    Console.WriteLineFormatted("{0}{1}{2} | {3} | {4} ",Color.LightGray,data);
                }
            }
        }
        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        public static string WordWrap (string text,int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();
            if(width < 1)
            {
                return text;
            }
            for(pos = 0;pos < text.Length;pos = next)
            {
                int eol = text.IndexOf(Environment.NewLine,pos);
                if(eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + Environment.NewLine.Length;
                if(eol > pos)
                {
                    do
                    {
                        int len = eol - pos;
                        if(len > width)
                            len = BreakLine(text,pos,width);
                        sb.Append(text,pos,len);
                        sb.Append(Environment.NewLine);
                        pos += len;
                        while(pos < eol && Char.IsWhiteSpace(text[pos]))
                            pos++;
                    } while(eol > pos);
                }
                else
                {
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        private static int BreakLine (string text,int pos,int max)
        {
            int i = max;
            while(i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }
            if(i < 0)
            {
                return max;
            }
            while(i >= 0 && Char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }
            return i + 1;
        }
    }
    public static class LoggerZ
    {
        private static LogBase logger = null;
        public static void Log (string message,Level level,params LogTarget[] targets)
        {
            foreach(LogTarget target in targets)
            {
                switch(target)
                {
                    case LogTarget.File:
                        switch(level)
                        {
                            case Level.Info:
                                if(!SettingsZ.loggerSettings["File.Info"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Debug:
                                if(!SettingsZ.loggerSettings["File.Debug"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Warn:
                                if(!SettingsZ.loggerSettings["File.Warn"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Error:
                                if(!SettingsZ.loggerSettings["File.Error"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Fatal:
                                if(!SettingsZ.loggerSettings["File.Fatal"])
                                {
                                    continue;
                                }
                                break;
                            default:
                                break;
                        }
                        logger = new FileLogger();
                        logger.Log(message,NameOfCallingClass(),level);
                        break;
                    case LogTarget.Database:
                        switch(level)
                        {
                            case Level.Info:
                                if(!SettingsZ.loggerSettings["Database.Info"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Debug:
                                if(!SettingsZ.loggerSettings["Database.Debug"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Warn:
                                if(!SettingsZ.loggerSettings["Database.Warn"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Error:
                                if(!SettingsZ.loggerSettings["Database.Error"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Fatal:
                                if(!SettingsZ.loggerSettings["Database.Fatal"])
                                {
                                    continue;
                                }
                                break;
                            default:
                                break;
                        }
                        logger = new DBLogger();
                        logger.Log(message,NameOfCallingClass(),level);
                        break;
                    case LogTarget.EventLog:
                        logger = new EventLogger();
                        switch(level)
                        {
                            case Level.Info:
                                if(!SettingsZ.loggerSettings["EventLog.Info"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Debug:
                                if(!SettingsZ.loggerSettings["EventLog.Debug"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Warn:
                                if(!SettingsZ.loggerSettings["EventLog.Warn"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Error:
                                if(!SettingsZ.loggerSettings["EventLog.Error"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Fatal:
                                if(!SettingsZ.loggerSettings["EventLog.Fatal"])
                                {
                                    continue;
                                }
                                break;
                            default:
                                break;
                        }
                        logger.Log(message,NameOfCallingClass(),level);
                        break;
                    case LogTarget.Console:
                        switch(level)
                        {
                            case Level.Info:
                                if(!SettingsZ.loggerSettings["Console.Info"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Debug:
                                if(!SettingsZ.loggerSettings["Console.Debug"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Warn:
                                if(!SettingsZ.loggerSettings["Console.Warn"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Error:
                                if(!SettingsZ.loggerSettings["Console.Error"])
                                {
                                    continue;
                                }
                                break;
                            case Level.Fatal:
                                if(!SettingsZ.loggerSettings["Console.Fatal"])
                                {
                                    continue;
                                }
                                break;
                            default:
                                break;
                        }
                        logger = new ConsoleLogger();
                        logger.Log(message,NameOfCallingClass(),level);
                        break;
                    default:
                        return;
                }
            }

        }
        public static void FatalError (string message = "")
        {
            Log(message,Level.Fatal,LogTarget.Console,LogTarget.File);
            Console.Write("Press Enter to Quit...",Color.Red);
            Console.ReadLine();
            Environment.Exit(1);
        }
        public static void LogConsole (string message,Level level)
        {
            Log(message,level,LogTarget.Console);
        }
        public static void LogFile (string message,Level level)
        {
            Log(message,level,LogTarget.File);
        }
        public static void LogDatabase (string message,Level level)
        {
            Log(message,level,LogTarget.Database);
        }
        public static string NameOfCallingClass ()
        {
            string fullName;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames,false).GetMethod();
                declaringType = method.DeclaringType;
                if(declaringType == null)
                {
                    return method.Name;
                }
                skipFrames++;
                fullName = declaringType.FullName;
            }
            while(declaringType.Module.Name.Equals("mscorlib.dll",StringComparison.OrdinalIgnoreCase));

            return fullName.Split('.')[1];
        }
        public static void CreateLogDatabase ()
        {
            // TODO
        }
    }
    public static class LoggerSettings
    {
        public static string LogFileDirectory;
    }
}
