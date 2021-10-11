using Godot;
using System;
using Godot.Collections;
//using static System.Linq.Enumerable;

public class MovementTween : Tween
{
	public const float STOP_MARGIN = 70f;//lower this if stuff looks weird
	
	PathFollow2D follow;
	
	public MovementTween() {}
	public MovementTween(Array<Vector3> act) => actions = act;
	
	[Export]
	public Array<Vector3> actions = new Array<Vector3>();
	//x = speed
	//y = offset to go
	//z = startup in frames
	
	public override void _Ready()
	{
		Repeat = true;
		follow = (PathFollow2D)GetParent();
		
		var delay = 0f;
		var offset = follow.Offset;
		
		follow.UnitOffset = 1f;
		var length = follow.Offset;
		follow.UnitOffset = offset/length;
		
		do foreach(var v in actions)
		{
			var speed = v.x*60f;
			var max = v.y;
			var startup = v.z/60f;
			var duration = Math.Abs(max/speed);
			delay += startup;
			
			InterpolateProperty(
				follow, "offset", 
				offset, offset + max, duration,
				TransitionType.Linear, EaseType.InOut,
				delay
			);
			
			delay += duration;
			offset += max;
			//GD.Print(offset+" "+length+" "+offset%length);
		}
		while(follow.Loop && (offset%length) > STOP_MARGIN);//continues to loop
		
		Start();
	}
	
	public override void _PhysicsProcess(float delta)
	{
		foreach(var n in follow.GetChildren())
		if(n is KinematicPlatform2D m)
			m.GlobalTransform = follow.GlobalTransform.Translated(m.Position);
	}
}
