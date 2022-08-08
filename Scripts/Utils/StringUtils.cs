using Godot;
using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

public static class StringUtils
{
	public const string FLOAT_PATTERN = @"[+-]?(?:\d*.)?\d+";
	
	public static string GetExtension(string str) => System.IO.Path.GetExtension(str);
	public static string RemoveExtension(string str) => System.IO.Path.GetFileNameWithoutExtension(str);
	public static string PascalCaseToSentence(string str) => Regex.Replace(str, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
	public static bool RegexMatch(this object o, string regex) => Regex.IsMatch(o.ToString(), regex);
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
		var result = new string[2]{"",""};
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
	
	public readonly static HashSet<char> VOWELS = new HashSet<char>(new char[]{'a','A','e','E','i','I','o','O','u','U'});
	public static string AAN(this string s)
	{
		var firstchar = s[0];
		if(VOWELS.Contains(firstchar)) return $"an {s}";
		else return $"a {s}";
	}
}
