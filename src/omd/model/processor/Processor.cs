/*
 * 
 */
using System;
using System.Reflection;

namespace OMD.omd.model.processor
{
	/// <summary>
	/// Description of Processor.
	/// </summary>
	public class Processor
	{
		
		private readonly Object c;
		private readonly MethodInfo supports;
		private readonly MethodInfo process;		
		
		public Processor(Object claz, MethodInfo supportsMethod, MethodInfo processMethod)
		{
			c = claz;
			supports = supportsMethod;
			process = processMethod;
		}
		
		public bool IsSupported(string key)
		{
			return (bool)supports.Invoke(c, new Object[]{key});
		}
		
		public void Process(OMD.omd.model.xml.CommandStep step)
		{
			process.Invoke(c, new Object[]{step});
		}
	}
}
