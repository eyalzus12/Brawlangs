using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TestCreation : MapBase
{
	public const string PATH = "res://IcerMap.ini";
	
	public override void _Ready()
	{
		LoadCharacters();
		new MapCreator(PATH).Build(this);
	}
	
	/*public const float MARGIN = 50f;
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
	}*/
}
