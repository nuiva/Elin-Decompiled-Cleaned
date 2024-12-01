using UnityEngine;
using UnityEngine.UI;

public class UIGameBG : EMono
{
	public static UIGameBG Instance;

	public bool indoor;

	private Image image;

	public static bool IsActive
	{
		get
		{
			if ((bool)Instance)
			{
				return Instance.image.color.a >= 1f;
			}
			return false;
		}
	}

	private void Awake()
	{
		image = GetComponent<Image>();
		InvokeRepeating("Refresh", 0.5f, 0.5f);
		Refresh();
		Instance = this;
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
		if (!indoor)
		{
			image.color = color2;
		}
	}
}
