using Godot;
using System;
using System.Collections.Generic;

public struct HitData
{
	public Vector2 SKB {get; set;}
	public Vector2 VKB {get; set;}
	
	public float Damage {get; set;}
	
	public float SStun {get; set;}
	public float VStun{get; set;}
	
	public float SHitPause {get; set;}
	public float VHitPause {get; set;}
	
	public float SHitLag {get; set;}
	public float VHitLag {get; set;}
	
	public Hitbox Hitter {get; set;}
	public Hurtbox Hitee {get; set;}
	
	public HitData(
		Vector2 skb, Vector2 vkb,
		float damage,
		float sstun, float vstun,
		float shitpause, float vhitpause,
		float shitlag, float vhitlag,
		Hitbox hitter, Hurtbox hitee)
	{
		SKB = skb;
		VKB = vkb;
		
		Damage = damage;
		
		SStun = sstun;
		VStun = vstun;
		
		SHitPause = shitpause;
		VHitPause = vhitpause;
		
		SHitLag = shitlag;
		VHitLag = vhitlag;
		
		Hitter = hitter;
		Hitee = hitee;
	}
}
