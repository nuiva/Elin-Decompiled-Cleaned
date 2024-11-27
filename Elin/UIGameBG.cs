using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGameBG : EMono
{
	public static bool IsActive
	{
		get
		{
			return UIGameBG.Instance && UIGameBG.Instance.image.color.a >= 1f;
		}
	}

	private void Awake()
	{
		this.image = base.GetComponent<Image>();
		base.InvokeRepeating("Refresh", 0.5f, 0.5f);
		this.Refresh();
		UIGameBG.Instance = this;
	}

	public void Refresh()
	{
		float timeRatio = EMono.scene.timeRatio;
		SceneColorProfile color = EMono.scene.profile.color;
		color.sun.Evaluate(timeRatio);
		Color color2 = color.sky.Evaluate(timeRatio);
		color.skyBG.Evaluate(timeRatio);
		if (timeRatio < 0.3f)
		{
			Mathf.Min((0.3f - timeRatio) * 10f, 1f);
		}
		if (!this.indoor)
		{
			this.image.color = color2;
		}
	}

	public static UIGameBG Instance;

	public bool indoor;

	private Image image;
}
