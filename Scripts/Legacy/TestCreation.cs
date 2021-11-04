using Godot;
using System;
using System.Linq;

public class TestCreation : Node2D
{
	public const string path = "res://IcerMap.ini";
	
	public override void _Ready()
	{
		var data = this.GetPublicData();
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
		
		new MapCreator(path).Build(this);
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
	}
}
