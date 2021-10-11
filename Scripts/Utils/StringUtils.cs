using Godot;
using System;
using System.Text.RegularExpressions;

public static class StringUtils
{
	public static string GetExtension(string str) => System.IO.Path.GetExtension(str);
	public static string RemoveExtension(string str) => System.IO.Path.GetFileNameWithoutExtension(str);
	public static string PascalCaseToSentence(string str) => Regex.Replace(str, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
	public static bool RegexMatch(this object o, string regex) => Regex.IsMatch(o.ToString(), regex);
	public static char FromEnd(this string s, int index) => s[s.Length-index];
}
