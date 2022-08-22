using Godot;
using System;
using System.Text;

public class DebugLabel : InfoLabel
{
	public Character ch = null;
	
	public override void UpdateLabel()
	{
		if(!Valid(ch) || !Visible) return;
		Add("Name", ch.Name);
		Add("Script", ch.GetType().Name);
		Add("TeamNumber", ch.TeamNumber);
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
		Add("State", ch.States.Current?.ToString()??"None");
		Add("PrevState", ch.States.Prev?.ToString()??"None");
		Add("StateFrame", ch.States.Current?.frameCount??0);
		Add("Actionable", ch.States.Current?.Actionable);
		Add("ShouldDrop", ch.States.Current?.ShouldDrop);
		Newline();
		Add("Attack", ch.CurrentAttack?.Name??"None");
		Add("AttackFrame", ch.CurrentAttack?.frameCount??0);
		Add("AttackScript", ch.CurrentAttack?.GetType()?.Name??"None");
		Newline();
		var part = ch.CurrentAttack?.CurrentPart??null;
		Add("AttackPart", part?.Name??"None");
		Add("AttackPartFrame", part?.frameCount??0);
		Add("NextPart", part?.GetNextPart()??"None");
		Add("AttackPartScript", part?.GetType()?.Name??"None");
		Add("AttackPartHit", part?.HasHit ?? false);
		Newline();
		Add("LastHitee", Valid(ch.LastHitee)?ch.LastHitee.ToString():"None");
		Add("LastHitter", Valid(ch.LastHitter)?ch.LastHitter.ToString():"None");
		Newline();
		Add("PlayedAnimation", ch.CharacterSprite.currentSheet.name);
		Add("QueuedAnimation", ch.CharacterSprite.queuedSheet?.name??"None");
		Add("AnimationLooping", ch.CharacterSprite.currentSheet.loop);
		Add("AnimationFrame", ch.CharacterSprite.Frame);
		Newline();
		Add("Velocity", ch.RoundedVelocity);
		Add("Position", ch.RoundedPosition);
		Newline();
		Add("PlatformNormal", ch.Norm.Round(2));
		Add("PlatformFriction", ch.PlatFric);
		Add("PlatformBounce", ch.PlatBounce);
		Add("PlatformVelocity", ch.PlatVel.Round(2));
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
		Add("Jump", GetInputString("Jump"));
		Add("Dodge", GetInputString("Dodge"));
		Add("Run", GetInputString("Run"));
		Add("Light", GetInputString("Light"));
		Add("Special", GetInputString("Special"));
		Add("Taunt", GetInputString("Taunt"));
		Newline();
		Add("PlayedSounds", "\n"+ch.Audio);
		
		if(ch.Inputs is BufferInputManager buff)
		{
			Newline();
			
			var buffstring = new StringBuilder();
			foreach(var entry in buff.buffer) if(entry.Value.bufferTimeLeft >= 0)
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
		Add("TimedActions", "\n"+ch.TimedActions);
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
	
	private bool Valid(object o) => o is Godot.Object god && Godot.Object.IsInstanceValid(god);
}
