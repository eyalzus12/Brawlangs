using Godot;
using System;
using System.Collections.Generic;

public struct HitData
{
	public Vector2 Skb {get; set;}
	public Vector2 Vkb {get; set;}
	public float Damage {get; set;}
	public int Stun {get; set;}
	public int Hitpause {get; set;}
	public Hitbox Hitter {get; set;}
	public Hurtbox Hitee {get; set;}
	public Dictionary<string, object> ExtraData {get; set;}
	public object this[string s]
	{
		get => ExtraData[s];
		set
		{
			try
			{
				ExtraData[s] = value;
			}
			catch(KeyNotFoundException)
			{
				try
				{
					ExtraData.Add(s, value);
				}
				catch(ArgumentException)
				{
					GD.Print($"WTF. For some fucking reason, the value {value} was not in the dictionary of this hit data, but couldnt be fucking added. WHY???");
				}
			}
		}
	}
	
	public HitData(Vector2 skb, Vector2 vkb, float damage, int stun, int hitpause, Hitbox hitter, Hurtbox hitee)
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
