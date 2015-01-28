using Galactic.EventLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace Galactic.PowerShell
{
    /// <summary>
    /// A class that provides the ability to run and verify the results from
    /// PowerShell scripts. Provides the ability to run the scripts both
    /// synchronously and asynchronously.
    /// </summary>
    public class PowerShellScript : IPowerShellScript, IDisposable, IExceptionLogger
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        /// <summary>
        /// The results of the Powershell Script call.
        /// </summary>
        private Collection<PSObject> results = new Collection<PSObject>();

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The event log that will receive events from this logger.
        /// </summary>
        public EventLog.EventLog Log { get; set; }

        /// <summary>
        /// The results of the Powershell Script call.
        /// </summary>
        public Collection<PSObject> Results
        {
            get
            {
                return results;
            }
            set
            {
                if (value != null)
                {
                    results = value;
                }
            }
        }

        /// <summary>
        /// A specific Runspace pool used to invoke pipelines for this PowerShell Script. (Optional)
        /// </summary>
        public RunspacePool RunspacePool { get; set; }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Inititalizes the PowerShellScript environment.
        /// </summary>
        /// <param name="log">An event log to log exceptions to. May be null if no exception logging is desired.</param>
        /// <param name="pool">A runspace pool for the scripting environment to use. May be null if a new Runspace pool is desired.</param>
        public PowerShellScript(EventLog.EventLog log, RunspacePool pool)
        {
            RunspacePool = null;
            Log = log;
            if (pool != null)
            {
                RunspacePool = pool;

                // Open the Runspace Pool so it's ready for use.
                if (RunspacePool.RunspacePoolStateInfo.State != RunspacePoolState.Opened)
                {
                    RunspacePool.Open();
                }
            }
            else
            {
                InitializeRunspacePool();
            }
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Disposes of system resources opened by the script.
        /// </summary>
        public virtual void Dispose()
        {
            // Dispose of the Script's RunspacePool.
            if (RunspacePool != null)
            {
                RunspacePool.Dispose();
            }
            RunspacePool = null;
        }

        /// <summary>
        /// Creates a new pool of Runspaces for this script to utilize.
        /// </summary>
        public void InitializeRunspacePool()
        {
            RunspacePool = RunspaceFactory.CreateRunspacePool();

            // Open the Runspace Pool so it's ready for use.
            RunspacePool.Open();
        }

        /// <summary>
        /// Logs an exception to the event log.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        /// <returns>True if the exception was logged successfully. False otherwise.</returns>
        public bool LogException(Exception e)
        {
            if (Log != null)
            {
                Log.Log(new Event(typeof(PowerShellScript).FullName, DateTime.Now, Event.SeverityLevels.Error, e.GetType().FullName,
                    "Description:\n" +
                   e.Message + "\n" +
                   "Stack Trace:\n" +
                   e.StackTrace));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Runs a PowerShell script/command asynchronously.
        /// </summary>
        /// <param name="commandText">The text of the command to run.</param>
        /// <param name="callback">The callback function used to process the results of the asynchronous run.</param>
        /// <param name="stateValues">[Optional] A collection of named state values that should be passed through the invocation to the callback function.</param>
        /// <param name="input">[Optional] A collection of strings representing command-line input sent to the command/script during execution.</param>
        /// <param name="parameterList">A dictionary of additional parameters to supply to the PowerShell script/command.</param>
        public WaitHandle RunAsynchronously(string commandText, PowerShell.ProcessResults callback, Dictionary<String, Object> stateValues = null, PSDataCollection<string> input = null, params KeyValuePair<String, Object>[] parameterList)
        {
            // Run the script asynchronously.
            RunspacePool pool = RunspacePool;
            return PowerShell.RunAsynchronously(commandText, ref pool, callback, Log, input, stateValues, parameterList);
        }

        /// <summary>
        /// Runs a PowerShell command/script synchronously.
        /// </summary>
        /// <param name="commandText">The text of the command to run.</param>
        /// <param name="runspace">An open PowerShell Runspace this script will use to invoke its pipeline.</param>
        /// <param name="log">[Optional] The event log to log execptions to. May be null for no logging.</param>
        /// <param name="input">[Optional] A collection of strings representing command-line input sent to the script during execution.</param>
        /// <param name="parameterList">An array of key/value objects defining additional parameters to supply to the PowerShell script.</param>
        /// <returns>A collection of PSObjects that are the result of the script/command run. Null if an error occurred while processing the command / script.</returns>
        public Collection<PSObject> RunSynchronously(string commandText, ref Runspace runspace, EventLog.EventLog log = null, PSDataCollection<string> input = null, params KeyValuePair<String, Object>[] parameterList)
        {
            // Run the script synchronously.
            return PowerShell.RunSynchronously(commandText, ref runspace, Log, input, parameterList);
        }

        /// <summary>
        /// Verifies that one of the results returned contains a property that matches the supplied value. 
        /// </summary>
        /// <param name="propertyName">The name of the property in the result object to verify.</param>
        /// <param name="value">The value to match against.</param>
        /// <returns>True if the property value matches the supplied value. False otherwise.</returns>
        public bool VerifyResultProperty(string propertyName, string value)
        {
            // Check that the propertyName and value specified are not null or empty.
            if (!String.IsNullOrWhiteSpace(propertyName) && !String.IsNullOrWhiteSpace(value))
            {
                // The propertyName and value contain text.

                // Check that the result is not null.
                if (Results[0] != null)
                {
                    foreach (PSObject result in Results)
                    {
                        // Check that the result object has the property specified.
                        if (result.Properties[propertyName] != null)
                        {
                            // The result has the property.
                            string propertyValue = (string)result.Properties[propertyName].Value;

                            // Check that the property value is not null or empty and that it matches the value specified.
                            if (!String.IsNullOrWhiteSpace(propertyValue) && propertyValue == value)
                            {
                                // The property value matches the value specified.
                                return true;
                            }
                        }
                    }
                }
                // None of the results has a property value that matches the specified value.
                return false;
            }
            else
            {
                // The propertyName or value do not contain text.
                return false;
            }
        }
    }
}
