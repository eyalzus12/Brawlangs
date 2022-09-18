using Godot;
using System;

public partial class InputDisplayLabel : InfoLabel
{
	BufferInputManager bim;
	
	public override void Connect()
	{
		bim = new BufferInputManager(0);
	}
	
	public override void UpdateLabel()
	{
		bim.playerDeviceNumber = this.GetDataOrDefault("CurrentInfoLabelCharacter",0).i();
		
		Add("", bim, false);
	}
}
