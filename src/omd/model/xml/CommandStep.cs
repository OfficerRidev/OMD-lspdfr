/*
 * 
 */
using System;
using System.Xml.Serialization;

namespace OMD.omd.model.xml
{
	/// <summary>
	/// Description of CommandStep.
	/// </summary>
	public class CommandStep
	{
		[XmlElement("Key")]
		public String Key { get; set; }
		[XmlElement("Arg")]
		public String[] Args { get; set; }
		
		public override string ToString()
		{
			String r = "CommandAction: Key[" + Key + "], Args: [";
			foreach (String s in Args) {
				r += s + " / ";
			}
			r += "]";
			return r;
		}
	}
}
