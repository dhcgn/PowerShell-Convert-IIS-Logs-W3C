# PowerShell Convert IIS Logs (W3C)
Get type safe log entries from an IIS Log file in W3C format

## Install
```Powershell
Import-Module "C:\temp\ConvertFromIISLogFile.dll" -Verbose
```
Very simple, just import the ddl-file. You should consider to put this cmd in your powershell profil.
![][Install]

[Install]: http://i.imgur.com/hF5HPC3.png

## Usage
```Powershell
ConvertFrom-IISLogFile C:\temp\u_ex150113.log | Select-Object -First 10 | 
Format-Table DateTimeLocalTime, ClientIpAddress, HttpStatus, Method, UriStem, ServerSentBytes, ServerReceivedBytes, TimeTaken -AutoSize
```
![][SimpleUsage]

[SimpleUsage]: http://i.imgur.com/tGWDX7P.png 

## Return Type

```
   TypeName: ConvertFromIISLogFile.LogEntry

Name                MemberType Definition
----                ---------- ----------
Equals              Method     bool Equals(System.Object obj)
GetHashCode         Method     int GetHashCode()
GetType             Method     type GetType()
ToString            Method     string ToString()
ClientIpAddress     Property   ipaddress ClientIpAddress {get;set;}
Date                Property   datetime Date {get;set;}
DateTime            Property   datetime DateTime {get;}
DateTimeLocalTime   Property   datetime DateTimeLocalTime {get;}
HttpStatus          Property   string HttpStatus {get;set;}
Method              Property   string Method {get;set;}
Port                Property   int Port {get;set;}
ProtocolSubstatus   Property   string ProtocolSubstatus {get;set;}
Referrer            Property   string Referrer {get;set;}
ServerReceivedBytes Property   int ServerReceivedBytes {get;set;}
ServerSentBytes     Property   int ServerSentBytes {get;set;}
SourceIpAddress     Property   ipaddress SourceIpAddress {get;set;}
SystemErrorCodes    Property   string SystemErrorCodes {get;set;}
Time                Property   datetime Time {get;set;}
TimeTaken           Property   int TimeTaken {get;set;}
UriQuery            Property   string UriQuery {get;set;}
UriStem             Property   string UriStem {get;set;}
UserAgent           Property   string UserAgent {get;set;}
Username            Property   string Username {get;set;}
```

### Return Type Explanation
```C#
using System;
using System.Net;

namespace ConvertFromIISLogFile
{
    /// <summary>
    /// W3C Extended Log File Format
    /// http://www.w3.org/TR/WD-logfile.html
    /// 
    /// Table 10.1 W3C Extended Log File Fields
    /// http://www.microsoft.com/technet/prodtechnol/WindowsServer2003/Library/IIS/676400bc-8969-4aa7-851a-9319490a9bbb.mspx?mfr=true
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// The date on which the activity occurred.
        /// 
        ///     Field: date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The time, in coordinated universal time (UTC), at which the activity occurred.
        /// 
        ///     Field: time
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The date and time (UTC) on which the activity occurred.
        /// </summary>
        public DateTime DateTime => new DateTime(this.Date.Year, this.Date.Month, this.Date.Day, this.Time.Hour, this.Time.Minute, this.Time.Second);

        /// <summary>
        /// The date and time (Local) on which the activity occurred.
        /// </summary>
        public DateTime DateTimeLocalTime => DateTime.SpecifyKind(this.DateTime, DateTimeKind.Utc);

        /// <summary>
        ///     Field: s-ip
        /// </summary>
        public IPAddress SourceIpAddress { get; set; }

        /// <summary>
        /// The requested action, for example, a GET method.
        /// 
        ///     Field: cs-method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The target of the action, for example, Default.htm.
        /// 
        ///     Field: cs-uri-stem
        /// </summary>
        public string UriStem { get; set; }

        /// <summary>
        /// The query, if any, that the client was trying to perform. A Universal Resource Identifier (URI) query is necessary only for dynamic pages.
        /// 
        ///     Field: cs-uri-query
        /// </summary>
        public string UriQuery { get; set; }

        /// <summary>
        /// The server port number that is configured for the service.
        /// 
        ///     Field: s-port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The name of the authenticated user who accessed your server. Anonymous users are indicated by a hyphen.
        /// 
        ///     Field: cs-username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The IP address of the client that made the request.
        /// 
        ///     Field: c-ip
        /// </summary>
        public IPAddress ClientIpAddress { get; set; }

        /// <summary>
        /// The browser type that the client used.
        /// 
        ///     Field: cs(User-Agent)
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// The site that the user last visited. This site provided a link to the current site. 
        /// 
        ///     Field: cs(Referrer)
        /// </summary>
        public string Referrer { get; set; }

        /// <summary>
        /// The HTTP status code.
        /// 
        ///     Field: sc-status
        /// </summary>
        public string HttpStatus { get; set; }

        /// <summary>
        /// The substatus error code. 
        /// 
        ///     Field: sc-substatus
        /// </summary>
        public string ProtocolSubstatus { get; set; }

        /// <summary>
        /// Win32 error code
        ///     Field: sc-win32-status
        /// 
        /// https://msdn.microsoft.com/en-us/library/ms681381.aspx
        /// </summary>
        public string SystemErrorCodes { get; set; }

        /// <summary>
        /// The number of bytes that the server sent.
        /// 
        ///     Field: sc-bytes
        /// </summary>
        public int ServerSentBytes { get; set; }

        /// <summary>
        /// The number of bytes that the server received.
        /// 
        ///     Field: cs-bytes
        /// </summary>
        public int ServerReceivedBytes { get; set; }

        /// <summary>
        /// The length of time that the action took, in milliseconds. 
        /// 
        ///     Field: time-taken
        /// </summary>
        public int TimeTaken { get; set; }
    }
}
```
