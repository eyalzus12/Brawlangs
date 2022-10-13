//List of compilation conditions: MULTI_KEYBOARD, DEBUG_INPUT_EVENTS, DEBUG_INPUT_MAP, DEBUG_BUFFER, DEBUG_ATTACKS

using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public class UpdateScript : Node
{
	public UpdateScript() {}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;
		if(Input.IsActionJustPressed("dump_orphans"))
			new Task(PrintStrayNodes).Start();//to not block game
	}
}
