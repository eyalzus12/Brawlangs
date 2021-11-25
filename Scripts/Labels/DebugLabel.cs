using Godot;
using System;


public class DebugLabel : InfoLabel
{
	public Character ch = null;
	
	public override void Connect()
	{
		//ch = GetParent() as Character;
		//if(ch is null) ch = GetParent().GetParent() as Character;
	}
	
	public override void UpdateLabel()
	{
		if(ch is null || !Godot.Object.IsInstanceValid(ch)) return;
		Add("Name", ch.Name);
		Add("Script", ch.GetType().Name);
		Add("TeamNumber", ch.teamNumber);
		Newline();
		Add("Damage", Math.Round(ch.damage,2));
		Add("Stocks", ch.stocks);
		Newline();
		Add("Fastfalling", ch.fastfalling);
		Add("Crouch", ch.crouching);
		Add("FallingThrough", !ch.GetCollisionMaskBit(1));
		Add("FacingDirection", GetStringDirection(ch.direction));
		Add("Idle", ch.IsIdle());
		Newline();
		Add("SemiSolid", ch.onSemiSolid);
		Add("Slope", ch.onSlope);
		Add("Ground", ch.grounded);
		Add("Wall", ch.walled);
		Add("Ceil", ch.ceilinged);
		Newline();
		Add("State", ch.currentState?.ToString()??"None");
		Add("PrevState", ch.prevState?.ToString()??"None");
		Add("PrevPrevState", ch.prevPrevState?.ToString()??"None");
		Add("StateFrame", ch.currentState?.frameCount??0);
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
		Newline();
		Add("CollisionSetting", ch.currentSetting.Name);
		Add("CollisionMask", ch.CollisionMask);
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
		Add("Left", ch.leftHeld);
		Add("Right", ch.rightHeld);
		Add("Down", ch.downHeld);
		Add("Up", ch.upHeld);
		Newline();
		Add("Air jumps Used", ch.jumpCounter);
		Add("Wall touches", ch.wallJumpCounter);
		Newline();
		Add("LeftInput",GetInputString("player_left"));
		Add("RightInput", GetInputString("player_right"));
		Add("DownInput", GetInputString("player_down"));
		Add("UpInput", GetInputString("player_up"));
		Newline();
		Add("JumpInput", GetInputString("player_jump"));
		Newline();
		Add("LightAttackInput", GetInputString("player_light_attack"));
		Add("HeavyAttackInput", GetInputString("player_heavy_attack"));
		Add("SpecialAttackInput", GetInputString("player_special_attack"));
		//Add("TauntAttackInput", GetInputString("player_taunt_attack"));
		Newline();
		Newline();
		Add("Buffer", "\n"+ch.Inputs.ToString());
		Newline();
		Add("FPS", Engine.GetFramesPerSecond());
		Add("Physics frame", Engine.GetPhysicsFrames());
		Add("Debug build", OS.IsDebugBuild());
	}
	
	protected override bool EnsureCorrectAppearence() => (this.GetDataOrDefault("CurrentInfoLabelCharacter",0).i() == ch.teamNumber);
	
	private string GetInputString(string s) =>
		ch.Inputs.IsActionJustPressed(s)?"Pressed":
		ch.Inputs.IsActionPressed(s)?"Held":
		ch.Inputs.IsActionJustReleased(s)?"Released":
		"Free";
		
	private string GetStringDirection(int dir)
	{
		switch(dir)
		{
			case 1: return "Right";
			case -1: return "Left";
			case 0: return "None";
			default: return "ERROR";
		}
	}
}
