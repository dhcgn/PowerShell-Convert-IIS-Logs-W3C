using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ConvertFromIISLogFile;
using NUnit.Framework;

namespace ReadIISLog.Test
{
    [TestFixture(Ignore = "Test files are mising")]
    public class ConvertFromKernelLogFileTest
    {
        private string wellFormedLogFile;
        private string kernelSingleRecordFile;

        [SetUp]
        public void Setup()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            this.wellFormedLogFile = Helper.GetValue(executingAssembly, "Kernel_httperr24.log");
            this.kernelSingleRecordFile = Helper.GetValue(executingAssembly, "Kernel_SingleRecord.log");
        }

        [Test()]
        public void MultipleRecords()
        {
            var list = new List<KernelLogEntry>();

            LogReader.ProcessKernelLogFiles(new[] { new FileInfo(this.wellFormedLogFile) }, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Assert.Fail(exception1.ToString()), true, () => false);

            Assert.AreEqual(22, list.Count);
        }

        [Test]
        public void SingleRecord()
        {
            var list = new List<KernelLogEntry>();

            LogReader.ProcessKernelLogFiles(new[] { new FileInfo(this.kernelSingleRecordFile) }, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Assert.Fail(exception1.ToString()), true, () => false);

            Assert.AreEqual(1, list.Count);

            Assert.AreEqual(DateTime.Parse("2015-10-24 06:49:44"), list[0].DateTime, "DateTime");
            Assert.AreEqual(DateTime.Parse("24.10.2015 08:49:44"), list[0].DateTimeLocalTime, "DateTimeLocalTime");
            Assert.AreEqual("192.168.0.8", list[0].ClientIpAddress.ToString(), "ClientIpAddress");
            Assert.AreEqual("GET", list[0].Method, "Method");
            Assert.AreEqual(80, list[0].Port, "Port");
            Assert.AreEqual(47288, list[0].ClientPort, "ClientPort");
            Assert.AreEqual("500", list[0].ProtocolStatus, "ProtocolStatus");
            Assert.AreEqual("Main", list[0].QueueName, "QueueName");
            Assert.AreEqual("Timer_ConnectionIdle", list[0].Reason, "Reason");
            Assert.AreEqual("1", list[0].SiteId, "SiteId");
            Assert.AreEqual("192.168.86.255", list[0].SourceIpAddress.ToString(), "SourceIpAddress");
            Assert.AreEqual("1", list[0].Version, "Version");
            Assert.AreEqual("index.html", list[0].Uri, "Uri");
        }
    }
}