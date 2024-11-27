using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RefractionProfile : EScriptable
{
	public static RefractionProfile Get(string id)
	{
		return Resources.Load<RefractionProfile>("Scene/Profile/Refraction/" + id);
	}

	public static void Apply(string id)
	{
		RefractionProfile.Get(id).Apply();
	}

	public void Apply()
	{
		BaseTileMap tileMap = EClass.scene.screenElin.tileMap;
		this.<Apply>g__Set|6_0(tileMap.passFloorWater.mat);
		this.<Apply>g__Set|6_0(tileMap.passLiquid.mat);
		this.<Apply>g__Set|6_0(tileMap.passAutoTileWater.mat);
	}

	[CompilerGenerated]
	private void <Apply>g__Set|6_0(Material mat)
	{
		mat.SetVector("_GrabAnime", this.grabAnime);
		mat.SetVector("_GrabPos", this.grabPos);
		mat.SetTexture("_BumpTex", this.tex);
	}

	public Texture2D tex;

	public float transparency;

	public Vector4 grabAnime;

	public Vector4 grabPos;
}
