using Godot;
using System;
using System.Text.RegularExpressions;

public class SelectScreenLineEdit : LineEdit
{
	public int number = 0;
	public LineEdit profile;
	public LineEdit device;
	public const string PREFIX = "LoadedCharacter";
	
	private PublicData data;
	
	public const string NAME_PATTERN = @"Load(?<number>[0-9]+)Path";
	public static readonly Regex NAME_REGEX = new Regex(NAME_PATTERN, RegexOptions.Compiled);
	
	public override void _Ready()
	{
		number = int.Parse(NAME_REGEX.Match(Name).Groups["number"].Value);
		var datanode = GetParent().GetNode($"Load{number}Data");
		profile = datanode.GetNode<LineEdit>($"Load{number}Profile");
		device = datanode.GetNode<LineEdit>($"Load{number}Device");
		
		data = this.GetPublicData();
		object o;
		if(data.TryGetValue($"{PREFIX}{number-1}", out o) && o is ValueTuple<string, int, int> v)
		{
			Text = v.Item1;
			profile.Text = v.Item2.ToString();
			device.Text = v.Item3.ToString();
		}
		
		//bruh wtf is this
		GetParent().GetParent().GetParent().GetNode<Button>("ExitButton").Connect("pressed", this, nameof(OnExit));
	}
	
	public void OnExit()
	{
		if(Text == "")
		{
			data.Remove($"{PREFIX}{number-1}");
		}
		else
		{
			data[$"{PREFIX}{number-1}"] = (Text,int.Parse(profile.Text),int.Parse(device.Text));
		}
	}
}
