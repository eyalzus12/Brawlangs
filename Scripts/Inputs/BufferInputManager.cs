using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class BufferInputManager : InputManager
{
	protected const string CONFIG_PATH = "res://buffer.ini";
	protected const string LENGTH_SECTION = "length";
		
	public Dictionary<string, BufferInfo> buffer = 
		new Dictionary<string, BufferInfo>();
		
	IniFile config = new IniFile();
	
	//public bool filterByDeviceID = false;
	//public HashSet<int> deviceIDFilter = new HashSet<int>();
	
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
			GD.Print($"failed to parse buffer config at path {CONFIG_PATH}");
			return;
		}
		
		//GD.Print(config.ToString());
		
		foreach(var key in config[LENGTH_SECTION].Keys)
		{
			var res = config[LENGTH_SECTION, key, 1].i();
			buffer.Add($"{key}_{playerDeviceNumber}", new BufferInfo(res));
		}
	}
		
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventMouseMotion) return;
		//yeets the input if it's a mouse motion
		//because fuck mouse motions
		//GD.Print($"Input {@event.AsText()} has device id {@event.Device}");
		//GD.Print("filtering by:");
		//foreach(var i in deviceIDFilter) GD.Print(i);
		//GD.Print($"is {filterByDeviceID}");
		//GD.Print($"contain is {deviceIDFilter.Contains(@event.Device)}");
		//if(filterByDeviceID && !deviceIDFilter.Contains(@event.Device)) return;
		//yeets the input if it doesn't have the proper device ID
		//GD.Print("passed input");
		foreach(string action in InputMap.GetActions())
		{
			if(InputMap.ActionHasEvent(action, @event))
			{
				try
				{
					if(!@event.IsEcho() && @event.IsPressed())
					//only saves input if it was just pressed
						buffer[action].Activate(-1);
				}
				catch(KeyNotFoundException) {}
			}
		}
	}
	
	public override void MarkForDeletion(string action, bool now=false)
	{
		action = $"{action}_{playerDeviceNumber}";
		
		try
		{
			if(now) buffer[action].Delete();
			else buffer[action].markedForDeletion = true;
		}
		catch(KeyNotFoundException) {}
	}
	
	public override bool IsActionJustPressed(string str)
	{
		str = $"{str}_{playerDeviceNumber}";
		
		try
		{
			return buffer[str].IsActive();
		}
		catch(KeyNotFoundException)
		{
			return base.IsActionJustPressed(str);
		}
	}
	
	public override bool IsActionPressed(string str)
	{
		str = $"{str}_{playerDeviceNumber}";
		
		try
		{
			return buffer[str].IsActive() || base.IsActionPressed(str);
		}
		catch(KeyNotFoundException)
		{
			return base.IsActionPressed(str);
		}
	}
	
	public override bool IsActionJustReleased(string str)
	{
		str = $"{str}_{playerDeviceNumber}";
		
		try
		{
			return
			!base.IsActionPressed(str) && //ensure input not existent
			buffer[str].bufferTimeLeft == 0;//ensure just released
		}
		catch(KeyNotFoundException)
		{
			return base.IsActionJustReleased(str);
		}
	}
	
	public override bool IsActionReallyJustPressed(string str) => base.IsActionJustPressed($"{str}_{playerDeviceNumber}");
	public override bool IsActionReallyPressed(string str) => base.IsActionPressed($"{str}_{playerDeviceNumber}");
	public override bool IsActionReallyJustReleased(string str) => base.IsActionJustReleased($"{str}_{playerDeviceNumber}");
	
	public override void _PhysicsProcess(float delta)
	{
		foreach(var buff in buffer.Values)
		{
			if(buff.markedForDeletion) buff.Delete();
			else buff.Advance();
		}
	}
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var entry in buffer) res.Append($"{{{entry.Key}, {entry.Value.ToString()}_{playerDeviceNumber}}}\n");
		return res.ToString();
	}
	
	public override void MarkAllForDeletion()
	{
		foreach(var buff in buffer.Values)
			buff.Delete();
	}
}
