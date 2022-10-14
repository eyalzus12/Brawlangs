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
	
	//NOTE: must run before BufferInputManager
	public override void _Ready()
	{
		LoadKeybinds();
	}
	
	public void LoadKeybinds()
	{
		var d = new Directory();
		if(!d.DirExists(KEYBINDS_PATH)) d.MakeDir(KEYBINDS_PATH);//create dir if doesn't exist
		
		foreach(var k in Profiles) k.Clear(); Profiles.Clear();
		foreach(var k in DefaultProfiles) k.Clear(); DefaultProfiles.Clear();
		
		DefaultProfiles = LoadAllProfiles(DEFAULT_KEYBINDS_PATH);
		Profiles = LoadAllProfiles(KEYBINDS_PATH);
		
		for(int i = 0; i < Profiles.Count; ++i) Profiles[i].ApplyParsedData();
		
		for(int i = Profiles.Count; i < DefaultProfiles.Count; ++i)//copy defaults if missing profiles
		{
			DefaultProfiles[i].ApplyParsedData();
			Utils.SaveFile($"{KEYBINDS_PATH}/Profile{i}.kbd", DefaultProfiles[i].ToString());
		}
	}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("reload_keybinds"))
			LoadKeybinds();
	}
	
	public const string PROFILE_PATTERN = @"^Profile(?<profile>[0-9]+).kbd$";
	public static readonly Regex PROFILE_REGEX = new Regex(PROFILE_PATTERN, RegexOptions.Compiled);
	public List<KeybindsFile> LoadAllProfiles(string path)
	{
		var result = new List<KeybindsFile>();
		List<string> files;
		var er = Utils.ListDirectoryFiles(path, out files);
		if(er != Error.Ok) return result;
		
		foreach(var file in files)
		{
			var match = PROFILE_REGEX.Match(file);
			
			int profile;
			if(!int.TryParse(match.Groups["profile"].Value, out profile))
			{
				GD.PushError($"[{nameof(KeybindsSetupHandler)}.cs]: Bad keybinds profile file name {file} found");
				continue;
			}
			
			var k = new KeybindsFile();
			k.Profile = profile;
			k.Load($"{path}/{file}");
			result.Add(k);
		}
		
		return result;
	}
}
