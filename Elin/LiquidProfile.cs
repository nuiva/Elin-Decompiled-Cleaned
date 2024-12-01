using UnityEngine;

public class LiquidProfile : EScriptable
{
	public Color modColor;

	public float transparency;

	public Texture2D surface;

	public Vector4 surfaceAnime;

	public static LiquidProfile Get(string id)
	{
		return Resources.Load<LiquidProfile>("Scene/Profile/Liquid/" + id);
	}

	public static void Apply(string id)
	{
		Get(id).Apply();
	}

	public void Apply(Color? previewColor = null)
	{
		BaseTileMap tileMap = EClass.scene.screenElin.tileMap;
		Set(tileMap.passLiquid.mat);
		Set(tileMap.passFloorWater.mat);
		Set(tileMap.passAutoTileWater.mat);
		void Set(Material mat)
		{
			mat.SetFloat("_Transparency", transparency);
			mat.SetColor("_ModColor", previewColor ?? EClass._map.config.colorLiquid?.Get() ?? modColor);
			mat.SetVector("_SurfaceAnime", surfaceAnime);
			mat.SetTexture("_SurfaceTex", surface);
		}
	}

	public void ApplyColor()
	{
		modColor = EClass._map.config.colorLiquid.Get();
	}
}
