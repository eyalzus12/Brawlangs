using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CameraFocus : Node2D
{
	public readonly static Vector2 DEFAULT_LIMITS = new Vector2(1300, 900);
	public const float CAMERA_SPEED = 6.5f;
	public const float MIN_ZOOM = 1.5f;
	public const float GROW_H = 100f;
	public const float GROW_V = 70f;
	public const float ZOOM_WEIGHT = 0.5f;
	
	public List<Node2D> Followed{get; set;} = new List<Node2D>();
	
	private bool _debug = false;
	public bool Debug{get => _debug; set {if(_debug != value) {_debug = value; Update();}}}
	
	public Vector2 Limits{get; set;} = DEFAULT_LIMITS;
	
	private bool _limitOn = true;
	public bool LimitOn
	{
		get => _limitOn;
		set
		{
			_limitOn = value;
			FollowingCamera.LimitLeft = -(int)(value?Limits.x:10000000);
			FollowingCamera.LimitRight = (int)(value?Limits.x:10000000);
			FollowingCamera.LimitTop = -(int)(value?Limits.y:10000000);
			FollowingCamera.LimitBottom = (int)(value?Limits.y:10000000);
		}
	}
	
	public Camera2D FollowingCamera{get; set;}
	public Listener2D FollowingListener{get; set;}
	public Rect2 DesiredCameraRect{get; set;}
	
	public CameraFocus() {}
	
	public CameraFocus(IEnumerable<Node2D> toFollow, Vector2 limits)
	{
		Followed = new List<Node2D>(toFollow);
		Limits = limits;
	}
	
	public override void _Ready()
	{
		SetProcess(Followed.Count > 0);
		
		foreach(var followed in Followed) followed.Connect("Dead", this, nameof(CharacterGone));
		
		FollowingCamera = new Camera2D();
		FollowingCamera.Name = "FollowingCamera";
		FollowingCamera.SmoothingEnabled = true;
		FollowingCamera.LimitSmoothed = true;
		FollowingCamera.SmoothingSpeed = CAMERA_SPEED;
		FollowingCamera.LimitLeft = -(int)Limits.x;
		FollowingCamera.LimitRight = (int)Limits.x;
		FollowingCamera.LimitTop = -(int)Limits.y;
		FollowingCamera.LimitBottom = (int)Limits.y;
		FollowingCamera.Current = true;
		AddChild(FollowingCamera);
		
		FollowingListener = new Listener2D();
		FollowingListener.Name = "FollowingListener";
		FollowingListener.MakeCurrent();
		AddChild(FollowingListener);
	}

	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_camera_debug"))
			Debug = !Debug;
		if(Input.IsActionJustPressed("toggle_camera_limits"))
			LimitOn = !LimitOn;
		
		DesiredCameraRect = Followed
			.Select(n=>n.Position
				.Clamp(
					FollowingCamera.LimitLeft,
					FollowingCamera.LimitRight,
					FollowingCamera.LimitTop,
					FollowingCamera.LimitBottom
				)
			)
			.Append(Vector2.Zero)
			.RectWithAll()
			.GrowIndividual(GROW_H,GROW_V,GROW_H,GROW_V)
			.Limit(FollowingCamera.LimitRight,FollowingCamera.LimitBottom);
		Position = DesiredCameraRect.Center();
		
		var cameraZoomXY = DesiredCameraRect.Size/GetViewportRect().Size;
		var cameraZoom = Math.Max(Math.Max(cameraZoomXY.x, cameraZoomXY.y), MIN_ZOOM)*Vector2.One;
		FollowingCamera.Zoom = FollowingCamera.Zoom.LinearInterpolate(cameraZoom, ZOOM_WEIGHT);
		
		//NaN failsafe
		if(float.IsNaN(FollowingCamera.Zoom.x) || float.IsNaN(FollowingCamera.Zoom.y)) FollowingCamera.Zoom = MIN_ZOOM*Vector2.One;
		if(Debug) Update();
	}
	
	public override void _Draw()
	{
		if(!Debug) return;
		DrawSetTransform(-Position,0f,Vector2.One);
		DrawCircle(Position, 5, new Color(1,0,0));
		DrawRect(DesiredCameraRect, new Color(0,1,0), false);
		DrawRect(GeometryUtils.RectFrom(Vector2.Zero, Limits), new Color(0,0,1), false);
	}
	
	public void CharacterGone(Node2D who) => Followed.Remove(who);
}
