using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class IterUtils
{
	public static IEnumerable<T> Enumerable<T>(this Godot.Collections.Array<T> a)
	{
		foreach(var h in a) yield return h;
	}
	
	public static IEnumerable<T> Enumerable<T>(this Godot.Collections.Array a)
	{
		foreach(var h in a) yield return (T)h;
	}
	
	public static IEnumerable<object> Enumerable(this Godot.Collections.Array a) => a.Enumerable<object>();
	
	public static IEnumerable<(int, T)> Indexed<T>(this IEnumerable<T> e)
	{
		int i = 0; foreach(var o in e) yield return (i++, o);
	}
	
	public static IEnumerable<(int, object)> Indexed(this IEnumerable<object> e) => e.Indexed<object>();
	
	public static IEnumerable<TResult> Accumulate<TResult, T>(this IEnumerable<T> e, TResult seed, Func<TResult, T, TResult> accum)
	{
		foreach(var o in e) yield return (seed = accum(seed, o));
	}
	
	public static IEnumerable<object> Accumulate(this IEnumerable<object> e, object seed, Func<object, object, object> accum) => e.Accumulate<object>(seed, accum);
	public static IEnumerable<T> Accumulate<T>(this IEnumerable<object> e, T seed, Func<T, object, T> accum) => e.Accumulate<T, object>(seed, accum);
	
	public static IEnumerable<TResult> SelectWhere<T, TResult>(this IEnumerable<T> e, Func<T, TResult> selector, Func<T, bool> checker)
	{
		foreach(var o in e) if(checker(o)) yield return selector(o);
	}
	
	public static IEnumerable<object> SelectWhere(this IEnumerable<object> e, Func<object, object> selector, Func<object, bool> checker) => e.SelectWhere<object, object>(selector, checker);
	
	public static IEnumerable<TResult> FilterType<T, TResult>(this IEnumerable<T> e)
	{
		foreach(var o in e) if(o is TResult t) yield return t;
	}
	
	public static IEnumerable<T> FilterType<T>(this IEnumerable<object> e) => e.FilterType<object, T>();
	
	public static IEnumerable<T> FilterType<T>(this Godot.Collections.Array a)
	{
		foreach(var o in a) if(o is T t) yield return t;
	}
	
	public static IEnumerable<int> To(this int a, int b)
	{
		for(int i = a; i <= b; ++i) yield return i;
	}
	
	public static int Mult(this IEnumerable<int> e) => e.Aggregate(1, (a,n)=>a*n);
	
	public static IEnumerable<int> Indexes(this IEnumerable<object> e)
	{
		int i = 0; foreach(var o in e) yield return i++;
	}
	
	public static IEnumerable<T> Flatten<T>(params IEnumerable<T>[] earr)
	{
		foreach(var e in earr) foreach(var o in e) yield return o;
	}
	
	public static IEnumerable<(T1, T2)> Product<T1, T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2)
	{
		foreach(var o1 in e1) foreach(var o2 in e2) yield return (o1, o2);
	}
	
	public static IEnumerable<(object, object)> Product(this IEnumerable<object> e1, IEnumerable<object> e2) => e1.Product<object, object>(e2);
	
	public static IEnumerable<T> Concat<T>(this IEnumerable<T> e, T @add) => e.Concat<T>(new T[]{@add});
	public static IEnumerable<object> Concat(this IEnumerable<object> e, object @add) => e.Concat<object>(@add);
	
	public static void Debug(this IEnumerable<object> e)
	{
		foreach(var o in e) GD.Print(o.ToString());
	}
	
	public static void Debug(this object o) => ((IEnumerable<object>)o).Debug();
	
	public static TResult GetOrDefault<T, TResult>(this Dictionary<T, TResult> dict, T index, TResult @default)
	{
		//TOOD: create
		return @default;
	}
	
	public static Vector2 Sum(this IEnumerable<Vector2> e) => e.Aggregate(Vector2.Zero, (v1,v2)=>v1+v2);
	public static Vector2 Avg(this IEnumerable<Vector2> e) => e.Sum()/e.Count();
}
