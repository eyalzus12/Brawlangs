using Godot;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class KeybindsFile
{
	public Dictionary<string, (float, List<InputEvent>)> Data{get; set;} = new Dictionary<string, (float, List<InputEvent>)>();
	public int Profile{get; set;} = 0;
	
	public KeybindsFile() {}
	
	public void Clear() => Data.Clear();
	
	public void Add(string action, float deadzone, List<InputEvent> events) => Data.TryAdd(action, (deadzone, events));
	
	public override string ToString()
	{
		var res = new StringBuilder();
		foreach(var ae in Data)
		{
			res.Append($"{ae.Key} = @{ae.Value.Item1}");
			foreach(var input in ae.Value.Item2)
				res.Append($" {TypeForInputEvent(input)}{DataForInputEvent(input)}{ExDataForInputEvent(input)}");
			res.AppendLine();
		}
		return res.ToString();
	}
	
	public Error Load(string path)
	{
		string content;
		var er = Utils.ReadFile(path, out content);
		if(er != Error.Ok) return er;
		Parse(content);
		return Error.Ok;
	}
	
	public void Parse(string s) => ParseData(RemoveRedundantLines(s));
	
	public void ApplyParsedData()
	{
		foreach(var ae in Data)
		{
			var an = $"P{Profile}_{ae.Key}";
			if(!InputMap.HasAction(an)) InputMap.AddAction(an, ae.Value.Item1);
			foreach(var k in ae.Value.Item2) InputMap.ActionAddEvent(an, k);
		}
	}
	
	private const string IGNORE_LINE = @"^(?:;.*)?[\s\n\r]*";
	private static readonly Regex IGNORE_REGEX = new Regex(IGNORE_LINE, RegexOptions.Compiled | RegexOptions.Multiline);
	private string RemoveRedundantLines(string s) => IGNORE_REGEX.Replace(s, "");
	
	private const string DATA_PARSER = @"^(?<action>\w+)\s*=\s*(?:@(?<deadzone>[+-]?[0-9]*\.?[0-9]*))?(?:\s+(?<type>[a-zA-Z]*)(?<data>[0-9]+)_?(?<exdata>[+-]?[0-9]*\.?[0-9]*)?)*\s*(?:;.*)?$";
	private static readonly Regex DATA_REGEX = new Regex(DATA_PARSER, RegexOptions.Compiled | RegexOptions.Multiline);
	private void ParseData(string s)
	{
		var matches = DATA_REGEX.Matches(s);
		if(matches.Count == 0)
		{
			GD.PushError($"[{nameof(KeybindsFile)}.cs]: Failed match on string {s}");
			return;
		}
		
		foreach(Match match in matches)
		{
			var groups = match.Groups;
			
			var action = groups["action"].Value;
			
			var string_deadzone = groups["deadzone"].Value;
			var deadzone = (string_deadzone == "")?0.5f:float.Parse(string_deadzone);
			
			var types = groups["type"].Captures.Cast<Capture>().Select(c=>c.Value[0]);
			var datas = groups["data"].Captures.Cast<Capture>().Select(c=>int.Parse(c.Value));
			var exdatas = groups["exdata"].Captures.Cast<Capture>().Select(c=>(c.Value=="")?1f:float.Parse(c.Value));
			
			var inputs = IterUtils.Zip(types, datas, exdatas).Select(CreateInput).ToList<InputEvent>();
			Data[action] = (deadzone, inputs);
		}
	}
	
	private InputEvent CreateInput((char, int, float) k)
	{
		InputEvent input = InputEventForType(k.Item1);
		
		input.Device = -1;
		
		input.Set("button_index", k.Item2);
		input.Set("scancode", k.Item2);
		input.Set("axis", k.Item2);
		
		//input.Set("factor", k.Item3);
		//input.Set("pressure", k.Item3);
		input.Set("axis_value", k.Item3);
		
		return input;
	}
	
	private InputEvent InputEventForType(char type)
	{
		switch(type)
		{
			case 'B': return new InputEventJoypadButton();
			case 'J': return new InputEventJoypadMotion();
			case 'K': return new InputEventKey();
			case 'M': return new InputEventMouseButton();
			default: throw new FormatException($"[KeybindsParser.cs]: Invalid input type {type}.");
		}
	}
	
	private string TypeForInputEvent(InputEvent @event)
	{
		if(@event is InputEventJoypadButton) return "B";
		if(@event is InputEventJoypadMotion) return "J";
		if(@event is InputEventKey) return "K";
		if(@event is InputEventMouseButton) return "M";
		return @event.GetType().Name;
	}
	
	private string DataForInputEvent(InputEvent @event)
	{
		if(@event is InputEventJoypadButton b) return b.ButtonIndex.ToString();
		if(@event is InputEventJoypadMotion j) return j.Axis.ToString();
		if(@event is InputEventKey k) return k.Scancode.ToString();
		if(@event is InputEventMouseButton m) return m.ButtonIndex.ToString();
		return "";
	}
	
	private string ExDataForInputEvent(InputEvent @event)
	{
		if(@event is InputEventJoypadMotion j) return "_" + j.AxisValue.ToString();
		return "";
	}
}
