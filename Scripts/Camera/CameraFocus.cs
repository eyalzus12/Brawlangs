using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CameraFocus : Node2D
{
	public readonly static Vector2 DEFAULT_LIMITS = new Vector2(1300, 900);
	public const float CAMERA_SPEED = 6.5f;
	public const float MIN_ZOOM = 1.5f;
	public const float ZOOM_MULT = 1.3f;
	
	public List<Node2D> Followed{get; set;} = new List<Node2D>();
	
	private bool _debug = false;
	public bool Debug{get => _debug; set {_debug = value; Update();}}
	
	public Vector2 Limits{get; set;} = DEFAULT_LIMITS;
	
	public bool LimitOn{get; set;} = true;
	
	public Camera2D FollowingCamera{get; set;}
	
	public CameraFocus() {}
	
	public CameraFocus(IEnumerable<Node2D> toFollow, Vector2 limits)
	{
		Followed = new List<Node2D>(toFollow);
		Limits = limits;
	}
	
	public override void _Ready()
	{
		SetProcess(Followed.Count > 0);
		
		Followed.ForEach(c => c.Connect("Dead", this, nameof(CharacterGone)));
		
		FollowingCamera = new Camera2D();
		FollowingCamera.Name = "FollowingCamera";
		
		FollowingCamera.SmoothingEnabled = true;
		FollowingCamera.SmoothingSpeed = CAMERA_SPEED;
		FollowingCamera.Current = true;
		AddChild(FollowingCamera);
	}

	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_camera_debug"))
			Debug = !Debug;
		if(Input.IsActionJustPressed("toggle_camera_limits"))
			LimitOn = !LimitOn;
		
		var desiredRect = Followed
			.Select(n=>n.Position
				.Clamp(
					FollowingCamera.LimitLeft,
					FollowingCamera.LimitRight,
					FollowingCamera.LimitTop,
					FollowingCamera.LimitBottom
				)
			)
			.Aggregate(new Rect2(Vector2.Zero, Vector2.Zero), (a,v)=>a.Expand(v));
		
		Position = desiredRect.Center();
		
		var cameraZoomXY = desiredRect.Size/GetViewportRect().Size;
		var cameraZoom = Math.Max(Math.Max(cameraZoomXY.x, cameraZoomXY.y), MIN_ZOOM);
		var maxZoomXY = Limits/GetViewportRect().Size;
		var maxZoom = LimitOn?Math.Min(maxZoomXY.x, maxZoomXY.y):float.PositiveInfinity;
		var desiredZoom = Math.Min(ZOOM_MULT*cameraZoom, maxZoom);
		FollowingCamera.Zoom = desiredZoom*Vector2.One;
		
		//NaN failsafe
		if(float.IsNaN(FollowingCamera.Zoom.x) || float.IsNaN(FollowingCamera.Zoom.y)) FollowingCamera.Zoom = Vector2.One;
	}
	
	public override void _Draw()
	{
		if(!Debug) return;
		DrawCircle(Vector2.Zero, 5, new Color(1,0,0));
	}
	
	public void CharacterGone(Node2D who) => Followed.Remove(who);
}
