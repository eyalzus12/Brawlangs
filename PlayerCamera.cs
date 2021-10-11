using Godot;
using System;

public class PlayerCamera : Camera2D
{
	public const float MAX_ZOOM_IN = 0.9f;
	public const float MAX_ZOOM_OUT = 4.0f;
	public const float ZOOM_IN_INTERVAL = 0.01f;
	public const float ZOOM_OUT_INTERVAL = 0.01f;
	
	public bool exist = true;
	
	public override void _Ready()
	{
		Character ch = (Character)GetParent();
		exist = !ch.dummy;
		
		Current = exist;
	}
	
	public override void _Process(float delta)
	{
		if(!exist) return;
		
		if(Input.IsActionJustPressed("camera_toggle"))
			Current = !Current;
		
		if(Input.IsActionPressed("camera_zoom_in"))
		{
			if(Zoom.x >= MAX_ZOOM_IN) 
				Zoom = new Vector2(Zoom.x - ZOOM_IN_INTERVAL, Zoom.y);
			if(Zoom.y >= MAX_ZOOM_IN) 
				Zoom = new Vector2(Zoom.x, Zoom.y - ZOOM_IN_INTERVAL);
		}
		
		if(Input.IsActionPressed("camera_zoom_out"))
		{
			if(Zoom.x <= MAX_ZOOM_OUT) 
				Zoom = new Vector2(Zoom.x + ZOOM_OUT_INTERVAL, Zoom.y);
			if(Zoom.y <= MAX_ZOOM_OUT)
				Zoom = new Vector2(Zoom.x, Zoom.y + ZOOM_OUT_INTERVAL);
		}
	}
}
