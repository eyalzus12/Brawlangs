using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AttackPartTransitionManager
{
	public List<AttackPartTransition> Transitions{get; set;} = new List<AttackPartTransition>();
	public void Add(AttackPartTransition apt) => Transitions.Add(apt);
	public void Clear() => Transitions.Clear();
	
	public string NextAttackPart(StateTagsManager tagsManager, int currentFrames) =>
		Transitions
		.FirstOrDefault(
			t => t.MatchesTag(tagsManager, currentFrames), 
			new AttackPartTransition()
		).NextPart;
}
