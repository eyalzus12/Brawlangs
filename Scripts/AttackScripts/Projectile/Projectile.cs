using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Projectile : Node2D, IHitter, IHittable
{
	[Signal]
	public delegate void ProjectileDiedEventHandler(Projectile who);
	
	public string Identifier{get; set;}
	public Vector2 SpawningPosition{get; set;}
	public int MaxLifetime{get; set;}
	
	public int Direction{get; set;}
	
	public ProjectileMovementFunction Movement{get; set;}
	public int frameCount = 0;
	
	public List<Hitbox> Hitboxes{get; set;} = new List<Hitbox>();
	
	public Dictionary<Hurtbox, Hitbox> HitList{get; set;} = new();
	public HashSet<IHittable> HitIgnoreList{get; set;} = new();
	
	public bool HasHit{get; set;}
	public bool GettingHit{get; set;}
	public IHitter LastHitter{get; set;}
	
	public IAttacker OwnerObject{get; set;}
	
	public string currentCollisionSetting;
	
	public InvincibilityManager IFrames{get; set;} = new();
	public bool Invincible => IFrames.Count > 0;
	
	public List<Hurtbox> Hurtboxes{get; set;} = new();
	
	public int TeamNumber{get => OwnerObject.TeamNumber; set => OwnerObject.TeamNumber = value;}
	
	public float DamageTakenMult{get=>1f;set{}}
	public float KnockbackTakenMult{get=>1f;set{}}
	public float StunTakenMult{get=>1f;set{}}

	public float DamageDoneMult{get => OwnerObject.DamageDoneMult; set => OwnerObject.DamageDoneMult = value;}
	public float KnockbackDoneMult{get => OwnerObject.KnockbackDoneMult; set => OwnerObject.KnockbackDoneMult = value;}
	public float StunDoneMult{get => OwnerObject.StunDoneMult; set => OwnerObject.StunDoneMult = value;}
	
	public AudioManager Audio{get; set;} = new();
	public void PlaySound(string sound) => Audio.Play(sound);
	public void PlaySound(AudioStream sound) => Audio.Play(sound);
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new();
	public virtual void LoadProperties() {}
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
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
	
	public override void _Ready()
	{
		frameCount = 0;
		Position = SpawningPosition + OwnerObject.Position;
		ConnectSignals();
		HasHit = false;
		GettingHit = false;
		Init();
		Active = true;
		Hitboxes.ForEach(InitHitbox);
		Hurtboxes.ForEach(h=>h.ChangeState("Default"));
	}
	
	protected void InitHitbox(Hitbox h)
	{
		h.Active = true;
		h.frameCount = 0;
		h.Init();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(!Active) return;
		GettingHit = false;
		++frameCount;
		HandleHits();
		Loop();
		Position = Movement.GetNext(this);
		
		if(frameCount >= MaxLifetime)
			Destruct();
	}
	
	public virtual void Init() {}
	public virtual void OnSpawn() {}
	public virtual void Loop() {}
	public virtual void OnRemove() {}
	
	public void Destruct()
	{
		OnRemove();
		GettingHit = false;
		Hitboxes.ForEach(h => h.Active = false);
		DisonnectSignals();
		Active = false;
		HitList.Clear();
		HitIgnoreList.Clear();
		EmitSignal(nameof(ProjectileDied), this);
	}
	
	public virtual void Reset()
	{
		//TOFIX: this is unsafe
		Hitboxes = GetChildren().FilterType<Hitbox, Node>().ToList();
		PostHitboxInit();
	}
	
	public virtual void PostHitboxInit() {}
	
	public virtual void ConnectSignals()
	{
		//Hitboxes.ForEach(h => h.Connect("HitboxHit",new Callable(this,nameof(HandleInitialHit))));
		Hitboxes.ForEach(h => h.HitboxHit += HandleInitialHit);
	}
	
	public virtual void DisonnectSignals()
	{
		foreach(var h in Hitboxes)
		{
			//h.Disconnect("HitboxHit",new Callable(this,nameof(HandleInitialHit)));
			h.HitboxHit -= HandleInitialHit;
			h.OwnerObject = this;
		}
	}
	
	public void HandleInitialHit(Hitbox hitbox, Hurtbox hurtbox)
	{
		if(!hitbox.Active) return;
		var hitChar = hurtbox.OwnerObject;
		if(!CanHit(hitChar)) return;
		
		Hitbox current;
		if(HitList.TryGetValue(hurtbox, out current))
		{
			if(hitbox.HitPriority > current.HitPriority)
				HitList[hurtbox] = hitbox;
		}
		else
		{
			HitList.Add(hurtbox, hitbox);
		}
	}
	
	public virtual void HandleHits()
	{
		foreach(var entry in HitList)
		{
			Hitbox hitbox = entry.Value;
			Hurtbox hurtbox = entry.Key;
			var hitChar = hurtbox.OwnerObject;
			if(!CanHit(hitChar)) continue;//already hit or cant hit
			HitEvent(hitbox, hurtbox);
			
			var kmult = OwnerObject.KnockbackDoneMult*KnockbackDoneMult*hitbox.GetKnockbackMultiplier(hitChar);
			var dmult = OwnerObject.DamageDoneMult*DamageDoneMult*hitbox.GetDamageMultiplier(hitChar);
			var smult = OwnerObject.StunDoneMult*StunDoneMult*hitbox.GetStunMultiplier(hitChar);
			
			var dirvec = hitbox.KnockbackDir(hitChar)*kmult;
			var skb = dirvec*hitbox.SetKnockback;
			var vkb = dirvec*hitbox.VarKnockback;
			var damage = hitbox.Damage*dmult;
			var sstun = hitbox.SetStun*smult;
			var vstun = hitbox.VarStun*smult;
 			
			var data = new HitData(skb, vkb, damage, sstun, vstun, hitbox.SetHitpause, hitbox.VarHitpause, hitbox, hurtbox);
			
			hitChar.HandleGettingHit(data);
			OwnerObject.HandleHitting(data);
			HitIgnoreList.Add(hitChar);
			GD.Print($"{(hitChar as Node2D).Name} was hit by {hitbox.Name}");
		}
		if(HitList.Count > 0) DidHit();
		HitList.Clear();
	}
	
	public void ApplySettings(string setting)
	{
		currentCollisionSetting = setting;
		Hurtboxes.ForEach(h=>h.ChangeState(setting));
	}
	
	public virtual void HitEvent(Hitbox hitbox, Hurtbox hurtbox) {}
	public virtual void DidHit() => Destruct();
	public virtual void HandleGettingHit(HitData data) => Destruct();
	
	public bool CanHit(IHittable h) => (h != this) && OwnerObject.CanHit(h) && !HitIgnoreList.Contains(h);
}
