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
	
	public Dictionary<string, Type> LoadExtraProperties = new Dictionary<string, Type>();
	
	public bool active = false;
	
	public (Hitbox, Area2D) activator;
	
	[Export]
	public int startup = 0;
	
	[Export]
	public int endlag = 0;
	
	[Export]
	public int length = 0;
	
	[Export]
	public Vector2 movement = default;
	
	[Export]
	public bool hitPart = false;
	
	[Export]
	public int missEndlag = 0;
	
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
	
	public void LoadExtraProperty<T>(string s)
	{
		LoadExtraProperties.Add(s, typeof(T));
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
	
	public virtual void Pause()
	{
		hitboxPlayer.Stop(false);
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!active) return;
		++frameCount;
		Loop();
		ActualHit(/*activator*/);
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
	
	public void cnp(string dummy="")
	{
		if(!active) return;
		ChangePart(GetNextPart());
	}
	
	/*public virtual void CalculateNextPart()
	{
		if(hitPart) HitMissPart();
		else NextPart();
	}*/
	
	public virtual string GetNextPart() => hitPart?hit?"Hit":"Miss":"Next";
	
	public void ChangePart(string part)
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
			var skb = new Vector2(ch.direction,1)*hitbox.setKnockback*ch.knockbackDoneMult;
			var vkb = new Vector2(ch.direction,1)*hitbox.varKnockback*ch.knockbackDoneMult;
			var damage = hitbox.damage*ch.damageDoneMult;
			var stun = hitbox.stun*ch.stunDoneMult;
			hitChar.ApplyKnockback(skb, vkb, damage, stun, hitbox.hitpause);
			ignoreList.Add(hitChar);
			GD.Print($"{hitChar} was hit by {hitbox.Name}");
			att.OnHit(hitbox, hurtbox);
			ch.HandleHitting(hitbox, hurtbox, hitChar);
		}
		hitList.Clear();
	}
	
	public virtual int GetEndlag() => endlag + (hit?0:missEndlag);
}
