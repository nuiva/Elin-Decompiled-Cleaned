using UnityEngine;

public class TraitLight : TraitTorch
{
	public override ToggleType ToggleType => ToggleType.Light;

	public override Color? ColorExtra
	{
		get
		{
			if (owner.c_lightColor == 0)
			{
				return base.ColorExtra;
			}
			return owner.LightColor;
		}
	}
}
