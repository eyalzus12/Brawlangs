using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AttackPartTransitionTagExpression
{
	public object[] ExpressionParts{get; set;}
	
	public AttackPartTransitionTagExpression(){ExpressionParts = new object[]{};}
	public AttackPartTransitionTagExpression(IEnumerable<object> parts){ExpressionParts = parts.ToArray();}
	
	public bool Get(StateTagsManager tagsManager)
	{
		#if DEBUG_TAG_EXPRESSIONS
		GD.Print(ToString());
		#endif
		
		if(ExpressionParts.Length == 0) return true;
		
		var estack = new Stack<bool>();
		foreach(var part in ExpressionParts)
		{
			if(part is ValueTuple<string, StateTag> tag) estack.Push(tagsManager?[tag.Item1] == tag.Item2);
			else if(part is char c)
			{
				if(c == '!')
				{
					if(estack.Count < 1)
					{
						GD.PushError($"Attempt to run operator {c} on zero tags in attack part transition expression {this}");
						return false;
					}
					
					estack.Push(!estack.Pop());
				}
				else
				{
					if(estack.Count < 2)
					{
						GD.PushError($"Attempt to run operator {c} on one or zero tags in attack part transition expression {this}");
						return false;
					}
					
					var value1 = estack.Pop(); var value2 = estack.Pop();
					if(c == '&') estack.Push(value1&&value2);
					else if(c == '|') estack.Push(value1||value2);
					else
					{
						GD.PushError($"Unknown operator {c} in attack part transition expression {this}");
						return false;
					}
				}
			}
			else
			{
				GD.PushError($"Unkown object {part} with ToString {part?.ToString()} of type {part?.GetType()?.Name} found in attack part transition expression {this}");
				return false;
			}
		}
		
		if(estack.Count != 1)
		{
			GD.PushError($"Not enough operators in attack part transition expression {this}");
			return false;
		}
		
		return estack.Pop();
	}
	
	public override string ToString() => ExpressionParts.ToString();
}
