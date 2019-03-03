/*
 * 
 */
using System;
using System.Collections.Generic;
using OMD.omd.model.xml;

namespace OMD.omd.model.command
{
	/// <summary>
	/// Description of CommandsPerInput.
	/// </summary>
	public class CommandsPerInput
	{
		private readonly Dictionary<string, List<CommandAction>> commandsPerInput;
		private readonly Random rand;
		
		public CommandsPerInput()
		{
			commandsPerInput = new Dictionary<string, List<CommandAction>>();
			rand = new Random();
		}
		
		public void AddInput(string input, List<CommandAction> actions)
		{
			commandsPerInput.Add(input, actions);
		}
		
		public bool Exists(string input)
		{
			return commandsPerInput.ContainsKey(input);
		}
		
		public CommandAction GetAction(string input)
		{
			if (Exists(input))
			{
				List<CommandAction> actions;
				commandsPerInput.TryGetValue(input, out actions);
				int r = rand.Next(actions.Count);
				return actions[r];
			}
			return null;
		}
		
		public int NumberOfInputs()
		{
			return commandsPerInput.Count;
		}
		
		public string[] GetAllInputs()
		{
			string[] k = new string[NumberOfInputs()];
			int i = 0;
			Dictionary<string, List<CommandAction>>.KeyCollection keys = commandsPerInput.Keys;  
			foreach (string key in keys)  
			{  
				k[i] = key;
				i++;
			} 
			
			return k;
		}
	}
}
