using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MapBase : Node2D
{
	[Export]
	public Vector2 BlastZones = new Vector2(2300, 1200);
	[Export]
	public Vector2 CameraLimits = new Vector2(1300, 900);
	
	public readonly static Color blue = new Color(0, 0, 1);
	public readonly static Color red = new Color(1, 0, 0);
	public readonly static Color green = new Color(0, 1, 0);
	public readonly static Color yellow = new Color(1, 1, 0);
	public readonly static Color megenta = new Color(1, 0, 1);
	public readonly static Color cyan = new Color(0, 1, 1);
	public readonly static Color grey = new Color(0.5f, 0.5f, 0.5f);
	public readonly static Color pink = new Color(1, 0.5f, 0.5f);
	public readonly static Color[] colorlist = {blue, red, green, yellow, megenta, cyan, grey, pink};
	
	public override void _Ready()
	{
		GD.Print("Starting map load");
		LoadCharacters();
		GD.Print("Finished loading characters");
		var camera = new MatchCamera();
		camera.limits = CameraLimits;
		AddChild(camera);
		camera.Current = true;
		
		var bz = new BlastZone(Vector2.Zero, BlastZones);
		AddChild(bz);
		
		GD.Print("Finished base map load");
	}
	
	public void LoadCharacters()
	{
		var data = this.GetPublicData();
		data.Add("CurrentInfoLabelCharacter", 0);
		var chars = new List<Character>();
		foreach(var i in 1.To(8))
		{
			object o = "";
			if(data.TryGet($"LoadedCharacter{i}", out o))
			{
				var s = o.s();
				var ch = PathToCharacter(s, i);
				chars.Add(ch);
			}
		}
		
		SetDamageLabelLocations(chars.ToArray());
		var dh = new DeathHandeler(chars);
		AddChild(dh);
		dh.Connect("MatchEnds",new Callable(this,nameof(MatchEnd)));
	}
	
	public Character PathToCharacter(string path, int i)
	{
		var charname = path.SplitByLast('/')[1];
		path = $"{path}/{charname}.cfg";
		var cr = new CharacterCreator(path);
		var c = cr.Build(this, i-1);
		
		c.SpriteModulate = colorlist[i-1];
		c.Respawn();
		
		var im = new BufferInputManager(c.TeamNumber);
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
			dl.Position = v;
			cl.AddChild(dl);
		}
	}
	
	public const float MARGIN = 50f;
	public const float CENTER_OFFSET = 100f;
	public Vector2[] GetDamageLabelLocations(int[] counts)
	{
		var width = ProjectSettings.GetSetting("display/window/size/viewport_width").f();
		var height = ProjectSettings.GetSetting("display/window/size/viewport_height").f();
		var windowsize = new Vector2(width, height);
		var topleft = Vector2.Zero;
		var bottomright = windowsize;
		var bottomleft = new Vector2(topleft.x, bottomright.y);
		var leftedge = new Vector2(bottomleft.x,bottomleft.y-CENTER_OFFSET);
		var rightedge = new Vector2(bottomright.x,bottomright.y-CENTER_OFFSET);
		return counts.GetLabelLocations(leftedge,rightedge,MARGIN).ToArray();
	}
	
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("next_label_character"))
		{
			var data = this.GetPublicData();
			var current = data["CurrentInfoLabelCharacter"].i();
			++current;
			if(current > 7) current = 0;
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
			ExitToMainMenu();
	}
	
	public override void _Notification(long what)
	{
		//Node.NOTIFICATION_WM_CLOSE_REQUEST
		/*if(what == 1006)
		{
			GetTree().Quit();
		}*/
	}
	
	public override void _PhysicsProcess(double delta) => QueueRedraw();
	
	public override void _Draw()
	{
		//if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		DrawRect(GeometryUtils.RectFrom(Vector2.Zero, BlastZones), new Color(1,0,1), false);
	}
	
	public void MatchEnd()
	{
		this.ChangeSceneToFile("res://ResultsScreen.tscn");
	}
	
	public void ExitToMainMenu()
	{
		this.ChangeSceneToFile(ProjectSettings.GetSetting("application/run/main_scene").s());
	}
}
