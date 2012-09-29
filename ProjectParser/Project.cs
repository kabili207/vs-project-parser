using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ProjectParser
{
	public enum ProjectType
	{
		None,
		DataObjects,
		DataAccessLayer
		
	}
	
	public class Project
	{
		public const string MsBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
		XNamespace ns = MsBuildNamespace;
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
		
		public ProjectType ProjectType
		{
			get
			{
				XElement propGroup = _document.Root.Elements(ns + "PropertyGroup").FirstOrDefault(x => !x.HasAttributes);
				if (propGroup != null)
				{
					XElement type = propGroup.Element(ns + "DalProjectType");
					if (type != null)
					{
						switch(type.Value)
						{
							case "DataObjects":
								return ProjectType.DataObjects;
							case "DataAccessLayer":
								return ProjectType.DataObjects;
							default:
								return ProjectType.None;
						}
					}
				}
				return ProjectType.None;
			}
			set
			{
				XElement propGroup = _document.Root.Elements(ns + "PropertyGroup").FirstOrDefault(x => !x.HasAttributes);
				if (propGroup == null)
				{
					if (value == ProjectType.None)
						return;
					
					propGroup = new XElement(ns + "PropertyGroup");
					_document.Root.Add(propGroup);
				}
					
				XElement type = propGroup.Element(ns + "DalProjectType");
				if (type == null)
				{
					if (value == ProjectType.None)
						return;
					
					type = new XElement(ns + "DalProjectType");
					propGroup.Add(type);
				}
				switch(value)
				{
					case ProjectType.DataObjects:
						type.Value = "DataObjects";
						break;
					case ProjectType.DataAccessLayer:
						type.Value = "DataAccessLayer";
						break;
					default:
						type.Remove();
						break;
				}
			}
			
		}
		
		public Project(string filename)
		{
			if (!filename.EndsWith("csproj"))
				return;
			
			_origFileName = Path.GetFullPath(filename);
			
			using (StreamReader sReader = new StreamReader(new FileStream(filename, FileMode.Open)))
			{
				string projData = sReader.ReadToEnd();
				
				_document = XDocument.Parse(projData);
				XElement root = _document.Root;
				
				XElement itemGroup = root.Elements(ns + "ItemGroup").FirstOrDefault(i => i.Elements(ns + "Compile").Count() > 1);
				
				//var compileElmnt = itemGroup.Elements(ns + "Compile");
				//foreach (XElement compile in compileElmnt)
				//{
				//	compile.Add(new XElement(ns + "DalTable", "Derp!"));
				//}
				
			}
		}
		
		public IEnumerable<CompileItem> CompileItems
		{
			get
			{
				XElement itemGroup = _document.Root.Elements(ns + "ItemGroup").FirstOrDefault(i => i.Elements(ns + "Compile").Count() > 1);
				
				var compileElmnt = itemGroup.Elements(ns + "Compile");
				foreach (XElement compile in compileElmnt)
				{
					yield return new CompileItem(compile);
				}
			}
		}
		
		public CompileItem CreateCompileItem()
		{
			CompileItem item = new CompileItem();
			XElement itemGroup = _document.Root.Elements(ns + "ItemGroup").FirstOrDefault(i => i.Elements(ns + "Compile").Count() > 1);
			itemGroup.Add(item.Element);
			return item;
		}
		
		
		public CompileItem CreateCompileItem(string filename)
		{
			CompileItem item = CreateCompileItem();
			item.Path = filename;
			return item;
		}
		
		public Void Save()
		{
			_document.Save(_origFileName);
		}
		
		public Void Save(SaveOptions options)
		{
			_document.Save(_origFileName, options);
		}
		
		public void Save(string filename)
		{
			_document.Save(filename);
		}
		
		public void Save(TextWriter tw)
		{
			_document.Save(tw);
		}
		
		public void Save(Stream stream)
		{
			_document.Save(stream);
		}
		
		public void Save(Stream stream, SaveOptions options)
		{
			_document.Save(stream, options);
		}
		
		public void Save(string filename, SaveOptions options)
		{
			_document.Save(filename, options);
		}
	}
}

