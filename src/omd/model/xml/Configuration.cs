/*
 * 
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using OMD.omd.model.xml;

namespace OMD.omd.model.xml
{
	/// <summary>
	/// Description of Configuration.
	/// </summary>
	[XmlRoot("Configuration")]
	public class Configuration
	{
		[XmlElement("Input")]
		public List<Input> inputs { get; set; }
	}
}
