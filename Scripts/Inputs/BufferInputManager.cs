using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class BufferInputManager : InputManager
{
	protected const string CONFIG_PATH = "res://buffer.cfg";
		
	public Dictionary<string, BufferInfo> Buffer{get; set;} = new Dictionary<string, BufferInfo>();
		
	CfgFile Config{get; set;} = new CfgFile();
	
	private const string PROFILE_REMOVAL_PATTERN = @"^P[0-9]+_(?<action>.*?)$";
	private static readonly Regex PROFILE_REMOVAL_REGEX = new Regex(PROFILE_REMOVAL_PATTERN, RegexOptions.Compiled);
	public BufferInputManager(): base() {}
	
	//NOTE: must run after KeybindsSetupHandler
	public override void _Ready()
	{
		base._Ready();
		
		//reads from buffer.ini
		if(Config.Load(CONFIG_PATH) != Error.Ok)
		{
			GD.PushError($"failed to parse buffer config at path {CONFIG_PATH}");
			return;
		}
		
		foreach(string action in InputMap.GetActions())
		{
			var match = PROFILE_REMOVAL_REGEX.Match(action);
			if(!match.Success) continue;
			var baseAction = match.Groups["action"].Value;
			Buffer.Add(action, new BufferInfo(Config[baseAction, 1].i()));
		}
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventMouseMotion || @event.IsEcho()) return;
		
		#if DEBUG_INPUT_EVENTS
		GD.Print(@event.AsText());
		#endif
		
		foreach(string action in InputMap.GetActions()) if(@event.IsActionIgnoreDevice(action))
		{
			if(@event.IsPressed()) Input.ActionPress($"D{@event.Device}_" + action);
			else Input.ActionRelease($"D{@event.Device}_" + action);
			
			if(@event.IsPressed() && Buffer.ContainsKey(action))
			{
				var newName = $"D{@event.Device}_{action}";
				Buffer.TryAdd(newName, new BufferInfo(Buffer[action].DefaultBufferTime));
				Buffer[newName].Activate(-1);
			}
		}
		
		#if DEBUG_BUFFER
		GD.Print(this);
		#endif
	}
	
	public override void MarkForDeletion(string action, bool now=false)
	{
		if(Buffer.ContainsKey(action))
		{
			if(now) Buffer[action].Delete();
			else Buffer[action].MarkedForDeletion = true;
		}
	}
	
	public override bool IsActionJustPressed(string str)
	{
		if(Buffer.ContainsKey(str)) return Buffer[str].IsActive();
		else return base.IsActionJustPressed(str);
	}
	
	public override bool IsActionPressed(string str)
	{
		if(Buffer.ContainsKey(str)) return Buffer[str].IsActive() || base.IsActionPressed(str);
		else return base.IsActionPressed(str);
	}
	
	public override bool IsActionJustReleased(string str)
	{
		if(Buffer.ContainsKey(str)) return
			(base.IsActionJustReleased(str) && Buffer[str].BufferTimeLeft < 0) ||
			(!base.IsActionPressed(str) && Buffer[str].BufferTimeLeft == 0);
		else return base.IsActionJustReleased(str);
	}
	
	public override void _PhysicsProcess(float delta)
	{
		foreach(var buff in Buffer.Values)
		{
			if(buff.MarkedForDeletion) buff.Delete();
			else buff.Advance();
		}
	}
	
	public override string ToString() => ToString("");
	public override string ToString(string prefix)
	{
		var res = new StringBuilder();
		foreach(var entry in Buffer) if(entry.Key.StartsWith(prefix) && entry.Value.BufferTimeLeft >= 0) res.Append($"{{{entry.Key}, {entry.Value.ToString()}}}\n");
		return res.ToString();
	}
	
	public override void MarkAllForDeletion(string prefix = "")
	{
		foreach(var action in Buffer.Keys) if(action.StartsWith(prefix)) Buffer[action].Delete();
	}
}
