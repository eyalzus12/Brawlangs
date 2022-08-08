using Godot;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class KeybindsParser
{
	private const string DATA_PATTERN = @"^(?<device>[0-9]+)_(?<type>[BJKM])(?<data>[0-9]+)$";
	private static readonly Regex DATA_REGEX = new Regex(DATA_PATTERN, RegexOptions.Compiled);
	
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
	
	private static void Parse(string s) => ParseAndApplyLines(s.Split('\n'));
	
	private static void ParseAndApplyLines(IEnumerable<string> lines) => ParseLines(lines)
		.ForEach(h => ApplyLine(h.Item1, h.Item2, h.Item3, h.Item4));
	
	private static void ApplyLine(int section, string action, float deadzone, IEnumerable<InputEvent> inputs)
	{
		var actionName = $"{section}_{action}";
		if(!InputMap.HasAction(actionName)) InputMap.AddAction(actionName, deadzone);
		inputs.ForEach(input => InputMap.ActionAddEvent(actionName, input));
	}
	
	private static IEnumerable<(int, string, float, IEnumerable<InputEvent>)> ParseLines(IEnumerable<string> lines)
	{
		IEnumerable<(int, IEnumerable<string>)> partitionedLines = PartitionLines(lines);
		int i = 0;
		foreach((int section, IEnumerable<string> sectionLines) in partitionedLines)
		{
			//GD.Print($"{i} {section}:");
			++i;
			foreach(var line in sectionLines)
			{
				//GD.Print($"{i} {line}");
				if(!CommentLine(line))
				{
					(string action, float deadzone, IEnumerable<InputEvent> inputs) = ParseLine(line, i);
					yield return (section, action, deadzone, inputs);
				}
				++i;
			}
		}
	}
	
	private static bool CommentLine(string s) => s.StartsWith(";") || string.IsNullOrWhiteSpace(s);
	
	private static IEnumerable<(int, IEnumerable<string>)> PartitionLines(IEnumerable<string> lines)
	{
		if(!lines.SkipWhile(CommentLine).First().EndsWith(":")) throw new FormatException("Keybinds config: data before first section");
		int? currentSection = null;
		IEnumerable<string> currentLines = Enumerable.Empty<string>();
		foreach(var line in lines)//go over lines
		{
			if(!CommentLine(line) && line.EndsWith(":"))//found section
			{
				if(currentSection != null) yield return (currentSection ?? 0, currentLines);
				currentSection = int.Parse(line.Substring(0, line.Length-1));
				currentLines = Enumerable.Empty<string>();
			}
			else
			{
				currentLines = currentLines.Append(line);
			}
		}
		yield return (currentSection ?? 0, currentLines);
	}
	
	private static (string, float, IEnumerable<InputEvent>) ParseLine(string s, int line)
	{
		var parts = s.Split('=');
		if(parts.Length != 2) throw new FormatException($"Keybinds config: invalid input format in line {line}");
		string action = parts[0].Trim();
		(float deadzone, IEnumerable<InputEvent> inputs) = ParseActionList(parts[1].Trim(), line);
		return (action, deadzone, inputs);
	}
	
	private static (float, IEnumerable<InputEvent>) ParseActionList(string s, int line)
	{
		if(s.Contains("@"))
		{
			var parts = s.Split(new char[]{' '}, 2);
			if(parts.Length != 2) throw new FormatException($"Keybinds config: invalid input format in line {line}");
			float deadzone = float.Parse(parts[0].Trim().Substring(1));
			IEnumerable<InputEvent> inputs = ParseInputLine(parts[1].Trim(), line);
			return (deadzone, inputs);
		}
		else
		{
			return (0.5f, ParseInputLine(s.Trim(), line));
		}
	}
	
	private static IEnumerable<InputEvent> ParseInputLine(string s, int line) => s.Split(' ').Select(h => ParseInputString(h.Trim(), line)).Where(h => h != null);
	
	private static InputEvent ParseInputString(string s, int line)
	{
		if(CommentLine(s)) return null;
		Match match = DATA_REGEX.Match(s);
		if(!match.Success) throw new FormatException($"Keybinds config: invalid input format {s} in line {line}");
		
		int device = int.Parse(match.Groups["device"].Value);
		string s_type = match.Groups["type"].Value;
		char type = s_type[0];
		if(s_type.Length > 1) throw new FormatException($"Keybinds config: invalid input type {s_type} in line {line}");
		int data = int.Parse(match.Groups["data"].Value);
		
		InputEvent input = GenerateInputEvent(type, line);
		input.Set("button_index", data);
		input.Set("scancode", data);
		input.Set("axis", data);
		input.Device = device;
		return input;
	}
	
	private static InputEvent GenerateInputEvent(char type, int line)
	{
		switch(type)
		{
			case 'B': return new InputEventJoypadButton();
			case 'J': return new InputEventJoypadMotion();
			case 'K': return new InputEventKey();
			case 'M': return new InputEventMouseButton();
			default: throw new FormatException($"Keybinds config: invalid input type {type} in line {line}");
		}
	}
}
