using System;

public class ConTransmutePutit : ConTransmute
{
	public override RendererReplacer GetRendererReplacer()
	{
		return RendererReplacer.CreateFrom("putty_snow", 0);
	}
}
