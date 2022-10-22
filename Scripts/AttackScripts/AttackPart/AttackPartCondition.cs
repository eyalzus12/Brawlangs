using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AttackPartCondition
{
	public string FrameProperty{get; set;} = "End";
	public TagExpression Expression{get; set;} = new TagExpression();
	public string Result{get; set;} = "";
	
	public AttackPartCondition() {}
	
	public AttackPartCondition(string frameProperty, TagExpression expression, string result)
	{
		FrameProperty = frameProperty;
		Expression = expression;
		Result = result;
	}
	
	public bool Get(StateTagsManager stm, AttackPartFramePropertyManager apfpm, long cf) => apfpm[cf, FrameProperty] && Expression.Get(stm);
}
