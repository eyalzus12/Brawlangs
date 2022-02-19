using Godot;
using System;

public class WallJumpCancellableGroundTouchReplenishedAttack : ReplenishedLimitedAttack
{
	public bool walljumpUsed = false;
	
	public override void GroundTouch()
	{
		base.GroundTouch();
		walljumpUsed = false;
	}
	
	public override void Loop()
	{
		if(ch.walled && ch.Inputs.IsActionJustPressed("player_jump") && !walljumpUsed)
		{
			ch.Inputs.MarkForDeletion("player_jump", true);
			ch.ChangeState("WallJump");
			if(amountUsed > 0) amountUsed--;
			walljumpUsed = true;
		}
	}
}
