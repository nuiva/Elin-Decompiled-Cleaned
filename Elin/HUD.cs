using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HUD : EClass
{
	public void HideMouseInfo()
	{
		this.transRight.SetActive(false);
	}

	public void SetDragImage(Image i, string text = null, UIText uiText = null)
	{
		if (i == null)
		{
			this.imageDrag.SetActive(false);
			this.SetDragText(null, null);
			return;
		}
		this.imageDrag.sprite = i.sprite;
		this.imageDrag.SetNativeSize();
		this.imageDrag.color = i.color;
		RectTransform rectTransform = this.imageDrag.Rect();
		RectTransform rectTransform2 = i.Rect();
		rectTransform.sizeDelta = rectTransform2.sizeDelta;
		rectTransform.localScale = rectTransform2.localScale;
		rectTransform.pivot = rectTransform2.pivot;
		if (uiText)
		{
			RectTransform rectTransform3 = this.textDrag2.rectTransform;
			RectTransform rectTransform4 = uiText.rectTransform;
			rectTransform3.pivot = rectTransform4.pivot;
			rectTransform3.anchorMin = rectTransform4.anchorMin;
			rectTransform3.anchorMax = rectTransform4.anchorMax;
			this.textDrag2.size = uiText.size;
			this.textDrag2.fontStyle = uiText.fontStyle;
			this.imageDrag.transform.position = i.transform.position;
			rectTransform3.position = rectTransform4.position;
		}
		this.imageDrag.transform.position = EInput.mpos + (EClass.game.UseGrid ? this.imageDragFix2 : this.imageDragFix2);
		this.SetDragText(text, null);
		Util.ClampToScreen(this.imageDrag.Rect(), this.marginImageDrag);
		this.imageDrag.SetActive(true);
	}

	public void SetDragText(string text = null, string text2 = null)
	{
		if (text != null && text != this.textDrag.text)
		{
			this.textDrag.text = text.lang();
		}
		this.textDrag.transform.parent.SetActive(!text.IsEmpty());
		if (text2 != null && text2 != this.textDrag2.text)
		{
			this.textDrag2.text = text2.lang();
		}
		this.textDrag2.SetActive(!text2.IsEmpty());
	}

	public ActionHint hint;

	public Transform frame;

	public UIHangIcon hangCorner;

	public Image imageDrag;

	public Image imageCover;

	public CanvasGroup cgDrag;

	public UIText textDrag;

	public UIText textDrag2;

	public UIText textMouseHintRight;

	public UIText textMouseHintLeft;

	public Vector3 imageDragFix;

	public Vector3 imageDragFix2;

	public Vector3 textMouseHintFix;

	public Vector3 textMouseHintFixLeft;

	public int marginImageDrag;

	public Image imageRight;

	public UIText textRight;

	public UIText textLeft;

	public UIText textMiddle;

	public UIText textWheel;

	public Transform transRight;

	public Vector2 rightTextPos;

	public Vector2 leftTextPos;

	public Vector2 wheelTextPos;

	public Vector2 wheelTextPos2;

	public Vector3 transRightPos;
}
