using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ConvertFromIISLogFile
{
    public class LogReader
    {
        private static int CountLinesInFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var r = new StreamReader(fileStream))
                {
                    var i = 0;

                    while (r.ReadLine() != null)
                        i++;

                    return i;
                }
            }
        }

        public static void ProcessLogFiles(FileInfo[] inputFiles, Action<LogEntry> outputCallback, Action<int, int, string> progressCallback, Action<Exception> errorCallback, bool noLineByLineProgress, Func<bool> isCanceld)
        {
            foreach (var inputFile in inputFiles)
            {
                try
                {
                    if(isCanceld.Invoke())return;

                    ProcessLogFile(inputFile, outputCallback, progressCallback, errorCallback, noLineByLineProgress, isCanceld);
                }
                catch (Exception e)
                {
                    errorCallback(e);
                }
            }
        }

        public static void ProcessKernelLogFiles(FileInfo[] inputFiles, Action<KernelLogEntry> outputCallback, Action<int, int, string> progressCallback, Action<Exception> errorCallback, bool noLineByLineProgress, Func<bool> isCanceld)
        {
            foreach (var inputFile in inputFiles)
            {
                try
                {
                    if (isCanceld.Invoke()) return;

                    ProcessLogFile(inputFile, outputCallback, progressCallback, errorCallback, noLineByLineProgress, isCanceld);
                }
                catch (Exception e)
                {
                    errorCallback(e);
                }
            }
        }

        private static void ProcessLogFile<T>(FileInfo inputFile, Action<T> outputCallback, Action<int, int, string> progressCallback, Action<Exception> errorCallback, bool noLineByLineProgress, Func<bool> isCanceld) where T : ILogEntry, new()
        {
            var interpretation = new Dictionary<int, EntryValue>();

            var fullName = inputFile.FullName;

            int total = 0;
            if (!noLineByLineProgress)
                total = CountLinesInFile(fullName);

            var index = 0;

            progressCallback.Invoke(index, total, fullName);

            using (FileStream fileStream = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var file = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (isCanceld.Invoke()) return;

                        index++;
                        
                        if (line.StartsWith("#Fields:"))
                        {
                            interpretation = GetInterpretation(line);
                            continue;
                        }
                        if (line.StartsWith("#"))
                        {
                            continue;
                        }

                        var logEntry = CreateLogEntry<T>(line, interpretation, errorCallback, fullName);
                        if (logEntry != null)
                            outputCallback.Invoke(logEntry);

                        if (!noLineByLineProgress)
                        {
                            if (index%1000 == 0)
                            {
                                progressCallback.Invoke(index, total, fullName);
                            }
                        }
                    }
                    file.Close();
                }
            }

            progressCallback.Invoke(index, total, fullName);
        }



        private static T CreateLogEntry<T>(string line, Dictionary<int, EntryValue> interpretation, Action<Exception> errorCallback, string fullName) where T : ILogEntry, new()
        {
            ILogEntry result = new T();

            result.LogFile = fullName;
            result.LogFileRootFolder = Path.GetPathRoot(fullName);

            var values = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            if (values.Count() != interpretation.Count)
                return default(T);

            for (var i = 0; i < values.Length; i++)
            {
                try
                {
                    var value = values[i];
                    var valueInterpretation = interpretation[i];

                    SetProperties(value, valueInterpretation, result);

                    if (result.GetType() == typeof (LogEntry))
                    {
                        SetPropertiesLogEntry(value, valueInterpretation, (LogEntry) result);
                    }
                    else if (result.GetType() == typeof(KernelLogEntry))
                    {
                        SetPropertiesKernelLogEntry(value, valueInterpretation, (KernelLogEntry)result);
                    }
                }
                catch (Exception e)
                {
                    errorCallback.Invoke(e);
                    return default(T);
                }
            }

            return (T) result;
        }

        private static void SetProperties(string value, EntryValue valueInterpretation, ILogEntry result)
        {
            switch (valueInterpretation)
            {
                case EntryValue.date:
                    // 2015-01-13
                    var date = value == "-" ? DateTime.MinValue : DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    result.DateTime = new DateTime(date.Year, date.Month, date.Day, result.DateTime.Hour, result.DateTime.Minute, result.DateTime.Second);
                    break;
                case EntryValue.time:
                    // 00:32:17
                    var time = value == "-" ? DateTime.MinValue : DateTime.ParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture);
                    result.DateTime = new DateTime(result.DateTime.Year, result.DateTime.Month, result.DateTime.Day, time.Hour, time.Minute, time.Second);
                    break;
                case EntryValue.s_ip:
                    result.SourceIpAddress = IPAddress.Parse(value);
                    break;
                case EntryValue.cs_method:
                    result.Method = value;
                    break;
                case EntryValue.s_port:
                    result.Port = Int32.Parse(value);
                    break;
                case EntryValue.c_ip:
                    result.ClientIpAddress = IPAddress.Parse(value);
                    break;


            }
        }

        private static void SetPropertiesKernelLogEntry(string value, EntryValue valueInterpretation, KernelLogEntry result)
        {
            switch (valueInterpretation)
            {
                case EntryValue.c_port:
                    result.ClientPort = Int32.Parse(value);
                    break;
                case EntryValue.sc_status:
                    result.ProtocolStatus = value;
                    break;
                case EntryValue.s_queuename:
                    result.QueueName = value;
                    break;
                case EntryValue.s_reason:
                    result.Reason = value;
                    break;
                case EntryValue.SiteId:
                    result.SiteId = value;
                    break;
                case EntryValue.ProtocolStatus:
                    result.ProtocolStatus = value;
                    break;
                case EntryValue.Version:
                    result.Version = value;
                    break;
                case EntryValue.cs_uri:
                    result.Uri = value;
                    break;
            }
        }

        private static void SetPropertiesLogEntry(string value, EntryValue valueInterpretation, LogEntry result)
        {
            switch (valueInterpretation)
            {
                case EntryValue.cs_uri_stem:
                    result.UriStem = value;
                    break;
                case EntryValue.cs_uri_query:
                    result.UriQuery = value;
                    break;
                case EntryValue.cs_username:
                    result.Username = value;
                    break;
                case EntryValue.csUser_Agent:
                    // todo Fix Encoding
                    result.UserAgent = value.Replace("+", " ");
                    break;
                case EntryValue.csReferer:
                    result.Referrer = value;
                    break;
                case EntryValue.cs_host:
                    result.Host = value;
                    break;
                case EntryValue.sc_status:
                    result.HttpStatus = value;
                    break;
                case EntryValue.sc_substatus:
                    result.ProtocolSubstatus = value;
                    break;
                case EntryValue.sc_win32_status:
                    result.SystemErrorCodes = value;
                    break;
                case EntryValue.sc_bytes:
                    result.ServerSentBytes = Int32.Parse(value);
                    break;
                case EntryValue.cs_bytes:
                    result.ServerReceivedBytes = Int32.Parse(value);
                    break;
                case EntryValue.TimeTaken:
                    result.TimeTaken = Int32.Parse(value);
                    break;
                case EntryValue.ComputerName:
                    result.ComputerName = value;
                    break;
                case EntryValue.SiteName:
                    result.SiteName = value;
                    break;
            }
        }

        private static Dictionary<int, EntryValue> GetInterpretation(string line)
        {
            var result = new Dictionary<int, EntryValue>();

            line = line.Replace("#Fields:", String.Empty).Trim();

            var fieldNames = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < fieldNames.Count(); i++)
            {
                result.Add(i, ParseFieldName(fieldNames[i]));
            }

            return result;
        }

        private static EntryValue ParseFieldName(string fieldName)
        {
            switch (fieldName)
            {
                case "date":
                    return EntryValue.date;
                case "time":
                    return EntryValue.time;
                case "s-ip":
                    return EntryValue.s_ip;
                case "cs-method":
                    return EntryValue.cs_method;
                case "cs-uri-stem":
                    return EntryValue.cs_uri_stem;
                case "cs-uri-query":
                    return EntryValue.cs_uri_query;
                case "s-port":
                    return EntryValue.s_port;
                case "cs-username":
                    return EntryValue.cs_username;
                case "c-ip":
                    return EntryValue.c_ip;
                case "cs(User-Agent)":
                    return EntryValue.csUser_Agent;
                case "cs(Referer)":
                    return EntryValue.csReferer;
                case "sc-status":
                    return EntryValue.sc_status;
                case "sc-substatus":
                    return EntryValue.sc_substatus;
                case "sc-win32-status":
                    return EntryValue.sc_win32_status;
                case "sc-bytes":
                    return EntryValue.sc_bytes;
                case "cs-bytes":
                    return EntryValue.cs_bytes;
                case "time-taken":
                    return EntryValue.TimeTaken;
                case "s-computername":
                    return EntryValue.ComputerName;
                case "s-sitename":
                    return EntryValue.SiteName;
                case "cs-host":
                    return EntryValue.cs_host;
                case "c-port":
                    return EntryValue.c_port;
                case "s-queuename":
                    return EntryValue.s_queuename;
                case "s-reason":
                    return EntryValue.s_reason;
                case "s-siteid":
                    return EntryValue.SiteId;
                case "cs-version":
                    return EntryValue.Version;
                case "cs-uri":
                    return EntryValue.cs_uri;

                default:
                    return EntryValue.Unkown;
            }
        }

        private enum EntryValue
        {
            date,
            time,
            s_ip,
            cs_method,
            cs_uri_stem,
            cs_uri_query,
            s_port,
            cs_username,
            c_ip,
            csUser_Agent,
            csReferer,
            sc_status,
            sc_substatus,
            sc_win32_status,
            sc_bytes,
            cs_bytes,
            TimeTaken,
            Unkown,
            ComputerName,
            SiteName,
            cs_host,
            c_port,
            s_queuename,
            s_reason,
            SiteId,
            ProtocolStatus,
            Version,
            cs_uri
        }
    }

    internal interface ILogEntry
    {
        string LogFile { get; set; }
        string LogFileRootFolder { get; set; }
        IPAddress SourceIpAddress { get; set; }
        string Method { get; set; }
        int Port { get; set; }
        IPAddress ClientIpAddress { get; set; }
        DateTime DateTime { get; set; }
    }
}