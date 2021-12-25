using Godot;
using System;

public class StunState : State
{
	public int stunLength = 0;
	public Vector2 force = Vector2.Zero;
	public const float BOUNCE_FACTOR = 0.95f;
	
	public int framesSinceLastBounce = 0;
	public const int BOUNCE_PERIOD = 2;
	
	public StunState() : base() {}
	public StunState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		SetupCollisionParamaters();
		stunLength = 0;
		ch.vec = Vector2.Zero;
		framesSinceLastBounce = 0;
		ch.PlayAnimation("Stun");
		
		if(ch.grounded && !ch.onSemiSolid)
		{
			ch.voc = ch.voc.Bounce(ch.fnorm);;
			framesSinceLastBounce = 0;
		}
	}
	
	public Vector2 Force
	{
		get => force;
		set
		{
			force = value;
			ch.voc = force;
		}
	}
	
	protected override void RepeatActions()
	{
		ch.framesSinceLastHit = 0;
		++framesSinceLastBounce;
		var friction = ch.grounded?ch.groundFriction*ch.ffric:ch.airFriction;
		ch.voc.x *= (1f-friction);
		if(!ch.grounded) ch.voc.y.Lerp(ch.fallSpeed, ch.gravity);
		
		if(framesSinceLastBounce >= BOUNCE_PERIOD && !ch.aerial && (!ch.grounded || ch.voc.y > VCF))
		{
			var r = ch.voc.Bounce(ch.Norm);
			var bounce = ch.CharBounce;
			r.y *= bounce;
			ch.voc = r;
			framesSinceLastBounce = 0;
		}
	}
	
	protected override void FirstFrameAfterInit()
	{
		ch.SetCollisionMaskBit(DROP_THRU_BIT, Force.y <= 0);
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= stunLength)
		{
			ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
			ch.framesSinceLastHit = 0;
			ch.ChangeState(ch.grounded?"Idle":ch.walled?"Wall":"Air");
		}
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		ch.vec = ch.voc;
		Force = Vector2.Zero;
	}
}
