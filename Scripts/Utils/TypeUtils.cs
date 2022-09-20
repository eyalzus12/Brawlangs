using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class TypeUtils
{
	public static float f(this object o) => (o is Variant v)?v.AsSingle():Convert.ToSingle(o);
	public static bool b(this object o) => (o is Variant v)?v.AsBool():Convert.ToBoolean(o);
	public static int i(this object o) => (o is Variant v)?v.AsInt32():Convert.ToInt32(o);
	public static string s(this object o) => (o is Variant v)?v.AsString():(o is StringName sn)?sn.ToString():((string)o);
	public static object ob(this object o) => (o is Variant v)?v.Obj:o;
	public static Variant ToVariant(this object o) => Variant.CreateFrom((dynamic)o);
	
	
	public static Vector2 v2(this object o)//aslkfjhakusdfghausjkfhsakuh
	{
		if(o is Variant v) return v.AsVector2();
		
		Vector2 res;
		if(StringUtils.TryParseVector2(o.ToString(), out res)) return res;
		else throw new FormatException($"Bad Vector2 format {o}");
	}
	
	public static Vector3 v3(this object o)
	{
		if(o is Variant v) return v.AsVector3();
		
		Vector3 res;
		if(StringUtils.TryParseVector3(o.ToString(), out res)) return res;
		else throw new FormatException($"Bad Vector3 format {o}");
	}
	
	public static Vector4 v4(this object o)
	{
		if(o is Variant v) return v.AsVector4();
		
		Vector4 res;
		if(StringUtils.TryParseVector4(o.ToString(), out res)) return res;
		else throw new FormatException($"Bad Vector4 format {o}");
	}
	
	public static List<object> lo(this object o) => o.lt<object>(ob);//(o as IEnumerable<object>).ToList<object>();
	public static List<string> ls(this object o) => o.lt<string>(s);
	public static List<float> lf(this object o) => o.lt<float>(f);
	public static List<int> li(this object o) => o.lt<int>(i);
	public static List<bool> lb(this object o) => o.lt<bool>(b);
	public static List<Vector2> lv2(this object o) => o.lt<Vector2>(v2);
	public static List<Vector3> lv3(this object o) => o.lt<Vector3>(v3);
	public static List<Vector4> lv4(this object o) => o.lt<Vector4>(v4);
		
	public static List<T> lt<T>(this object o, Func<object, T> caster)
	{
		//if(o is Variant v) return v.AsArray<T>().ToList<T>();
		
		if(o is IEnumerable<T> et) return et.ToList();
		else if(o is IEnumerable<object> eo) return eo.Select(caster).ToList<T>();
		else if(o is T t) return new List<T>{t};
		else return null;
	}
	
	public static object cast(this object o, Type t, string debug = "")
	{
		if(t == typeof(object)) return o.ob();
		else if(t == typeof(string)) return o.s();
		else if(t == typeof(float)) return o.f();
		else if(t == typeof(int)) return o.i();
		else if(t == typeof(bool)) return o.b();
		else if(t == typeof(Vector2)) return o.v2();
		else if(t == typeof(Vector3)) return o.v3();
		else if(t == typeof(Vector4)) return o.v4();
		
		else if(t == typeof(List<object>)) return o.lo();
		else if(t == typeof(List<string>)) return o.ls();
		else if(t == typeof(List<float>)) return o.lf();
		else if(t == typeof(List<int>)) return o.li();
		else if(t == typeof(List<bool>)) return o.lb();
		else if(t == typeof(List<Vector2>)) return o.lv2();
		else if(t == typeof(List<Vector3>)) return o.lv3();
		else if(t == typeof(List<Vector4>)) return o.lv4();
		
		var ToPrint = (debug!="")?$"\nDebug message: {debug}":"";
		GD.PushError($"Type {t.Name} isn't available in the cast method" + ToPrint);
		return null;
		
		/*var t = typeof(T);
		var name = t.Name;
		switch(name)
		{
			case typeof(object).Name: return o;
			case typeof(string).Name: return o.s();
			case typeof(float).Name: return o.f();
			case typeof(int).Name: return o.i();
			case typeof(Vector2).Name: return o.v2();
			case typeof(Vector3).Name: return o.v3();
			case typeof(bool).Name: return o.b();
			
			case typeof(List<object>).Name: return o.lo();
			case typeof(List<string>).Name: return o.ls();
			case typeof(List<float>).Name: return o.lf();
			case typeof(List<int>).Name: return o.li();
			case typeof(List<Vector2>).Name: return o.lv2();
			case typeof(List<Vector3>).Name: return o.lv3();
			case typeof(List<bool>).Name: return o.lb();
			default:
				GD.Print($"ERROR: Type {name} isn't available in the cast method" + (debug=="")?"":$"\nDebug message: {debug}");
				return null;
		}*/
	}
	
	public static object cast<T>(this object o, string debug) => o.cast(typeof(T), debug);
	
	public const string PATH_IDENTIFIER = "mod://";
	public static T LoadScript<T>(string path, T @default, string relative="res://Scripts") where T : Node, new()
	{
		if(path == "") return @default;
		var typeName = typeof(T).Name;
		if(path.StartsWith(PATH_IDENTIFIER)) path = relative + "/" + path.Substring(PATH_IDENTIFIER.Length);
		
		var resource = ResourceLoader.Load(path);
		if(resource is object)
		{
			if(resource is CSharpScript script)
			{
				/*if(!path.StartsWith("res://"))
				{
					var filename = path.SplitByLast('/')[1];
					var newthing = "res://Scripts/"+filename;
					GD.Print(newthing);
					
					var f = new File();
					f.Open(newthing, File.ModeFlags.Write);
					f.Close();
					GD.Print(new Directory().Copy(path, newthing));
				}*/
				
				/*var h = new T();
				return h.SafelySetScript<T>(script);*/
				
				var o = script.New().Obj;
				if(o is not null)
				{
					if(o is T t)
					{
						return t;
					}
					else GD.PushError($"Attempt to load script {path} failed because the object in that path is not {typeName.AAN()} script");
				}
				else GD.PushError($"Attempt to construct script {path} failed because the constructor returned null for some reason");
			}
			else GD.PushError($"Attempt to load script {path} failed because the object in that path is not a C# script");
		}
		else GD.PushError($"Attempt to load script {path} failed because that file does not exist");
		
		return @default;
	}
	
	public static T SafelySetScript<T>(this Godot.Object obj, Resource resource) where T : Godot.Object
	{
		var godotObjectId = obj.GetInstanceId();
		// Replaces old C# instance with a new one. Old C# instance is disposed.
		obj.SetScript(resource);
		// Get the new C# instance
		return GD.InstanceFromId(godotObjectId) as T;
	}
}
