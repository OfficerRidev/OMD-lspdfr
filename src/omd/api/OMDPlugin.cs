/*
 * 
 */
using System;
using OMD.omd.utils;

namespace OMD.omd.api
{
	/// <summary>
	/// Description of OMDPlugin.
	/// </summary>
	public abstract class OMDPlugin
	{
		protected readonly string key;
		protected readonly IniFile config;
		
		protected OMDPlugin(string pluginKey, IniFile generalConfig)
		{
			key = pluginKey;
			config = generalConfig;
		}
		
		public virtual void Process(OMD.omd.model.xml.CommandStep step)
		{
			// Do nothing by default
		}
		
		public virtual bool Supports(string inputKey)
		{
			return key.Equals(inputKey);
		}
	}
}
