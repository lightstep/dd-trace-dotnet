using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Datadog.Trace.Configuration;
using Datadog.Trace.Logging;

namespace Datadog.Trace
{
    internal class TracerSubProcessManager
    {
        private static Task _traceAgentMonitor;
        private static Task _dogStatsDMonitor;

        public static void StartStandaloneAgentProcessesWhenConfigured()
        {
            try
            {
                var traceAgentPath = Environment.GetEnvironmentVariable(ConfigurationKeys.TraceAgentPath);

                if (!string.IsNullOrWhiteSpace(traceAgentPath))
                {
                    var traceProcessArgs = Environment.GetEnvironmentVariable(ConfigurationKeys.TraceAgentArgs);
                    _traceAgentMonitor = StartProcessWithKeepAlive(traceAgentPath, traceProcessArgs);
                }
                else
                {
                    DatadogLogging.RegisterStartupLog(log => log.Debug("There is no path configured for {0}.", ConfigurationKeys.TraceAgentPath));
                }

                var dogStatsDPath = Environment.GetEnvironmentVariable(ConfigurationKeys.DogStatsDPath);

                if (!string.IsNullOrWhiteSpace(dogStatsDPath))
                {
                    var dogStatsDArgs = Environment.GetEnvironmentVariable(ConfigurationKeys.DogStatsDArgs);
                    _dogStatsDMonitor = StartProcessWithKeepAlive(dogStatsDPath, dogStatsDArgs);
                }
                else
                {
                    DatadogLogging.RegisterStartupLog(log => log.Debug("There is no path configured for {0}.", ConfigurationKeys.DogStatsDPath));
                }
            }
            catch (Exception ex)
            {
                DatadogLogging.RegisterStartupLog(log => log.Error(ex, "Error when attempting to start standalone agent processes."));
            }
        }

        private static bool ProgramIsRunning(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return false;
            }

            var fileName = Path.GetFileNameWithoutExtension(fullPath);
            var processesByName = Process.GetProcessesByName(fileName);

            if (processesByName?.Length > 0)
            {
                // We enforce a unique enough naming within contexts where we would use sub-processes
                return true;
            }

            return false;
        }

        private static Task StartProcessWithKeepAlive(string path, string args)
        {
            DatadogLogging.RegisterStartupLog(log => log.Debug("Starting keep alive for {0}.", path));

            return Task.Run(
                () =>
                {
                    try
                    {
                        var circuitBreakerMax = 3;
                        var sequentialFailures = 0;

                        while (true)
                        {
                            try
                            {
                                if (ProgramIsRunning(path))
                                {
                                    DatadogLogging.RegisterStartupLog(log => log.Debug("{0} is already running.", path));
                                    continue;
                                }

                                var startInfo = new ProcessStartInfo { FileName = path };

                                if (!string.IsNullOrWhiteSpace(args))
                                {
                                    startInfo.Arguments = args;
                                }

                                DatadogLogging.RegisterStartupLog(log => log.Debug("Starting {0}.", path));

                                var process = Process.Start(startInfo);

                                Thread.Sleep(150);

                                if (process == null || process.HasExited)
                                {
                                    DatadogLogging.RegisterStartupLog(log => log.Error("{0} has failed to start.", path));
                                    sequentialFailures++;
                                }
                                else
                                {
                                    DatadogLogging.RegisterStartupLog(log => log.Debug("Successfully started {0}.", path));
                                    sequentialFailures = 0;
                                }
                            }
                            catch (Exception ex)
                            {
                                DatadogLogging.RegisterStartupLog(log => log.Error(ex, "Exception when trying to start an instance of {0}.", path));
                                sequentialFailures++;
                            }
                            finally
                            {
                                // Delay for a reasonable amount of time before we check to see if the process is alive again.
                                Thread.Sleep(20_000);
                            }

                            if (sequentialFailures >= circuitBreakerMax)
                            {
                                DatadogLogging.RegisterStartupLog(log => log.Error("Circuit breaker triggered for {0}. Max failed retries reached ({1}).", path, sequentialFailures));
                                break;
                            }
                        }
                    }
                    finally
                    {
                        DatadogLogging.RegisterStartupLog(log => log.Debug("Keep alive is dropping for {0}.", path));
                    }
                });
        }
    }
}
