using Godot;
using System;

public static class Utils
{
	public static Node Root(this Node n) => n.GetTree().Root;
	public static Node GetRootNode(this Node n, string s) => n.Root().GetNode(s);
	public static T GetRootNode<T>(this Node n, string s) where T : Node => (T)(n.GetRootNode(s));
	
	public static void SetProp(this object o, string s, object v) => o.GetType().GetProperty(s).SetValue(o, v);
	public static void SetProp<T>(this object o, string s, T v) => o.GetType().GetProperty(s).SetValue(o, v);
	
	public static bool Flip(ref this bool b) => (b = !b);
	
	public static TResult Fork<T, T1, T2, TResult>(this T o, Func<T, T1> f1, Func<T, T2> f2, Func<T1, T2, TResult> f) => f(f1(o), f2(o));
	public static object Fork(this object o, Func<object, object> f1, Func<object, object> f2, Func<object, object, object> f) => o.Fork<object, object, object, object>(f1, f2, f);
	
	public static void ChangeSceneToFile(this Node n, string path) => n.GetTree().CallDeferred("change_scene_to_file", path);
	
	public static T GetProp<T>(this Godot.Object o, string s, T @default = default(T)) =>  (T)(o.Get(s).Obj??@default);
	
	public static void ToggleFullscreen() =>
	DisplayServer.WindowSetMode(
		DisplayServer.WindowGetMode() switch
		{
			DisplayServer.WindowMode.Windowed or
			DisplayServer.WindowMode.Maximized => DisplayServer.WindowMode.Fullscreen,
			DisplayServer.WindowMode.Fullscreen or
			DisplayServer.WindowMode.ExclusiveFullscreen => DisplayServer.WindowMode.Windowed,
			_ => DisplayServer.WindowGetMode(),
		}
	);
}
