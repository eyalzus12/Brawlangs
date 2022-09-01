using Godot;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class KeybindsParser
{
	public static Error Load(string path)
	{
		File f = new File();//create new file
		var er = f.Open(path, File.ModeFlags.Read);//open file
		if(er != Error.Ok) return er;//if error, return
		string content = f.GetAsText();//read text
		f.Close();//flush buffer
		Parse(content);//parse
		return Error.Ok;
		//everything went well
		//unless there was a parse error
		//in which case an error was thrown anyways
	}
	
	private static void Parse(string s) => SplitToSections(EnsureNoPreData(RemoveRedundantLines(s))).ForEach(dat => ParseData(dat.Item1, dat.Item2));
	
	private const string IGNORE_LINE = @"^(?:;.*|[\s\n\r]*)";
	private static readonly Regex IGNORE_REGEX = new Regex(IGNORE_LINE, RegexOptions.Compiled | RegexOptions.Multiline);
	private static string RemoveRedundantLines(string s) => IGNORE_REGEX.Replace(s, "");
	
	private const string NO_DATA_PRE = @"^[0-9]+:";
	private static readonly Regex NO_PRE_REGEX = new Regex(NO_DATA_PRE, RegexOptions.Compiled);
	private static string EnsureNoPreData(string s)
	{
		if(!NO_PRE_REGEX.IsMatch(s)) throw new FormatException("Keybinds config: non comment lines before first section");
		return s;
	}
	
	private const string SECTION_SPLITTER = @"(?<section>[0-9]+):(?:\s*;\w*)?[\r\n]+(?<data>(?:.(?![0-9]+:))*)";
	private static readonly Regex SECTION_REGEX = new Regex(SECTION_SPLITTER, RegexOptions.Compiled | RegexOptions.Singleline);
	private static IEnumerable<(int, string)> SplitToSections(string s) => SECTION_REGEX
		.Matches(s)
		.Cast<Match>()
		.Select(m => (int.Parse(m.Groups["section"].Value), m.Groups["data"].Value));
	
	private const string DATA_PARSER = @"^(?<action>\w+)\s*=\s*(?:@(?<deadzone>[+-]?[0-9]*\.?[0-9]*))?(?:\s+(?<device>[0-9]+)_(?<type>[a-zA-Z]*)(?<data>[0-9]+))*\s*(?:;.*)?$";
	private static readonly Regex DATA_REGEX = new Regex(DATA_PARSER, RegexOptions.Compiled | RegexOptions.Multiline);
	private static void ParseData(int section, string s)
	{
		var matches = DATA_REGEX.Matches(s);
		foreach(Match match in matches)
		{
			var groups = match.Groups;
			
			var action = groups["action"].Value;
			var string_deadzone = groups["deadzone"].Value;
			var deadzone = (string_deadzone == "")?0.5f:float.Parse(string_deadzone);
			
			var actionName = $"{section}_{action}";
			if(!InputMap.HasAction(actionName)) InputMap.AddAction(actionName, deadzone);
			
			var devices = groups["device"].Captures.Cast<Capture>();
			var types = groups["type"].Captures.Cast<Capture>();
			var datas = groups["data"].Captures.Cast<Capture>();
			IterUtils
				.TriZip(devices, types, datas)
				.ForEach
				(
					t => InputMap.ActionAddEvent(
						actionName,
						CreateInput(int.Parse(t.Item1.Value), t.Item2.Value[0], int.Parse(t.Item3.Value))
					)
				);
		}
	}
	
	private static InputEvent CreateInput(int device, char type, int data)
	{
		InputEvent input = InputEventForType(type);
		input.Set("button_index", data);
		input.Set("scancode", data);
		input.Set("axis", data);
		input.Device = device;
		return input;
	}
	
	private static InputEvent InputEventForType(char type)
	{
		switch(type)
		{
			case 'B': return new InputEventJoypadButton();
			case 'J': return new InputEventJoypadMotion();
			case 'K': return new InputEventKey();
			case 'M': return new InputEventMouseButton();
			default: throw new FormatException($"Keybinds config: invalid input type {type}");
		}
	}
}
