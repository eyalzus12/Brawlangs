using Godot;
using System;

public static class DataUtils
{
	public static object GetData(this Node n, string str) => n.GetPublicData()[str];
	public static object GetDataOrDefault(this Node n, string str, object @default) => n.GetPublicData().GetValueOrDefault(str,@default);
	public static void AddData(this Node n, string str, object val) => n.GetPublicData().Add(str, val);
	public static bool TryAddData(this Node n, string str, object val) => n.GetPublicData().TryAdd(str, val);
	public static void SetData(this Node n, string str, object obj) => n.GetPublicData()[str] = obj;
	public static bool DataHasKey(this Node n, string str) => n.GetPublicData().HasKey(str);
	public static bool DataHasValue(this Node n, object obj) => n.GetPublicData().HasValue(obj);
	public static bool RemoveData(this Node n, string str) => n.GetPublicData().Remove(str);
	public static void ClearData(this Node n) => n.GetPublicData().Clear();
	public static int DataCount(this Node n) => n.GetPublicData().Count;
	public static bool DataEmpty(this Node n) => n.GetPublicData().Empty;
	public static PublicData GetPublicData(this Node n) => n.GetRootNode<PublicData>("PublicData");
}
