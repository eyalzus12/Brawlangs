using Godot;
using System;
using System.Collections.Generic;

public class CharacterHitbox : Hitbox
{
	public Dictionary<string, float> stateKnockbackMult;
	public Dictionary<string, float> stateDamageMult;
	public Dictionary<string, float> stateStunMult;
	
	private float StateMult(Node2D n, Dictionary<string, float> chooseFrom)
	{
		if(chooseFrom is null || !(n is Character c)) return 1f;//only works on characters obv
		
		var f = 1f;
		
		for(var t = c.currentState.GetType(); t.Name != "State"; t = t.BaseType)
		{
			if(chooseFrom.TryGetValue(t.Name.Replace("State", ""), out f))
				return f;
		}
		
		return 1f;
	}
	
	public virtual float GetKnockbackMultiplier(Node2D n) => TeamMult(n, teamKnockbackMult) * StateMult(n, stateKnockbackMult);
	public virtual float GetDamageMultiplier(Node2D n) => TeamMult(n, teamDamageMult) * StateMult(n, stateDamageMult);
	public virtual int GetStunMultiplier(Node2D n) => (int)(TeamMult(n, (float)teamStunMult) * StateMult(n, stateStunMult));
}
