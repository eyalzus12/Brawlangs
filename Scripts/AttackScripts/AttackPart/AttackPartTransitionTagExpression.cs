using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AttackPartTransitionTagExpression
{
	public Func<StateTagsManager,bool> TagExpression{get; set;}
	public AttackPartTransitionTagExpression(){TagExpression = (StateTagsManager t) => true;}
	public AttackPartTransitionTagExpression(string expression){TagExpression = ParsedExpressionToFunc(ParseExpression(expression));}
	public bool Get(StateTagsManager tagsManager) => TagExpression(tagsManager);
	
	public static Func<StateTagsManager,bool> ParsedExpressionToFunc(IEnumerable<object> parsedExpression)
	{
		var parts = parsedExpression.ToList();
		if(parts.Count == 0) return (StateTagsManager t) => false;
		var estack = new Stack<Func<StateTagsManager,bool>>();
		foreach(var part in parts)
		{
			if(part is ValueTuple<string, StateTag> tag) estack.Push((StateTagsManager t) => t?[tag.Item1] == tag.Item2);
			else if(part is char c)
			{
				if(c == '!')
				{
					if(estack.Count < 1)
					{
						GD.PushError($"Attempt to run operator {c} on zero tags in attack part transition expression {parts.ToArray().ToString()}");
						return (StateTagsManager t) => false;
					}
					
					estack.Push(estack.Pop().Not());
				}
				else
				{
					if(estack.Count < 2)
					{
						GD.PushError($"Attempt to run operator {c} on one or zero tags in attack part transition expression {parts.ToArray().ToString()}");
						return (StateTagsManager t) => false;
					}
					
					var value1 = estack.Pop(); var value2 = estack.Pop();
					if(c == '&') estack.Push(value1.And(value2));
					else if(c == '|') estack.Push(value1.Or(value2));
					else
					{
						GD.PushError($"Unknown operator {c} in attack part transition expression {parts.ToArray().ToString()}");
						return (StateTagsManager t) => false;
					}
				}
			}
			else
			{
				GD.PushError($"Unkown object {part} with ToString {part?.ToString()} of type {part?.GetType()?.Name} found in attack part transition expression {parts.ToArray().ToString()}");
				return (StateTagsManager t) => false;
			}
		}
		
		if(estack.Count != 1)
		{
			GD.PushError($"Not enough operators in attack part transition expression {parts.ToArray().ToString()}");
			return (StateTagsManager t) => false;
		}
		
		return estack.Pop();
	}
	
	private static readonly char[] OPERATORS = new char[]{'!', '|', '&', ')', '('};
	public static IEnumerable<object> ParseExpression(string s)
	{
		if(s == "") return Enumerable.Empty<object>();
		
		var result = new List<object>();
		var operators = new Stack<char>();
		
		var stateName = new StringBuilder();
		var tagValue = new StringBuilder();
		bool doingTagValue = false;
		
		foreach(var c in s)
		{
			if(c == '(') operators.Push(c);
			else if(OPERATORS.Contains(c))
			{
				if(stateName.Length > 0 && tagValue.Length > 0)
				{
					var _stateName = stateName.ToString();
					var _tagValue = tagValue.ToString();
					StateTag tag;
					
					if(!Enum.TryParse<StateTag>(_tagValue, out tag))
					{
						GD.PushError($"Unknown tag value {_tagValue} in attack part transition expression {s}");
						return Enumerable.Empty<object>();
					}
					
					result.Add((_stateName, tag));
					stateName.Clear(); tagValue.Clear(); doingTagValue = false;
				}
				else if(doingTagValue)
				{
					GD.PushError($"Operator or closing bracket immedietly after a period in attack part transition expression {s}");
					return Enumerable.Empty<object>();
				}
				
				int pre = OPERATORS.FindIndex(c);
				while(operators.Count > 0 && OPERATORS.FindIndex(operators.Peek()) <= pre) result.Add(operators.Pop());
				
				if(c == ')')
				{
					if(operators.Count == 0)
					{
						GD.PushError($"Attack part transition expression {s} has imbalanced brackets");
						return Enumerable.Empty<object>();
					}
					
					operators.Pop(); //remove the (
				}
				else operators.Push(c);
			}
			else if(c == '.')
			{
				if(doingTagValue)
				{
					GD.PushError($"Too many periods in attack part transition expression {s}");
					return Enumerable.Empty<object>();
				}
				
				doingTagValue = true;
			}
			else
			{
				if(doingTagValue) tagValue.Append(c);
				else stateName.Append(c);
			}
		}
		
		if(doingTagValue)
		{
			var _stateName = stateName.ToString();
			var _tagValue = tagValue.ToString();
			StateTag tag;
			if(!Enum.TryParse<StateTag>(_tagValue, out tag)) throw new FormatException($"Unknown tag value {_tagValue} in attack part transition expression {s}");
			result.Add((_stateName, tag));
			stateName.Clear(); tagValue.Clear(); doingTagValue = false;
		}
		else if(stateName.Length != 0)
		{
			GD.PushError($"Period without tag value in attack part transition expression {s}");
			return Enumerable.Empty<object>();
		}
		
		while(operators.Count > 0)
		{
			if(operators.Peek() == '(')
			{
				GD.PushError($"Attack part transition expression {s} has imbalanced brackets");
				return Enumerable.Empty<object>();
			}
			
			result.Add(operators.Pop());
		}
		
		return result;
	}
}
