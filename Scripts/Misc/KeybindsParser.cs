using Godot;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

using Action = System.ValueTuple<int, char, int, float>;
using ActionDict = System.Collections.Generic.Dictionary<string, (float, System.Collections.Generic.List<System.ValueTuple<int, char, int, float>>)>;

public class KeybindsParser
{
	public Dictionary<int, ActionDict> Data{get; set;} = new();
	
	public Error Load(string path)
	{
		GD.Print($"Loading keybinds from {path}");
		
		File f = new File();//create new file
		var er = f.Open(path, File.ModeFlags.Read);//open file
		if(er != Error.Ok) return er;//if error, return
		string content = f.GetAsText();//read text
		f.Close();//flush buffer
		Data.Clear();
		Parse(content);//parse
		
		GD.Print($"Finished loading keybinds from {path}");
		
		return Error.Ok;
		//everything went well
		//unless there was a parse error
		//in which case an error was thrown anyways
	}
	
	public void ApplyParsedData() => Data.ForEach(de => de.Value.ForEach(ae => 
	{
		var actionName = $"{de.Key}_{ae.Key}";
		if(!InputMap.HasAction(actionName)) InputMap.AddAction(actionName, ae.Value.Item1);
		ae.Value.Item2.ForEach(t => InputMap.ActionAddEvent(actionName, CreateInput(t.Item1, t.Item2, t.Item3, t.Item4)));
	}));
	
	public void Parse(string s) => SplitToSections(EnsureNoPreData(RemoveRedundantLines(s))).ForEach(dat => ParseData(dat.Item1, dat.Item2));
	
	private const string IGNORE_LINE = @"^(?:;.*|[\s\n\r]*)";
	private static readonly Regex IGNORE_REGEX = new(IGNORE_LINE, RegexOptions.Compiled | RegexOptions.Multiline);
	private string RemoveRedundantLines(string s) => IGNORE_REGEX.Replace(s, "");
	
	private const string NO_DATA_PRE = @"^[0-9]+:";
	private static readonly Regex NO_PRE_REGEX = new(NO_DATA_PRE, RegexOptions.Compiled);
	private string EnsureNoPreData(string s)
	{
		if(!NO_PRE_REGEX.IsMatch(s)) throw new FormatException("Keybinds config: non comment lines before first section");
		return s;
	}
	
	private const string SECTION_SPLITTER = @"(?<section>[0-9]+):(?:\s*;\w*)?[\r\n]+(?<data>(?:.(?![0-9]+:))*)";
	private static readonly Regex SECTION_REGEX = new(SECTION_SPLITTER, RegexOptions.Compiled | RegexOptions.Singleline);
	private IEnumerable<(int, string)> SplitToSections(string s) => SECTION_REGEX
		.Matches(s)
		.Cast<Match>()
		.Select(m => (int.Parse(m.Groups["section"].Value), m.Groups["data"].Value));
	
	private const string DATA_PARSER = @"^(?<action>\w+)\s*=\s*(?:@(?<deadzone>[+-]?[0-9]*\.?[0-9]*))?(?:\s+(?<device>[0-9]+)_(?<type>[a-zA-Z]*)(?<data>[0-9]+)_?(?<extra>[+-]?[0-9]*\.?[0-9]*)?)*\s*(?:;.*)?$";
	private static readonly Regex DATA_REGEX = new(DATA_PARSER, RegexOptions.Compiled | RegexOptions.Multiline);
	private void ParseData(int section, string s)
	{
		if(!Data.ContainsKey(section)) Data.Add(section, new ActionDict());
		
		var matches = DATA_REGEX.Matches(s);
		foreach(Match match in matches)
		{
			var groups = match.Groups;
			
			var action = groups["action"].Value;
			var string_deadzone = groups["deadzone"].Value;
			var deadzone = (string_deadzone == "")?0.5f:float.Parse(string_deadzone);
			
			if(!Data[section].ContainsKey(action)) Data[section].Add(action, (deadzone, new List<Action>()));
			
			var devices = groups["device"].Captures.Cast<Capture>().Select(c=>int.Parse(c.Value));
			var types = groups["type"].Captures.Cast<Capture>().Select(c=>c.Value[0]);
			var datas = groups["data"].Captures.Cast<Capture>().Select(c=>int.Parse(c.Value));
			var extras = groups["extra"].Captures.Cast<Capture>().Select(c=>(c.Value=="")?0f:float.Parse(c.Value));
			
			var inputs = IterUtils.Zip(devices, types, datas, extras);
			Data[section][action].Item2.AddRange(inputs);
		}
	}
	
	private InputEvent CreateInput(int device, char type, int data, float extra)
	{
		InputEvent input = InputEventForType(type);
		
		input.Device = device;
		
		input.Set("button_index", data);
		input.Set("keycode", data);
		input.Set("axis", data);
		
		input.Set("axis_value", extra);
		
		return input;
	}
	
	private InputEvent InputEventForType(char type) => type switch
	{
		'B' => new InputEventJoypadButton(),
		'J' => new InputEventJoypadMotion(),
		'K' => new InputEventKey(),
		'M' => new InputEventMouseButton(),
		_ => throw new FormatException($"Keybinds config: invalid input type {type}"),
	};
}
