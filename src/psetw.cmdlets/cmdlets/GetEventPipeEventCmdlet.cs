using System;
using Microsoft.Diagnostics.Tracing;
using System.Management.Automation;
using System.Xml;
using Microsoft.Diagnostics.Tracing.Analysis;
using Microsoft.Diagnostics.Tracing.Etlx;
using System.Management.Automation.Runspaces;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace psetw.cmdlets
{
    [Cmdlet(VerbsCommon.Get, "EventPipeEvent")]
    public class GetEventPipeEventCmdlet : PSCmdlet
    {
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public string Path;

        private EventPipeEventSource? _dataSource;

        private void _dataProcessor(TraceEvent data)
        {
            PSObject ret = ProcessCommonProperties(data);
            ProcessPayload(data, ret);

            WriteObject(ret);
        }

        private void ProcessPayload(TraceEvent data, PSObject ret)
        {
            string[] array = data.PayloadNames;
            for (int i = 0; i < array.Length; i++)
            {
                var n = string.IsNullOrEmpty(array[i]) ? "Arguments" : array[i];
                var v = data.PayloadValue(i);

                if (v is Array array3 && array3.Rank == 1)
                {
                    var bucket = new Dictionary<string, object>();
                    

                    foreach (object item in array3)
                    {
                        if (item is IDictionary<string, object> dictionary && dictionary.Count == 2 && dictionary.ContainsKey("Key") && dictionary.ContainsKey("Value"))
                        {
                            var keyName = dictionary["Key"].ToString();
                            CountKeyNames(keyName);

                            if (GetKeyNameCount(keyName) == 0)
                            {
                                bucket.Add(keyName, dictionary["Value"]);
                            }
                            else
                            {
                                bucket.Add(keyName + "." + GetKeyNameCount(keyName), dictionary["Value"]);
                            }

                        }
                        else
                        {
                            ret.Properties.Add(new PSNoteProperty(n, data.PayloadString(i)));
                        }
                    }
                    if (bucket.Count > 0)
                        ret.Properties.Add(new PSNoteProperty(n, bucket));
                }
                else
                {
                    ret.Properties.Add(new PSNoteProperty(n, data.PayloadString(i)));
                }
            }
        }

        //we count properties to handle a case when somehow we get properties with the same name
        private Dictionary<string, int> _keyCounts = new Dictionary<string, int>(); 
        private void CountKeyNames(string keyName)
        {
            // update properties count
            if (_keyCounts.ContainsKey(keyName))
            {
                _keyCounts[keyName] += 1;
            }
            else
            {
                _keyCounts[keyName] = 0;
            }
        }

        private int GetKeyNameCount(string keyName)
        {
            return _keyCounts[keyName];
        }

        private PSObject ProcessCommonProperties(TraceEvent data)
        {
            var ret = new PSObject();

            ret.Properties.Add(new PSNoteProperty("ActivityID", data.ActivityID));
            ret.Properties.Add(new PSNoteProperty("EventName", data.EventName));
            ret.Properties.Add(new PSNoteProperty("Opcode", data.Opcode));
            ret.Properties.Add(new PSNoteProperty("OpcodeName", data.OpcodeName));
            ret.Properties.Add(new PSNoteProperty("ProcessID", data.ProcessID));
            ret.Properties.Add(new PSNoteProperty("ProcessName", data.ProcessName));
            ret.Properties.Add(new PSNoteProperty("ProviderName", data.ProviderName));
            ret.Properties.Add(new PSNoteProperty("TaskName", data.TaskName));
            ret.Properties.Add(new PSNoteProperty("TaskGuid", data.TaskGuid));
            ret.Properties.Add(new PSNoteProperty("TimeStamp", data.TimeStamp));
            ret.Properties.Add(new PSNoteProperty("TimeStampRelativeMSec", data.TimeStampRelativeMSec));
            ret.Properties.Add(new PSNoteProperty("ThreadID", data.ThreadID));
            return ret;
        }

        protected override void BeginProcessing()
        {
            try
            {
                _dataSource = new EventPipeEventSource(Path);
                _dataSource.Dynamic.All += _dataProcessor;
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InitializationError", ErrorCategory.OpenError, Path));
            }
        }

        protected override void EndProcessing()
        {
            try
            {
                _dataSource?.Process();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ProcessingEndError", ErrorCategory.CloseError, _dataSource));
                _dataSource?.Dispose();
            }
        }
    }
}