using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MapBase : Node2D
{
	[Export]
	public Rect2 BlastZones = BlastZone.CalcRect(new Vector2(512, 300), new Vector2(1500, 1000));
	
	public override void _Ready()
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
				var cr = new CharacterCreator(s);
				var c = cr.Build(this);
				c.teamNumber = i-1;
				var numberlabel = new DamageLabel(false);
				numberlabel.MarginTop = -75f;
				numberlabel.Text = c.teamNumber.ToString();
				c.AddChild(numberlabel);
				numberlabel.ch = c;
				
				var shader = ResourceLoader.Load<Shader>("res://colormult.shader");
				var material = new ShaderMaterial();
				material.Shader = shader;
				c.sprite.Material = material;
				var blue = new Vector3(0, 0, 1);
				var red = new Vector3(1, 0, 0);
				var green = new Vector3(0, 1, 0);
				var yellow = new Vector3(1, 1, 0);
				var megenta = new Vector3(1, 0, 1);
				var cyan = new Vector3(0, 1, 1);
				var grey = new Vector3(0.5f, 0.5f, 0.5f);
				var pink = new Vector3(1, 0.5f, 0.5f);
				Vector3[] colorlist = {blue, red, green, yellow, megenta, cyan, grey, pink};
				
				(c.sprite.Material as ShaderMaterial).SetShaderParam("color", colorlist[i-1]);
				c.Respawn();
				
				var im = new BufferInputManager(c.teamNumber);
				c.AddChild(im);
				c.Inputs = im;
				chars.Add(c);
			}
		}
		
		SetDamageLabelLocations(chars.ToArray());
		
		var camera = new MatchCamera();
		AddChild(camera);
		camera.Current = true;
		
		var bz = new BlastZone(BlastZones);
		AddChild(bz);
	}
	
	public const float MARGIN = 50f;
	public const float CENTER_OFFSET = 100f;
	public void SetDamageLabelLocations(Character[] characters)
	{
		var windowsize = OS.WindowSize;
		var topleft = Vector2.Zero;
		var bottomright = windowsize;
		var bottomleft = new Vector2(topleft.x, bottomright.y);
		var leftedge = new Vector2(bottomleft.x,bottomleft.y-CENTER_OFFSET);
		var rightedge = new Vector2(bottomright.x,bottomright.y-CENTER_OFFSET);
		var counts = new int[]{characters.Length};
		var locations = counts.GetLabelLocations(leftedge,rightedge,MARGIN);
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
			var dl = new DamageLabel();
			dl.ch = ch;
			dl.RectPosition = v;
			cl.AddChild(dl);
		}
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
			GetTree()
				.CallDeferred("change_scene",
				ProjectSettings
				.GetSetting("application/run/main_scene"));
					
			Cleanup();
		}
	}
	
	public void Cleanup()
	{
		GetTree().Root.GetChildren().FilterType<Character>().ToList().ForEach(ch => ch.CallDeferred("queue_free"));
	}
}
