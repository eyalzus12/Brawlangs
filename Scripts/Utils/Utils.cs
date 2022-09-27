using Godot;
using System;

public static class Utils
{
	public static Node Root(this Node n) => n.GetTree().Root;
	public static Node GetRootNode(this Node n, string s) => n.Root().GetNode(s);
	public static T GetRootNode<T>(this Node n, string s) where T : Node => n.Root().GetNode<T>(s);
	
	public static void ChangeScene(this Node n, string path) => n.GetTree().ChangeScene(path);//n.GetTree().CallDeferred("change_scene", path);
	
	public static float Index(this Vector2 v, int i) => (i==0)?v.x:v.y;
	public static float Index(this Vector3 v, int i) => (i==0)?v.x:(i==1)?v.y:v.z;
	public static float Index(this Quat q, int i) => (i==0)?q.x:(i==1)?q.y:(i==2)?q.z:q.w;
	
	public static T GetProp<T>(this Godot.Object o, string s, T @default = default(T)) => (T)(o.Get(s)??@default);
}
