using Godot;
using System;
using Godot.Collections;

public partial class WaypointMovingPlatform2D : MovingPlatform2D
{
	[Export]
	public Array<Vector3> WayPoints;
	//x: direction to go by x
	//y: direction to go by y
	//z: for how many frames
	
	[Export]
	public bool Repeat = false;
	
	public int Index = 0;
	
	public override void Init()
	{
		Index = 0;
	}
	
	public override void Loop()
	{
		if(Index >= WayPoints.Count) return;
		else if(frameCount >= WayPoints[Index].z)
		{
			++Index;
			frameCount = 0;
			if(Index >= WayPoints.Count && Repeat)
				Index = 0;
		}
	}
	
	public override Vector2 GetNextPosition()
	{
		if(Index >= WayPoints.Count) return Position;
		var action = WayPoints[Index];
		return Position + new Vector2(action.x, action.y);
	}
}
