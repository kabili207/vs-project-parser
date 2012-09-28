using System;
using Gtk;
using System.IO;
using ProjectParser;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow(): base (Gtk.WindowType.Toplevel)
	{
		Build();
	}
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void OnButton1Clicked(object sender, System.EventArgs e)
	{
		string filename = filechooserbutton1.Filename;
		if(File.Exists(filename))
		{
			Solution s = new Solution(filename);	
		
		}
	}
}
