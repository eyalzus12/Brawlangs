using Godot;
using System;

public class WallJumpCancellableGroundTouchReplenishedAttack : GroundTouchReplenishedAttack
{
	public override void Loop()
	{
		if(ch.walled && ch.Inputs.IsActionJustPressed("player_jump"))
		{
			ch.Inputs.MarkForDeletion("player_jump", true);
			
			//do something to make the attack state not end
			
			ch.ChangeState("WallJump");
			
			if(ch.jumpCounter < 1) ch.jumpCounter = 1;
			
			if(amountUsed > 0) amountUsed--;;
		}
	}
}
