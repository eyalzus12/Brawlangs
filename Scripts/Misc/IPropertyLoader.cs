using Godot;
using System;
using System.Collections.Generic;

public interface IPropertyLoader
{
	Dictionary<string, ParamRequest> LoadExtraProperties{get; set;}
	void LoadExtraProperty<T>(string s, T @default = default(T));
	void LoadProperties();
}
