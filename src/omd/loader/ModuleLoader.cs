/*
 * 
 */
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace OMD.omd.loader
{
	/// <summary>
	/// Description of ModuleLoader.
	/// </summary>
	public class ModuleLoader
	{
		
		private const string modulePath = "plugins/LSPDFR/OMD/";
		private const string modulePattern = "*Module.dll";
		
		private readonly List<OMD.omd.model.processor.Processor> processors;
		
		public ModuleLoader(OMD.omd.utils.IniFile iniFile)
		{
			OMD.Main.Logger.Info("Start scanning the directory for module...");
			processors = new List<OMD.omd.model.processor.Processor>();
			string[] modulePaths = Directory.GetFiles(modulePath, modulePattern);
			
			for (int i = 0; i < modulePaths.Length; i++) {
				string currentModulePath = modulePaths[i];
				OMD.Main.Logger.Info("Read module file: " + currentModulePath);
				try {
					Assembly assembly = Assembly.LoadFrom(currentModulePath);
					Type[] types = assembly.GetTypes();
					
					for (int j = 0; j < types.Length; j++)
					{
						Type type = types[j];
						bool isProc = type.IsSubclassOf(Type.GetType("OMD.omd.api.OMDPlugin"));
						if (isProc)
						{
							OMD.Main.Logger.Debug("Found type: " + type.FullName);
							MethodInfo methodProcess = type.GetMethod("Process");
							MethodInfo methodSupports = type.GetMethod("Supports");
							
							if (methodProcess != null && methodSupports != null)
		            		{
								OMD.Main.Logger.Debug("Methods found.");
								
								object classInstance = Activator.CreateInstance(type, new object[]{iniFile});
								processors.Add(new OMD.omd.model.processor.Processor(classInstance, methodSupports, methodProcess));
							} 
							else
							{
								OMD.Main.Logger.Debug("Methods not found.");
							}
						}
					}
				} catch (Exception e) {
					OMD.Main.Logger.Error("Got error on module loading... (" + e.Message + ")", e);
				}
				
			}
		}
		
		public List<OMD.omd.model.processor.Processor> GetProcessors()
		{
			return processors;
		}
	}
}
