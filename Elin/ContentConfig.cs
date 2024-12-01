using System;
using UnityEngine.UI;

public class ContentConfig : EContent
{
	public CoreConfig config => Core.Instance.config;

	public void SetSlider(Slider slider, float value, Func<float, string> action)
	{
		slider.onValueChanged.RemoveAllListeners();
		slider.onValueChanged.AddListener(delegate(float a)
		{
			slider.GetComponentInChildren<UIText>(includeInactive: true).text = action(a);
		});
		slider.value = value;
		slider.GetComponentInChildren<UIText>(includeInactive: true).text = action(value);
	}
}
