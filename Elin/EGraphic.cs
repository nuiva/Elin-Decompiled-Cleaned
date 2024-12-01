using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(RectTransform))]
public class EGraphic : Graphic
{
	public Texture2D texture;

	public override Texture mainTexture => texture;
}
