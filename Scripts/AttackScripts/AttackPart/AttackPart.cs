using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using PartDir = System.Collections.Generic.Dictionary<string, AttackPart>;

public class AttackPart : Node2D
{
	public PartDir dir = new PartDir();
	public List<CharacterHitbox> hitboxes = new List<CharacterHitbox>();
	public int frameCount = 0;
	
	public HashSet<Node2D> ignoreList = new HashSet<Node2D>();
	public Dictionary<Hurtbox, CharacterHitbox> hitList = new Dictionary<Hurtbox, CharacterHitbox>();
	
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	public bool active = false;
	
	[Export]
	public int startup = 0;
	
	[Export]
	public int endlag = 0;
	
	[Export]
	public int length = 0;
	
	[Export]
	public Vector2 movement = default;
	
	[Export]
	public int missEndlag = 0;
	
	[Export]
	public int cooldown = 0;
	
	[Export]
	public int missCooldown = 0;
	
	[Export]
	public float damageMult = 1f;
	
	[Export]
	public float knockbackMult = 1f;
	
	[Export]
	public int stunMult = 1;
	
	[Export]
	public string startupAnimation;
	
	[Export]
	public string attackAnimation;
	
	[Export]
	public string endlagAnimation;
	
	[Export]
	public string attackSound;
	
	public bool hit = false;
	
	public AnimationPlayer hitboxPlayer;
	public Attack att;
	public Character ch;
	
	public override void _Ready()
	{
		frameCount = 0;
		hitboxes = GetChildren().FilterType<CharacterHitbox>().ToList();
		hitboxes.ForEach(h => h.Connect("HitboxHit", this, nameof(HandleHit)));
		BuildHitboxAnimator();
		Init();
	}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public void Connect(AttackPart ap)
	{
		dir.Add(ap.Name, ap);
	}
	
	public void Connect(string name, AttackPart ap)
	{
		dir.Add(name, ap);
	}
	
	public virtual void Init()
	{
		
	}
	
	public virtual void Activate()
	{
		ch.PlayAnimation(startupAnimation);
		ch.PlaySound(attackSound);
		active = true;
		hit = false;
		frameCount = 0;
		
		if(movement != Vector2.Zero)
			ch.vec = movement * new Vector2(ch.direction, 1);
		
		OnStart();
		//hitboxPlayer = GetNode("AttackPlayer") as AnimationPlayer;
		//GD.Print("activating");
		hitboxPlayer.Play("HitboxActivation");
		hitList.Clear();
		ignoreList.Clear();
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!active) return;
		++frameCount;
		if(frameCount > startup) ch.PlayAnimation(attackAnimation);
		Loop();
		ActualHit();
	}
	
	public void BuildHitboxAnimator()
	{
		hitboxPlayer = new AnimationPlayer();
		hitboxPlayer.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Physics;
		hitboxPlayer.Name = "AttackPlayer";
		AddChild(hitboxPlayer);
		var anm = new Animation();
		anm.Length = (startup+length/*+endlag*/)/60f;
		hitboxPlayer.AddAnimation("HitboxActivation", anm);
		foreach(var h in hitboxes)
		{
			int trc = anm.AddTrack(Animation.TrackType.Value);
			var path = h.GetPath() + ":Active";
			anm.TrackSetPath(trc, path);
			
			foreach(var v in h.activeFrames)
			{
				anm.TrackInsertKey(trc, (startup+v.x)/60f, true);
				anm.TrackInsertKey(trc, (startup+v.y)/60f, false);
			}
		}
		
		#if DEBUG_HITBOX_PLAYER
		GD.Print("Animation hitbox player debug ahead");
		GD.Print(anm.GetTrackCount());
		for(int i = 0; i < anm.GetTrackCount(); ++i)
		{
			GD.Print(anm.TrackGetPath(i));
			GD.Print(anm.TrackGetKeyCount(i));
			for(int j = 0; j < anm.TrackGetKeyCount(i); ++j)
			{
				GD.Print(anm.TrackGetKeyTime(i, j) + " " + anm.TrackGetKeyValue(i, j));
			}
		}
		GD.Print("-------------------------------------------");
		#endif
		
		hitboxPlayer.Connect("animation_finished", this, "cnp");
	}
	
	public virtual void Stop()
	{
		//GD.Print(hitboxes.Count);
		hitboxPlayer.Stop(true);
		hitboxes.ForEach(h => h.Active = false);
		active = false;
		OnEnd();
		hitList.Clear();
		ignoreList.Clear();
	}
	
	public virtual void Pause()
	{
		hitboxPlayer.Stop();
	}
	
	public virtual void Resume()
	{
		hitboxPlayer.Play();
	}
	
	public virtual void Loop()
	{
		
	}
	
	public virtual void OnStart()
	{
		
	}
	
	public virtual void OnEnd()
	{
		
	}
	
	public virtual void OnHit(Hitbox hitbox, Area2D hurtbox)
	{
		
	}
	
	public virtual void cnp(string dummy="")
	{
		if(!active) return;
		ChangePart(GetNextPart());
	}
	
	/*public virtual void CalculateNextPart()
	{
		if(hitPart) HitMissPart();
		else NextPart();
	}*/
	
	private List<string> Describe(Character c)
	{
		var l = new List<string>();
		if(c.ceilinged) l.Add("Ceiling");
		if(c.walled) l.Add("Wall");
		if(c.grounded) l.Add("Grounded");
		else l.Add("Aerial");
		if(true) l.Add("");
		return l;
	}
	
	public virtual string GetNextPart()
	{
		foreach(var property in Describe(ch))
		{
			if(hit && dir.ContainsKey($"{property}Hit")) return $"{property}Hit";
			if(dir.ContainsKey($"{property}Miss")) return $"{property}Miss";
			if(property != "" && dir.ContainsKey(property)) return property;
		}
		
		return "Next";
	}
	
	public virtual void ChangePart(string part)
	{
		if(!active || part == "") return;
		var changeTo = GetConnectedPart(part);
		att.SetPart(changeTo);
	}
	
	public AttackPart GetConnectedPart(string name)
	{
		try {return dir[name];}
		catch(KeyNotFoundException) {return null;}
	}
	
	public virtual void HandleHit(CharacterHitbox hitbox, Area2D hurtbox)
	{
		if(!hitbox.Active) return;
		if(!(hurtbox is Hurtbox realhurtbox)) return;//can only handle hurtboxes for hitting
		var hitChar = (Character)hurtbox.GetParent();
		if(!ch.CanHit(hitChar) || ignoreList.Contains(hitChar)) return;
		
		var current = new CharacterHitbox();
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
	
	public virtual void ActualHit(/*(Hitbox, Area2D) info*/)
	{
		if(!active) return;
		foreach(var entry in hitList)
		{
			Hitbox hitbox = entry.Value;
			Hurtbox hurtbox = entry.Key;
			var hitChar = (Character)hurtbox.GetParent();
			if(!ch.CanHit(hitChar) || ignoreList.Contains(hitChar)) continue;
			hit = true;
			OnHit(hitbox, hurtbox);
			
			var kmult = ch.knockbackDoneMult*knockbackMult*att.knockbackMult;
			var dmult = ch.damageDoneMult*damageMult*att.damageMult;
			var smult = ch.stunDoneMult*stunMult*att.stunMult;
			
			if(hitbox is CharacterHitbox chitbox)
			{
				kmult *= chitbox.GetKnockbackMultiplier(hitChar);
				dmult *= chitbox.GetDamageMultiplier(hitChar);
				smult *= chitbox.GetStunMultiplier(hitChar);
			}
			
			var dirvec = hitbox.KnockbackDir(hitChar)*kmult;
			var skb = dirvec*hitbox.setKnockback + hitbox.momentumCarry*ch.GetVelocity();
			var vkb = dirvec*hitbox.varKnockback;
			var damage = hitbox.damage*dmult;
			var stun = hitbox.stun*smult;
			
			var data = new HitData(skb, vkb, damage, stun, hitbox.hitpause, hitbox, hurtbox);
			
			hitChar.ApplyKnockback(data);
			ignoreList.Add(hitChar);
			GD.Print($"{hitChar} was hit by {hitbox.Name}");
			att.OnHit(hitbox, hurtbox);
			ch.HandleHitting(hitbox, hurtbox, hitChar);
		}
		hitList.Clear();
	}
	
	public virtual int GetEndlag() => endlag + (hit?0:missEndlag);
	public virtual int GetCooldown() => cooldown + (hit?0:missCooldown);
}
