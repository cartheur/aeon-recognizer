using System.Diagnostics;

namespace Aeon.Recognizer
{
    public static class Command
    {
        /// <summary>
        /// Executes a shell command synchronously.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public static void ExecuteCommandSync(object command)
        {
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process pocketSphinxProcess = new Process
                {
                    StartInfo = procStartInfo
                };
                pocketSphinxProcess.Start();

                string result = pocketSphinxProcess.StandardOutput.ReadToEnd();

                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.ProcessCommand);
            }
        }

        /// <summary>
        /// Execute the command Asynchronously.
        /// </summary>
        /// <param name="command">string command.</param>
        public static void ExecuteCommandAsync(string command)
        {
            try
            {
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSync))
                {
                    IsBackground = true,
                    Priority = ThreadPriority.AboveNormal
                };
                objThread.Start(command);
            }
            catch (ThreadStartException ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.ProcessCommand);
            }
            catch (ThreadAbortException ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.ProcessCommand);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.ProcessCommand);
            }
        }

    }
}
