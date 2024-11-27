using System;

public class ConTorch : BaseBuff
{
	public override string idSprite
	{
		get
		{
			return "ActTorch";
		}
	}

	public override void OnStart()
	{
		this.owner.RecalculateFOV();
	}

	public override void OnCalculateFov(Fov fov, ref int radius, ref float power)
	{
		if (radius < EClass.Colors.pcLights.torch.radius)
		{
			radius = EClass.Colors.pcLights.torch.radius;
		}
		float num = 0.01f * EClass.Colors.pcLights.torch.color.a * 256f;
		if (power < num)
		{
			power = num;
		}
	}

	public override void OnCreateFov(Fov fov)
	{
		fov.r += (byte)(EClass.Colors.pcLights.torch.color.r * 16f);
		fov.g += (byte)(EClass.Colors.pcLights.torch.color.g * 16f);
		fov.b += (byte)(EClass.Colors.pcLights.torch.color.b * 16f);
		fov.r += 4;
		fov.g += 3;
		fov.b += 2;
	}

	public override void OnRemoved()
	{
		this.owner.RecalculateFOV();
		Thing thing = EClass.player.currentHotItem.Thing;
		TraitToolTorch traitToolTorch = ((thing != null) ? thing.trait : null) as TraitToolTorch;
		if (traitToolTorch != null)
		{
			traitToolTorch.RefreshRenderer();
		}
	}
}
