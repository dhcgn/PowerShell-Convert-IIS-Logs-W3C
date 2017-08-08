using System;
using System.Collections.Generic;
using System.Net;

namespace ConvertFromIISLogFile
{
    /// <summary>
    /// W3C Extended Log File Format
    /// http://www.w3.org/TR/WD-logfile.html
    /// 
    /// Format of the HTTP API error logs
    /// https://support.microsoft.com/en-us/kb/820729#bookmark-1
    /// </summary>
    public class KernelLogEntry: ILogEntry
    {
        /// <summary>
        /// The date and time (UTC) on which the activity occurred.
        /// </summary>
        public DateTime DateTime { get; set; }

        

        /// <summary>
        /// The date and time (Local) on which the activity occurred.
        /// </summary>
        public DateTime DateTimeLocalTime => DateTime.SpecifyKind(this.DateTime, DateTimeKind.Utc).ToLocalTime();

        /// <summary>
        /// The IP address of the affected server. The value in this field can be either an IPv4 address or an IPv6 address. 
        /// If the server IP address is an IPv6 address, the ScopeId field is also included in the address.
        /// 
        ///     Field: s-ip
        /// </summary>
        public string SourceIpAddress { get; set; }

        /// <summary>
        /// The requested action, for example, a GET method.
        /// 
        ///     Field: cs-method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The URL and any query that is associated with it are logged as one field that is separated by a question mark (0x3F). 
        /// This field is truncated at its length limit of 4,096 bytes.
        /// 
        /// If this URL was parsed ("cooked"), it is logged with local code page conversion and is treated as a Unicode field.
        /// If this URL has not been parsed ("cooked") at the time of logging, it is copied exactly, without any Unicode conversion.
        /// If the HTTP API cannot parse this URL, a hyphen (0x002D) is used as a placeholder for the empty field.
        /// 
        ///     Field: cs-uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The version of the protocol that is being used.
        /// If the connection has not been parsed sufficiently to determine the protocol version, a hyphen(0x002D) is used as a placeholder for the empty field.
        /// If either the major version number or the minor version number that is parsed is greater than or equal to 10, the version is logged as HTTP/?.?.
        /// 
        ///     Field: cs-version
        /// </summary>
        public string Version { get; set; }


        /// <summary>
        /// The port number of the affected server.
        /// 
        ///     Field: s-port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The port number of the affected server.
        /// 
        ///     Field: c-port
        /// </summary>
        public int ClientPort { get; set; }

        /// <summary>
        /// The IP address of the client that made the request.
        /// 
        ///     Field: c-ip
        /// </summary>
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// The protocol status cannot be greater than 999.
        /// If the protocol status of the response to a request is available, it is logged in this field.
        /// If the protocol status is not available, a hyphen(0x002D) is used as a placeholder for the empty field.
        /// 
        ///     Field: sc-status
        /// </summary>
        public string ProtocolStatus { get; set; }

        /// <summary>
        /// Not used in this version of the HTTP API. A placeholder hyphen (0x002D) always appears in this field.
        /// 
        ///     Field: s-siteid
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// This the request queue name.
        /// 
        ///     Field: s-queuename
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// This field contains a string that identifies the kind of error that is being logged. This field is never left empty.
        /// 
        ///     Field: s-reason
        /// </summary>
        public string Reason { get; set; }

        public string LogFile { get; set; }
        public string LogFileRootFolder { get; set; }

        public Dictionary<string, string> NotedProperties { get; set; }
    }
}