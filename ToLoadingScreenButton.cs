using Godot;
using System;

public class ToLoadingScreenButton : OnPressButton
{
	public override void OnPress()
	{
		this.ChangeScene("res://CharacterLoadingScreen.tscn");
	}
}
