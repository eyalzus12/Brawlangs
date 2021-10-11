using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MatchCamera : Camera2D
{
	public List<Node2D> followed = new List<Node2D>();
	
	[Export(PropertyHint.Range, "0.1,0.5,or_greater")]
	public float zoomOffset = 0.5f;//no idea what this does
	[Export]
	public bool debugMode = false;
	[Export]
	public Vector2 limits = new Vector2(700, 500);//how far away from the screen center the camera is allowed to see into
	[Export]
	public Vector2 middle = new Vector2(448, 304);//center of screen
	[Export]
	public float interpolationWeight = 0.01f;//how interpolated the zoom and offset is
	[Export]
	public float baseZoom = 1.5f;//multiplier for CalculateZoom
	
	public Rect2 cameraRect = new Rect2();
	public Rect2 viewportRect = new Rect2();
	
	public override void _Ready()
	{
		Reset();
		SmoothingEnabled = true;
		
		viewportRect = GetViewportRect();
		SetProcess(followed.Count != 0);
	}
	
	public void Reset()
	{
		//TODO: linq
		foreach(var n in GetParent().GetChildren()) if(n is Character c)
		{
			followed.Add(c);
			c.Connect("Dead", this, nameof(CharacterGone));
		}
	}
	
	public override void _Process(float delta)
	{
		cameraRect = new Rect2(middle, Vector2.Zero);
		for(var i = 0; i < followed.Count; ++i)
		{
			//go over the followed characters, expanding the rect to them
			//the code here makes the expanded rect get more expanded the further you go from the center
			//being linear from 0 (no expand) to 1 (full expand)
			var pos = followed[i].Position;
			var x = pos.x-middle.x;
			var exX = x*Math.Abs(x/limits.x)+middle.x;
			var y = pos.y-middle.y;
			var exY = y*Math.Abs(y/limits.y)+middle.y;
			cameraRect = cameraRect.Expand(new Vector2(exX, exY));
		}
		cameraRect.Position -= middle;//get position relative to center
		cameraRect = cameraRect.Limit(limits.x, limits.y);//limit the rectangle to the limits
		cameraRect.Position += middle;//get new position
		Offset = Offset.LinearInterpolate(cameraRect.Center(), interpolationWeight);
		//interpolate between the desired position and the current one, to smoothen it out
		Zoom = Zoom.LinearInterpolate(baseZoom*CalculateZoom(cameraRect, viewportRect.Size), interpolationWeight);
		//interpolate between the desired zoom and the current one, to smoothen it out
		if(debugMode) Update();
	}
	
	public Vector2 CalculateZoom(Rect2 rect, Vector2 size)
	{
		//calculates the correct zoom for the camera rect and viewport size
		var maxX = Math.Max(1, rect.Size.x/size.x + zoomOffset);
		var maxY = Math.Max(1, rect.Size.y/size.y + zoomOffset);
		var maxZoom = Math.Max(maxX, maxY);
		return new Vector2(maxZoom, maxZoom);
	}
	
	public override void _Draw()
	{
		//debug draw of camera rect
		if(!debugMode) return;
		DrawRect(cameraRect, new Color(1, 1, 1), false);
		DrawCircle(cameraRect.Center(), 5, new Color(1, 1, 1));
	}
	
	public void CharacterGone(Node2D who)
	{
		followed.Remove(who);
	}
}
