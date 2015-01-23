using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using ConvertFromIISLogFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReadIISLog.Test
{
    [TestClass]
    public class UnitTest
    {
        private string wellFormedLogFile;
        private string badFormedLogFile;
        private string badValuesLogFile;

        [TestInitialize]
        public void Setup()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            this.wellFormedLogFile = this.GetValue(executingAssembly, "u_ex150113.log");
            this.badFormedLogFile = this.GetValue(executingAssembly, "BadSamples.log");
            this.badValuesLogFile = this.GetValue(executingAssembly, "BadValues.log");
        }

        private string GetValue(Assembly executingAssembly, string fileName)
        {
            var name = executingAssembly.GetManifestResourceNames().Single(x => x.EndsWith(fileName));
            var filepath = Path.GetDirectoryName(executingAssembly.Location) + @"\"+ fileName;


            using (Stream stream = executingAssembly.GetManifestResourceStream(name))
            using (var file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(file);
            }

            return filepath;
        }

        [TestMethod]
        public void WellFormedLogFile()
        {
            var list = new List<LogEntry>();

            LogReader.ProcessLogFiles(new[] {new FileInfo(this.wellFormedLogFile)}, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Assert.Fail(exception1.ToString()));

            Assert.AreEqual(40950, list.Count);
        }

        [TestMethod]
        public void BadFormedLogFile()
        {
            var list = new List<LogEntry>();

            LogReader.ProcessLogFiles(new[] {new FileInfo(this.badFormedLogFile)}, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Console.WriteLine(exception1.ToString()));

            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void BadValuesLogFile()
        {
            var list = new List<LogEntry>();

            LogReader.ProcessLogFiles(new[] {new FileInfo(this.badValuesLogFile)}, entry => list.Add(entry), (i, i1, arg3) => { }, exception1 => Console.WriteLine(exception1.ToString()));

            Assert.AreEqual(2, list.Count);
        }
    }
}