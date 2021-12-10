using Godot;
using System;

public class LimitedWallJumpCancellableGroundTouchReplenishedAttack : WallJumpCancellableGroundTouchReplenishedAttack
{
	public int walljumpAmountUsed;
	public int WallJumpsPossible = 1;
	
	public override void Init()
	{
		base.Init();
		LoadExtraProperty<int>("WallJumpsPossible");
		walljumpAmountUsed = 0;
	}
	
	public override void Loop()
	{
		if(ch.walled && ch.Inputs.IsActionJustPressed("player_jump") && walljumpAmountUsed < WallJumpsPossible)
		{
			ch.Inputs.MarkForDeletion("player_jump", true);
			ch.ChangeState("WallJump");
			if(ch.jumpCounter < 1) ch.jumpCounter = 1;
			if(amountUsed > 0) amountUsed--;;
			walljumpAmountUsed++;
		}
	}
}
