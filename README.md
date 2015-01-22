# PowerShell Convert IIS Logs (W3C)
Get type safe log entries from an IIS Log file in W3C format

Usage
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
