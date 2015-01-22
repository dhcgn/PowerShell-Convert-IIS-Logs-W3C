using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace ReadIISLog
{
    public class LogReader
    {
        private static int CountLinesInFile(string filePath)
        {
            using (var r = new StreamReader(filePath))
            {
                var i = 0;

                while (r.ReadLine() != null)
                    i++;

                return i;
            }
        }

        public static void ProcessLogFile(FileInfo inputFile, Action<LogEntry> callback, Action<int, int, string> progressCallback, Action<Exception> errorCallback)
        {
            var interpretation = new Dictionary<int, EntryValue>();

            var fullName = inputFile.FullName;

            var total = CountLinesInFile(fullName);
            var index = 0;

            progressCallback.Invoke(index, total, fullName);

            using (var file = new StreamReader(fullName))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
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

                    var logEntry = CreateLogEntry(line, interpretation, errorCallback);
                    if(logEntry!=null)
                        callback.Invoke(logEntry);

                    if (index%1000 == 0)
                    {
                        progressCallback.Invoke(index, total, fullName);
                    }
                }
                file.Close();
            }

            progressCallback.Invoke(index, total, fullName);
        }

        private static LogEntry CreateLogEntry(string line, Dictionary<int, EntryValue> interpretation, Action<Exception> errorCallback)
        {
            var result = new LogEntry();

            var values = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            if(values.Count()!=interpretation.Count)
                return null;
            

            for (var i = 0; i < values.Length; i++)
            {
                try
                {
                    var value = values[i];
                    var valueInterpretation = interpretation[i];

                    switch (valueInterpretation)
                    {
                        case EntryValue.date:
                            result.Date = value == "-" ? DateTime.MinValue : DateTime.Parse(value);
                            result.Date = result.Date.Date;
                            break;
                        case EntryValue.time:
                            result.Time = value == "-" ? DateTime.MinValue : DateTime.Parse(value);
                            result.Time = new DateTime(1, 1, 1, result.Time.Hour, result.Time.Minute, result.Time.Second);
                            break;
                        case EntryValue.s_ip:
                            result.SourceIpAddress = IPAddress.Parse(value);
                            break;
                        case EntryValue.cs_method:
                            result.Method = value;
                            break;
                        case EntryValue.cs_uri_stem:
                            result.UriStem = value;
                            break;
                        case EntryValue.cs_uri_query:
                            result.UriQuery = value;
                            break;
                        case EntryValue.s_port:
                            result.Port = Int32.Parse(value);
                            break;
                        case EntryValue.cs_username:
                            result.Username = value;
                            break;
                        case EntryValue.c_ip:
                            result.ClientIpAddress = IPAddress.Parse(value);
                            break;
                        case EntryValue.csUser_Agent:
                            result.UserAgent = value.Replace("+", " ");
                            break;
                        case EntryValue.csReferer:
                            result.Referrer = value;
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
                    }
                }
                catch (Exception e)
                {
                    errorCallback.Invoke(e);
                    return null;
                }
            }

            return result;
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
            Unkown
        }
    }
}