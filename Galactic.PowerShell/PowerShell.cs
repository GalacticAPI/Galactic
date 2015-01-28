using Galactic.EventLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using PS = System.Management.Automation.PowerShell;

namespace Galactic.PowerShell
{
    /// <summary>
    /// A collection of helper methods for using PowerShell.
    /// </summary>
    static public class PowerShell
    {
        // ---------- CONSTANTS ----------

        /// <summary>
        /// The default application name used when making a PowerShell connection.
        /// </summary>
        public const string DEFAULT_APP_NAME = "/wsman";

        /// <summary>
        /// The default length of time in seconds that a PowerShell operation has to complete before timing out.
        /// </summary>
        public const int DEFAULT_OPERATION_TIMEOUT_IN_SEC = 300;

        /// <summary>
        /// The default length of time in seconds that a PowerShell connection has to open a connection before timing out.
        /// </summary>
        public const int DEFAULT_OPEN_TIMEOUT_IN_SEC = 60;

        /// <summary>
        /// The default port number used by PowerShell connections.
        /// </summary>
        public const int DEFAULT_PORT_NUMBER = 5985;

        /// <summary>
        /// The default port number used by SSL based PowerShell connections.
        /// </summary>
        public const int DEFAULT_PORT_NUMBER_SSL = 5986;

        /// <summary>
        /// The default shell URI used by PowerShell connections.
        /// </summary>
        public const string DEFAULT_SHELL_URI = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";

        /// <summary>
        /// The default minimum number of runspaces initialized when a pool is created.
        /// </summary>
        public const int DEFAULT_MIN_RUNSPACES_IN_POOL = 1;

        /// <summary>
        /// The default maximum number of runspaces a pool can grow to during it's lifetime.
        /// </summary>
        public const int DEFAULT_MAX_RUNSPACES_IN_POOL = 4;

        /// <summary>
        /// Defines the name of the ProcessResults callback delegate variable in the script state dictionary.
        /// </summary>
        private const string PROCESS_RESULTS_CALLBACK = "callback";

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        // ---------- CONSTRUCTORS ----------

        // ---------- DELEGATES ----------

        /// <summary>
        /// Processes the results of a script invocation.
        /// </summary>
        /// <param name="stateValues">A collection of named state values that should are passed through the script invocation.</param>
        /// <param name="results">A collection of the results returned from the script's execution.</param>
        public delegate void ProcessResults(PSDataCollection<PSObject> results, Dictionary<string, object> stateValues);

        // ---------- NESTED CLASSES ----------

        /// <summary>
        /// Contains program state that can be passed through an asyncronous invocation of a PowerShell script.
        /// </summary>
        private class PowerShellScriptState
        {
            // ---------- PROPERTIES ----------

            /// <summary>
            /// The PowerShell script object that originated the invocation.
            /// </summary>
            public PS Script { get; private set; }

            /// <summary>
            /// A dictionary of variables that are attached to the state before script invocation.
            /// </summary>
            public Dictionary<string, Object> StateVariables { get; private set; }

            // ---------- CONSTRUCTOR ----------

            /// <summary>
            /// Creates a new PowerShell script state object, with the supplied originating script.
            /// </summary>
            /// <param name="script">The PowerShell object that originated the invocation.</param>
            public PowerShellScriptState(PS script)
            {
                // Set the originating script object.
                if (script != null)
                {
                    Script = script;
                }
                else
                {
                    // The script object was not supplied, throw an exception.
                    throw new ArgumentNullException("script");
                }

                // Initialize the stateVariables property.
                StateVariables = new Dictionary<string, object>();
            }
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Logs an exception to the event log.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        /// <param name="log">The event log to log the execption to.</param>
        /// <returns>True if the exception was logged successfully. False otherwise.</returns>
        static private bool LogException(Exception e, EventLog.EventLog log)
        {
            if (log != null)
            {
                log.Log(new Event(typeof(PowerShell).FullName, DateTime.Now, Event.SeverityLevels.Error, e.GetType().FullName,
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
        /// Creates an object with the information necessary to create a connection for a Runspace or RunspacePool.
        /// Note: computerName and connectionURI are mutually exclusive. If both are provided, the computerName parameter will be used.
        /// computerName and connectionURI need only be supplied for remote connections. If neither are provided a configuration for a
        /// local connection using all defaults will be created.
        /// </summary>
        /// <param name="computerName">[Optional] The hostname or FQDN of the computer to connect to. Defaults to null for a local connection.</param>
        /// <param name="connectionURI">[Optional] The URI to the PowerShell service to connect with. Defaults to DEFAULT_SHELL_URI.</param>
        /// <param name="shellURI">[Optional] The URI of the shell that is launched once the connection is made.</param>
        /// <param name="credential">[Optional] The PowerShell credentials to use when making the connection. Defaults to null which does not provide implicit credentials for the connection.</param>
        /// <param name="authMechanism">[Optional] The authentication mechanism employed when creating the connection. Defaults to null which uses the PowerShell default mechanism.</param>
        /// <param name="appName">[Optional] The application name used when making the PowerShell connection. Defaults to DEFAULT_APP_NAME which is the default PowerShell application name used for connections.</param>
        /// <param name="operationTimeoutInSec">[Optional] The time in seconds that a PowerShell connection will wait for an operation to complete before timing out. Defaults to DEFAULT_OPERATION_TIMEOUT_IN_SEC.</param>
        /// <param name="openTimeoutInSec">[Optional] The time in seconds that a PowerShell connection will wait for a connection to be established before timing out. Defaults to DEFAULT_OPEN_TIMEOUT_IN_SEC.</param>
        /// <param name="useSSL">[Optional] Whether to use SSL when creating the connection. Defaults to false.</param>
        /// <param name="port">[Optional] The port number to use when making the connection. Defaults to the PowerShell default port number appropriate to the SSL state of the connection.</param>
        /// <returns></returns>
        static public RunspaceConnectionInfo CreateRunspaceConnectionInfo(string computerName = null, string connectionURI = null, string shellURI = DEFAULT_SHELL_URI, PSCredential credential = null,
            AuthenticationMechanism authMechanism = AuthenticationMechanism.Default, string appName = DEFAULT_APP_NAME, int operationTimeoutInSec = DEFAULT_OPERATION_TIMEOUT_IN_SEC, int openTimeoutInSec = DEFAULT_OPEN_TIMEOUT_IN_SEC,
            bool useSSL = false, int port = 0)
        {
            WSManConnectionInfo info = null;

            // If invalid timeouts are supplied reset them to the defaults.
            if (operationTimeoutInSec <= 0)
            {
                operationTimeoutInSec = DEFAULT_OPERATION_TIMEOUT_IN_SEC;
            }
            if (openTimeoutInSec <= 0)
            {
                openTimeoutInSec = DEFAULT_OPEN_TIMEOUT_IN_SEC;
            }

            // Check whether we should use a default port.
            // Reset the connection to use the default port if invalid port numbers are supplied.
            if (port == 0 || port < 0)
            {
                // We need to initialize port with a default PowerShell port appropriate to it's SSL status.
                if (useSSL)
                {
                    port = DEFAULT_PORT_NUMBER_SSL;
                }
                else
                {
                    port = DEFAULT_PORT_NUMBER;
                }
            }

            // Determine whether to initialize the connection info object via computer name or connection URI.
            if (!string.IsNullOrWhiteSpace(computerName))
            {
                // Use the computer name to initialize the connection info object.
                info = new WSManConnectionInfo(useSSL, computerName, port, appName, shellURI, credential, openTimeoutInSec);
            }
            else if (!string.IsNullOrWhiteSpace(connectionURI))
            {
                // Use the connection URI to initialize the connection info object.
                info = new WSManConnectionInfo(new Uri(connectionURI), shellURI, credential);
            }
            else
            {
                // Create a connection information object using all defaults.
                info = new WSManConnectionInfo
                {
                    // Set the timeout to use when opening the connection.
                    OpenTimeout = openTimeoutInSec
                };

            }

            // Configure the object to use the supplied authentication mechanism.
            info.AuthenticationMechanism = authMechanism;

            // Set the operation timeout.
            info.OperationTimeout = operationTimeoutInSec;

            // Return the configuration connection object.
            return info;
        }

        /// <summary>
        /// Runs a PowerShell command/script asynchronously.
        /// </summary>
        /// <param name="commandText">The text of the command to run.</param>
        /// <param name="pool">An open PowerShell Runspace pool this script will use to invoke its pipeline.</param>
        /// <param name="callback">The callback function used to process the results of the asynchronous run.</param>
        /// <param name="log">[Optional] The event log to log execptions to. May be null for no logging.</param>
        /// <param name="input">[Optional] A collection of strings representing command-line input sent to the script during execution.</param>
        /// <param name="stateValues">[Optional] A collection of named state values that should be passed through the invocation to the callback function.</param>
        /// <param name="parameterList">An array of key/value objects defining additional parameters to supply to the PowerShell script.</param>
        /// <returns>An WaitHandle object that can be used to determine when the scrip has completed execution. Null if an error occurred while processing the command / script.</returns>
        static public WaitHandle RunAsynchronously(string commandText, ref RunspacePool pool, ProcessResults callback, EventLog.EventLog log = null, PSDataCollection<string> input = null, Dictionary<String, Object> stateValues = null, params KeyValuePair<String, Object>[] parameterList)
        {
            try
            {
                // Create the script object.
                PS script = PS.Create();

                // Use the runspace pool supplied or create a new one if not supplied.
                if (pool == null)
                {
                    pool = RunspaceFactory.CreateRunspacePool(DEFAULT_MIN_RUNSPACES_IN_POOL, DEFAULT_MAX_RUNSPACES_IN_POOL, CreateRunspaceConnectionInfo());
                }

                // Verify that the pool is open, otherwise open it.
                if (pool.RunspacePoolStateInfo.State != RunspacePoolState.Opened)
                {
                    pool.Open();
                }

                // Add the runspace pool to the script object.
                script.RunspacePool = pool;

                // Create the PowerShell command object.
                Command command = new Command(commandText, true);

                // Add parameters to the command.
                if (parameterList != null)
                {
                    foreach (KeyValuePair<string, object> param in parameterList)
                    {
                        command.Parameters.Add(new CommandParameter(param.Key, param.Value));
                    }
                }

                // Add the command to the script object.
                script.Commands.AddCommand(command);

                // Initialize the script input object if nothing was supplied.
                if (input == null)
                {
                    input = new PSDataCollection<string>();
                }

                // Initialize the state object to maintain data across the invocation.
                PowerShellScriptState state = new PowerShellScriptState(script);
                // Add the callback function used to process the results of the script invocation.
                state.StateVariables.Add(PROCESS_RESULTS_CALLBACK, callback);
                // Add any state values passed into the method.
                if (stateValues != null)
                {
                    foreach (string key in stateValues.Keys)
                    {
                        state.StateVariables.Add(key, stateValues[key]);
                    }
                }

                // Invoke the command asyncronously.
                return (script.BeginInvoke(input, new PSInvocationSettings(), ProcessAsynchronousResults, state)).AsyncWaitHandle;
            }
            catch (Exception e)
            {
                LogException(e, log);
                return null;
            }

        }

        /// <summary>
        /// Processes the results from an asynchronous script call. Packages the results for forwarding to a ProcessResults() delegate callback.
        /// </summary>
        /// <param name="results">The results of the script's invocation.</param>
        static private void ProcessAsynchronousResults(IAsyncResult results)
        {
            // Get the variables maintained in the result's state object.
            PowerShellScriptState state = (PowerShellScriptState)results.AsyncState;

            // Verify that the state was retrieved.
            if (state != null)
            {
                // End the script invocation and retrieve the results of the script run.
                PSDataCollection<PSObject> scriptResults = state.Script.EndInvoke(results);

                // Create the list of state values that will be returned to the callback function.
                // Filter the callback function object from the list.
                Dictionary<String, Object> stateValues = new Dictionary<string, object>();
                foreach (string valueName in state.StateVariables.Keys)
                {
                    if (valueName != PROCESS_RESULTS_CALLBACK)
                    {
                        stateValues.Add(valueName, state.StateVariables[valueName]);
                    }
                }

                // Forward the script results returned to the callback function contained in the invocation state.
                ((ProcessResults)state.StateVariables[PROCESS_RESULTS_CALLBACK])(scriptResults, stateValues);
            }
            else
            {
                // Throw an exception.
                throw new NullReferenceException("The PowerShell script execution returned a null state.");
            }
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
        static public Collection<PSObject> RunSynchronously(string commandText, ref Runspace runspace, EventLog.EventLog log = null, PSDataCollection<string> input = null, params KeyValuePair<String, Object>[] parameterList)
        {
            try
            {
                // Create the script object.
                PS script = PS.Create();

                // Use the runspace supplied or create a new one if not supplied.
                if (runspace == null)
                {
                    runspace = RunspaceFactory.CreateRunspace(CreateRunspaceConnectionInfo());
                }

                // Verify that the runspace is open, otherwise open it.
                if (runspace.RunspaceStateInfo.State != RunspaceState.Opened)
                {
                    runspace.Open();
                }

                // Add the runspace to the script object.
                script.Runspace = runspace;

                // Create the PowerShell command object.
                Command command = new Command(commandText, true);

                // Add parameters to the command.
                if (parameterList != null)
                {
                    foreach (KeyValuePair<string, object> param in parameterList)
                    {
                        command.Parameters.Add(new CommandParameter(param.Key, param.Value));
                    }
                }

                // Add the command to the script object.
                script.Commands.AddCommand(command);

                // Initialize the script input object if nothing was supplied.
                if (input == null)
                {
                    input = new PSDataCollection<string>();
                }

                // Invoke the command syncronously.
                return (script.Invoke(input));
            }
            catch (Exception e)
            {
                LogException(e, log);
                return null;
            }
        }
    }
}
