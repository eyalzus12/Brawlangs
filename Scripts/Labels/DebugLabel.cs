using Godot;
using System;


public class DebugLabel : InfoLabel
{
	Character ch;
	
	public override void Connect()
	{
		ch = GetParent() as Character;
		if(ch is null) ch = GetParent().GetParent() as Character;
	}
	
	public override void UpdateLabel()
	{
		if(ch is null) return;
		Add("Name", ch.Name);
		Add("Script", ch.GetType().Name);
		Newline();
		Add("Fastfalling", ch.fastfalling);
		Add("Crouch", ch.crouching);
		Add("FallThrough", ch.onSemiSolid);
		Add("Slope", ch.onSlope);
		Add("Direction", ch.GetStringDirection());
		Newline();
		Add("Ground", ch.grounded);
		Add("Wall", ch.walled);
		Add("Ceil", ch.ceilinged);
		Newline();
		Add("State", ch.currentState);
		Add("PrevState", ch.prevState);
		Add("PrevPrevState", ch.prevPrevState);
		Add("StateFrame", ch.currentState.frameCount);
		Newline();
		var attack = (ch.currentState is AttackState a)?a.att:null;
		Add("Attack", attack?.Name??"None");
		Add("AttackFrame", attack?.frameCount??0);
		Add("AttackScript", attack?.GetType()?.Name??"None");
		Newline();
		var part = attack?.currentPart??null;
		Add("AttackPart", part?.Name??"None");
		Add("AttackPartFrame", part?.frameCount??0);
		Add("AttackPartHit", part?.hit??false);
		Add("NextPart", part?.GetNextPart()??"None");
		Add("AttackPartScript", part?.GetType()?.Name??"None");
		Newline();
		Add("PlayedAnimation", ch.sprite.currentSheet.name);
		Add("QueuedAnimation", ch.sprite.queuedSheet?.name??"None");
		Add("AnimationLooping", ch.sprite.currentSheet.loop);
		Add("AnimationFrame", ch.sprite.Frame);
		Add("AnimationCoord", ch.sprite.FrameCoords);
		Newline();
		Add("CollisionSetting", ch.currentSetting.Name);
		Newline();
		var ext = (ch.collision.Shape as RectangleShape2D).Extents;
		Add("CollisionWidth", ext.x);
		Add("CollisionHeight", ext.y);
		Add("CollisionPosition", ch.collision.Position);
		//Add("CollisionRotation", Math.Round(ch.collision.Rotation * 180f / Math.PI, 2));
		Newline();
		Add("HurtboxRadius", ch.hurtbox.Shape.Radius);
		Add("HurtboxHeight", ch.hurtbox.Shape.Height);
		Add("HurtboxPosition", ch.hurtbox.col.Position);
		Add("HurtboxRotation", Math.Round(ch.hurtbox.col.Rotation * 180f / Math.PI, 2));
		Add("HurtboxScript", ch.hurtbox.GetType().Name);
		Newline();
		Add("Vel",  ch.GetRoundedVelocity());
		Add("Pos", ch.GetRoundedPosition());
		Newline();
		Add("Fnorm", ch.fnorm.Round(2));
		Add("Fvel", ch.fvel.Round());
		Add("Ffric", ch.ffric);
		Add("Fbounce", ch.fbounce);
		Newline();
		Add("Wnorm", ch.wnorm.Round(2));
		Add("Wvel", ch.wvel.Round());
		Add("Wfric", ch.ffric);
		Add("Wbounce", ch.fbounce);
		Newline();
		Add("Cnorm", ch.cnorm.Round(2));
		Add("Cvel", ch.cvel.Round(2));
		Add("Cfric", ch.ffric);
		Add("Cbounce", ch.fbounce);
		Newline();
		Add("Damage", Math.Round(ch.damage,2));
		Add("Stocks", ch.stocks);
		Newline();
		Add("Left", ch.leftHeld);
		Add("Right", ch.rightHeld);
		Add("Down", ch.downHeld);
		Add("Up", ch.upHeld);
		Newline();
		Add("LeftInput", ch.Inputs.IsActionPressed("player_left"));
		Add("RightInput", ch.Inputs.IsActionPressed("player_right"));
		Add("DownInput", ch.Inputs.IsActionPressed("player_down"));
		Add("UpInput", ch.Inputs.IsActionPressed("player_up"));
		Newline();
		Add("JumpInput", ch.Inputs.IsActionPressed("player_jump"));
		Newline();
		Add("LightAttackInput", ch.Inputs.IsActionPressed("player_light_attack"));
		//Add("HeavyAttackInput", ch.Inputs.IsActionPressed("player_heavy_attack"));
		Add("SpecialAttackInput", ch.Inputs.IsActionPressed("player_special_attack"));
		//Add("TauntAttackInput", ch.Inputs.IsActionPressed("player_taunt_attack"));
		Newline();
		Add("Jumps Used", ch.jumpCounter);
		//Add("Wall jumps used", ch.wallJumpCounter);
		Newline();
		Add("FPS", Engine.GetFramesPerSecond());
	}
	
	protected override bool EnsureCorrectAppearence() => (this.GetDataOrDefault("CurrentInfoLabelCharacter",0).i() == ch.teamNumber);
}
