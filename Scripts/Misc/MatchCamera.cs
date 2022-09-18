using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MatchCamera : Camera2D
{
	public readonly static Vector2 DEFAULT_LIMITS = new Vector2(1300, 900);
	
	public List<Node2D> followed = new();
	
	[Export(PropertyHint.Range, "0.1,0.5,or_greater")]
	public float zoomOffset = 0.5f;//no idea what this does
	[Export]
	public bool debugMode = false;
	[Export]
	public Vector2 limits = DEFAULT_LIMITS;//how far away from the screen center the camera is allowed to see into
	[Export]
	public Vector2 middle = Vector2.Zero;//center of screen
	[Export]
	public float interpolationWeight = 0.05f;//how interpolated the zoom and offset is
	[Export]
	public float curveMultiplier = 0.8f;
	[Export]
	public float baseZoom = 2.1f;//multiplier for CalculateZoom
	[Export]
	public float startZoomMult = 5f;
	[Export]
	public Vector2 startPositionOffset = new Vector2(0, -500);
	
	public Vector2 center;
	
	public Rect2 cameraRect = new();
	public Rect2 viewportRect = new();
	public bool limit = true;
	
	public override void _Ready()
	{
		Reset();
		SmoothingEnabled = false;
		
		viewportRect = GetViewportRect();
		SetProcess(followed.Count != 0);
	}
	
	public void Reset()
	{
		limit = true;
		Position = middle+startPositionOffset;
		Zoom = startZoomMult*baseZoom*Vector2.One;
		
		foreach(var n in GetParent().GetChildren()) if(n is Character c)
		{
			followed.Add(c);
			c.Connect("Dead",new Callable(this,nameof(CharacterGone)));
		}
	}
	
	public override void _Process(double delta)
	{
		//toggle debug display
		if(Input.IsActionJustPressed("toggle_camera_debug"))
			debugMode = !debugMode;
		if(Input.IsActionJustPressed("toggle_camera_limits"))
			limit = !limit;
		
		//get viewport rect
		viewportRect = GetViewportRect();
		
		var mx = middle.x-limits.x;
		var Mx = middle.x+limits.x;
		var my = middle.y-limits.y;
		var My = middle.y+limits.y;
		
		//get positions, confined inside the limits
		var positions = followed.Select(ch=>ch.Position.Clamp(mx,Mx,my,My)).Append(middle);
		
		//get average position, including map center. this will be used for following
		center = positions.Avg();
		
		//create a rect, starting at the center, that includes all positions, and is a bit zoomed out
		var initialRect = new Rect2(center, Vector2.Zero);
		cameraRect = positions.Aggregate(initialRect, (a,v)=>a.Expand(v));
		
		//limit rect
		if(limit)
		{
			cameraRect.Position -= middle;//make rect relative to map center
			cameraRect = cameraRect.Limit(limits.x, limits.y);//limit the rectangle to the limits
			cameraRect.Position += middle;//get non relative position
		}
		
		//interpolate between the desired position and the current one, to smoothen it out
		var desiredPosition = cameraRect.Center();//get center (desired position)
		Position = Position.SmoothLinearInterpolate(desiredPosition, interpolationWeight, curveMultiplier);
		
		//interpolate between the desired zoom and the current one, to smoothen it out
		var desiredZoom = CalculateZoom(cameraRect);
		Zoom = Zoom.SmoothLinearInterpolate(desiredZoom, interpolationWeight, curveMultiplier);
		if(float.IsNaN(Zoom.x) || float.IsNaN(Zoom.y)) Zoom = Vector2.One;
		
		//draw debug things
		QueueRedraw();
	}
	
	public Vector2 CalculateZoom(Rect2 cameraRect, bool addOffset = true)
	{
		//don't change zoom if the viewport rect has 0 size
		if(viewportRect.Size.x == 0 || viewportRect.Size.y == 0) return Vector2.One; 
		
		//get the zoom that'll match on the xy
		var cameraZoomXY = cameraRect.Size/viewportRect.Size;
		//get desired matching zoom
		var cameraZoom = Math.Max(Math.Max(cameraZoomXY.x, cameraZoomXY.y), 1);
		//get max zoom possible on the xy
		var maxZoomXY = limits/viewportRect.Size;
		//get max total zoom possible (conditionally)
		var maxZoom = limit?Math.Max(maxZoomXY.x, maxZoomXY.y):float.PositiveInfinity;
		//get resulting zoom that fits the max
		return Math.Min(cameraZoom*baseZoom+zoomOffset, maxZoom)*Vector2.One;
	}
	
	public override void _Draw()
	{
		//debug draw of camera rect
		if(!debugMode) return;
		
		DrawSetTransform(-Position,0f,Vector2.One);
		
		var White = new Color(1, 1, 1);
		var Red = new Color(1, 0, 0);
		var Blue = new Color(0, 0, 1);
		var Yellow = new Color(1, 1, 0);
		var Orange = new Color(1, 0.5f, 0);
		
		DrawRect(cameraRect, Yellow, false);
		var limitRect = GeometryUtils.RectFrom(middle, limits);
		DrawRect(limitRect, Red, false);
		DrawRect(viewportRect, White, false);
		DrawRect(viewportRect.Zoomed(Zoom), Blue, false);
		DrawCircle(cameraRect.Center(), 5, Red);
		DrawCircle(Position, 5, Yellow);
		DrawCircle(middle, 5, White);
		DrawCircle(center, 5, Blue);
		//.Zoomed(CalculateZoom(GeometryUtils.RectFrom(middle, viewportRect.Size))
	}
	
	public void CharacterGone(Node2D who) => followed.Remove(who);
}
