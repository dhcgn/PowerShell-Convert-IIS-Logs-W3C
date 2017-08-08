using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Resources;
using ConvertFromIISLogFile;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ReadIISLog.Test
{
    [TestFixture()]
    public class ConvertFromIisLogFile_Simple
    {
        [Test]
        public void Import_module()
        {
            var dir = TestContext.CurrentContext.TestDirectory;
            var filePath = Path.Combine(dir, "ConvertFromIISLogFile.dll");

            Assert.That(File.Exists(filePath));

            var sessionState = InitialSessionState.CreateDefault();
            
            sessionState.ImportPSModule(new[] { filePath});

            var runspace = RunspaceFactory.CreateRunspace(sessionState);
            runspace.Open();

            Pipeline pipeLine = runspace.CreatePipeline();
            pipeLine.Commands.AddScript("ConvertFrom-IISLogFile Umlaute.log");
            Collection<PSObject> resultObjects = pipeLine.Invoke();

            runspace.Close();

        }
    }

    [TestFixture]
    public class ConvertFromIisLogFileTest
    {
        private string wellFormedLogFile;
        private string badFormedLogFile;
        private string badValuesLogFile;
        private string umlauteLogFile;
        private string oneLogEntry;
        private string oneLogEntryAttachedProperty;

        [SetUp]
        public void Setup()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            this.wellFormedLogFile = Helper.GetValue(executingAssembly, "u_ex150113.log");
            this.badFormedLogFile = Helper.GetValue(executingAssembly, "BadSamples.log");
            this.badValuesLogFile = Helper.GetValue(executingAssembly, "BadValues.log");
            this.umlauteLogFile = Helper.GetValue(executingAssembly, "Umlaute.log");
            this.oneLogEntry = Helper.GetValue(executingAssembly, "IIS_OneLogEntry.log");
            this.oneLogEntryAttachedProperty = Helper.GetValue(executingAssembly, "IIS_OneLogEntryAttachedProperty.log");
        }


        [Test]
        public void ProcessLogFiles()
        {
            var list = new List<LogEntry>();

            LogReader.ProcessLogFiles(new[] {new FileInfo(this.wellFormedLogFile)}, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Assert.Fail(exception1.ToString()),true, () => false);

            Assert.AreEqual(40950, list.Count);
        }

        [Test]
        public void ProcessLogFiles_OneEntry()
        {
            var list = new List<LogEntry>();

            LogReader.ProcessLogFiles(new[] {new FileInfo(this.oneLogEntry) }, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Assert.Fail(exception1.ToString()),true, () => false);

            Console.Out.WriteLine(JsonConvert.SerializeObject(list,Formatting.Indented));

            Assert.That(list, Has.Count.EqualTo(1));
        }

        [Test]
        public void ProcessLogFiles_OneEntryAttachedProperty()
        {
            var list = new List<LogEntry>();

            LogReader.ProcessLogFiles(new[] { new FileInfo(this.oneLogEntryAttachedProperty) }, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Assert.Fail(exception1.ToString()), true, () => false);

            Console.Out.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].NotedProperties, Is.Not.Null);
            Assert.That(list[0].NotedProperties, Has.Count.EqualTo(2));
        }

        [Test]
        public void BadFormedLogFile()
        {
            var list = new List<LogEntry>();

            LogReader.ProcessLogFiles(new[] {new FileInfo(this.badFormedLogFile)}, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Console.WriteLine(exception1.ToString()), true, () => false);

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void BadValuesLogFile()
        {
            var list = new List<LogEntry>();

            LogReader.ProcessLogFiles(new[] {new FileInfo(this.badValuesLogFile)}, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Console.WriteLine(exception1.ToString()), true, () => false);

            Assert.AreEqual(2, list.Count);
        }


        [Test]
        public void Umlaute()
        {
            var list = new List<LogEntry>();

            LogReader.ProcessLogFiles(new[] { new FileInfo(this.umlauteLogFile) }, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Console.WriteLine(exception1.ToString()), true, () => false);

            Assert.AreEqual(true, list.First().UserAgent.Contains("ä"));
            Assert.AreEqual(true, list.First().UserAgent.Contains("ö"));
            Assert.AreEqual(true, list.First().UserAgent.Contains("ü"));
            Assert.AreEqual(true, list.First().UserAgent.Contains("Ä"));
            Assert.AreEqual(true, list.First().UserAgent.Contains("Ö"));
            Assert.AreEqual(true, list.First().UserAgent.Contains("Ü"));
            Assert.AreEqual(true, list.First().UserAgent.Contains("ß"));
        }
    }
}