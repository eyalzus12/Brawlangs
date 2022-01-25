using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MatchCamera : Camera2D
{
	public readonly static Vector2 DEFAULT_LIMITS = new Vector2(1300, 900);
	
	public List<Node2D> followed = new List<Node2D>();
	
	[Export(PropertyHint.Range, "0.1,0.5,or_greater")]
	public float zoomOffset = 0f;//no idea what this does
	[Export]
	public bool debugMode = false;
	[Export]
	public Vector2 limits = DEFAULT_LIMITS;//how far away from the screen center the camera is allowed to see into
	[Export]
	public Vector2 middle = Vector2.Zero;//center of screen
	[Export]
	public float interpolationWeight = 0.01f;//how interpolated the zoom and offset is
	[Export]
	public float baseZoom = 1.7f;//multiplier for CalculateZoom
	[Export]
	public float startZoomMult = 1.3f;
	[Export]
	public Vector2 startOffsetOffset = new Vector2(0, -500);
	
	public Vector2 center;
	
	public Rect2 cameraRect = new Rect2();
	public Rect2 viewportRect = new Rect2();
	
	public override void _Ready()
	{
		Reset();
		SmoothingEnabled = false;
		
		viewportRect = GetViewportRect();
		SetProcess(followed.Count != 0);
	}
	
	public void Reset()
	{
		Offset = middle+startOffsetOffset;
		Zoom = startZoomMult*baseZoom*Vector2.One;
		
		foreach(var n in GetParent().GetChildren()) if(n is Character c)
		{
			followed.Add(c);
			c.Connect("Dead", this, nameof(CharacterGone));
		}
	}
	
	public override void _Process(float delta)
	{
		//toggle debug display
		if(Input.IsActionJustPressed("toggle_camera_debug"))
			debugMode = !debugMode;
		
		if(OS.WindowMinimized) return;
		
		//get viewport rect
		viewportRect = GetViewportRect();
		
		var mx = middle.x-limits.x;
		var Mx = middle.x+limits.x;
		var my = middle.y-limits.y;
		var My = middle.y+limits.y;
		//get positions, confined inside the limits
		var positions = followed.Select(ch=>ch.Position.Clamp(mx,Mx,my,My));
		//get average position, including map center. this will be used for following
		center = positions.Concat(middle).Avg();
		//create a rect, starting at the center, that includes all positions
		
		var initialRect = new Rect2(center, Vector2.Zero);
		cameraRect = positions.Aggregate(initialRect, (a,v)=>a.Expand(v));
		cameraRect.Position -= middle;//make rect relative to map center
		cameraRect = cameraRect.Limit(limits.x, limits.y);//limit the rectangle to the limits
		cameraRect.Position += middle;//get non relative position
		//now cameraRect is in the desired place and size for the camera;
		//so we need to set the camera's position and zoom to match
		//interpolate between the desired position and the current one, to smoothen it out
		var desiredOffset = cameraRect.Center();
		Offset = Offset.LinearInterpolate(desiredOffset, interpolationWeight);
		//interpolate between the desired zoom and the current one, to smoothen it out
		var desiredZoom = CalculateZoom(cameraRect);
		Zoom = Zoom.LinearInterpolate(desiredZoom, interpolationWeight);
		
		//draw debug things
		Update();
	}
	
	public Vector2 CalculateZoom(Rect2 cameraRect)
	{
		//get the zoom that'll match on the xy
		var cameraZoomXY = cameraRect.Size/viewportRect.Size;
		//get desired matching zoom
		var cameraZoom = Math.Max(Math.Max(cameraZoomXY.x, cameraZoomXY.y), 1)+zoomOffset;
		//get max zoom possible on the xy
		var maxZoomXY = limits/viewportRect.Size;
		//get desired max zoom possible
		var maxZoom = Math.Min(maxZoomXY.x, maxZoomXY.y);
		//get resulting zoom that fits the max
		var zoomResult = baseZoom*Math.Min(cameraZoom, maxZoom);
		return zoomResult*Vector2.One;
	}
	
	public override void _Draw()
	{
		//debug draw of camera rect
		if(!debugMode) return;
		
		var White = new Color(1, 1, 1);
		var Red = new Color(1, 0, 0);
		var Blue = new Color(0, 0, 1);
		
		DrawRect(cameraRect, Red, false);
		var limitRect = BlastZone.CalcRect(middle, limits);
		DrawRect(limitRect, Red, false);
		var vrect = BlastZone.CalcRect(middle, viewportRect.Size);
		DrawRect(vrect, White, false);
		DrawCircle(cameraRect.Center(), 5, Red);
		DrawCircle(middle, 5, White);
		DrawCircle(center, 5, Blue);
	}
	
	public void CharacterGone(Node2D who) => followed.Remove(who);
}
