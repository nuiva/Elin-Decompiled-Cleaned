using System;

public class DramaEventWait : DramaEventMethod
{
	public DramaEventWait(float _duration, Action method = null) : base(method, _duration, true)
	{
		this.hideDialog = true;
	}
}
