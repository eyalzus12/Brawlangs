using Godot;
using System;

public partial class ToLoadingScreenButton : OnPressButton
{
	public override void OnPress()
	{
		this.ChangeSceneToFile("res://CharacterLoadingScreen.tscn");
	}
}
