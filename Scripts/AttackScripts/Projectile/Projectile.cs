using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Projectile : Node2D, IHitter, IHittable
{
	[Signal]
	public delegate void ProjectileDied(Projectile who);
	
	public Vector2 spawningPosition = default;
	public int frameCount = 0;
	public int maxLifetime = 600;
	public int direction = 1;
	public ProjectileMovementFunction Movement;
	
	private List<Hitbox> _hitboxes = new List<Hitbox>();
	private HashSet<IHittable> _ignoreList = new HashSet<IHittable>();
	private Dictionary<Hurtbox, Hitbox> _hitList = new Dictionary<Hurtbox, Hitbox>();
	private bool _hit = false;
	private IAttacker _owner;
	
	public List<Hitbox> Hitboxes{get => _hitboxes; set => _hitboxes=value;}
	public HashSet<IHittable> HitIgnoreList{get => _ignoreList; set => _ignoreList=value;}
	public Dictionary<Hurtbox, Hitbox> HitList{get => _hitList; set => _hitList=value;}
	public bool Hit{get => _hit; set => _hit=value;}
	public IAttacker OwnerObject{get => _owner; set => _owner=value;}
	
	public int TeamNumber{get => OwnerObject.TeamNumber; set => OwnerObject.TeamNumber = value;}
	
	public float DamageTakenMult{get=>1f;set{}}
	public float KnockbackTakenMult{get=>1f;set{}}
	public int StunTakenMult{get=>1;set{}}

	public float DamageDoneMult{get => OwnerObject.DamageDoneMult; set => OwnerObject.DamageDoneMult = value;}
	public float KnockbackDoneMult{get => OwnerObject.KnockbackDoneMult; set => OwnerObject.KnockbackDoneMult = value;}
	public int StunDoneMult{get => OwnerObject.StunDoneMult; set => OwnerObject.StunDoneMult = value;}
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	private bool _active = false;
	public bool Active
	{
		get => _active;
		set
		{
			_active = value;
			Visible = value;
			if(value) OnSpawn();
		}
	}
	
	public IAttacker owner;
	
	public Projectile() {}
	public Projectile(IAttacker owner)
	{
		this.owner = owner;
	}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public override void _Ready()
	{
		frameCount = 0;
		Position = spawningPosition;
		ConnectSignals();
		_hit = false;
		Init();
		Active = false;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!Active) return;
		++frameCount;
		HandleHits();
		Loop();
		Position = Movement.GetNext(this);
		
		if(frameCount >= maxLifetime)
			Destruct();
	}
	
	public virtual void Init() {}
	public virtual void LoadProperties() {}
	public virtual void OnSpawn() {}
	public virtual void Loop() {}
	public virtual void OnRemove() {}
	
	public void Destruct()
	{
		OnRemove();
		_hitboxes.ForEach(h => h.Active = false);
		_active = false;
		_hitList.Clear();
		_ignoreList.Clear();
		EmitSignal(nameof(ProjectileDied), this);
	}
	
	public virtual void Reset()
	{
		_hitboxes = GetChildren().FilterType<Hitbox>().ToList();
		PostHitboxInit();
	}
	
	public virtual void PostHitboxInit() {}
	
	public virtual void ConnectSignals()
	{
		foreach(var h in Hitboxes)
		{
			h.Connect("HitboxHit", this, nameof(HandleInitialHit));
			h.owner = this;
		}
	}
	
	public void HandleInitialHit(Hitbox hitbox, Area2D hurtbox)
	{
		if(!hitbox.Active) return;
		if(!(hurtbox is Hurtbox realhurtbox)) return;//can only handle hurtboxes for hitting
		var hitChar = realhurtbox.owner;
		if(!OwnerObject.CanHit(hitChar) || HitIgnoreList.Contains(hitChar)) return;
		
		Hitbox current;
		if(_hitList.TryGetValue(realhurtbox, out current))
		{
			if(hitbox.hitPriority > current.hitPriority)
				HitList[realhurtbox] = hitbox;
		}
		else
		{
			HitList.Add(realhurtbox, hitbox);
		}
	}
	
	public virtual void HandleHits()
	{
		foreach(var entry in HitList)
		{
			Hitbox hitbox = entry.Value;
			Hurtbox hurtbox = entry.Key;
			var hitChar = hurtbox.owner;
			if(!OwnerObject.CanHit(hitChar) || HitIgnoreList.Contains(hitChar)) continue;
			HitEvent(hitbox, hurtbox);
			Hit = true;
			
			var kmult = owner.KnockbackDoneMult;
			var dmult = owner.DamageDoneMult;
			var smult = owner.StunDoneMult;
			var skb = hitbox.setKnockback*kmult;
			var vkb = hitbox.varKnockback*kmult;
			var damage = hitbox.damage*dmult;
			var stun = hitbox.stun*smult;
			
			var data = new HitData(skb, vkb, damage, stun, hitbox.hitpause, hitbox, hurtbox);
			
			hitChar.HandleGettingHit(data);
			OwnerObject.HandleHitting(data);
			HitIgnoreList.Add(hitChar);
			GD.Print($"{hitChar} was hit by {hitbox.Name}");
		}
		HitList.Clear();
	}
	
	public virtual void HitEvent(Hitbox hitbox, Hurtbox hurtbox) {}
	
	public virtual void HandleGettingHit(HitData data)
	{
		
	}
}
