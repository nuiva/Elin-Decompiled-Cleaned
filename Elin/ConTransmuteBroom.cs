using System;

public class ConTransmuteBroom : ConTransmute
{
	public override RendererReplacer GetRendererReplacer()
	{
		return RendererReplacer.CreateFrom("broom_chara", 0);
	}
}
