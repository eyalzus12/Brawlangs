using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using IntervalTree;

public class AttackPartFramePropertyManager
{
	public IntervalTree<long,string> Ranges{get; set;} = new IntervalTree<long,string>();
	public IEnumerable<string> this[long frame] => Ranges.Query(frame);
	public bool this[long frame, string property] => this[frame].Any(r => r == property);
	
	public void Add(long from, long to, string property)
	{
		if(from > to)
		{
			GD.PushError($"Cannot add range {from}-{to} to property {property} because {from} is greater than {to}");
			return;
		}
		
		Ranges.Add(from, to, property);
	}
	
	public override string ToString() => Ranges.ToString();
}
