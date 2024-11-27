using System;
using UnityEngine;

public class TraitLight : TraitTorch
{
	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Light;
		}
	}

	public override Color? ColorExtra
	{
		get
		{
			if (this.owner.c_lightColor == 0)
			{
				return base.ColorExtra;
			}
			return new Color?(this.owner.LightColor);
		}
	}
}
