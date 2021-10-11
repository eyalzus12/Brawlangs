using Godot;
using System;
using System.Linq;

public class MapBase : Node2D
{
	public override void _Ready()
	{
		/*var loaded1 = (PackedScene)ResourceLoader
			.Load("res://Character.tscn");
		
		Character instance1 = (Character)(loaded1?.Instance());
		
		var loaded2 = (PackedScene)ResourceLoader
			.Load("res://TestCharacter.tscn");
				
		Character instance2 = (Character)(loaded2?.Instance());
		instance2.dummy = true;
		
		object path_obj = this.GetData("ini_path");
		if(!(path_obj is null))
		{
			string path = (string)path_obj;
			instance1.statConfigPath = path;
			instance2.statConfigPath = path;
		}
		
		
		GetTree().Root.CallDeferred("add_child", instance1);
		GetTree().Root.CallDeferred("add_child", instance2);*/
		
		/*var cr1 = new CharacterCreator("res://inicharactertest.ini");
		var c1 = cr1.Build(this);
		c1.teamNumber = 0;
		c1.Respawn();
		
		var cr2 = new CharacterCreator("res://inicharactertest - Copy.ini");
		var c2 = cr2.Build(this);
		c2.teamNumber = 1;
		c2.dummy = true;
		c2.Respawn();*/
		
		var data = this.GetPublicData();
		data.Add("CurrentInfoLabelCharacter", 0);
		foreach(var i in 1.To(8))
		{
			object o = "";
			if(data.TryGet("LoadedCharacter" + i, out o))
			{
				var s = o.s();
				var cr = new CharacterCreator(s);
				var c = cr.Build(this);
				c.teamNumber = i-1;
				//c.dummy = data.GetOrDefault($"LoadedCharacter{i}Dummy", false).b();
				c.Respawn();
				
				var im = new BufferInputManager(c.teamNumber);
				c.AddChild(im);
				c.Inputs = im;
				//c.SetDeviceIDFilterForInputManager();
			}
		}
		
		var camera = new MatchCamera();
		AddChild(camera);
		camera.Current = true;
		
		var bz = new BlastZone(new Vector2(512, 300), new Vector2(2000, 1200));
		AddChild(bz);
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
