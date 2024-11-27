using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(RectTransform))]
public class EGraphic : Graphic
{
	public override Texture mainTexture
	{
		get
		{
			return this.texture;
		}
	}

	public Texture2D texture;
}
