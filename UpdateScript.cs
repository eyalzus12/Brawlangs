using Godot;
using System;
using System.Linq;

public class UpdateScript : Node
{
	public bool debugCollision = false;
	
	private const string FIRST_KEYBINDS_PATH = "user://keybinds.kbd";
	private const string SECOND_KEYBINDS_PATH = "res://keybinds.kbd";
	
	public override void _Ready()
	{
		var kbd = new KeybindsParser();
		kbd.Load((new File()).FileExists(FIRST_KEYBINDS_PATH)?FIRST_KEYBINDS_PATH:SECOND_KEYBINDS_PATH);
		kbd.ApplyParsedData();
		
		#if DEBUG_INPUT_MAP
		foreach(var h in InputMap.GetActions()) GD.Print(h);
		#endif
	}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;
		if(Input.IsActionJustPressed("debug_collision"))
			debugCollision = !debugCollision;
	}
}
