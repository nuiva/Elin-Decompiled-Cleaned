using System;
using UnityEngine;
using UnityEngine.UI;

public class EffectText : Effect
{
	public static RectTransform transRoot;

	public Text textMain;

	public RectTransform rect;

	public RectTransform body;

	public bool addBrackets;

	[NonSerialized]
	public Vector3 pos;

	public EffectText Play(Vector3 _pos, string text, float _duration = 0f)
	{
		base.transform.SetParent(transRoot, worldPositionStays: false);
		pos = _pos + posFix + randomRange.Random();
		if (addBrackets)
		{
			text = "* " + text + " *";
		}
		textMain.text = text;
		LateUpdate();
		return this;
	}

	public EffectText SetColor(Color c)
	{
		textMain.color = c;
		return this;
	}

	public EffectText SetSize(float a)
	{
		textMain.fontSize = (int)(a * (float)textMain.fontSize);
		return this;
	}

	public void LateUpdate()
	{
		rect.localPosition = Util.WorldToUIPos(pos, rect.parent as RectTransform);
	}
}
