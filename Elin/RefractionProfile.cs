using UnityEngine;

public class RefractionProfile : EScriptable
{
	public Texture2D tex;

	public float transparency;

	public Vector4 grabAnime;

	public Vector4 grabPos;

	public static RefractionProfile Get(string id)
	{
		return Resources.Load<RefractionProfile>("Scene/Profile/Refraction/" + id);
	}

	public static void Apply(string id)
	{
		Get(id).Apply();
	}

	public void Apply()
	{
		BaseTileMap tileMap = EClass.scene.screenElin.tileMap;
		Set(tileMap.passFloorWater.mat);
		Set(tileMap.passLiquid.mat);
		Set(tileMap.passAutoTileWater.mat);
		void Set(Material mat)
		{
			mat.SetVector("_GrabAnime", grabAnime);
			mat.SetVector("_GrabPos", grabPos);
			mat.SetTexture("_BumpTex", tex);
		}
	}
}
