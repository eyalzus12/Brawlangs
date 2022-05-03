using Godot;
using System;

public class ToMainMenuButton : OnPressButton
{
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("exit_game")) OnPress();
	}
	
	public override void OnPress()
	{
		this.ChangeScene(ProjectSettings.GetSetting("application/run/main_scene").s());
	}
}
