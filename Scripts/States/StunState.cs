using Godot;
using System;

public class StunState : State
{
	public const float BOUNCE_FACTOR = 0.95f;
	
	public int framesSinceLastBounce = 0;
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
		
		framesSinceLastBounce = 0;
		ch.PlayAnimation("Stun", true);
		
		if(ch.grounded && !ch.onSemiSolid)
		{
			ch.voc = ch.voc.Bounce(ch.fnorm);
			framesSinceLastBounce = 0;
		}
		originalModulate = ch.CharacterSprite.Modulate;
		ch.CharacterSprite.Modulate = new Color(0,0,0,1);
	}
	
	protected override void RepeatActions()
	{
		ch.framesSinceLastHit = 0;
		++framesSinceLastBounce;
		var friction = ch.grounded?ch.groundFriction*ch.ffric:ch.airFriction;
		ch.voc.x *= (1f-friction);
		if(!ch.grounded) ch.voc.y.Towards(ch.stunFallSpeed, ch.stunGravity);
		
		if(framesSinceLastBounce >= BOUNCE_PERIOD && !ch.aerial && (!ch.grounded || ch.voc.y > VCF))
		{
			var r = ch.voc.Bounce(ch.Norm);
			var bounce = ch.CharBounce;
			r.y *= bounce;
			ch.voc = r;
			framesSinceLastBounce = 0;
		}
	}
	
	protected override void SetPlatformDropping() {}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.StunFrames)
		{
			ch.framesSinceLastHit = 0;
			ch.States.Change(ch.grounded?"Idle":ch.walled?"Wall":"Air");
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
