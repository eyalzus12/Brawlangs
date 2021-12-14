using Godot;
using System;

public struct HitData
{
	public Vector2 Skb {get; set;}
	public Vector2 Vkb {get; set;}
	public float Damage {get; set;}
	public int Stun {get; set;}
	public int Hitpause {get; set;}
	public Area2D Hitter {get; set;}//FIXME: should be hitbox
	public Area2D Hitee {get; set;}//FIXME: should be hurtbox
	
	public HitData(Vector2 skb, Vector2 vkb, float damage, int stun, int hitpause, Area2D hitter, Area2D hitee)
	{
		Skb = skb;
		Vkb = vkb;
		Damage = damage;
		Stun = stun;
		Hitpause = hitpause;
		Hitter = hitter;
		Hitee = hitee;
	}
}
