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
		if(ch is null || !Godot.Object.IsInstanceValid(ch) || !Visible) return;
		Add("Name", ch.Name);
		Add("Script", ch.GetType().Name);
		Add("TeamNumber", ch.TeamNumber);
		Newline();
		Add("Damage", Math.Round(ch.damage,2));
		Add("Stocks", ch.stocks);
		Newline();
		Add("Fastfalling", ch.fastfalling);
		Add("Crouch", ch.crouching);
		Add("FallingThrough", !ch.GetCollisionMaskBit(1));
		Add("Direction", GetStringDirection(ch.direction));
		Add("Idle", ch.IsIdle());
		Add("Turning", ch.InputtingTurn());
		Newline();
		Add("IFramesLeft", ch.InvincibilityLeft);
		Newline();
		Add("Ground", ch.grounded);
		Add("Wall", ch.walled);
		Add("Ceil", ch.ceilinged);
		Add("Air", ch.aerial);
		Add("SemiSolid", ch.onSemiSolid);
		Add("Slope", ch.onSlope);
		Newline();
		Add("State", ch.currentState?.ToString()??"None");
		Add("PrevState", ch.prevState?.ToString()??"None");
		Add("PrevPrevState", ch.prevPrevState?.ToString()??"None");
		Add("StateFrame", ch.currentState?.frameCount??0);
		Newline();
		var attack = ch.currentAttack;
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
		Add("PlayedSounds", ch.audioManager.ToString());
		Newline();
		Add("CollisionSetting", ch.currentCollisionSetting);
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
		Add("Wfric", ch.wfric);
		Add("Wbounce", ch.wbounce);
		Newline();
		Add("Cnorm", ch.cnorm.Round(2));
		Add("Cvel", ch.cvel.Round(2));
		Add("Cfric", ch.cfric);
		Add("Cbounce", ch.cbounce);
		Newline();
		Add("Left", ch.leftHeld);
		Add("Right", ch.rightHeld);
		Add("Down", ch.downHeld);
		Add("Up", ch.upHeld);
		Newline();
		Add("AirJumpsUsed", ch.currentAirJumpsUsed);
		Add("ClingsUsed", ch.currentClingsUsed);
		Add("GotOptionsFromHitting", ch.gotOptionsFromHitting);
		Add("GotOptionsFromGettingHit", ch.gotOptionsFromGettingHit);
		Newline();
		Add("LeftInput",GetInputString("player_left"));
		Add("RightInput", GetInputString("player_right"));
		Add("DownInput", GetInputString("player_down"));
		Add("UpInput", GetInputString("player_up"));
		Newline();
		Add("JumpInput", GetInputString("player_jump"));
		Add("DodgeInput", GetInputString("player_dodge"));
		Newline();
		Add("LightAttackInput", GetInputString("player_light_attack"));
		Add("HeavyAttackInput", GetInputString("player_heavy_attack"));
		Add("SpecialAttackInput", GetInputString("player_special_attack"));
		Add("TauntAttackInput", GetInputString("player_taunt"));
		Newline();
		Newline();
		Add("Buffer", "\n"+ch.Inputs.ToString());
		Newline();
		Add("FPS", Engine.GetFramesPerSecond());
		Add("PhysicsFrame", Engine.GetPhysicsFrames());
		Add("DebugBuild", OS.IsDebugBuild());
		Add("OS", OS.GetName());
	}
	
	protected override bool EnsureCorrectAppearence() => (this.GetDataOrDefault("CurrentInfoLabelCharacter",0).i() == ch.TeamNumber);
	
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
