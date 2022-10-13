using Godot;
using System;
using System.Collections.Generic;

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
	
	public static Action<T> Chain<T>(this Action<T> a1, Action<T> a2) => (T t) => {a1(t); a2(t);};
	
	public static Action<T> Chain<T>(this IEnumerable<Action<T>> ae) => (T t) => {foreach(var a in ae) a(t);};
	
	public static Action<T> Chain<T>(this Action<T> a1, params Action<T>[] ae) => (T t) => {a1(t); ae.Chain<T>()(t);};
	public static Action<T> Chain<T>(this Action<T> a1, IEnumerable<Action<T>> ae) => (T t) => {a1(t); ae.Chain<T>()(t);};
	
	public static bool IsActionIgnoreDevice(this InputEvent e, string action)
	{
		var temp = e.Device;
		e.Device = -1;
		var result = e.IsAction(action);
		e.Device = temp;
		return result;
	}
	
	public static Error ReadFile(string path, out string content)
	{
		var f = new File();//create new file
		var er = f.Open(path, File.ModeFlags.Read);//open file
		
		if(er != Error.Ok)//if error, return
		{
			content = "";
			return er;
		}
		
		content = f.GetAsText();//read text
		f.Close();//flush buffer
		return Error.Ok;
	}
	
	public static Error SaveFile(string path, string content)
	{
		var f = new File();
		var er = f.Open(path, File.ModeFlags.Write);
		
		if(er != Error.Ok) return er;
		
		f.StoreString(content);
		f.Close();
		return Error.Ok;
	}
}
