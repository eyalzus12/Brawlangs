using Godot;
using System;

public static class Utils
{
	public static Node Root(this Node n) => n.GetTree().Root;
	public static Node GetRootNode(this Node n, string s) => n.Root().GetNode(s);
	public static T GetRootNode<T>(this Node n, string s) where T : Node => (T)(n.GetRootNode(s));
	
	public static bool NotNull(this object o) => !(o is null);
	
	//public static object GetProp(this object o, string s, object @default = null) => o.GetType().GetProperty(s).GetValue(o, @default);
	//public static T GetProp<T>(this T o, string s, T @default = default) => o.GetType().GetProperty(s).GetValue(o, @default);
	
	public static void SetProp(this object o, string s, object v) => o.GetType().GetProperty(s).SetValue(o, v);
	public static void SetProp<T>(this object o, string s, T v) => o.GetType().GetProperty(s).SetValue(o, v);
	
	public static bool Flip(ref this bool b) => (b = !b);
	
	
	public static TResult Fork<T, T1, T2, TResult>(this T o, Func<T, T1> f1, Func<T, T2> f2, Func<T1, T2, TResult> f) => f(f1(o), f2(o));
	public static object Fork(this object o, Func<object, object> f1, Func<object, object> f2, Func<object, object, object> f) => o.Fork<object, object, object, object>(f1, f2, f);
	
	public static void ChangeScene(this Node n, string path) => n.GetTree().CallDeferred("change_scene", path);
	
	public static float Index(this Vector2 v, int i) => (i==0)?v.x:v.y;
	public static float Index(this Vector3 v, int i) => (i==0)?v.x:(i==1)?v.y:v.z;
	public static float Index(this Quat q, int i) => (i==0)?q.x:(i==1)?q.y:(i==2)?q.z:q.w;
}
