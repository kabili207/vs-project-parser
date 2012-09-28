using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ProjectParser
{
	public class Project
	{
		XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
		XNamespace nsDal = "http://bthreesolutions.com/dalbuilder";
		XDocument _document;
		private string _origFileName;
		
		private string _Name;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}
		
		private Guid _ProjectGuid;

		public Guid ProjectGuid
		{
			get { return _ProjectGuid; }
			set { _ProjectGuid = value; }
		}
		
		private Guid _TypeGuid;

		public Guid TypeGuid
		{
			get { return _TypeGuid; }
			set { _TypeGuid = value; }
		}
		
		public Project(string filename)
		{
			if (!filename.EndsWith("csproj"))
				return;
			
			_origFileName = Path.GetFullPath(filename);
			
			using (StreamReader sReader = new StreamReader(new FileStream(filename, FileMode.Open)))
			{
				string projData = sReader.ReadToEnd();
				
				XDocument _document = XDocument.Parse(projData);
				XElement root = _document.Root;
				
				if (root.GetPrefixOfNamespace(nsDal) == null)
					root.Add(new XAttribute(XNamespace.Xmlns + "dal", nsDal));
				
				XElement itemGroup = root.Elements(ns + "ItemGroup").FirstOrDefault(i => i.Elements(ns + "Compile").Count() > 1);
				
				var compileElmnt = itemGroup.Elements(ns + "Compile");
				foreach (XElement compile in compileElmnt)
				{
					compile.Add(new XAttribute(nsDal + "File", "Derp!"));
				}
				
				string outFile = string.Format("/tmp/{0}.csproj", Path.GetFileNameWithoutExtension(filename));
				_document.Save(outFile);
			}
		}
	}
}

