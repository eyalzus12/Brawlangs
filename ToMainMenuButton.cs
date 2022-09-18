using Godot;
using System;

public partial class ToMainMenuButton : OnPressButton
{
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("exit_game")) OnPress();
	}
	
	public override void OnPress()
	{
		this.ChangeSceneToFile(ProjectSettings.GetSetting("application/run/main_scene").AsString());
	}
}
