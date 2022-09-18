using Godot;
using System;

public partial class BufferInfo
{
	//bufferTimeLeft > 0: the input is currently buffered
	//bufferTimeLeft = 0: the input has just stopped being buffered
	//bufferTimeLeft < 0: the input isnt currently buffered

	public int bufferTimeLeft = 0;
	public int defaultBufferTime = 1;
	public bool markedForDeletion = false;

	public BufferInfo() {}

	public BufferInfo(int buffer) {defaultBufferTime = buffer;}

	public void Activate(int time = -1)
	{
		bufferTimeLeft = (time == -1)?defaultBufferTime:time;
		bufferTimeLeft++;
	}

	public void Advance()
	{
		if(bufferTimeLeft > -1) --bufferTimeLeft;
	}

	public void Disable() => bufferTimeLeft = -1;

	public void Delete()
	{
		Disable();
		markedForDeletion = false;
	}

	public bool IsActive() => bufferTimeLeft > 0;
	public override string ToString() => $"{bufferTimeLeft}, {defaultBufferTime}, {markedForDeletion}";
}
