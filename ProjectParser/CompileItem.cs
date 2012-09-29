using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ProjectParser
{
	public class CompileItem
	{
		XNamespace ns = Project.MsBuildNamespace;
		XElement _element;
		
		internal XElement Element
		{
			get { return _element; }
			set { _element = value; }
		}
		
		public string Path
		{
			get { return (string)_element.Attribute("Include"); }
			set { _element.Attribute("Include"); }
		}
		
		//public string FullPath
		//{
		//	
		//}
		
		internal CompileItem()
		{
			Element = new XElement(ns + "Compile", new XAttribute("Include", ""));
		}
		
		internal CompileItem(XElement element)
		{
			Element = element;
		}
	}
}

