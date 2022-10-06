using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class IterUtils
{
	public static IEnumerable<T> ToEnumerable<T>(this Godot.Collections.Array<T> a) {foreach(var h in a) yield return h;}
	public static IEnumerable<T> ToEnumerable<T>(this Godot.Collections.Array a){foreach(var h in a) yield return (T)h;}
	
	public static IEnumerable<(int, T)> Indexed<T>(this IEnumerable<T> e) {int i = 0; foreach(var o in e) yield return (i++, o);}
	
	public static IEnumerable<TResult> Accumulate<TResult, T>(this IEnumerable<T> e, TResult seed, Func<TResult, T, TResult> accum)
		{foreach(var o in e) yield return (seed = accum(seed, o));}
	
	public static IEnumerable<TResult> FilterType<T, TResult>(this IEnumerable<T> e)
		{foreach(var o in e) if(o is TResult t) yield return t;}
	
	public static IEnumerable<T> FilterType<T>(this Godot.Collections.Array a)
		{foreach(var o in a) if(o is T t) yield return t;}
	
	public static int Mult(this IEnumerable<int> e) => e.Aggregate(1, (a,n)=>a*n);
	
	public static IEnumerable<T> Flatten<T>(params IEnumerable<T>[] earr)
		{foreach(var e in earr) foreach(var o in e) yield return o;}
	
	public static IEnumerable<(T1, T2)> Product<T1, T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2)
		{foreach(var o1 in e1) foreach(var o2 in e2) yield return (o1, o2);}
	
	public static Vector2 Sum(this IEnumerable<Vector2> e) => e.Aggregate(Vector2.Zero, (v1,v2)=>v1+v2);
	public static Vector2 Avg(this IEnumerable<Vector2> e) => e.Sum()/e.Count();
	
	public static void Rotate<T>(this Queue<T> q) => q.Enqueue(q.Dequeue());
	
	public static void ForEach<T>(this IEnumerable<T> e, Action<T> a) {foreach(var h in e) a(h);}
	
	public static IEnumerable<(T1,T2)> Zip<T1,T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2)
	{
		var en1 = e1.GetEnumerator();
		var en2 = e2.GetEnumerator();
		while(en1.MoveNext() && en2.MoveNext()) yield return (en1.Current, en2.Current);
	}
	
	public static IEnumerable<(T1,T2,T3)> Zip<T1,T2,T3>(IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3)
	{
		var en1 = e1.GetEnumerator();
		var en2 = e2.GetEnumerator();
		var en3 = e3.GetEnumerator();
		while(en1.MoveNext() && en2.MoveNext() && en3.MoveNext()) yield return (en1.Current, en2.Current, en3.Current);
	}
	
	public static IEnumerable<(T1,T2,T3,T4)> Zip<T1,T2,T3,T4>(IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3, IEnumerable<T4> e4)
	{
		var en1 = e1.GetEnumerator();
		var en2 = e2.GetEnumerator();
		var en3 = e3.GetEnumerator();
		var en4 = e4.GetEnumerator();
		while(en1.MoveNext() && en2.MoveNext() && en3.MoveNext() && en4.MoveNext()) yield return (en1.Current, en2.Current, en3.Current, en4.Current);
	}
	
	public static IEnumerable<(T1,T2,T3,T4,T5)> Zip<T1,T2,T3,T4,T5>(
		IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3, IEnumerable<T4> e4,
		IEnumerable<T5> e5)
	{
		var en1 = e1.GetEnumerator();
		var en2 = e2.GetEnumerator();
		var en3 = e3.GetEnumerator();
		var en4 = e4.GetEnumerator();
		var en5 = e5.GetEnumerator();
		while(en1.MoveNext() && en2.MoveNext() && en3.MoveNext() && en4.MoveNext()
			&& en5.MoveNext())
			yield return (en1.Current, en2.Current, en3.Current, en4.Current, en5.Current);
	}
	
	public static IEnumerable<(T1,T2,T3,T4,T5,T6)> Zip<T1,T2,T3,T4,T5,T6>(
		IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3, IEnumerable<T4> e4,
		IEnumerable<T5> e5, IEnumerable<T6> e6)
	{
		var en1 = e1.GetEnumerator();
		var en2 = e2.GetEnumerator();
		var en3 = e3.GetEnumerator();
		var en4 = e4.GetEnumerator();
		var en5 = e5.GetEnumerator();
		var en6 = e6.GetEnumerator();
		while(en1.MoveNext() && en2.MoveNext() && en3.MoveNext() && en4.MoveNext()
			&& en5.MoveNext() && en6.MoveNext())
			yield return (en1.Current, en2.Current, en3.Current, en4.Current, en5.Current, en6.Current);
	}
	
	public static IEnumerable<(T1,T2,T3,T4,T5,T6,T7)> Zip<T1,T2,T3,T4,T5,T6,T7>(
		IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3, IEnumerable<T4> e4,
		IEnumerable<T5> e5, IEnumerable<T6> e6, IEnumerable<T7> e7)
	{
		var en1 = e1.GetEnumerator();
		var en2 = e2.GetEnumerator();
		var en3 = e3.GetEnumerator();
		var en4 = e4.GetEnumerator();
		var en5 = e5.GetEnumerator();
		var en6 = e6.GetEnumerator();
		var en7 = e7.GetEnumerator();
		while(en1.MoveNext() && en2.MoveNext() && en3.MoveNext() && en4.MoveNext()
			&& en5.MoveNext() && en6.MoveNext() && en7.MoveNext())
			yield return (en1.Current, en2.Current, en3.Current, en4.Current, en5.Current, en6.Current, en7.Current);
	}
	
	public static IEnumerable<(T1,T2,T3,T4,T5,T6,T7,TRest)> Zip<T1,T2,T3,T4,T5,T6,T7,TRest>(
		IEnumerable<T1> e1, IEnumerable<T2> e2, IEnumerable<T3> e3, IEnumerable<T4> e4,
		IEnumerable<T5> e5, IEnumerable<T6> e6, IEnumerable<T7> e7, IEnumerable<TRest> erest)
	{
		var en1 = e1.GetEnumerator();
		var en2 = e2.GetEnumerator();
		var en3 = e3.GetEnumerator();
		var en4 = e4.GetEnumerator();
		var en5 = e5.GetEnumerator();
		var en6 = e6.GetEnumerator();
		var en7 = e7.GetEnumerator();
		var enrest = erest.GetEnumerator();
		while(en1.MoveNext() && en2.MoveNext() && en3.MoveNext() && en4.MoveNext()
			&& en5.MoveNext() && en6.MoveNext() && en7.MoveNext() && enrest.MoveNext())
			yield return (en1.Current, en2.Current, en3.Current, en4.Current, en5.Current, en6.Current, en7.Current, enrest.Current);
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
	
	public static int FirstIndexWhere<T>(this IEnumerable<T> e, Func<T, bool> predicate)
	{
		int i = 0;
		foreach(var o in e)
			if(predicate(o)) return i;
			else ++i;
		return i;
	}
	
	public static IEnumerable<int> IndicesWhere<T>(this IEnumerable<T> e, Func<T, bool> predicate)
	{
		int i = 0;
		foreach(var h in e)
			if(predicate(h)) yield return i;
			else i++;
	}
	
	public static int FindIndex<T>(this IEnumerable<T> e, T v) where T : IEquatable<T> => e.FirstIndexWhere<T>(v.Equals);
	
	public static TValue GetValueOrDefault<TKey,TValue>(this Dictionary<TKey,TValue> d, TKey k, TValue @default = default)
		{TValue res; return d.TryGetValue(k, out res)?res:@default;}
	
	public static bool TryAdd<TKey,TValue>(this Dictionary<TKey,TValue> d, TKey k, TValue v)
	{
		if(!d.ContainsKey(k))
		{
			d.Add(k,v);
			return true;
		}
		return false;
	}
	
	public static IEnumerable<T> RepeatListOf<T>(this T t, int times)
		{for(int i = 0; i < times; ++i) yield return t;}
	
	public static IEnumerable<T> CircularTake<T>(this IEnumerable<T> e, int amount)
	{
		var en = e.GetEnumerator();
		for(int i = 0; i < amount; en.Reset()) for(; i < amount && en.MoveNext(); ++i) yield return en.Current;
	}
	
	public static IEnumerable<T> CircularTakeOrDefault<T>(this IEnumerable<T> e, int amount, T @default = default(T))
	{
		var a = e.ToArray<T>();//make sure not to generate it multiple times
		if(a.Length == 0) return @default.RepeatListOf<T>(amount);
		else return a.CircularTake<T>(amount);
	}
	
	public static T Choice<T>(this List<T> l, Randomizer RNG) => l[RNG.RNG.Next(l.Count)];
	public static T Choice<T>(this T[] a, Randomizer RNG) => a[RNG.RNG.Next(a.Length)];
	public static T Choice<T>(this IEnumerable<T> e, Randomizer RNG) => e.ToArray<T>().Choice<T>(RNG);
	
	public static T ChoiceOrDefault<T>(this List<T> l, Randomizer RNG, T @default = default(T)) => (l.Count == 0)?@default:l.Choice<T>(RNG);
	public static T ChoiceOrDefault<T>(this T[] a, Randomizer RNG, T @default = default(T)) => (a.Length == 0)?@default:a.Choice<T>(RNG);
	public static T ChoiceOrDefault<T>(this IEnumerable<T> e, Randomizer RNG, T @default = default(T)) => e.ToArray<T>().ChoiceOrDefault(RNG, @default);
}
