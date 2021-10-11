using Godot;
using System;

public class MapSelectorButton : Button
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

	public override void _Ready()
	{
		Connect("pressed", this, nameof(OnPress));
	}

	public void OnPress()
	{
		var te = GetParent().GetParent().GetNode<TextEdit>("TextEdit");
		var str = te.Text;
		if(str != "") this.AddData("ini_path", str);
		
		GetTree().CallDeferred("change_scene", path + dest);
	}
}
