using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Projectile : Node2D, IHitter, IHittable
{
	[Signal]
	public delegate void ProjectileDied(Projectile who);
	
	[Export]
	public string identifier;
	[Export]
	public Vector2 spawningPosition = default;
	[Export]
	public int maxLifetime = 600;
	
	public int direction = 1;
	public ProjectileMovementFunction Movement;
	public int frameCount = 0;
	
	private List<Hitbox> _hitboxes = new List<Hitbox>();
	public List<Hitbox> Hitboxes{get => _hitboxes; set => _hitboxes=value;}
	private HashSet<IHittable> _ignoreList = new HashSet<IHittable>();
	public HashSet<IHittable> HitIgnoreList{get => _ignoreList; set => _ignoreList=value;}
	private Dictionary<Hurtbox, Hitbox> _hitList = new Dictionary<Hurtbox, Hitbox>();
	public Dictionary<Hurtbox, Hitbox> HitList{get => _hitList; set => _hitList=value;}
	private bool _hit = false;
	public bool Hit{get => _hit; set => _hit=value;}
	private IAttacker _owner;
	public IAttacker OwnerObject{get => _owner; set => _owner=value;}
	
	public string currentCollisionSetting;
	private List<Hurtbox> _hurtboxes = new List<Hurtbox>();
	public List<Hurtbox> Hurtboxes{get => _hurtboxes; set => _hurtboxes=value;}
	
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
	
	public Projectile() {}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public override void _Ready()
	{
		frameCount = 0;
		Position = spawningPosition+((Node2D)OwnerObject).Position;
		ConnectSignals();
		_hit = false;
		Init();
		Active = true;
		Hitboxes.ForEach(h => h.Active = true);
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
		//GD.Print("Hmmmmm");
		OnRemove();
		Hitboxes.ForEach(h => h.Active = false);
		Active = false;
		HitList.Clear();
		HitIgnoreList.Clear();
		EmitSignal(nameof(ProjectileDied), this);
		//GetParent().RemoveChild(this);
	}
	
	public virtual void Reset()
	{
		Hitboxes = GetChildren().FilterType<Hitbox>().ToList();
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
		if(hitChar == this || !OwnerObject.CanHit(hitChar) || HitIgnoreList.Contains(hitChar)) return;
		
		Hitbox current;
		if(HitList.TryGetValue(realhurtbox, out current))
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
			if(!OwnerObject.CanHit(hitChar) || HitIgnoreList.Contains(hitChar)) continue;//already hit or cant hit
			HitEvent(hitbox, hurtbox);
			Hit = true;
			
			var kmult = OwnerObject.KnockbackDoneMult*KnockbackDoneMult*hitbox.GetKnockbackMultiplier(hitChar);
			var dmult = OwnerObject.DamageDoneMult*DamageDoneMult*hitbox.GetDamageMultiplier(hitChar);
			var smult = OwnerObject.StunDoneMult*StunDoneMult*hitbox.GetStunMultiplier(hitChar);
			
			var dirvec = hitbox.KnockbackDir((Node2D)hurtbox.GetParent())*kmult;//owner is IHittable, so use parent
			
			var skb = dirvec*hitbox.setKnockback;
			var vkb = dirvec*hitbox.varKnockback;
			var damage = hitbox.damage*dmult;
			var stun = hitbox.stun*smult;
			
			var data = new HitData(skb, vkb, damage, stun, hitbox.hitpause, hitbox, hurtbox);
			
			hitChar.HandleGettingHit(data);
			OwnerObject.HandleHitting(data);
			HitIgnoreList.Add(hitChar);
			GD.Print($"{hitChar} was hit by {hitbox.Name}");
		}
		if(HitList.Count > 0) Destruct();
		HitList.Clear();
	}
	
	public void ApplySettings(string setting)
	{
		currentCollisionSetting = setting;
		Hurtboxes.ForEach(h=>h.ChangeState(setting));
	}
	
	public virtual void HitEvent(Hitbox hitbox, Hurtbox hurtbox) {}
	
	public virtual void HandleGettingHit(HitData data)
	{
		Destruct();
	}
}
