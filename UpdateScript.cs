//#define DEBUG_INPUT_MAP

using Godot;
using System;
using System.Linq;

public partial class UpdateScript : Node
{
	public bool debugCollision = false;
	
	private const string FIRST_KEYBINDS_PATH = "user://keybinds.kbd";
	private const string SECOND_KEYBINDS_PATH = "res://keybinds.kbd";
	
	public override void _Ready()
	{
		var kbd = new KeybindsParser();
		kbd.Load(File.FileExists(FIRST_KEYBINDS_PATH)?FIRST_KEYBINDS_PATH:SECOND_KEYBINDS_PATH);
		kbd.ApplyParsedData();
		
		#if DEBUG_INPUT_MAP
		foreach(var h in InputMap.GetActions())
			foreach(var i in InputMap.ActionGetEvents(h))
				GD.Print($"{h} : {i.AsText()}");
		#endif
	}
	
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("toggle_fullscreen"))
			Utils.ToggleFullscreen();
		if(Input.IsActionJustPressed("debug_collision"))
			debugCollision = !debugCollision;
	}
}
