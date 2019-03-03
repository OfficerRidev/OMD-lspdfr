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
	/// Description of CommandAction.
	/// </summary>
	public class CommandAction
	{
		[XmlElement("Step")]
		public List<CommandStep> steps { get; set; }
	}
}
