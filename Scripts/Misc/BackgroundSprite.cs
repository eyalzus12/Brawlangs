using Godot;
using System;

public class BackgroundSprite : Sprite
{
	public BackgroundSprite(): base() {}
	
	public override void _Process(float delta)
	{
		if(Texture is null) return;
		Scale = GetViewport().Size / Texture.GetSize();
	}
}
