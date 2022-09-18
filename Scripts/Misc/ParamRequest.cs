using Godot;
using System;

public record ParamRequest(Type ParamType, string ParamName, object ParamDefault);
/*
public struct ParamRequest
{
	public Type ParamType;
	public string ParamName;
	public object ParamDefault;
	
	public ParamRequest(Type type, string name, object @default)
	{
		ParamType = type;
		ParamName = name;
		ParamDefault = @default;
	}
}
*/
