using Godot;
using System;

public class BackgroundSprite : Sprite
{
	public BackgroundSprite(): base() {}
	
	public override void _Process(float delta)
	{
		Scale = GetViewport().Size / Texture.GetSize();
	}
}
