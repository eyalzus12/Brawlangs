using Godot;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public class DebugLabel : InfoLabel
{
	public DebugLabel() {}
	public DebugLabel(IEnumerable<Character> characters){Characters = characters.ToArray();}
	
	private int whichlabel = 0;
	private int currentCharacter = 0;
	
	private Action<Character>[] labels;
	
	public Character[] Characters{get; set;}
	
	public override void Init()
	{
		labels = new Action<Character>[]{UpdateLabel_0,UpdateLabel_1};
	}
	
	public override void UpdateLabel()
	{
		if(Input.IsActionJustPressed("next_label_character"))
		{currentCharacter++; currentCharacter %= Characters.Length;}
		if(Input.IsActionJustPressed("prev_label_character"))
		{currentCharacter += Characters.Length-1; currentCharacter %= Characters.Length;}
		if(Input.IsActionJustPressed("debug_label_switch"))
		{whichlabel++; whichlabel %= labels.Length;}
		
		var ch = Characters[currentCharacter];
		
		if(!Valid(ch)) return;
		labels[whichlabel](ch);
	}
	
	public void UpdateLabel_0(Character ch)
	{
		Add("Name", ch.Name);
		Add("Script", ch.GetType().Name);
		Add("TeamNumber", ch.TeamNumber);
		Add("Damage", Math.Round(ch.Damage,2));
		Add("Stocks", ch.Stocks);
		Newline();
		Add("Fastfalling", ch.Fastfalling);
		Add("Crouch", ch.Crouching);
		Add("FallThrough", !ch.GetCollisionMaskBit(1));
		Add("Direction", GetStringDirection(ch.Direction));
		Add("Idle", ch.Idle);
		Add("Turning", ch.InputtingTurn);
		Newline();
		Add("Ground", ch.Grounded);
		Add("Wall", ch.Walled);
		Add("WallClinging", ch.WallClinging);
		Add("Ceil", ch.Ceilinged);
		Add("Air", ch.Aerial);
		Add("SemiSolid", ch.OnSemiSolid);
		Add("Slope", ch.OnSlope);
		Newline();
		Add("CollisionSetting", ch.CurrentCollisionSetting);
		Newline();
		Add("State", ch.States.Current?.ToString()??"None");
		Add("PrevState", ch.States.Prev?.ToString()??"None");
		Add("StateFrame", ch.States.Current?.frameCount??0);
		Add("Actionable", ch.States.Current?.Actionable);
		Add("ShouldDrop", ch.States.Current?.ShouldDrop);
		Newline();
		Add("Attack", ch.CurrentAttack?.Name??"None");
		Add("AttackFrame", ch.CurrentAttack?.FrameCount??0);
		Add("AttackScript", ch.CurrentAttack?.GetType()?.Name??"None");
		Newline();
		var part = ch.CurrentAttack?.CurrentPart;
		Add("AttackPart", part?.Name??"None");
		Add("AttackPartFrame", part?.FrameCount??0);
		Add("NextPart", part?.NextPart()??"None");
		Add("AttackPartScript", part?.GetType()?.Name??"None");
		Add("AttackPartHit", part?.HasHit ?? false);
		Add("LastAttackPart", ch.CurrentAttack?.LastUsedPart?.Name??"None");
		Newline();
		Add("LastHitee", (Valid(ch.LastHitee) && ch.LastHitee is Node nodehitee)?nodehitee.Name:"None");
		Add("LastHitter", (Valid(ch.LastHitter) && ch.LastHitter is Node nodehitter)?nodehitter.Name:"None");
		Newline();
		Add("CurrentAnimation", ch.CharacterSprite.Current?.Name ?? "");
		Add("AnimationPlaying", ch.CharacterSprite.Playing);
		Add("AnimationFrame", ch.CharacterSprite.Frame);
		Add("AnimationLooping", ch.CharacterSprite.Looping);
		Add("AnimationQueue", ch.CharacterSprite.QueueToString);
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
		Add("Left", ch.LeftInput);
		Add("Right", ch.RightInput);
		Add("Down", ch.DownInput);
		Add("Up", ch.UpInput);
		Newline();
		Add("Jump", GetInputString(ch, "Jump"));
		Add("Dodge", GetInputString(ch, "Dodge"));
		Add("Run", GetInputString(ch, "Run"));
		Add("Light", GetInputString(ch, "Light"));
		Add("Special", GetInputString(ch, "Special"));
		Add("Taunt", GetInputString(ch, "Taunt"));
		Newline();
		Add("FPS", Engine.GetFramesPerSecond());
		Add("PhysicsFrame", Engine.GetPhysicsFrames());
		Add("DebugBuild", OS.IsDebugBuild());
		Add("OS", OS.GetName());
	}
	
	public void UpdateLabel_1(Character ch)
	{
		Add("Name", ch.Name);
		Add("Script", ch.GetType().Name);
		Add("TeamNumber", ch.TeamNumber);
		Newline();
		Add("Sounds", "\n"+ch.Audio);
		Newline();
		Add("Buffer", "\n"+ch.Inputs);
		Newline();
		Add("Cooldowns", "\n"+ch.Cooldowns);
		Newline();
		Add("InvincibilityTimers", "\n"+ch.IFrames);
		Newline();
		Add("Resources", "\n"+ch.Resources);
		Newline();
		Add("TimedActions", "\n"+ch.TimedActions);
		Newline();
		Add("Tags", "\n"+ch.Tags);
	}
	
	private string GetInputString(Character ch, string s) =>
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
