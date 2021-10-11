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
	
	public const string NUMBER_REGEX = @"-?\d+(?:\.\d+)?";
	
	public static Vector2 v2(this object o)//aslkfjhakusdfghausjkfhsakuh
	{
		string str = o.ToString();
		str = str.Substring(1, str.Length-2);//turns to string and cuts ()
		var split = str.Split(' ');//split by the space
		var x = float.Parse(split[0]);//parse x
		var y = float.Parse(split[1]);//parse y
		return new Vector2(x, y);//uses the parts
	}

	public static string V2_REGEX = $@"^\({NUMBER_REGEX},? {NUMBER_REGEX}\)$";
	public static bool is_v2(this object o) => o.RegexMatch(V2_REGEX);
	//detects if str matches a Vector2 ToString output
	
	public static Vector3 v3(this object o)
	{
		string str = o.ToString();
		str = str.Substring(1, str.Length-2);//turns to string and cuts ()
		var split = str.Split(' ');//split by the space
		var x = float.Parse(split[0]);//parse x
		var y = float.Parse(split[1]);//parse y
		var z = float.Parse(split[2]);//parse z
		return new Vector3(x, y, z);//uses the parts
	}
	
	public static string V3_REGEX = $@"^\({NUMBER_REGEX},? {NUMBER_REGEX},? {NUMBER_REGEX}\)$";
	public static bool is_v3(this object o) => o.RegexMatch(V3_REGEX);
	//detects if str matches a Vector3 ToString output
	
	public static Quat q(this object o)
	{
		string str = o.ToString();
		str = str.Substring(1, str.Length-2);//turns to string and cuts ()
		var split = str.Split(' ');//split by the space
		var x = float.Parse(split[0]);//parse x
		var y = float.Parse(split[1]);//parse y
		var z = float.Parse(split[2]);//parse z
		var w = float.Parse(split[3]);//parse w
		return new Quat(x, y, z, w);//uses the parts
	}
	
	public static string Q_REGEX = $@"^\({NUMBER_REGEX},? {NUMBER_REGEX},? {NUMBER_REGEX},? {NUMBER_REGEX}\)$";
	public static bool is_q(this object o) => o.RegexMatch(Q_REGEX);
	//detects if str matches a Quat ToString output
	
	public static List<object> lo(this object o) => (o as IEnumerable<object>).ToList<object>();
	public static List<string> ls(this object o) => o.lt<string>(h => h.s());
	public static List<float> lf(this object o) => o.lt<float>(h => h.f());
	public static List<int> li(this object o) => o.lt<int>(h => h.i());
	public static List<bool> lb(this object o) => o.lt<bool>(h => h.b());
	public static List<Vector2> lv2(this object o) => o.lt<Vector2>(h => h.v2());
	public static List<Vector3> lv3(this object o) => o.lt<Vector3>(h => h.v3());
	public static List<Quat> lq(this object o) => o.lt<Quat>(h => h.q());
		
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
