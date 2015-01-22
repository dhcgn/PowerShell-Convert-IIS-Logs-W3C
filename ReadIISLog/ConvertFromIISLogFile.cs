using System;
using System.IO;
using System.Management.Automation;

namespace ConvertFromIISLogFile
{
    [Cmdlet(VerbsData.ConvertFrom, "IISLogFile")]
    public class ConvertFromIISLogFile : Cmdlet
    {
        #region InputFile

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "IIS LogFile"
            )]
        [ValidateNotNullOrEmpty]
        public FileInfo[] InputFile { get; set; }

        #endregion

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            LogReader.ProcessLogFile(this.InputFile, this.WriteOutput, this.WriteProgress, this.ErrorHandling);
        }

        private void ErrorHandling(Exception obj)
        {
            this.WriteError(new ErrorRecord(obj,"0001", ErrorCategory.InvalidData, this));
        }

        private void WriteProgress(int index, int total, string fullname)
        {
            var progressRecord = new ProgressRecord(0, String.Format("Read file: {0}", fullname), String.Format("Read line {0} of {1}", index, total));
            if (index > 0)
            {
                progressRecord.PercentComplete = (int) ((double) index/total*100);
            }

            this.WriteProgress(progressRecord);
        }

        private void WriteOutput(LogEntry logEntry)
        {
            this.WriteObject(logEntry);
        }
    }
}