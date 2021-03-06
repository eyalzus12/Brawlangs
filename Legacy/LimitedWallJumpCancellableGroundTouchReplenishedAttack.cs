using Godot;
using System;

public class LimitedWallJumpCancellableGroundTouchReplenishedAttack : WallJumpCancellableGroundTouchReplenishedAttack
{
	public int walljumpAmountUsed;
	public int WallJumpsPossible = -1;
	
	public override void LoadProperties()
	{
		base.LoadProperties();
		LoadExtraProperty<int>("WallJumpsPossible", -1);
		walljumpAmountUsed = 0;
	}
	
	public override void Loop()
	{
		var available = (WallJumpsPossible <= -1)||(walljumpAmountUsed < WallJumpsPossible);
		if(ch.walled && ch.Inputs.IsActionJustPressed("player_jump") && available)
		{
			ch.Inputs.MarkForDeletion("player_jump", true);
			ch.ChangeState("WallJump");
			if(amountUsed > 0) amountUsed--;
			walljumpAmountUsed++;
		}
	}
}
