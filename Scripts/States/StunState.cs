using Godot;
using System;

public class StunState : State
{
	public const float BOUNCE_FACTOR = 0.95f;
	
	public int FramesSinceLastBounce{get; set;} = 0;
	public const int BOUNCE_PERIOD = 2;
	
	private Color originalModulate;
	
	public StunState() : base() {}
	public StunState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		SetupCollisionParamaters();
		
		ch.SetCollisionMaskBit(DROP_THRU_BIT, ch.Knockback.y <= 0);
		ch.ResetVelocity();
		
		ch.voc = ch.Knockback;
		
		FramesSinceLastBounce = 0;
		ch.PlayAnimation("Stun", true);
		
		if(ch.Grounded && !ch.OnSemiSolid)
		{
			ch.voc = ch.voc.Bounce(ch.FNorm);
			FramesSinceLastBounce = 0;
		}
		originalModulate = ch.CharacterSprite.Modulate;
		ch.CharacterSprite.Modulate = new Color(0,0,0,1);
	}
	
	protected override void RepeatActions()
	{
		ch.FramesSinceLastHit = 0;
		++FramesSinceLastBounce;
		ch.voc.x *= (1f-ch.AppropriateFriction);
		if(!ch.Grounded) ch.voc.y.Towards(ch.StunFallSpeed, ch.StunGravity);
		
		if(FramesSinceLastBounce >= BOUNCE_PERIOD && !ch.Aerial && (!ch.Grounded || ch.voc.y > VCF))
		{
			var r = ch.voc.Bounce(ch.Norm);
			var bounce = ch.CharBounce;
			r.y *= bounce;
			ch.voc = r;
			FramesSinceLastBounce = 0;
		}
	}
	
	protected override void SetPlatformDropping() {}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.StunFrames)
		{
			ch.FramesSinceLastHit = 0;
			ch.States.Change(ch.Grounded?"Idle":ch.Walled?"Wall":"Air");
			ch.Knockback = Vector2.Zero;
			ch.StunFrames = 0;
		}
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		ch.vuc = ch.voc;
		ch.voc = Vector2.Zero;
		ch.CharacterSprite.Modulate = originalModulate;
	}
}
