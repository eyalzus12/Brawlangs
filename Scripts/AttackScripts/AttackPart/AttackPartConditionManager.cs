using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AttackPartConditionManager
{
	public AttackPartConditionManager() {}
	public List<AttackPartCondition> Conditions{get; set;} = new List<AttackPartCondition>();
	public void Add(AttackPartCondition a) => Conditions.Add(a);
	
	public IEnumerable<(string,bool)> Get(StateTagsManager stm, AttackPartFramePropertyManager apfpm, long cf) =>
		Conditions.Select(c => (c.Result, c.Get(stm,apfpm,cf)));
}
