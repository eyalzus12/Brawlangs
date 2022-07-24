using Godot;
using System;
using System.Text;

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
		Add("FallThrough", !ch.GetCollisionMaskBit(1));
		Add("Direction", GetStringDirection(ch.direction));
		Add("Idle", ch.IsIdle);
		Add("Turning", ch.InputtingTurn);
		Newline();
		Add("Ground", ch.grounded);
		Add("Wall", ch.walled);
		Add("Ceil", ch.ceilinged);
		Add("Air", ch.aerial);
		Add("SemiSolid", ch.onSemiSolid);
		Add("Slope", ch.onSlope);
		Newline();
		Add("CollisionSetting", ch.currentCollisionSetting);
		Newline();
		Add("State", ch.currentState?.ToString()??"None");
		Add("PrevState", ch.prevState?.ToString()??"None");
		Add("PrevPrevState", ch.prevPrevState?.ToString()??"None");
		Add("StateFrame", ch.currentState?.frameCount??0);
		Add("Actionable", ch.currentState?.actionable);
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
		Add("Velocity",  ch.GetRoundedVelocity());
		Add("Position", ch.GetRoundedPosition());
		Newline();
		Add("PlatformNormal", ch.Norm.Round(2));
		Add("PlatformFriction", ch.PlatFric);
		Add("PlatformBounce", ch.PlatBounce);
		Add("Fvel", ch.fvel.Round());
		Add("Wvel", ch.wvel.Round());
		Add("Cvel", ch.cvel.Round(2));
		Newline();
		Add("Friction", ch.AppropriateFriction);
		Add("Bounce", ch.AppropriateBounce);
		Add("Acceleration", ch.AppropriateAcceleration);
		Add("Speed", ch.AppropriateSpeed);
		Add("Gravity", ch.AppropriateGravity);
		Add("FallSpeed", ch.AppropriateFallingSpeed);
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
		Add("HasDodge", ch.hasDodge);
		Newline();
		Add("LeftInput",GetInputString("player_left"));
		Add("RightInput", GetInputString("player_right"));
		Add("DownInput", GetInputString("player_down"));
		Add("UpInput", GetInputString("player_up"));
		Add("JumpInput", GetInputString("player_jump"));
		Add("DodgeInput", GetInputString("player_dodge"));
		Newline();
		Add("LightAttackInput", GetInputString("player_light_attack"));
		Add("SpecialAttackInput", GetInputString("player_special_attack"));
		Add("TauntAttackInput", GetInputString("player_taunt"));
		Newline();
		
		if(ch.Inputs is BufferInputManager buff)
		{
			Newline();
			
			var buffstring = new StringBuilder();
			foreach(var entry in buff.buffer)
				if(entry.Value.bufferTimeLeft >= 0)
					buffstring.Append($"{entry.Key} : {entry.Value.bufferTimeLeft}\n");
		
			Add("Buffer", "\n"+buffstring.ToString());
		}
		
		Newline();
		
		var cdstring = new StringBuilder();
		foreach(var entry in ch.actionCooldowns)
			if(entry.Value != 0)
				cdstring.Append($"{entry.Key.ToString()} : {entry.Value.ToString()}\n");
		
		Add("Cooldowns", "\n"+cdstring.ToString());
		Newline();
		
		var itstring = new StringBuilder();
		foreach(var entry in ch.invincibilityTimers)
			itstring.Append($"{entry.Key.ToString()} : {entry.Value.ToString()}\n");
		
		Add("InvincibilityTimers", "\n"+itstring.ToString());
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
