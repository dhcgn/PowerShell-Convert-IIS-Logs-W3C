using System;
using System.IO;
using System.Management.Automation;

namespace ConvertFromIISLogFile
{
    [Cmdlet(VerbsData.ConvertFrom, "KernelLog")]
    public class ConvertFromKernelLogFile : Cmdlet
    {
        private bool stopRequest;

        #region InputFiles

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "Kernel LogFiles"
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

        protected override void ProcessRecord()
        {
            //  Todo use generic?
            LogReader.ProcessKernelLogFiles(this.InputFiles, this.WriteOutput, this.WriteProgress, this.ErrorHandling, this.NoProgress.IsPresent, () => { return this.stopRequest; });
        }

        private void ErrorHandling(Exception obj)
        {
            if (this.stopRequest) return;
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
                    progressRecord.PercentComplete = (int)((double)index / total * 100);
                }
            }

            this.WriteProgress(progressRecord);
        }

        private void WriteOutput(KernelLogEntry logEntry)
        {
            if (this.stopRequest) return;

            try
            {
                this.WriteObject(logEntry);
            }
            catch (PipelineStoppedException)
            {
                this.stopRequest = true;
            }
        }
    }
}