using System;

public class DramaEventExit : DramaEvent
{
	public override bool Play()
	{
		this.sequence.Exit();
		return false;
	}
}
