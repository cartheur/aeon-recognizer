//
// Those parts of the recognizer belonging to the aeon AGI are the intellectual property of Dr. Christopher A. Tucker. Copyright 2023, all rights reserved. No rights are explicitly granted to persons who have obtained this source code.
//
//#define Linux
#define Windows
using Aeon.Library;
using System.Diagnostics;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Aeon.Recognizer
{
    public partial class MainForm : Form
    {
        // Configuration of the application.
        public static LoaderPaths Configuration;
        public static string PythonLocation { get; set; }
        public static string UserInput { get; set; }
        public static string LastOutput { get; set; }
        // Speech recognition and synthesizer engines.
        public static bool SapiWindowsUsed { get; set; }
        public static bool SpeechSynthesizerUsed { get; set; }
        // Speech recognizer and synthesizer.
        SpeechRecognitionEngine Recognizer { get; set; }
        GrammarBuilder GrammarBuilder { get; set; }
        SpeechSynthesizer SpeechSynth { get; set; }
        PromptBuilder PromptBuilder { get; set; }
        // Pocketsphinx voice recognition system
        public static bool SphinxUsed { get; set; }
        public static bool PocketSphinxPythonUsed { get; set; }
        public static Process PocketSphinxProcess { get; set; }
        public static string PocketSphinxProcessOutput { get; set; }
        public static int PocketSphinxProcessExitCode { get; set; }
        public static string PocketsphinxFileDrop { get; set; }
        public static StreamReader PythonProcessStream { get; set; }
#if Linux
        public static readonly string SphinxStartup = @"pocketsphinx_continuous -hmm /usr/share/pocketsphinx/model/hmm/en_US/en-us -dict /usr/share/pocketsphinx/model/lm/en_US/cmudict-en-us.dict -lm /usr/share/pocketsphinx/model/lm/en_US/en-us.lm.bin -inmic yes -backtrace yes -logfn /dev/null";
#endif
#if Windows
        public static readonly string SphinxStartup = @"pocketsphinx_continuous -hmm /usr/share/pocketsphinx/model/hmm/en_US/en-us -dict /usr/share/pocketsphinx/model/lm/en_US/cmudict-en-us.dict -lm /usr/share/pocketsphinx/model/lm/en_US/en-us.lm.bin -inmic yes -backtrace yes -logfn /dev/null";
#endif
        // For RabbitMQ messaging on pocketsphinx output.
        public static ConnectionFactory Factory { get; set; }
        public static IModel Channel { get; private set; }
        public static EventingBasicConsumer Consumer { get; set; }

        public MainForm()
        {
            InitializeComponent();
            Configuration = new LoaderPaths("Debug");
            Logging.ActiveConfiguration = Configuration.ActiveRuntime;
            SpeechSynthesizerUsed = true; //Convert.ToBoolean(_thisAeon.GlobalSettings.GrabSetting("speechsynthesizerused"));

#if Linux
            InitializePocketSphinx();
#endif
#if Windows
            BuildWindowsSpeech();
            InitializeSpeechRecognizer();
#endif
        }

        #region Voice recognition as per the operating system

        #region Windows
        private void BuildWindowsSpeech()
        {
            Recognizer = new SpeechRecognitionEngine()
            {

            };
            GrammarBuilder = new GrammarBuilder()
            {
                Culture = Recognizer.RecognizerInfo.Culture
            };

            SpeechSynth = new SpeechSynthesizer();
            PromptBuilder = new PromptBuilder();
        }
        /// <summary>
        /// Receive the output from SAPI.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpeechRecognizedEventArgs"/> instance containing the event data.</param>
        private void RecognizerSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            UserInput = e.Result.Text;
            //ProcessInput();
            SpeakText("I heard you.");
            WriteToConsole("The voice recognizer is working properly. You said: " + UserInput);
            Logging.WriteLog(LastOutput, Logging.LogType.Information, Logging.LogCaller.AeonRuntime);
        }
        /// <summary>
        /// Initializes the speech recognizer.
        /// </summary>
        /// <returns></returns>
        public bool InitializeSpeechRecognizer()
        {
            try
            {
                GrammarBuilder.Append(
                    new Choices(File.ReadAllLines(Path.Combine(Configuration.ActiveRuntime, "grammar", "valid-grammar.txt"))));
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.SpeechRecognizer);
                return false;
            }
            var builder = new Grammar(GrammarBuilder);
            try
            {
                Recognizer.UnloadAllGrammars();
                Recognizer.RecognizeAsyncCancel();
                Recognizer.RequestRecognizerUpdate();
                Recognizer.LoadGrammar(builder);
                Recognizer.SpeechRecognized += RecognizerSpeechRecognized;
                Recognizer.SetInputToDefaultAudioDevice();
                Recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.SpeechRecognizer);
            }
            return true;
        }
        #endregion

        #region Linux
        /// <summary>
        /// Receive the output from PocketSphinx recognition.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpeechRecognizedEventArgs"/> instance containing the event data.</param>
        private void PocketSphinxSpeechRecognized(object sender, EventArgs e)
        {
            //Todo: Fix this recognizer.
            //UserInput = e.Result.Text;
            ProcessInput();
            Logging.WriteLog(LastOutput, Logging.LogType.Information, Logging.LogCaller.AeonRuntime);
        }
        /// <summary>
        /// Initializes the speech recognizer.
        /// </summary>
        /// <returns></returns>
        public static bool InitializePocketSphinx()
        {
            // Do something here.
            return false;
        }
        #endregion

        #endregion

        #region Speak the output
        public void SpeakText(string input)
        {
            if (SpeechSynthesizerUsed)
            {
                try
                {
                    PromptBuilder.ClearContent();
                    PromptBuilder.AppendText(input);
                    SpeechSynth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
                    SpeechSynth.Speak(PromptBuilder);
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AeonRuntime);
                }
            }
            else
            {
                // Use epeak or the customized festival voices.
                WriteToConsole("Linux voices not implemented.");
            }
        }
        #endregion

        public bool ProcessInput()
        {
            if (UserInput == null)
            {
                UserInput = "hello there";
            }

            PocketSphinxProcess = new Process
            {
                StartInfo =
                        {
                            FileName = PythonLocation,
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        }
            };
            try
            {
                PocketSphinxProcess.StartInfo.Arguments = SphinxStartup;
                PocketSphinxProcess.Start();
                var stream = PocketSphinxProcess.StandardOutput;
                if (PocketSphinxProcess.Responding)
                {
                    Logging.WriteLog("Pocketsphinx decoder intitate mic listen - Running.", Logging.LogType.Information,
                        Logging.LogCaller.SpeechRecognizer);
                }
                PocketSphinxProcessOutput = stream.ReadToEnd();
                while (!PocketSphinxProcess.WaitForExit(1000))
                    PocketSphinxProcessExitCode = PocketSphinxProcess.ExitCode;

            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AeonRuntime);
                WriteToConsole("Pocketsphinx is not responding.");
            }
            finally
            {
                if (PocketSphinxProcess != null)
                {
                    PocketSphinxProcess.Close();
                    PocketSphinxProcess.Dispose();
                }
            }
            return true;
        }

        void WriteToConsole(string message)
        {
            CommandWindow.Text += message;
            CommandWindow.Text += Environment.NewLine;
            CommandWindow.ScrollToCaret();
        }

        #region Events
        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

    }
}