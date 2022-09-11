using Godot;
using System;
using System.Collections.Generic;

public class FirstTupleItemComparer<T1,T2> : IComparer<(T1, T2)> where T1 : IComparable<T1>
{
	public int Compare((T1, T2) fc1, (T1, T2) fc2) => fc1.Item1.CompareTo(fc2.Item1);
}
