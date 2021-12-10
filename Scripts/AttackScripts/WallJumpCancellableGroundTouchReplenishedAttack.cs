using Godot;
using System;

public class WallJumpCancellableGroundTouchReplenishedAttack : GroundTouchReplenishedAttack
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
			if(ch.jumpCounter < 1) ch.jumpCounter = 1;
			if(amountUsed > 0) amountUsed--;;
			walljumpUsed = true;
		}
	}
}
