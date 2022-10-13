using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Linearstar.Windows.RawInput;
using System.Threading.Tasks;
using RawInputDeviceHandle = Linearstar.Windows.RawInput.Native.RawInputDeviceHandle;

public class RawInputMap : Node
{
	public RawInputMap() {}
	
	#if MULTI_KEYBOARD
	public Dictionary<RawInputDeviceHandle, int> KeyboardDevices{get; set;} = new Dictionary<RawInputDeviceHandle, int>();
	public Dictionary<int, uint> KeycodeDict{get; set;} = new Dictionary<int, uint>();
	public HashSet<(int,uint)> HeldEvents{get; set;} = new HashSet<(int,uint)>();
	
	public RawInputReceiverForm InputForm{get; set;}
	
	public bool DebugInput{get; set;} = false;
	
	public override void _Ready()
	{
		ReadKeyMap();
		
		var keyboards = RawInputDevice.GetDevices().OfType<RawInputKeyboard>();
		
		int i = 0;
		foreach(var device in keyboards)
		{
			KeyboardDevices.Add(device.Handle, i);
			++i;
		}
		
		InputForm = new RawInputReceiverForm();
		InputForm.KeycodePress += (sender, t) => KeycodePress(t);
		
		RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard, RawInputDeviceFlags.ExInputSink | RawInputDeviceFlags.NoLegacy, InputForm.Window.Handle);
	
		new Task(RunForm).Start();
	}
	
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_input_debug")) DebugInput = !DebugInput;
		
		if(Input.IsActionJustPressed("list_devices"))
		{
			int i = 0;
			foreach(var device in RawInputDevice.GetDevices())
			{
				if(device is RawInputKeyboard)
				{
					GD.PrintT(i.ToString(), device.DeviceType.ToString(), device.Handle.ToString(), $"{device.VendorId:X4}:{device.ProductId:X4}", device.ProductName, device.ManufacturerName);
					++i;
				}
				else
					GD.PrintT("", device.DeviceType.ToString(), "", device.Handle.ToString(), $"{device.VendorId:X4}:{device.ProductId:X4}", device.ProductName, device.ManufacturerName);
			}
		}
	}
	
	public void RunForm() => Application.Run(InputForm);
	public void CloseForm() => InputForm.Close();
	
	public override void _ExitTree()
	{
		RawInputDevice.UnregisterDevice(HidUsageAndPage.Keyboard);
		CloseForm();
	}
	
	public void KeycodePress(RawInputKeyEvent rawInput)
	{
		var device = KeyboardDevices.GetValueOrDefault(rawInput.Handle, 0);
		
		if(!KeycodeDict.ContainsKey(rawInput.Key))
		{
			if(DebugInput) GD.PrintT("Unknwon raw input", $"Keycode: {rawInput.Key}", $"Pressed: {rawInput.Pressed}", $"Device: {device}");
			return;
		}
		
		var scancode = KeycodeDict[rawInput.Key];
		
		var data = (device, scancode);
		if(rawInput.Pressed && HeldEvents.Contains(data)) return;//holding
		
		HeldEvents.Remove(data);
		
		var input = new InputEventKey();
		
		input.Device = device;
		input.Scancode = scancode;
		input.Pressed = rawInput.Pressed;
		
		if(input.Pressed) HeldEvents.Add(data);
		
		if(DebugInput) GD.PrintT($"Raw Input: {input.AsText()}", $"Keycode: {rawInput.Key}", $"Mapped to: {input.Scancode}", $"Pressed: {rawInput.Pressed}", $"Device: {input.Device}");
		
		Input.ParseInputEvent(input);
	}
	
	private const string KEY_MAP_PATH = "res://Scripts/Inputs/RawInput/KeyToScancode.keys";
	private void ReadKeyMap()
	{
		File f = new File();//create new file
		var er = f.Open(KEY_MAP_PATH, File.ModeFlags.Read);//open file
		if(er != Error.Ok) return;//if error, return
		string content = f.GetAsText();//read text
		f.Close();//flush buffer
		ParseKeyMap(content);//parse
	}
	
	private void ParseKeyMap(string str)
	{
		str
			.Split("\n")
			.Select(s => s.Substring(0,s.IndexOf(';')).Trim())
			.Select(s => s.Split(" "))
			.ForEach(t => KeycodeDict.Add(int.Parse(t[0].Trim()), uint.Parse(t[1].Trim())));
	}
	#endif
}
