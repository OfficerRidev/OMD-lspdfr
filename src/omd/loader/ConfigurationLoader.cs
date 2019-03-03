/*
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using OMD.omd.model.xml;
using OMD.omd.model.command;

namespace OMD.omd.loader
{
	/// <summary>
	/// Description of ConfigurationLoader.
	/// </summary>
	public class ConfigurationLoader
	{
		private const string configPath = "plugins/LSPDFR/OMD/";
		private const string configPattern = "config-*.xml";
		
		private readonly CommandsPerInput commandsPerInput;
		
		public ConfigurationLoader()
		{
			commandsPerInput = new CommandsPerInput();
			OMD.Main.Logger.Info("Start scanning the directory for config...");
			string[] configPaths = Directory.GetFiles(configPath, configPattern);
			
			for (int i = 0; i < configPaths.Length; i++) {
				string currentConfigPath = configPaths[i];
				OMD.Main.Logger.Info("Read config file: " + currentConfigPath);
				XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
				using (FileStream fileStream = new FileStream(currentConfigPath, FileMode.Open)) {
					Configuration config = (Configuration)serializer.Deserialize(fileStream);
				    
					foreach (Input input in config.inputs) {
						commandsPerInput.AddInput(input.phrase, input.actions);
					}   	
				}
			}
			
			OMD.Main.Logger.Info("Number of inputs in total: " + commandsPerInput.NumberOfInputs());
		}
		
		public CommandsPerInput GetInputs()
		{
			return commandsPerInput;
		}
		
	}
}
