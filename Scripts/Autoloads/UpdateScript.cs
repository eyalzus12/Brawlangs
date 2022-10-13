#define MULTI_KEYBOARD

//#define DEBUG_INPUT_EVENTS
//#define DEBUG_INPUT_MAP

//#define DEBUG_ATTACKS

using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public class UpdateScript : Node
{
	//private const string FIRST_KEYBINDS_PATH = "user://keybinds.kbd";
	//private const string SECOND_KEYBINDS_PATH = "res://keybinds.kbd";
	
	//public KeybindsParser Keybinds{get; set;} = new KeybindsParser();
	
	public UpdateScript() {}
	
	public override void _Ready()
	{
		//#if MULTI_KEYBOARD//no need to globalize them if there aren't gonna be multiple devices
		//GlobalizeDevices();
		//#endif
		
		//LoadKeybinds();
	}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;
		if(Input.IsActionJustPressed("dump_orphans"))
			new Task(PrintStrayNodes).Start();//to not block game
	}
	
	/*public void GlobalizeDevices()
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
	}*/
}
