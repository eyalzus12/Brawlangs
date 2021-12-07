using Godot;
using System;

public class GroundTouchReplenishedAttack : LimitedAttack
{
	public static readonly string[] TrackedStates = new string[]{"Land", "Attack", "Endlag"};
	
	public override void Init()
	{
		base.Init();
		foreach(var s in TrackedStates)
			ch.GetState(s).Connect("JumpsRestored", this, nameof(GroundTouch));
	}
	
	public virtual void GroundTouch() => amountUsed = 0;
}
