using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class HitProjectile : Projectile
{
	public List<Hitbox> hitboxes = new List<Hitbox>();
	public HashSet<Node2D> ignoreList = new HashSet<Node2D>();
	public Dictionary<Hurtbox, Hitbox> hitList = new Dictionary<Hurtbox, Hitbox>();
	public bool hit = false;
	
	public void ConnectSignals()
	{
		foreach(var h in hitboxes)
		{
			h.Connect("HitboxHit", this, nameof(HandleHit));
			h.owner = this;
		}
	}
	
	public virtual void Reset()
	{
		hitboxes = GetChildren().FilterType<Hitbox>().ToList();
		PostHitboxInit();
	}
	
	public virtual void PostHitboxInit() {}
	
	public override void Init()
	{
		ConnectSignals();
		hit = false;
		base.Init();
	}
	
	public override void LoadProperties()
	{
		LoadExtraProperty<List<Hitbox>>("Hitboxes", new List<Hitbox>());
	}
	
	public virtual void OnHit(Hitbox hitbox, Area2D hurtbox) {}
	
	public virtual void HandleHit(Hitbox hitbox, Area2D hurtbox)
	{
		if(!hitbox.Active) return;
		if(!(hurtbox is Hurtbox realhurtbox && owner is Character ch)) return;//can only handle hurtboxes for hitting
		var hitChar = (Character)hurtbox.GetParent();
		if(!ch.CanHit(hitChar) || ignoreList.Contains(hitChar)) return;//TOOO: find a way to extend this to non character objects
		
		var current = new Hitbox();
		if(hitList.TryGetValue(realhurtbox, out current))
		{
			if(hitbox.hitPriority > current.hitPriority)
				hitList[realhurtbox] = hitbox;
		}
		else
		{
			hitList.Add(realhurtbox, hitbox);
		}
	}
	
	public override void Loop()
	{
		base.Loop();
		ActualHit();
	}
	
	public override void OnRemove()
	{
		hitboxes.ForEach(h => h.Active = false);
		active = false;
		hitList.Clear();
		ignoreList.Clear();
	}
	
	public virtual void ActualHit()
	{
		if(!active) return;
		foreach(var entry in hitList)
		{
			Hitbox hitbox = entry.Value;
			Hurtbox hurtbox = entry.Key;
			var hitChar = (Character)hurtbox.GetParent();
			if(!(owner is Character ch)) continue;//TOOO: find a way to extend this to non character objects
			if(!ch.CanHit(hitChar) || ignoreList.Contains(hitChar)) continue;
			hit = true;
			OnHit(hitbox, hurtbox);
			
			var kmult = ch.knockbackDoneMult;
			var dmult = ch.damageDoneMult;
			var smult = ch.stunDoneMult;
			var skb = hitbox.setKnockback*kmult;
			var vkb = hitbox.varKnockback*kmult;
			var damage = hitbox.damage*dmult;
			var stun = hitbox.stun*smult;
			
			var data = new HitData(skb, vkb, damage, stun, hitbox.hitpause, hitbox, hurtbox);
			
			hitChar.ApplyKnockback(data);
			ignoreList.Add(hitChar);
			GD.Print($"{hitChar} was hit by {hitbox.Name}");
			ch.HandleHitting(hitbox, hurtbox, hitChar);
		}
		hitList.Clear();
	}
}
