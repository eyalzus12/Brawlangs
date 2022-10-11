using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MapBase : Node2D
{
	[Export]
	public Vector2 BlastZones = new Vector2(2300, 1200);
	[Export]
	public Vector2 CameraLimits = new Vector2(1300, 900);
	[Export]
	public Texture BackgroundTexture = null;
	
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
		var chars = LoadCharacters();
		
		SetDamageLabelLocations(chars.ToArray());
		
		//death handler
		var dh = new DeathHandeler(chars);
		dh.Name = "DeathHandler";
		AddChild(dh);
		dh.Connect("MatchEnds", this, nameof(MatchEnd));
		
		//camera
		var camera = new CameraFocus(chars, CameraLimits);
		camera.Name = "CameraFocus";
		AddChild(camera);
		
		//blastzones
		var bz = new BlastZone(Vector2.Zero, BlastZones);
		bz.Name = "Blastzones";
		AddChild(bz);
		
		//background
		var pb = new ParallaxBackground();
		pb.Name = "ParallaxBackground";
		pb.ScrollIgnoreCameraZoom = true;
		var pl = new ParallaxLayer();
		pl.Name = "BackgroundParallaxLayer";
		pl.MotionScale = Vector2.Zero;
		var sp = new BackgroundSprite();
		sp.Name = "BackgroundSprite";
		sp.Texture = BackgroundTexture;
		pl.AddChild(sp);
		pb.AddChild(pl);
		AddChild(pb);
	}
	
	public List<Character> LoadCharacters()
	{
		var data = this.GetPublicData();
		var chars = new List<Character>();
		for(int i = 0; i < 8 ; ++i)
		{
			object o = "";
			if(data.TryGet($"LoadedCharacter{i}", out o))
			{
				var s = o.s();
				var ch = PathToCharacter(s, i);
				chars.Add(ch);
			}
		}
		
		return chars;
	}
	
	public Character PathToCharacter(string path, int i)
	{
		var charname = path.SplitByLast('/')[1];
		path = $"{path}/{charname}.cfg";
		var cr = new CharacterCreator(path);
		var c = cr.Build(this, i);
		
		c.SpriteModulate = colorlist[i];
		c.Respawn();
		
		var im = new BufferInputManager(c.TeamNumber);
		im.Name = "InputManager";
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
		var lb = new DebugLabel(characters);
		lb.Name = "DebugLabel";
		cl.AddChild(lb);
		for(int i = 0; i < characters.Length; ++i)
		{
			var ch = characters[i];
			var v = locations[i];
			var dl = new DamageLabel(ch);
			dl.Name = $"DamageLabel{i}";
			dl.RectPosition = v;
			cl.AddChild(dl);
		}
		AddChild(cl);
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
		return counts.GetLabelLocations(leftedge,rightedge,MARGIN).ToArray();
	}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("exit_game")) ExitToMainMenu();
	}
	
	public override void _Notification(int what)
	{
		if(what == MainLoop.NotificationWmQuitRequest)
		{
			GetTree().Quit();
		}
	}
	
	public override void _Draw()
	{
		DrawRect(GeometryUtils.RectFrom(Vector2.Zero, BlastZones), new Color(1,0,1), false);
	}
	
	public void MatchEnd()
	{
		this.ChangeScene("res://ResultsScreen.tscn");
	}
	
	public void ExitToMainMenu()
	{
		this.ChangeScene(ProjectSettings.GetSetting("application/run/main_scene").s());
	}
}
