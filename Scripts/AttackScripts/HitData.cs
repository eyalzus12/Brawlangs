using Godot;
using System;
using System.Collections.Generic;

public struct HitData
{
	public Vector2 Skb {get; set;}
	public Vector2 Vkb {get; set;}
	public float Damage {get; set;}
	public float Stun {get; set;}
	public int Hitpause {get; set;}
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
	
	public HitData(Vector2 skb, Vector2 vkb, float damage, float stun, int hitpause, Hitbox hitter, Hurtbox hitee)
	{
		Skb = skb;
		Vkb = vkb;
		Damage = damage;
		Stun = stun;
		Hitpause = hitpause;
		Hitter = hitter;
		Hitee = hitee;
		ExtraData = new Dictionary<string, object>();
	}
}
