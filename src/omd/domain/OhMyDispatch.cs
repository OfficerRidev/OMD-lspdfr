/*
 * 
 */
using System;
using Rage;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Globalization;
using OMD.omd.model.command;
using OMD.omd.model.xml;
using OMD.omd.model.processor;
using System.Collections.Generic;

namespace OMD.omd.domain
{
	/// <summary>
	/// Description of OhMyDispatch.
	/// </summary>
	public class OhMyDispatch
	{
		private bool isRecording;
		
		private SpeechRecognitionEngine speechRecognitionEngine;
		
		private OMD.omd.utils.IniFile iniConfiguration;
		
		private readonly CommandsPerInput commandsPerInput;
		private readonly List<Processor> processors;
		
		private string language;
		private Keys startRecordingKey;
		private Keys startRecordingModifier;
		private Keys stopRecordingKey;
		private Keys stopRecordingModifier;
		
		public OhMyDispatch(OMD.omd.utils.IniFile iniFile)
		{
			isRecording = false;
			iniConfiguration = iniFile;
			language = iniFile.Read("language", "RECOGNITION");
			OMD.Main.Logger.Trace("Config of language: " + language);
			string startRecordingKeyS = iniFile.Read("startReco", "KEYS");
			OMD.Main.Logger.Trace("Config of key start: " + startRecordingKeyS);
			startRecordingKey = ConvertStringToKeys(startRecordingKeyS);
			string startRecordingModifierS = iniFile.Read("startModifier", "KEYS");
			OMD.Main.Logger.Trace("Config of modifer start: " + startRecordingModifierS);
			startRecordingModifier = ConvertStringToKeys(startRecordingModifierS);
			string stopRecordingKeyS = iniFile.Read("stopReco", "KEYS");
			OMD.Main.Logger.Trace("Config of key stop: " + stopRecordingKeyS);
			stopRecordingKey = ConvertStringToKeys(stopRecordingKeyS);
			string stopRecordingModifierS = iniFile.Read("stopModifier", "KEYS");
			OMD.Main.Logger.Trace("Config of modifer stop: " + stopRecordingModifierS);
			stopRecordingModifier = ConvertStringToKeys(stopRecordingModifierS);
			OMD.omd.loader.ConfigurationLoader configurationLoader = new OMD.omd.loader.ConfigurationLoader();
			commandsPerInput = configurationLoader.GetInputs();
			OMD.omd.loader.ModuleLoader moduleLoader = new OMD.omd.loader.ModuleLoader(iniFile);
			processors = moduleLoader.GetProcessors();
			speechRecognitionEngine = InitSpeechRecognitionEngine();
		}
		
		public void process()
		{
			if (IsRequestingWithKeys(startRecordingKey, startRecordingModifier) && isRecording == false)
			{
				Game.DisplayNotification("~b~OhMyDispatch~s~ is ~r~recording~s~.");
				StartRecording();
				isRecording = true;
			}
			else if (IsRequestingWithKeys(stopRecordingKey, stopRecordingModifier) && isRecording == true)
			{
				Game.DisplayNotification("~b~OhMyDispatch~s~ is ~y~not recording.");
				StopRecording();
				isRecording = false;
			}
		}
		
		private SpeechRecognitionEngine InitSpeechRecognitionEngine()
		{
			OMD.Main.Logger.Trace("InitSpeechRecognitionEngine()");
			SpeechRecognitionEngine _reco = null;
			Boolean selectedReco = false;
			CultureInfo cultureInfo = null;
			foreach (RecognizerInfo recoInfo in SpeechRecognitionEngine.InstalledRecognizers()) {
				OMD.Main.Logger.Debug("Recoginzer:");  
				OMD.Main.Logger.Debug(" Name:          " + recoInfo.Name);  
				OMD.Main.Logger.Debug(" Culture:       " + recoInfo.Culture);
				OMD.Main.Logger.Debug(" Description:   " + recoInfo.Description);  
				OMD.Main.Logger.Debug(" ID:            " + recoInfo.Id); 
				OMD.Main.Logger.Debug("-------");
				
				Game.LogTrivial(" Name:          " + recoInfo.Name);  
				Game.LogTrivial(" Culture:       " + recoInfo.Culture);
				Game.LogTrivial(" Description:   " + recoInfo.Description);  
				Game.LogTrivial(" ID:            " + recoInfo.Id);
				Game.LogTrivial("");
				
				if (recoInfo.Culture.Name.Contains(language)) {
					OMD.Main.Logger.Debug("Found recognizer matching the language.");  
					_reco = new SpeechRecognitionEngine(recoInfo);
					cultureInfo = recoInfo.Culture;
					selectedReco = true;
					OMD.Main.Logger.Debug("Recognizer set.");  
					break;
				}
				
			}
						
			OMD.Main.Logger.Debug("Recognizers browse...");  
			
			if (selectedReco == false || _reco == null) {
				OMD.Main.Logger.Error("No recognizer selected for language " + language);  
				throw new Exception("No recognizer selected for language " + language);
			}
			
			OMD.Main.Logger.Debug("Do choices...");  
			Choices choices = new Choices();
			choices.Add(commandsPerInput.GetAllInputs());
			OMD.Main.Logger.Debug("Do choices... Done."); 
			OMD.Main.Logger.Debug("Do GrammarBuilder...");  
			GrammarBuilder gBuilder = new GrammarBuilder();
			gBuilder.Culture = cultureInfo;
			gBuilder.Append(choices);
			OMD.Main.Logger.Debug("Do GrammarBuilder... Done.");  
			Grammar grammar = new Grammar(gBuilder);
			OMD.Main.Logger.Debug("Do Grammar... Done.");  
			
			OMD.Main.Logger.Debug("Load Grammar..."); 
			_reco.LoadGrammar(grammar);
			OMD.Main.Logger.Debug("Load Grammar... Done."); 
			OMD.Main.Logger.Debug("Set up input..."); 
			_reco.SetInputToDefaultAudioDevice();
			OMD.Main.Logger.Debug("Set up input... Done."); 
			OMD.Main.Logger.Debug("Set up handler..."); 
			_reco.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
			OMD.Main.Logger.Debug("Set up handler... Done."); 
			
			return _reco;
		}
		
		private void StartRecording()
		{
			OMD.Main.Logger.Trace("SpeechRecognitionEngine_RecognizeAsync()"); 
			speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
		}
		
		private void StopRecording()
		{
			OMD.Main.Logger.Trace("SpeechRecognitionEngine_RecognizeAsyncStop()"); 
			speechRecognitionEngine.RecognizeAsyncStop();
		}
		
		private Keys ConvertStringToKeys(string key)
		{
			return (Keys)Enum.Parse(typeof(Keys), key, true);
		}
		
		private bool IsRequestingWithKeys(Keys key, Keys modifier)
		{
			if (modifier.Equals(Keys.None))
			{
				return Game.IsKeyDown(key);
			}
			
			return Game.IsKeyDown(key) && Game.IsKeyDown(modifier);
		}

		void CheckProcessorsFor(CommandAction action)
		{
			if (action == null) {
				return;
			}
			
			List<CommandStep> steps = action.steps;
			
			for (int s = 0; s < steps.Count; s++) {
				CommandStep step = steps[s];
				for (int i = 0; i < processors.Count; i++) {
					Processor currentProc = processors[i];
					if (currentProc.IsSupported(step.Key)) {
						currentProc.Process(step);
					}
				}
			}
		}
		void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			OMD.Main.Logger.Trace("SpeechRecognitionEngine_SpeechRecognized()"); 
			string recoginzedText = e.Result.Text;
			if (e.Result.Confidence >= 0.70) {
	           	Game.LogTrivial("Recognized text: " + recoginzedText);
				Game.DisplayNotification("Recognized text: " + recoginzedText);
				OMD.Main.Logger.Debug("Recognized text: " + recoginzedText);
				CheckProcessorsFor(commandsPerInput.GetAction(recoginzedText));
				StopRecording();
			} else {
				Game.LogTrivial("Speech not recognized: " + recoginzedText);
				OMD.Main.Logger.Debug("Speech not recognized:" + recoginzedText);
				Game.DisplayNotification("Did not copy, please repeat."); 
			}
		}
	}
}
