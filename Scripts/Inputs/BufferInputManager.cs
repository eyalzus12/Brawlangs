//#define DEBUG_INPUT_EVENTS

using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class BufferInputManager : InputManager
{
	protected const string CONFIG_PATH = "res://buffer.cfg";
		
	public Dictionary<string, BufferInfo> buffer = new Dictionary<string, BufferInfo>();
		
	CfgFile config = new CfgFile();
	
	public int playerDeviceNumber = 0;
		
	public BufferInputManager(): base() 
	{
		ParseConfig();
	}
	
	public BufferInputManager(int device): base() 
	{
		playerDeviceNumber = device;
		ParseConfig();
	}
	
	public void ParseConfig()
	{
		//reads from buffer.ini
		if(config.Load(CONFIG_PATH) != Error.Ok)
		{
			GD.PushError($"failed to parse buffer config at path {CONFIG_PATH}");
			return;
		}
		
		//GD.Print(config.ToString());
		
		foreach(var key in config.Keys)
		{
			int res = config[key, 1].i();
			buffer.Add($"{playerDeviceNumber}_{key}", new BufferInfo(res));
		}
	}
		
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventMouseMotion || @event.IsEcho() || !@event.IsPressed()) return;
		
		#if DEBUG_INPUT_EVENTS
		GD.Print(@event.AsText());
		#endif
		
		buffer.Keys
			.Where(action => @event.IsAction(action))
			.ForEach(action => buffer[action].Activate(-1));
	}
	
	public override void MarkForDeletion(string action, bool now=false)
	{
		action = $"{playerDeviceNumber}_{action}";
		
		if(buffer.ContainsKey(action))
		{
			if(now) buffer[action].Delete();
			else buffer[action].markedForDeletion = true;
		}
	}
	
	public override bool IsActionJustPressed(string str)
	{
		str = $"{playerDeviceNumber}_{str}";
		
		if(buffer.ContainsKey(str)) return buffer[str].IsActive();
		else return base.IsActionJustPressed(str);
	}
	
	public override bool IsActionPressed(string str)
	{
		str = $"{playerDeviceNumber}_{str}";
		
		if(buffer.ContainsKey(str)) return buffer[str].IsActive() || base.IsActionPressed(str);
		else return base.IsActionPressed(str);
	}
	
	public override bool IsActionJustReleased(string str)
	{
		str = $"{playerDeviceNumber}_{str}";
		
		if(buffer.ContainsKey(str)) return
			(base.IsActionJustReleased(str) && buffer[str].bufferTimeLeft < 0) ||
			(!base.IsActionPressed(str) && buffer[str].bufferTimeLeft == 0);
		else return base.IsActionJustReleased(str);
	}
	
	public override bool IsActionReallyJustPressed(string str) => base.IsActionJustPressed($"{playerDeviceNumber}_{str}");
	public override bool IsActionReallyPressed(string str) => base.IsActionPressed($"{playerDeviceNumber}_{str}");
	public override bool IsActionReallyJustReleased(string str) => base.IsActionJustReleased($"{playerDeviceNumber}_{str}");
	
	public override void _PhysicsProcess(float delta)
	{
		//if(GetTree().Paused) return;
		foreach(var buff in buffer.Values)
		{
			if(buff.markedForDeletion) buff.Delete();
			else buff.Advance();
		}
	}
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var entry in buffer) res.Append($"{{{entry.Key}, {entry.Value.ToString()}}}\n");
		return res.ToString();
	}
	
	public override void MarkAllForDeletion()
	{
		foreach(var buff in buffer.Values) buff.Delete();
	}
}
