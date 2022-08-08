using Godot;
using System;
using System.Text;

public class DebugLabel : InfoLabel
{
	public Character ch = null;
	
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
		Add("Direction", GetStringDirection(ch.Direction));
		Add("Idle", ch.Idle);
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
		var currentState = ch.States.Current;
		Add("State", currentState?.ToString()??"None");
		Add("PrevState", ch.States.Prev?.ToString()??"None");
		Add("StateFrame", currentState?.frameCount??0);
		Add("Actionable", currentState?.Actionable);
		Newline();
		var attack = ch.CurrentAttack;
		Add("Attack", attack?.Name??"None");
		Add("AttackFrame", attack?.frameCount??0);
		Add("AttackScript", attack?.GetType()?.Name??"None");
		Newline();
		var part = attack?.currentPart??null;
		Add("AttackPart", part?.Name??"None");
		Add("AttackPartFrame", part?.frameCount??0);
		Add("NextPart", part?.GetNextPart()??"None");
		Add("AttackPartScript", part?.GetType()?.Name??"None");
		Add("AttackPartHit", part?.HasHit ?? false);
		Newline();
		Add("LastHitee", ch.LastHitee?.ToString() ?? "None");
		Add("LastHitter", ch.LastHitter?.ToString() ?? "None");
		Newline();
		Add("PlayedAnimation", ch.CharacterSprite.currentSheet.name);
		Add("QueuedAnimation", ch.CharacterSprite.queuedSheet?.name??"None");
		Add("AnimationLooping", ch.CharacterSprite.currentSheet.loop);
		Add("AnimationFrame", ch.CharacterSprite.Frame);
		Newline();
		Add("PlayedSounds", ch.Audio.ToString());
		Newline();
		Add("Velocity", ch.RoundedVelocity);
		Add("Position", ch.RoundedPosition);
		Newline();
		Add("PlatformNormal", ch.Norm.Round(2));
		Add("PlatformFriction", ch.PlatFric);
		Add("PlatformBounce", ch.PlatBounce);
		Add("Fvel", ch.fvel.Round(2));
		Add("Wvel", ch.wvel.Round(2));
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
		Add("LeftInput",GetInputString("left"));
		Add("RightInput", GetInputString("right"));
		Add("DownInput", GetInputString("down"));
		Add("UpInput", GetInputString("up"));
		Add("JumpInput", GetInputString("jump"));
		Add("DodgeInput", GetInputString("dodge"));
		Newline();
		Add("LightAttackInput", GetInputString("light"));
		Add("SpecialAttackInput", GetInputString("special"));
		Add("TauntAttackInput", GetInputString("taunt"));
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
		
		Add("Cooldowns", "\n"+ch.Cooldowns);
		Newline();
		Add("InvincibilityTimers", "\n"+ch.IFrames);
		Newline();
		Add("Resources", "\n"+ch.Resources);
		Newline();
		
		Add("FPS", Engine.GetFramesPerSecond());
		Add("PhysicsFrame", Engine.GetPhysicsFrames());
		Add("DebugBuild", OS.IsDebugBuild());
		Add("OS", OS.GetName());
	}
	
	protected override bool EnsureCorrectAppearence() => (this.GetDataOrDefault("CurrentInfoLabelCharacter",0).i() == ch.TeamNumber);
	
	private string GetInputString(string s) =>
		ch.Inputs.IsActionJustPressed(s)?"P"://press
		ch.Inputs.IsActionPressed(s)?"H"://hold
		ch.Inputs.IsActionJustReleased(s)?"R"://release
		"F";//free
		
	private string GetStringDirection(int dir)
	{
		switch(dir)
		{
			case 1: return "R";
			case -1: return "L";
			case 0: return "N";
			default: return "ERROR";
		}
	}
}
