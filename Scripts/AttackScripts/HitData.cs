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
	
	public Hitbox Hitter {get; set;}
	public Hurtbox Hitee {get; set;}
	
	
	public Dictionary<string, object> ExtraData {get; set;}
	public object this[string s]
	{
		get => ExtraData[s];
		set
		{
			if(ExtraData.ContainsKey(s)) ExtraData[s] = value;
			else ExtraData.Add(s, value);
		}
	}
	
	public HitData(
		Vector2 skb, Vector2 vkb,
		float damage,
		float sstun, float vstun,
		float shitpause, float vhitpause,
		Hitbox hitter, Hurtbox hitee)
	{
		SKB = skb;
		VKB = vkb;
		
		Damage = damage;
		
		SStun = sstun;
		VStun = vstun;
		
		SHitPause = shitpause;
		VHitPause = vhitpause;
		
		Hitter = hitter;
		Hitee = hitee;
		
		ExtraData = new Dictionary<string, object>();
	}
}
