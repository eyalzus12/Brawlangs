using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;

public class MapBase : Node2D
{
	[Export]
	public Rect2 BlastZones = BlastZone.CalcRect(new Vector2(512, 300), new Vector2(1500, 1000));
	
	public const string PACK_EXT = ".zip";
	
	public override void _Ready()
	{
		LoadCharacters();
		var camera = new MatchCamera();
		AddChild(camera);
		camera.Current = true;
		
		var bz = new BlastZone(BlastZones);
		AddChild(bz);
	}
	
	public void LoadCharacters()
	{
		var data = this.GetPublicData();
		data.Add("CurrentInfoLabelCharacter", 0);
		var chars = new List<Character>();
		foreach(var i in 1.To(8))
		{
			object o = "";
			if(data.TryGet("LoadedCharacter" + i, out o))
			{
				var s = o.s();
				var ch = PathToCharacter(s, i, data);
				chars.Add(ch);
			}
		}
		
		SetDamageLabelLocations(chars.ToArray());
	}
	
	public Character PathToCharacter(string path, int i, PublicData data)
	{
		if(StringUtils.GetExtension(path) == PACK_EXT)
		{
			var modfolderpath = StringUtils.GlobalizePath("res://LoadedMods");
			var zippath = StringUtils.GlobalizePath(path);
			ZipFile.ExtractToDirectory(zippath, modfolderpath);
			
			//ProjectSettings.LoadResourcePack(path, false);
			var foldertree = path.Split('/');
			var filename = foldertree[foldertree.Length-1];
			var charname = StringUtils.RemoveExtension(filename);
			path = $"res://LoadedMods/{charname}/{charname}.cfg";
			
			var folderpath = StringUtils.GlobalizePath($"res://LoadedMods/{charname}");
			object o = null;
			if(data.TryGet("ModResiduals", out o))
			{
				var residualList = o.ls();
				residualList.Add(folderpath);
				data.AddOverride("ModResiduals", residualList);
			}
			else data.Add("ModResiduals", new List<string>(new string[]{folderpath}));
		}
		
		var cr = new CharacterCreator(path);
		var c = cr.Build(this);
		c.teamNumber = i-1;
		var blue = new Color(0, 0, 1);
		var red = new Color(1, 0, 0);
		var green = new Color(0, 1, 0);
		var yellow = new Color(1, 1, 0);
		var megenta = new Color(1, 0, 1);
		var cyan = new Color(0, 1, 1);
		var grey = new Color(0.5f, 0.5f, 0.5f);
		var pink = new Color(1, 0.5f, 0.5f);
		Color[] colorlist = {blue, red, green, yellow, megenta, cyan, grey, pink};
				
		c.Modulate = colorlist[i-1];
		c.Respawn();
				
		var im = new BufferInputManager(c.teamNumber);
		c.AddChild(im);
		c.Inputs = im;
		return c;
	}
	
	public void SetDamageLabelLocations(Character[] characters)
	{
		var counts = new int[]{characters.Length};
		var locations = GetDamageLabelLocations(counts);
		var cl = new CanvasLayer();
		cl.Name = "UI";
		AddChild(cl);
		for(int i = 0; i < characters.Length; ++i)
		{
			var ch = characters[i];
			var v = locations[i];
			var lb = new DebugLabel();
			lb.ch = ch;
			cl.AddChild(lb);
			var dl = new DamageLabel(ch);
			dl.RectPosition = v;
			cl.AddChild(dl);
		}
	}
	
	public const float MARGIN = 50f;
	public const float CENTER_OFFSET = 100f;
	public Vector2[] GetDamageLabelLocations(int[] counts)
	{
		var width = ProjectSettings.GetSetting("display/window/size/width").f();
		var height = ProjectSettings.GetSetting("display/window/size/height").f();
		var windowsize = new Vector2(width, height);
		var topleft = Vector2.Zero;
		var bottomright = windowsize;
		var bottomleft = new Vector2(topleft.x, bottomright.y);
		var leftedge = new Vector2(bottomleft.x,bottomleft.y-CENTER_OFFSET);
		var rightedge = new Vector2(bottomright.x,bottomright.y-CENTER_OFFSET);
		return counts.GetLabelLocations(leftedge,rightedge,MARGIN);
	}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("next_label_character"))
		{
			var data = this.GetPublicData();
			var current = data["CurrentInfoLabelCharacter"].i();
			++current;
			if(current > 8) current = 0;
			data["CurrentInfoLabelCharacter"] = current;
		}
		
		if(Input.IsActionJustPressed("prev_label_character"))
		{
			var data = this.GetPublicData();
			var current = data["CurrentInfoLabelCharacter"].i();
			--current;
			if(current < 0) current = 8;
			data["CurrentInfoLabelCharacter"] = current;
		}
		
		if(Input.IsActionJustPressed("exit_game"))
		{
			Cleanup();
			
			GetTree()
				.CallDeferred("change_scene",
				ProjectSettings
				.GetSetting("application/run/main_scene"));
		}
	}
	
	public override void _Notification(int what)
	{
		if(what == MainLoop.NotificationWmQuitRequest)
		{
			Cleanup();
			GetTree().Quit();
		}
	}
	
	public void Cleanup()
	{
		var data = this.GetPublicData();
		object o = null;
		if(data.TryGet("ModResiduals", out o))
		{
			var reslist = o.ls();
			foreach(var s in reslist)
				System.IO.Directory.Delete(s, true);
			data.Remove("ModResiduals");
		}
		
		GetTree().Root.GetChildren().FilterType<Character>().ToList().ForEach(ch => ch.CallDeferred("queue_free"));
	}
}
