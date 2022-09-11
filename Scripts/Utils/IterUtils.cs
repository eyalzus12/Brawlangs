using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
	
	public static IEnumerable<TResult> Accumulate<TResult, T>(this IEnumerable<T> e, TResult seed, Func<TResult, T, TResult> accum)
	{
		foreach(var o in e) yield return (seed = accum(seed, o));
	}
	
	public static IEnumerable<TResult> SelectWhere<T, TResult>(this IEnumerable<T> e, Func<T, TResult> selector, Func<T, bool> checker)
	{
		foreach(var o in e) if(checker(o)) yield return selector(o);
	}
	
	public static IEnumerable<TResult> FilterType<T, TResult>(this IEnumerable<T> e)
	{
		foreach(var o in e) if(o is TResult t) yield return t;
	}
	
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
	
	public static void Debug<T>(this IEnumerable<T> e)
	{
		foreach(var o in e) GD.Print(o.ToString());
	}
	
	public static Vector2 Sum(this IEnumerable<Vector2> e) => e.Aggregate(Vector2.Zero, (v1,v2)=>v1+v2);
	public static Vector2 Avg(this IEnumerable<Vector2> e) => e.Sum()/e.Count();
	
	public static void Rotate<T>(this Queue<T> q) => q.Enqueue(q.Dequeue());
	
	public static void ForEach<T>(this IEnumerable<T> e, Action<T> a)
	{
		foreach(var h in e) a(h);
	}
	
	public static IEnumerable<(T1,T2)> Zip<T1,T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2)
	{
		var en1 = e1.GetEnumerator();
		var en2 = e2.GetEnumerator();
		while(en1.MoveNext() && en2.MoveNext()) yield return (en1.Current, en2.Current);
	}
	
	public static IEnumerable<(T1,T2,T3)> TriZip<T1,T2,T3>(IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3)
	{
		var en1 = e1.GetEnumerator();
		var en2 = e2.GetEnumerator();
		var en3 = e3.GetEnumerator();
		while(en1.MoveNext() && en2.MoveNext() && en3.MoveNext()) yield return (en1.Current, en2.Current, en3.Current);
	}
	
	public static IEnumerable<IEnumerable<T>> MultiZip<T>(params IEnumerable<T>[] es)
	{
		var ens = es.Select(e => e.GetEnumerator());
		while(ens.All(en => en.MoveNext())) yield return ens.Select(en => en.Current);
	}
	
	public static T FirstOrDefault<T>(this IEnumerable<T> e, Func<T, bool> predicate, T @default = default)
	{
		foreach(var h in e) if(predicate(h)) return h;
		return @default;
	}
	
	public static IEnumerable<int> IndicesWhere<T>(this IEnumerable<T> e, Func<T, bool> predicate)
	{
		int i = 0;
		foreach(var h in e)
		{
			if(predicate(h)) yield return i;
			i++;
		}
	}
	
	public static TValue GetValueOrDefault<TKey,TValue>(this Dictionary<TKey,TValue> d, TKey k, TValue @default = default)
	{
		TValue res; return d.TryGetValue(k, out res)?res:@default;
	}
	
	public static int FindIndex<T>(this IEnumerable<T> e, T v) where T : IEquatable<T>
	{
		int i = 0; foreach(var o in e) {++i; if(o.Equals(v)) return i;}
		return i;
	}
}
