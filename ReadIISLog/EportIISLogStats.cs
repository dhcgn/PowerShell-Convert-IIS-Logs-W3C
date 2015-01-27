using System.Linq;
using System.Management.Automation;

namespace ConvertFromIISLogFile
{
    [Cmdlet(VerbsData.Export, "IISLogStats")]
    public class EportIISLogStats : Cmdlet
    {
        private bool stopRequest;

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "IIS Log"
            )]
        [ValidateNotNull]
        public LogEntry[] LogEntries { get; set; }

        #region Resolution

        public const string ResolutionMinute = "Minute";
        public const string ResolutionHour = "Hour";
        public const string ResolutionDay = "Day";
        public const string ResolutionWeek = "Week";

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "IIS Log"
            )]
        [ValidateNotNull]
        [ValidateSet(ResolutionMinute, ResolutionHour, ResolutionDay, ResolutionWeek)]
        public string Resolution { get; set; }

        #endregion

        protected override void ProcessRecord()
        {
            Statistik.CreateStatistik(this.LogEntries.ToList(), this.Resolution, this.WriteOutputCallback, this.WriteVerboseCallback, () => this.stopRequest);
        }

        private void WriteOutputCallback(string entry)
        {
            if (this.stopRequest) return;

            try
            {
                this.WriteObject(entry);
            }
            catch (PipelineStoppedException)
            {
                this.stopRequest = true;
            }
        }
        private void WriteVerboseCallback(string logEntry)
        {
            if (this.stopRequest) return;

            try
            {
                this.WriteVerbose(logEntry);
            }
            catch (PipelineStoppedException)
            {
                this.stopRequest = true;
            }
        }
    }
}