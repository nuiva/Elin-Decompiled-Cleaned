using System;
using UnityEngine;
using UnityEngine.UI;

public class EffectText : Effect
{
	public EffectText Play(Vector3 _pos, string text, float _duration = 0f)
	{
		base.transform.SetParent(EffectText.transRoot, false);
		this.pos = _pos + this.posFix + this.randomRange.Random();
		if (this.addBrackets)
		{
			text = "* " + text + " *";
		}
		this.textMain.text = text;
		this.LateUpdate();
		return this;
	}

	public EffectText SetColor(Color c)
	{
		this.textMain.color = c;
		return this;
	}

	public EffectText SetSize(float a)
	{
		this.textMain.fontSize = (int)(a * (float)this.textMain.fontSize);
		return this;
	}

	public void LateUpdate()
	{
		this.rect.localPosition = Util.WorldToUIPos(this.pos, this.rect.parent as RectTransform);
	}

	public static RectTransform transRoot;

	public Text textMain;

	public RectTransform rect;

	public RectTransform body;

	public bool addBrackets;

	[NonSerialized]
	public Vector3 pos;
}
