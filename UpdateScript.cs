using Godot;
using System;
using System.Linq;

public class UpdateScript : Node
{
	private const string FIRST_KEYBINDS_PATH = "user://keybinds.kbd";
	private const string SECOND_KEYBINDS_PATH = "res://keybinds.kbd";
	
	public KeybindsParser Keybinds{get; set;} = new KeybindsParser();
	
	public UpdateScript() {}
	
	public override void _Ready()
	{
		GlobalizeDevices();
		LoadKeybinds();
	}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;
		if(Input.IsActionJustPressed("reload_keybinds"))
			LoadKeybinds();
		if(Input.IsActionJustPressed("dump_orphans"))
			PrintStrayNodes();
	}
	
	public void GlobalizeDevices()
	{
		foreach(string action in InputMap.GetActions())
		{
			var inputs = InputMap.GetActionList(action).FilterType<InputEventKey>();
			foreach(var input in inputs) for(int i = 0; i < 8; ++i)
			{
				var newInput = new InputEventKey();
				newInput.Scancode = input.Scancode;
				newInput.Device = i;
				InputMap.ActionAddEvent(action, newInput);
			}
		}
	}
	
	public void LoadKeybinds()
	{
		Keybinds.Load(new File().FileExists(FIRST_KEYBINDS_PATH)?FIRST_KEYBINDS_PATH:SECOND_KEYBINDS_PATH);
		Keybinds.ApplyParsedData();
		
		#if DEBUG_INPUT_MAP
		foreach(var h in InputMap.GetActions()) GD.Print(h);
		#endif
	}
}
