using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace Galactic.PowerShell
{
    /// <summary>
    /// An interface for classes that implement PowerShell scripting functionality.
    /// </summary>
    public interface IPowerShellScript
    {
        // ---------- PROPERTIES ----------

        /// <summary>
        /// The results of the Powershell Script call.
        /// </summary>
        Collection<PSObject> Results { get; set; }

        /// <summary>
        /// A specific Runspace pool used to invoke pipelines for this PowerShell Script. (Optional)
        /// </summary>
        RunspacePool RunspacePool { get; set; }

        // ---------- METHODS ----------

        /// <summary>
        /// Runs a PowerShell script/command asynchronously.
        /// </summary>
        /// <param name="commandText">The text of the command to run.</param>
        /// <param name="callback">The callback function used to process the results of the asynchronous run.</param>
        /// <param name="stateValues">[Optional] A collection of named state values that should be passed through the invocation to the callback function.</param>
        /// <param name="input">[Optional] A collection of strings representing command-line input sent to the command/script during execution.</param>
        /// <param name="parameterList">A dictionary of additional parameters to supply to the PowerShell script/command.</param>
        WaitHandle RunAsynchronously(string commandText, PowerShell.ProcessResults callback, Dictionary<String, Object> stateValues = null, PSDataCollection<string> input = null, params KeyValuePair<String, Object>[] parameterList);

        /// <summary>
        /// Runs a PowerShell command/script synchronously.
        /// </summary>
        /// <param name="commandText">The text of the command to run.</param>
        /// <param name="runspace">An open PowerShell Runspace this script will use to invoke its pipeline.</param>
        /// <param name="log">[Optional] The event log to log execptions to. May be null for no logging.</param>
        /// <param name="input">[Optional] A collection of strings representing command-line input sent to the script during execution.</param>
        /// <param name="parameterList">An array of key/value objects defining additional parameters to supply to the PowerShell script.</param>
        /// <returns>A collection of PSObjects that are the result of the script/command run. Null if an error occurred while processing the command / script.</returns>
        Collection<PSObject> RunSynchronously(string commandText, ref Runspace runspace, EventLog.EventLog log = null, PSDataCollection<string> input = null, params KeyValuePair<String, Object>[] parameterList);

        /// <summary>
        /// Verifies that one of the results returned contains a property that matches the supplied value. 
        /// </summary>
        /// <param name="propertyName">The name of the property in the result object to verify.</param>
        /// <param name="value">The value to match against.</param>
        /// <returns>True if the property value matches the supplied value. False otherwise.</returns>
        bool VerifyResultProperty(string propertyName, string value);
    }
}
