using Godot;
using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

public static class StringUtils
{
	public static string GetExtension(string str) => System.IO.Path.GetExtension(str);
	public static string RemoveExtension(string str) => System.IO.Path.GetFileNameWithoutExtension(str);
	public static string PascalCaseToSentence(string str) => Regex.Replace(str, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
	public static char FromEnd(this string s, int index) => s[s.Length-index];
	
	public static string GlobalizePath(string path)
	{
		if(OS.HasFeature("editor"))
			return ProjectSettings.GlobalizePath(path);
		else
		{
			var filename = path.Substring("res://".Length);
			return OS.GetExecutablePath().GetBaseDir().PlusFile(filename);
		}
	}
	
	public static string[] SplitByLast(this string s, char c)
	{
		int idx = s.LastIndexOf(c);
		string[] result = {"",""};
		if (idx != -1)
		{
			result[0] = s.Substring(0, idx);
			result[1] = s.Substring(idx + 1);
		}
		return result;
	}
	
	public static string IntToWord(int i) => i + IntToWordSuffix(i);
	
	public static string IntToWordSuffix(int i)
	{
		var s = i.ToString();
		var c = s.FromEnd(1);
		var lastnum = (int)(c-'0');
		switch(lastnum)
		{
			case 1: return "st";
			case 2: return "nd";
			case 3: return "rd";
			default: return "th";
		}
	}
	
	public readonly static HashSet<char> VOWELS = new HashSet<char>{'a','A','e','E','i','I','o','O','u','U'};
	public static string AAN(this string s)
	{
		var firstchar = s[0];
		if(VOWELS.Contains(firstchar)) return $"an {s}";
		else return $"a {s}";
	}
	
	public const string FLOAT_PATTERN = @"[+-]?[0-9]*\.?[0-9]*";
	
	public static readonly string V2_PATTERN = $@"^\s*\(\s*(?<x>{FLOAT_PATTERN})\s*,\s*(?<y>{FLOAT_PATTERN})\s*\)\s*$";
	public static readonly Regex V2_REGEX = new Regex(V2_PATTERN, RegexOptions.Compiled);
	public static bool TryParseVector2(string s, out Vector2 v)
	{
		var match = V2_REGEX.Match(s);
		float x,y;
		if(match.Success
			&& float.TryParse(match.Groups["x"].Value, out x)
			&& float.TryParse(match.Groups["y"].Value, out y))
		{
			v = new Vector2(x,y);
			return true;
		}
		else
		{
			v = default;
			return false;
		}
	}
	
	public static readonly string V3_PATTERN = $@"^\s*\(\s*(?<x>{FLOAT_PATTERN})\s*,\s*(?<y>{FLOAT_PATTERN})\s*,\s*(?<z>{FLOAT_PATTERN})\s*\)\s*$";
	public static readonly Regex V3_REGEX = new Regex(V3_PATTERN, RegexOptions.Compiled);
	public static bool TryParseVector3(string s, out Vector3 v)
	{
		var match = V3_REGEX.Match(s);
		float x,y,z;
		if(match.Success
			&& float.TryParse(match.Groups["x"].Value, out x)
			&& float.TryParse(match.Groups["y"].Value, out y)
			&& float.TryParse(match.Groups["z"].Value, out z))
		{
			v = new Vector3(x,y,z);
			return true;
		}
		else
		{
			v = default;
			return false;
		}
	}
	
	public static readonly string Q_PATTERN = $@"^\s*\(\s*(?<x>{FLOAT_PATTERN})\s*,\s*(?<y>{FLOAT_PATTERN})\s*,\s*(?<z>{FLOAT_PATTERN})\s*,\s*(?<w>{FLOAT_PATTERN})\s*\)\s*$";
	public static readonly Regex Q_REGEX = new Regex(Q_PATTERN, RegexOptions.Compiled);
	public static bool TryParseQuat(string s, out Quat v)
	{
		var match = Q_REGEX.Match(s);
		float x,y,z,w;
		if(match.Success
			&& float.TryParse(match.Groups["x"].Value, out x)
			&& float.TryParse(match.Groups["y"].Value, out y)
			&& float.TryParse(match.Groups["z"].Value, out z)
			&& float.TryParse(match.Groups["w"].Value, out w))
		{
			v = new Quat(x,y,z,w);
			return true;
		}
		else
		{
			v = default;
			return false;
		}
	}
	
	public static IEnumerable<string> LeftQuotient(this IEnumerable<string> e, string q)
	{
		foreach(var s in e) if(s.StartsWith(q)) yield return s.Substring(q.Length);
	}
	
	public static IEnumerable<string> RightQuotient(this IEnumerable<string> e, string q)
	{
		foreach(var s in e) if(s.EndsWith(q)) yield return s.Substring(0, s.Length-q.Length);
	}
	
	public static bool EqualsIgnoreCase(this string s1, string s2) => string.Equals(s1, s2,  StringComparison.OrdinalIgnoreCase);
}
