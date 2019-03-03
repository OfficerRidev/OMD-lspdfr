/*
 * 
 */
using System;
using System.IO;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Speech.AudioFormat;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Globalization;

using voicereco.model.xml;

namespace voicereco.model
{
	/// <summary>
	/// Description of Xena.
	/// </summary>
	public class Xena
	{
		readonly SpeechSynthesizer synth;
		static Dictionary<String, CommandAction> commandPerInput;
		static SpeechRecognitionEngine reco;
		
		public Xena(String lang)
		{
			commandPerInput = new Dictionary<String, CommandAction>();
			
			synth = new SpeechSynthesizer();
			
			initSynth(lang);
			reco = initReco(lang);
		}
		
		private void initSynth(String language)
		{
			// Output information about all of the installed voices.   
			Console.WriteLine("Installed voices -");
			
			Boolean selectedVoice = false;
			
			foreach (InstalledVoice voice in synth.GetInstalledVoices()) { 
				VoiceInfo info = voice.VoiceInfo;  
				String AudioFormats = "";  
				foreach (SpeechAudioFormatInfo fmt in info.SupportedAudioFormats) {  
					AudioFormats += String.Format("{0}",  
						fmt.EncodingFormat.ToString());  
				}  
				Console.WriteLine(" Name:          " + info.Name);  
				Console.WriteLine(" Culture:       " + info.Culture);  
				Console.WriteLine(" Age:           " + info.Age);  
				Console.WriteLine(" Gender:        " + info.Gender);  
				Console.WriteLine(" Description:   " + info.Description);  
				Console.WriteLine(" ID:            " + info.Id);  
				Console.WriteLine(" Enabled:       " + voice.Enabled);  
				if (info.SupportedAudioFormats.Count != 0) {  
					Console.WriteLine(" Audio formats: " + AudioFormats);  
				} else {  
					Console.WriteLine(" No supported audio formats found");  
				}  
	
				String AdditionalInfo = "";  
				foreach (String key in info.AdditionalInfo.Keys) {  
					AdditionalInfo += String.Format("  {0}: {1}", key, info.AdditionalInfo[key]);  
				}  
	
				Console.WriteLine(" Additional Info - " + AdditionalInfo);  
				Console.WriteLine();
	          
				if (info.Culture.Name.Contains(language + "-")) {
					synth.SelectVoice(info.Name);
					selectedVoice = true;
					break;
				}
			}
			
			if (selectedVoice == false) {
				throw new Exception("No voice selected for language " + language);
			}
		}
		
		private SpeechRecognitionEngine initReco(String language)
		{
			SpeechRecognitionEngine _reco = null;
			Boolean selectedReco = false;
			CultureInfo cultureInfo = null;
			foreach (RecognizerInfo recoInfo in SpeechRecognitionEngine.InstalledRecognizers()) {
				Console.WriteLine(" Name:          " + recoInfo.Name);  
				Console.WriteLine(" Culture:       " + recoInfo.Culture);
				Console.WriteLine(" Description:   " + recoInfo.Description);  
				Console.WriteLine(" ID:            " + recoInfo.Id);
				Console.WriteLine();
				
				if (recoInfo.Culture.Name.Contains(language + "-")) {
					_reco = new SpeechRecognitionEngine(recoInfo);
					cultureInfo = recoInfo.Culture;
					selectedReco = true;
					break;
				}
				
			}
			
			if (selectedReco == false || _reco == null) {
				throw new Exception("No recognizer selected for language " + language);
			}
			
			// Create and load a dictation grammar.  
			_reco.LoadGrammar(initGrammar(cultureInfo));
        	
			// Add a handler for the speech recognized event.  
			_reco.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);  

			// Configure input to the speech recognizer.  
			_reco.SetInputToDefaultAudioDevice();  

			// Start asynchronous, continuous speech recognition.  
			_reco.RecognizeAsync(RecognizeMode.Multiple); 
			
			return _reco;
			
		}
		
		// Handle the SpeechRecognized event.
		static void recognizer_SpeechRecognized(Object sender, SpeechRecognizedEventArgs e)
		{
			if (e.Result.Confidence >= 0.70) {
				Console.WriteLine("Recognized text: " + e.Result.Text); 
				
				if(commandPerInput.ContainsKey(e.Result.Text)){
					CommandAction action = commandPerInput[e.Result.Text];
					Console.WriteLine("Action: " + action.ToString());
				}
				
				//synth.Speak("Got it");
				//synth.Speak(e.Result.Text);
			} else {
				Console.WriteLine("Recognized text: ... "); 
			}
			
			/*Console.WriteLine("-------");
			Console.WriteLine("Confidence: " + e.Result.Confidence);
			Console.Write("Alternates: ");	
			foreach (RecognizedPhrase phrase in e.Result.Alternates) {
				Console.Write(phrase.Text + "(" + phrase.Confidence + ") / ");
			}
			Console.WriteLine();
			Console.Write("Words: ");	
			
			foreach (RecognizedWordUnit word in e.Result.Words) {
				Console.Write(word.Text + "(" + word.Confidence + ") / ");
			}
			Console.WriteLine();
			
			Console.WriteLine("-------");
			Console.WriteLine();*/
		}
		
		private Grammar initGrammar(CultureInfo cultureInfo)
		{
			GrammarBuilder item = new GrammarBuilder();
			item.Culture = cultureInfo;
			
			List<String> inputs = new List<String>();
			
			Console.WriteLine("Folder for configuration: " + Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
			
			XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
			using (FileStream fileStream = new FileStream(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"/config/config.xml", FileMode.Open)) {
				Configuration config = (Configuration)serializer.Deserialize(fileStream);
			    
				foreach (Command command in config.Commands) {
					Console.WriteLine("Load input: " + command.Input);
					inputs.Add(command.Input);
					commandPerInput.Add(command.Input, command.Actions);
				}   	
			}
			item.Append(new Choices(inputs.ToArray()));

			return new Grammar(item);
		}
	}
}
