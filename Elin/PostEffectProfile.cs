using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class PostEffectProfile : EScriptable
{
	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.Apply(EClass.scene.cam);
		}
	}

	public void OnChangeProfile()
	{
		CoreConfig.GraphicSetting graphic = EClass.core.config.graphic;
		graphic.sharpen = this.sharpen;
		graphic.sharpen2 = this.sharpen2;
		graphic.blur = this.blur;
		graphic.kuwahara = this.kuwahara;
	}

	public void Apply(Camera cam)
	{
		Antialiasing component = cam.transform.GetComponent<Antialiasing>();
		if (component)
		{
			component.enabled = this.enableAA;
			component.mode = this.aaMode;
			component.offsetScale = this.offsetScale;
			component.blurRadius = this.blurRadius;
		}
		if (this.enableCharaAA)
		{
			EClass.scene.screenElin.tileMap.passChara.mat.EnableKeyword("AA_ON");
			EClass.scene.screenElin.tileMap.passCharaL.mat.EnableKeyword("AA_ON");
			return;
		}
		EClass.scene.screenElin.tileMap.passChara.mat.DisableKeyword("AA_ON");
		EClass.scene.screenElin.tileMap.passCharaL.mat.DisableKeyword("AA_ON");
	}

	public float Brightness;

	public float Saturation;

	public float Contrast;

	public int sharpen = 20;

	public int sharpen2 = 80;

	public int blur;

	public bool enableAA;

	public bool enableCharaAA;

	public bool disable;

	public bool kuwahara;

	public AAMode aaMode;

	public float offsetScale;

	public float blurRadius;
}
