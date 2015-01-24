using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace ConvertFromIISLogFile
{
    [Cmdlet(VerbsData.ConvertFrom, "IISLogFile")]
    public class ConvertFromIISLogFile : Cmdlet
    {
        #region InputFiles

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "IIS LogFiles"
            )]
        [ValidateNotNull]
        public FileInfo[] InputFiles { get; set; }

        #endregion

        #region NoProgress

        [Parameter(
            Mandatory = false,
            Position = 1,
            HelpMessage = "Don't display current line for more perfomance."
            )]
        [ValidateNotNull]
        public SwitchParameter NoProgress { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            // todo get the encoding shit working
            //GetEncoding.CPINFOEX info;
            //if (GetEncoding.GetCPInfoEx(65001, 0, out info) != 0)
            //{
            //    var oemEncoding = Encoding.GetEncoding(info.CodePage);

            //    var currentProcess = Process.GetCurrentProcess();
            //    currentProcess.StartInfo.StandardOutputEncoding = oemEncoding;

            //    this.WriteVerbose(oemEncoding.ToString());
            //}
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            LogReader.ProcessLogFiles(this.InputFiles, this.WriteOutput, this.WriteProgress, this.ErrorHandling, this.NoProgress.IsPresent);
        }

        private void ErrorHandling(Exception obj)
        {
            this.WriteError(new ErrorRecord(obj, "0001", ErrorCategory.InvalidData, this));
        }

        private void WriteProgress(int index, int total, string fullname)
        {
            ProgressRecord progressRecord;

            if (this.NoProgress.IsPresent)
            {
                progressRecord = new ProgressRecord(0, String.Format("Read file: {0}", fullname), String.Format("No line count because of switch NoProgress"));
            }
            else
            {
                progressRecord = new ProgressRecord(0, String.Format("Read file: {0}", fullname), String.Format("Read line {0} of {1}", index, total));
                if (index > 0)
                {
                    progressRecord.PercentComplete = (int) ((double) index/total*100);
                }
            }

            this.WriteProgress(progressRecord);
        }

        private void WriteOutput(LogEntry logEntry)
        {
            this.WriteObject(logEntry);
        }
    }
}