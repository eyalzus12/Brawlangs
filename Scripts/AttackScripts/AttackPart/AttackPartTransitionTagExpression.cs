using Godot;
using System;
using System.Collections.Generic;

public class AttackPartTransitionTagExpression
{
	public List<object> ExpressionParts{get; set;} = new List<object>();
	
	public AttackPartTransitionTagExpression()
	{
		ExpressionParts = new List<object>();
	}
	
	public AttackPartTransitionTagExpression(IEnumerable<object> parts)
	{
		ExpressionParts = new List<object>(parts);
	}
	
	public bool Get(StateTagsManager tagsManager)
	{
		if(ExpressionParts.Count == 0) return true;
		
		var estack = new Stack<bool>();
		foreach(var part in ExpressionParts)
		{
			if(part is ValueTuple<string, StateTag> tag) estack.Push(tagsManager?[tag.Item1] == tag.Item2);
			else if(part is char c)
			{
				if(c == '!')
				{
					if(estack.Count < 1) throw new FormatException($"Attempt to run operator {c} on less than two tags in attack part transition expression {this}");
					estack.Push(!estack.Pop());
				}
				else
				{
					if(estack.Count < 2) throw new FormatException($"Attempt to run operator {c} on less than two tags in attack part transition expression {this}");
					var value1 = estack.Pop(); var value2 = estack.Pop();
					if(c == '&') estack.Push(value1&&value2);
					else if(c == '|') estack.Push(value1||value2);
					else throw new FormatException($"Unknown operator {c} in attack part transition expression {this}");
				}
			}
			else throw new Exception($"Unkown object {part} of type {part.GetType().Name} found in attack part transition expression {this}");
		}
		
		if(estack.Count != 1) throw new FormatException($"Not enough operators in attack part transition expression {this}");
		return estack.Pop();
	}
	
	public override string ToString() => ExpressionParts.ToArray().ToString();
}
