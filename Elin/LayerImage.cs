using UnityEngine;
using UnityEngine.UI;

public class LayerImage : ELayer
{
	public Image image;

	public Vector2 margin;

	private float zoom = 1f;

	public void SetImage(Sprite sprite)
	{
		image.sprite = sprite;
		image.SetNativeSize();
		float num = ((float)Screen.height - margin.y) / ELayer.core.uiScale;
		RectTransform rectTransform = image.rectTransform;
		float num2 = rectTransform.sizeDelta.y;
		if (num2 > num)
		{
			num2 = num;
		}
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, num2);
		Zoom(0f);
	}

	public override void OnUpdateInput()
	{
		if (EInput.wheel != 0)
		{
			Zoom((EInput.wheel > 0) ? 0.25f : (-0.25f));
		}
		if (EInput.middleMouse.clicked)
		{
			image.SetNativeSize();
			zoom = 1f;
			Zoom(0f);
		}
	}

	public void Zoom(float a)
	{
		zoom += a;
		if (zoom < 0.5f)
		{
			zoom = 0.5f;
		}
		if (zoom > 2f)
		{
			zoom = 2f;
		}
		image.rectTransform.localScale = new Vector3(zoom, zoom, zoom);
	}

	public override void OnKill()
	{
		EInput.Consume(2);
	}
}
