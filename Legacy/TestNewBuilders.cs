using Godot;
using System;

public class TestNewBuilders : Node2D
{
	/*public override void _Ready()
	{
		var cr1 = new CharacterCreator("res://inicharactertest.ini");
		cr1.Build(this);
		var c1 = GetNode("Josh") as Character;
		c1.teamNumber = 0;
		c1.Respawn();
		
		var cr2 = new CharacterCreator("res://inicharactertest - Copy.ini");
		cr2.Build(this);
		var c2 = (GetNode("Mom") as Character);
		c2.teamNumber = 1;
		c2.dummy = true;
		//var dum = (DummyInputManager)this.GetRootNode("DummyInput");
		//foreach(var s in c2.states.Values) s.Inputs = dum;
		c2.Respawn();
		
		var camera = new MatchCamera();
		AddChild(camera);
		camera.Current = true;
		
		//var dmg = (GetNode("UI").GetNode("DMG")) as DamageLabel;
		//dmg.ch = c2;
	}*/
	
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
		foreach(var h in GetTree().Root.GetChildren())
			if(h is Character ch) 
				ch.CallDeferred("queue_free");
	}
}
