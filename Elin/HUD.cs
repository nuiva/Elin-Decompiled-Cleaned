using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HUD : EClass
{
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

	public void HideMouseInfo()
	{
		transRight.SetActive(enable: false);
	}

	public void SetDragImage(Image i, string text = null, UIText uiText = null)
	{
		if (i == null)
		{
			imageDrag.SetActive(enable: false);
			SetDragText();
			return;
		}
		imageDrag.sprite = i.sprite;
		imageDrag.SetNativeSize();
		imageDrag.color = i.color;
		RectTransform rectTransform = imageDrag.Rect();
		RectTransform rectTransform2 = i.Rect();
		rectTransform.sizeDelta = rectTransform2.sizeDelta;
		rectTransform.localScale = rectTransform2.localScale;
		rectTransform.pivot = rectTransform2.pivot;
		if ((bool)uiText)
		{
			RectTransform rectTransform3 = textDrag2.rectTransform;
			RectTransform rectTransform4 = uiText.rectTransform;
			rectTransform3.pivot = rectTransform4.pivot;
			rectTransform3.anchorMin = rectTransform4.anchorMin;
			rectTransform3.anchorMax = rectTransform4.anchorMax;
			textDrag2.size = uiText.size;
			textDrag2.fontStyle = uiText.fontStyle;
			imageDrag.transform.position = i.transform.position;
			rectTransform3.position = rectTransform4.position;
		}
		imageDrag.transform.position = EInput.mpos + (EClass.game.UseGrid ? imageDragFix2 : imageDragFix2);
		SetDragText(text);
		Util.ClampToScreen(imageDrag.Rect(), marginImageDrag);
		imageDrag.SetActive(enable: true);
	}

	public void SetDragText(string text = null, string text2 = null)
	{
		if (text != null && text != textDrag.text)
		{
			textDrag.text = text.lang();
		}
		textDrag.transform.parent.SetActive(!text.IsEmpty());
		if (text2 != null && text2 != textDrag2.text)
		{
			textDrag2.text = text2.lang();
		}
		textDrag2.SetActive(!text2.IsEmpty());
	}
}
