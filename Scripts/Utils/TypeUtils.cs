using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class TypeUtils
{
	public static float f(this object o) => Convert.ToSingle(o);
	public static bool b(this object o) => Convert.ToBoolean(o);
	public static int i(this object o) => Convert.ToInt32(o);
	public static string s(this object o) => (string)o;
	public static object ob(this object o) => o;
	
	public static Vector2 v2(this object o)//aslkfjhakusdfghausjkfhsakuh
	{
		var str = o.ToString();
		str = str.Substring(1, str.Length-2);//turns to string and cuts ()
		var split = str.Split(',');//split by the space
		var x = float.Parse(split[0].Trim());//parse x
		var y = float.Parse(split[1].Trim());//parse y
		return new Vector2(x, y);//uses the parts
	}
	
	public static Vector3 v3(this object o)
	{
		var str = o.ToString();
		str = str.Substring(1, str.Length-2);//turns to string and cuts ()
		var split = str.Split(',');//split by the space
		var x = float.Parse(split[0].Trim());//parse x
		var y = float.Parse(split[1].Trim());//parse y
		var z = float.Parse(split[2].Trim());//parse z
		return new Vector3(x, y, z);//uses the parts
	}
	
	public static Quat q(this object o)
	{
		var str = o.ToString();
		str = str.Substring(1, str.Length-2);//turns to string and cuts ()
		var split = str.Split(',');//split by the space
		var x = float.Parse(split[0].Trim());//parse x
		var y = float.Parse(split[1].Trim());//parse y
		var z = float.Parse(split[2].Trim());//parse z
		var w = float.Parse(split[3].Trim());//parse w
		return new Quat(x, y, z, w);//uses the parts
	}
	
	public static List<object> lo(this object o) => o.lt<object>(ob);//(o as IEnumerable<object>).ToList<object>();
	public static List<string> ls(this object o) => o.lt<string>(s);
	public static List<float> lf(this object o) => o.lt<float>(f);
	public static List<int> li(this object o) => o.lt<int>(i);
	public static List<bool> lb(this object o) => o.lt<bool>(b);
	public static List<Vector2> lv2(this object o) => o.lt<Vector2>(v2);
	public static List<Vector3> lv3(this object o) => o.lt<Vector3>(v3);
	public static List<Quat> lq(this object o) => o.lt<Quat>(q);
		
	public static List<T> lt<T>(this object o, Func<object, T> caster) =>
		(o as IEnumerable<object>).Select(caster).ToList<T>();
	
	public static object cast(this object o, Type t, string debug = "")
	{
		if(t == typeof(object)) return o;
		else if(t == typeof(string)) return o.s();
		else if(t == typeof(float)) return o.f();
		else if(t == typeof(int)) return o.i();
		else if(t == typeof(bool)) return o.b();
		else if(t == typeof(Vector2)) return o.v2();
		else if(t == typeof(Vector3)) return o.v3();
		else if(t == typeof(Quat)) return o.q();
		
		else if(t == typeof(List<object>)) return o.lo();
		else if(t == typeof(List<string>)) return o.ls();
		else if(t == typeof(List<float>)) return o.lf();
		else if(t == typeof(List<int>)) return o.li();
		else if(t == typeof(List<bool>)) return o.lb();
		else if(t == typeof(List<Vector2>)) return o.lv2();
		else if(t == typeof(List<Vector3>)) return o.lv3();
		else if(t == typeof(List<Quat>)) return o.lq();
		
		var ToPrint = (debug!="")?$"\nDebug message: {debug}":"";
		GD.Print($"ERROR: Type {t.Name} isn't available in the cast method" + ToPrint);
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
}
