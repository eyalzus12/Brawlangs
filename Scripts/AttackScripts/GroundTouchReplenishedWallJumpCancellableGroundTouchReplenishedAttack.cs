using Godot;
using System;

public class GroundTouchReplenishedWallJumpCancellableGroundTouchReplenishedAttack : LimitedWallJumpCancellableGroundTouchReplenishedAttack
{
	public override void GroundTouch()
	{
		base.GroundTouch();
		walljumpAmountUsed = 0;
	}
}
