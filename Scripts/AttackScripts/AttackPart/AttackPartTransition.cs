using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AttackPartTransition
{
	public List<Vector2> Frames{get; set;} = new List<Vector2>();
	public AttackPartTransitionTagExpression TagExpression{get; set;} = new AttackPartTransitionTagExpression();
	public string NextPart{get; set;} = "";
	
	public AttackPartTransition() {}
	
	public AttackPartTransition(IEnumerable<Vector2> frames, AttackPartTransitionTagExpression tagExpression, string nextPart)
	{
		tagExpression.Get(null);//test expression
		TagExpression = tagExpression;
		Frames = new List<Vector2>(frames);
		NextPart = nextPart;
	}
	
	public bool ContainsFrame(int frame) => Frames.Any(v => v.x <= frame && frame <= v.y);
	public bool MatchesTag(StateTagsManager tagsManager, int currentFrame) =>
		(currentFrame == -1 || ContainsFrame(currentFrame)) &&
		TagExpression.Get(tagsManager);
}
