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
	public Vector2 limits = new Vector2(700, 700);//how far away from the screen center the camera is allowed to see into
	[Export]
	public Vector2 middle = new Vector2(512, 300);//new Vector2(448, 304);//center of screen
	[Export]
	public float interpolationWeight = 0.01f;//how interpolated the zoom and offset is
	[Export]
	public float baseZoom = 1.5f;//multiplier for CalculateZoom
	[Export]
	public float startZoomMult = 1.3f;
	[Export]
	public Vector2 startOffsetOffset = new Vector2(0, -500);
	[Export]
	public float maxCenterOffset = 100f;
	
	public Vector2 OffsetPos;
	
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
		Offset = middle+startOffsetOffset;
		Zoom = new Vector2(baseZoom,baseZoom)*startZoomMult;
		
		//TODO: linq
		foreach(var n in GetParent().GetChildren()) if(n is Character c)
		{
			followed.Add(c);
			c.Connect("Dead", this, nameof(CharacterGone));
		}
	}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_camera_debug"))
			debugMode = !debugMode;
		
		viewportRect = GetViewportRect();
		var center = followed.Select(ch=>ch.Position).Avg();
		OffsetPos = middle.MoveToward(center, maxCenterOffset);
		cameraRect = new Rect2(OffsetPos, Vector2.Zero);
		foreach(var ch in followed)
		{
			//go over the followed characters, expanding the rect to them
			//the code here makes the expanded rect get more expanded the further you go from the middle
			//being linear from 0 (no expand) to 1 (full expand)
			
			var pos = ch.Position;//get position
			var rpos = pos-OffsetPos;//get position relative to middle
			var frac = rpos/limits;//get position fraction
			var afrac = frac.Abs().Min(1,1);//get absoulte position fraction
			var nrpos = rpos*afrac;//get new relative position
			var npos = nrpos+OffsetPos;//get new posiiton
			cameraRect = cameraRect.Expand(npos);//expand rect to position
		}
		
		cameraRect.Position -= middle;//get position relative to center
		cameraRect = cameraRect.Limit(limits.x, limits.y);//limit the rectangle to the limits
		cameraRect.Position += middle;//get new position
		Offset = Offset.LinearInterpolate(cameraRect.Center(), interpolationWeight);
		//interpolate between the desired position and the current one, to smoothen it out
		Zoom = Zoom.LinearInterpolate(CalculateZoom(cameraRect, viewportRect.Size), interpolationWeight);
		//interpolate between the desired zoom and the current one, to smoothen it out
		Update();
	}
	
	public Vector2 CalculateZoom(Rect2 rect, Vector2 size)
	{
		//calculates the correct zoom for the camera rect and viewport size
		var zoomVector = (rect.Size/size + zoomOffset.Diagonal()).Max(1,1);
		var maxZoom = baseZoom*Math.Max(zoomVector.x, zoomVector.y);
		return maxZoom.Diagonal();
	}
	
	public override void _Draw()
	{
		//debug draw of camera rect
		if(!debugMode) return;
		
		var White = new Color(1, 1, 1);
		var Red = new Color(1, 0, 0);
		var Blue = new Color(0, 0, 1);
		
		var limitRect = BlastZone.CalcRect(middle, limits);
		DrawRect(cameraRect, Red, false);
		DrawRect(limitRect, Red, false);
		DrawRect(viewportRect, White, false);
		DrawCircle(cameraRect.Center(), 5, Red);
		DrawCircle(middle, 5, White);
		DrawCircle(OffsetPos, 5, Blue);
	}
	
	public void CharacterGone(Node2D who) => followed.Remove(who);
}
