using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class KeybindsSetupHandler : Node
{
	public const string KEYBINDS_PATH = "user://Keybinds";
	public const string DEFAULT_KEYBINDS_PATH = "res://DefaultKeybinds";
	
	public List<KeybindsFile> DefaultProfiles{get; set;} = new List<KeybindsFile>();
	public List<KeybindsFile> Profiles{get; set;} = new List<KeybindsFile>();
	
	public override void _Ready()
	{
		LoadKeybinds();
	}
	
	public void LoadKeybinds()
	{
		foreach(var k in Profiles) k.Clear(); Profiles.Clear();
		foreach(var k in DefaultProfiles) k.Clear(); DefaultProfiles.Clear();
		
		DefaultProfiles = LoadAllProfiles(DEFAULT_KEYBINDS_PATH);
		Profiles = LoadAllProfiles(KEYBINDS_PATH);
		
		for(int i = Profiles.Count; i < DefaultProfiles.Count; ++i)//copy defaults if missing profiles
		{
			Utils.SaveFile($"{KEYBINDS_PATH}/Profile{i}", DefaultProfiles[i].ToString());
		}
	
		#if DEBUG_INPUT_MAP
		foreach(var h in InputMap.GetActions()) GD.Print(h);
		#endif
	}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("reload_keybinds"))
			LoadKeybinds();
	}
	
	public const string PROFILE_PATTERN = @"^Profile(?<profile>[0-9]+)$";
	public static readonly Regex PROFILE_REGEX = new Regex(PROFILE_PATTERN, RegexOptions.Compiled);
	public List<KeybindsFile> LoadAllProfiles(string path)
	{
		var result = new List<KeybindsFile>();
		
		var dir = new Directory();
		var er = dir.Open(path);
		if(er != Error.Ok)
		{
			GD.PushError($"[{nameof(KeybindsSetupHandler)}.cs]: Error opening folder {path}. Error is {er}");
			return result;
		}
		
		dir.ListDirBegin();
		string file;
		int i = 0;
		
		while((file = dir.GetNext()) != "")
		if(!dir.CurrentIsDir() && file[0] != '.')
		{
			var k = new KeybindsFile();
			k.Load($"{path}/{file}");
			var profile = int.Parse(PROFILE_REGEX.Match(file).Groups["profile"].Value);
			k.ApplyParsedData(profile);
			result.Add(k);
		}
		
		dir.ListDirEnd();
		return result;
	}
}
