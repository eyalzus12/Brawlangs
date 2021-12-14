using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using PartDir = System.Collections.Generic.Dictionary<string, AttackPart>;

public class AttackPart : Node2D
{
	public PartDir dir = new PartDir();
	public List<Hitbox> hitboxes = new List<Hitbox>();
	public int frameCount = 0;
	
	public HashSet<Character> ignoreList = new HashSet<Character>();
	public Dictionary<Area2D, Hitbox> hitList = new Dictionary<Area2D, Hitbox>();
	
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
	public float damageMult = 1f;
	
	[Export]
	public float knockbackMult = 1f;
	
	[Export]
	public int stunMult = 1;
	
	public static float globalDamageMult = 1f;
	public static float globalKnockbackMult = 1f;
	public static int globalStunMult = 1;
	
	public bool hit = false;
	
	public AnimationPlayer hitboxPlayer;
	public Attack att;
	public Character ch;
	
	public override void _Ready()
	{
		frameCount = 0;
		att = GetParent() as Attack;
		ch = att.ch;
		ConnectSignals();
		Init();
	}
	
	public static void ResetGlobals()
	{
		globalDamageMult = 1f;
		globalKnockbackMult = 1f;
		globalStunMult = 1;
	}
	
	public virtual void Reset()
	{
		hitboxes = GetChildren().FilterType<Hitbox>().ToList();
		/*foreach(var n in GetChildren()) if(n is Hitbox h)
		{
			hitboxes.Add(h);
		}*/
	}
	
	public void ConnectSignals()
	{
		hitboxes.ForEach(h => h.Connect("HitboxHit", this, nameof(HandleHit)));
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
		Loop();
		ActualHit();
	}
	
	public void BuildHitboxAnimator()
	{
		var hplayer = new AnimationPlayer();
		hplayer.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Physics;
		hplayer.Name = "AttackPlayer";
		AddChild(hplayer);
		hitboxPlayer = GetNode<AnimationPlayer>("AttackPlayer");
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
		
		/*
		GD.Print(anm.GetTrackCount());
		for(int i = 0; i < anm.GetTrackCount(); ++i)
		{
			GD.Print(anm.TrackGetKeyCount(i));
			for(int j = 0; j < anm.TrackGetKeyCount(i); ++j)
			{
				GD.Print(anm.TrackGetKeyTime(i, j));
				GD.Print(anm.TrackGetKeyValue(i, j));
			}
			GD.Print(anm.TrackGetPath(i));
		}
		*/
		
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
	
	public virtual void HandleHit(Hitbox hitbox, Area2D hurtbox)
	{
		if(!hitbox.Active) return;
		var hitChar = (Character)hurtbox.GetParent();
		if(!ch.CanHit(hitChar) || ignoreList.Contains(hitChar)) return;
		
		var current = new Hitbox();
		if(hitList.TryGetValue(hurtbox, out current))
		{
			if(hitbox.hitPriority > current.hitPriority)
				hitList[hurtbox] = hitbox;
		}
		else
		{
			hitList.Add(hurtbox, hitbox);
		}
	}
	
	public virtual void ActualHit(/*(Hitbox, Area2D) info*/)
	{
		if(!active) return;
		foreach(var entry in hitList)
		{
			Hitbox hitbox = entry.Value;
			Area2D hurtbox = entry.Key;
			var hitChar = (Character)hurtbox.GetParent();
			if(!ch.CanHit(hitChar) || ignoreList.Contains(hitChar)) continue;
			hit = true;
			OnHit(hitbox, hurtbox);
			var kmult = ch.knockbackDoneMult*hitbox.GetKnockbackMultiplier(hitChar)*knockbackMult*globalKnockbackMult;
			var dirvec = hitbox.KnockbackDir(ch, hitChar)*kmult;
			var skb = dirvec*hitbox.setKnockback + hitbox.momentumCarry*ch.GetVelocity();
			var vkb = dirvec*hitbox.varKnockback;
			var dmult = ch.damageDoneMult*hitbox.GetDamageMultiplier(hitChar)*damageMult*globalDamageMult;
			var damage = hitbox.damage*dmult;
			var smult = ch.stunDoneMult*hitbox.GetStunMultiplier(hitChar)*stunMult*globalStunMult;
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
}
