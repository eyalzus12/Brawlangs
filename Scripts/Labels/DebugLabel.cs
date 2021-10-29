using Godot;
using System;


public class DebugLabel : InfoLabel
{
	Character ch;
	
	public override void Connect()
	{
		ch = GetParent() as Character;
		if(ch is null)
		{
			ch = (GetParent().GetParent()) as Character;
		}
	}
	
	public override void UpdateLabel()
	{
		if(ch is null || ch.dummy) return;
		
		Add("Fastfalling", ch.fastfalling);
		Add("Crouch", ch.crouching);
		Add("Soft", ch.onSemiSolid);
		Add("Slope", ch.onSlope);
		Add("Dir", ch.GetStringDirection());
		Newline();
		Add("Ground", ch.grounded);
		Add("Wall", ch.walled);
		Add("Ceil", ch.ceilinged);
		Newline();
		Add("State", ch.currentState);
		Add("Frame", ch.currentState.frameCount);
		Add("Attack", (ch.currentState is AttackState a)?a.att.Name:"None");
		Newline();
		Add("PlayedAnimation", ch.sprite.currentSheet.name);
		Add("QueuedAnimation", ch.sprite.queuedSheet?.name ?? "None");
		Add("AnimationFrame", ch.sprite.Frame);
		Add("AnimationCoord", ch.sprite.FrameCoords);
		Newline();
		Add("CollisionSetting", ch.currentSetting.Name);
		Newline();
		Add("CollisionExtents", (ch.collision.Shape as RectangleShape2D).Extents);
		Add("CollisionPosition", ch.collision.Position);
		Add("CollisionRotation", Math.Round(ch.collision.Rotation * 180f / Math.PI, 2));
		Newline();
		Add("HurtboxRadius", ch.hurtbox.Shape.Radius);
		Add("HurtboxHeight", ch.hurtbox.Shape.Height);
		Add("HurtboxPosition", ch.hurtbox.col.Position);
		Add("HurtboxRotation", Math.Round(ch.hurtbox.col.Rotation * 180f / Math.PI, 2));
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
		Add("Falling Through", !ch.GetCollisionMaskBit(1));
		Add("Jumps Used", ch.jumpCounter);
		Newline();
		Add("FPS", Engine.GetFramesPerSecond());
	}
	
	protected override bool EnsureCorrectAppearence() => (this.GetDataOrDefault("CurrentInfoLabelCharacter",0).i() == ch.teamNumber);
}
