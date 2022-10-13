using Godot;
using System;
using Linearstar.Windows.RawInput;
using RawInputDeviceHandle = Linearstar.Windows.RawInput.Native.RawInputDeviceHandle;

public struct RawInputKeyEvent
{
	public int Key{get; set;}
	public bool Pressed{get; set;}
	public RawInputDeviceHandle Handle{get; set;}
	
	public RawInputKeyEvent(int key, bool pressed, RawInputDeviceHandle handle)
	{
		Key = key;
		Pressed = pressed;
		Handle = handle;
	}
}
