using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LiquidProfile : EScriptable
{
	public static LiquidProfile Get(string id)
	{
		return Resources.Load<LiquidProfile>("Scene/Profile/Liquid/" + id);
	}

	public static void Apply(string id)
	{
		LiquidProfile.Get(id).Apply(null);
	}

	public void Apply(Color? previewColor = null)
	{
		LiquidProfile.<>c__DisplayClass6_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.previewColor = previewColor;
		BaseTileMap tileMap = EClass.scene.screenElin.tileMap;
		this.<Apply>g__Set|6_0(tileMap.passLiquid.mat, ref CS$<>8__locals1);
		this.<Apply>g__Set|6_0(tileMap.passFloorWater.mat, ref CS$<>8__locals1);
		this.<Apply>g__Set|6_0(tileMap.passAutoTileWater.mat, ref CS$<>8__locals1);
	}

	public void ApplyColor()
	{
		this.modColor = EClass._map.config.colorLiquid.Get();
	}

	[CompilerGenerated]
	private void <Apply>g__Set|6_0(Material mat, ref LiquidProfile.<>c__DisplayClass6_0 A_2)
	{
		mat.SetFloat("_Transparency", this.transparency);
		string name = "_ModColor";
		Color? previewColor = A_2.previewColor;
		Color value;
		if (previewColor == null)
		{
			SerializableColor colorLiquid = EClass._map.config.colorLiquid;
			value = ((colorLiquid != null) ? colorLiquid.Get() : this.modColor);
		}
		else
		{
			value = previewColor.GetValueOrDefault();
		}
		mat.SetColor(name, value);
		mat.SetVector("_SurfaceAnime", this.surfaceAnime);
		mat.SetTexture("_SurfaceTex", this.surface);
	}

	public Color modColor;

	public float transparency;

	public Texture2D surface;

	public Vector4 surfaceAnime;
}
