using System;
using UnityEngine;

public class UIScaleFix : EMono
{
	private void Awake()
	{
		int scale = EMono.core.config.ui.scale;
		if (scale > 20 && scale <= 22)
		{
			Vector3 vector = new Vector3(1f, 1f / (1f * (float)scale * 0.05f), 1f);
			base.transform.localScale = vector;
			if (vector.y < 0.91f && base.transform.localPosition.y * 10f % 10f == 0f)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y + 0.1f, 1f);
				return;
			}
		}
		else
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}
}
