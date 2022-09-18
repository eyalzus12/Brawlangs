using Godot;
using System;

public partial class MapSelectorButton : OnPressButton
{
	[Export]
	public string dest = "FallBackMap";
	[Export]
	public string path = "res://maps/";
	
	public MapSelectorButton(): base() {}
	
	public MapSelectorButton(string dest, string path): base() 
	{
		this.dest = dest;
		this.path = path;
	}

	public override void OnPress()
	{
		//TOFIX: unsafe
		var te = GetParent().GetParent().GetNode<TextEdit>("TextEdit");
		var str = te.Text;
		if(str != "") this.AddData("ini_path", str);
		
		this.ChangeSceneToFile(path + dest);
	}
}
