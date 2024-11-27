using System;

public class TraitLightSource : TraitTorch
{
	public int LightRadius
	{
		get
		{
			return base.GetParam(1, null).ToInt();
		}
	}
}
