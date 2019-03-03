/*
 * 
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using OMD.omd.model.xml;

namespace OMD.omd.model.xml
{
	/// <summary>
	/// Description of Input.
	/// </summary>
	public class Input
	{
		[XmlElement("Name")]
		public String name { get; set; }
		[XmlElement("Phrase")]
		public String phrase { get; set; }
		[XmlElement("Action")]
		public List<CommandAction> actions { get; set; }
	}
}
