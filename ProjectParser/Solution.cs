using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace ProjectParser
{
	public class Solution
	{
		// static regexes
		static Regex projectRegex = null;

		internal static Regex ProjectRegex
		{
			get
			{
				if (projectRegex == null)
					projectRegex = new Regex(@"Project\(""(\{[^}]*\})""\) = ""(.*)"", ""(.*)"", ""(\{[^{]*\})""");
				return projectRegex;
			}
		}

		static Regex globalSectionRegex = null;

		static Regex GlobalSectionRegex
		{
			get
			{
				if (globalSectionRegex == null)
					globalSectionRegex = new Regex(@"GlobalSection\s*\(([^)]*)\)\s*=\s*(\w*)"); 
				return globalSectionRegex;
			}
		}

		static Regex slnVersionRegex = null;

		internal static Regex SlnVersionRegex
		{
			get
			{
				if (slnVersionRegex == null)
					slnVersionRegex = new Regex(@"Microsoft Visual Studio Solution File, Format Version (\d?\d.\d\d)");
				return slnVersionRegex;
			}
		}

		public Solution(string filename)
		{
			using (StreamReader sReader = new StreamReader(new FileStream(filename, FileMode.Open)))
			{
				string slnData = sReader.ReadToEnd();
				Match slnVersion = SlnVersionRegex.Match(slnData);
				MatchCollection projectMatches = ProjectRegex.Matches(slnData);
				List<Project> projects = new List<Project>();
				foreach (Match proj in projectMatches)
				{
					string typeGuid = proj.Groups[1].Value;
					string projName = proj.Groups[2].Value;
					string relPath = proj.Groups[3].Value;
					string projGuid = proj.Groups[4].Value;
					
					string spath = Path.GetFullPath(filename);
					string directory = new FileInfo(spath).Directory.FullName;
					relPath = relPath.Replace('\\', Path.DirectorySeparatorChar);
					string projectPath = directory + Path.DirectorySeparatorChar + relPath;
					
					Project p = new Project(projectPath)
					{
						Name = projName,
						ProjectGuid = new Guid(projGuid),
						TypeGuid = new Guid(typeGuid)
					};
					p.ProjectType = ProjectType.DataObjects;
					projects.Add(p);
					
					Console.WriteLine("Found project: " + p.Name);
					foreach(CompileItem item in p.CompileItems)
					{
						Console.WriteLine("    Compile item: " + item.Path);
					}
					Console.WriteLine();
					
					string outFile = string.Format("/tmp/{0}.csproj", p.Name);
			
					p.Save(outFile);
				}
			}
		}
		
		
	}
}

