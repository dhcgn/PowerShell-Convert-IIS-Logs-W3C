using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace ConvertFromIISLogFile
{
    [Cmdlet(VerbsData.Export, "IISLogStats")]
    public class EportIISLogStats : Cmdlet
    {
        private bool stopRequest;

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = false,
            Position = 0,
            HelpMessage = "IIS Log"
        )]
        [ValidateNotNull]
        public LogEntry[] LogEntries { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = false,
            Position = 0,
            HelpMessage = "IIS Log"
        )]
        [ValidateNotNull]
        public FileInfo[] InputFiles { get; set; }

        #region Resolution

        #region NoProgress

        [Parameter(
            Mandatory = false,
            Position = 1,
            HelpMessage = "Don't display current line for more perfomance."
        )]
        [ValidateNotNull]
        public SwitchParameter NoProgress { get; set; }

        #endregion

        public const string ResolutionSecond = "Second";
        public const string ResolutionMinute = "Minute";
        public const string ResolutionQuarterHour = "QuarterHour";
        public const string ResolutionHour = "Hour";
        public const string ResolutionDay = "Day";
        public const string ResolutionWeek = "Week";

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = false,
            Position = 1,
            HelpMessage = "Resolution from second to week"
        )]
        [ValidateSet(ResolutionSecond, ResolutionMinute, ResolutionHour, ResolutionQuarterHour, ResolutionDay, ResolutionWeek)]
        public string Resolution { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = false,
            Position = 1,
            HelpMessage = "The Resolution in seconds, or use \"Resolution\" with an predefinied value"
        )]
        [ValidateRange(1, long.MaxValue)]
        public long ResolutionInSeconds { get; set; }

        public const string GroupByMethod = "Method";
        public const string GroupByNone = "None";

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = false,
            Position = 2,
            HelpMessage = "GroupBy"
        )]
        [ValidateSet(GroupByNone, GroupByMethod)]
        public string GroupBy { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            ErrorRecord error = null;
            if (this.InputFiles == null && this.LogEntries == null)
            {
                error = new ErrorRecord(new NullReferenceException("InputFiles and LogEntries are null."), "0002", ErrorCategory.InvalidData, this);
            }

            if (String.IsNullOrEmpty(this.Resolution) && this.ResolutionInSeconds == 0)
            {
                error = new ErrorRecord(new NullReferenceException("Parameter Resolution or ResolutionInSeconds must be used."), "0002", ErrorCategory.InvalidData, this);
            }

            if (!String.IsNullOrEmpty(this.Resolution) && this.ResolutionInSeconds != 0)
            {
                error = new ErrorRecord(new NullReferenceException("Parameter Resolution or ResolutionInSeconds must be used, not both."), "0002", ErrorCategory.InvalidData, this);
            }

            if (error != null)
                this.ThrowTerminatingError(error);
        }

        protected override void ProcessRecord()
        {
            var settings = new Settings
            {
                GroupByMethod = this.GroupBy == GroupByMethod,
                ResolutionDuration = this.ResolutionInSeconds == 0 ? StatsGenerator.GetTimeSpanResolution(this.Resolution) : TimeSpan.FromSeconds(this.ResolutionInSeconds)
            };

            if (this.InputFiles == null && this.LogEntries != null)
            {
                StatsGenerator.Create(this.LogEntries.ToList(), this.WriteOutputCallback, this.WriteVerboseCallback, () => this.stopRequest, settings);
            }

            var supressCsvHeader = false;
            if (this.InputFiles != null && this.LogEntries == null)
            {
                this.WriteVerbose(string.Format("{0} files will be read.", this.InputFiles.Count()));

                foreach (var fileInfoGroup in this.InputFiles.GroupBy(x => x.Name).OrderByDescending(x => x.Key))
                {
                    List<LogEntry> logEntries = new List<LogEntry>();

                    this.WriteVerbose(string.Format("Reading {0} files with filename {1}", fileInfoGroup.Count(), fileInfoGroup.Key));
                    LogReader.ProcessLogFiles(fileInfoGroup.ToArray(), entry => logEntries.Add(entry), this.WriteProgress, this.ErrorHandling, this.NoProgress.IsPresent, () => { return this.stopRequest; });

                    this.WriteVerbose(String.Format("Create stats."));
                    StatsGenerator.Create(logEntries, this.WriteOutputCallback, this.WriteVerboseCallback, () => this.stopRequest, settings, supressCsvHeader);

                    supressCsvHeader = true;
                }
            }
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
                    progressRecord.PercentComplete = (int) ((double) index / total * 100);
                }
            }

            this.WriteProgress(progressRecord);
        }

        private void ErrorHandling(Exception obj)
        {
            if (this.stopRequest) return;
            this.WriteError(new ErrorRecord(obj, "0001", ErrorCategory.InvalidData, this));
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