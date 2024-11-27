using System;
using UnityEngine;
using UnityEngine.UI;

public class LayerImage : ELayer
{
	public void SetImage(Sprite sprite)
	{
		this.image.sprite = sprite;
		this.image.SetNativeSize();
		float num = ((float)Screen.height - this.margin.y) / ELayer.core.uiScale;
		RectTransform rectTransform = this.image.rectTransform;
		float num2 = rectTransform.sizeDelta.y;
		if (num2 > num)
		{
			num2 = num;
		}
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, num2);
		this.Zoom(0f);
	}

	public override void OnUpdateInput()
	{
		if (EInput.wheel != 0)
		{
			this.Zoom((EInput.wheel > 0) ? 0.25f : -0.25f);
		}
		if (EInput.middleMouse.clicked)
		{
			this.image.SetNativeSize();
			this.zoom = 1f;
			this.Zoom(0f);
		}
	}

	public void Zoom(float a)
	{
		this.zoom += a;
		if (this.zoom < 0.5f)
		{
			this.zoom = 0.5f;
		}
		if (this.zoom > 2f)
		{
			this.zoom = 2f;
		}
		this.image.rectTransform.localScale = new Vector3(this.zoom, this.zoom, this.zoom);
	}

	public override void OnKill()
	{
		EInput.Consume(2);
	}

	public Image image;

	public Vector2 margin;

	private float zoom = 1f;
}
