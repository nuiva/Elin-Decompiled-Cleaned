using System;
using UnityEngine;
using UnityEngine.UI;

public class WidgetSideScreen : Widget
{
	public override object CreateExtra()
	{
		return new WidgetSideScreen.Extra();
	}

	public override bool AlwaysBottom
	{
		get
		{
			return true;
		}
	}

	public WidgetSideScreen.Extra extra
	{
		get
		{
			return base.config.extra as WidgetSideScreen.Extra;
		}
	}

	public Color bgColor
	{
		get
		{
			return IntColor.FromInt(this.extra.bgColor);
		}
		set
		{
			this.extra.bgColor = IntColor.ToInt(ref value);
		}
	}

	public override void OnActivate()
	{
		Layer.blurStopInstance = base.transform;
		this.Refresh();
		this.OnChangeResolution();
		WidgetSideScreen.Instance = this;
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		SkinConfig skin = base.config.skin;
		UIContextMenu uicontextMenu = m.AddChild("setting");
		uicontextMenu.AddToggle("alignUI", this.extra.alignUI, delegate(bool a)
		{
			this.extra.alignUI = !this.extra.alignUI;
			this.OnChangeResolution();
		});
		uicontextMenu.AddToggle("reverse", this.extra.reverse, delegate(bool a)
		{
			this.extra.reverse = !this.extra.reverse;
			this.OnChangeResolution();
		});
		uicontextMenu.AddSlider("width", (float a) => a.ToString() ?? "", (float)this.extra.width, delegate(float a)
		{
			this.extra.width = (int)a;
			this.OnChangeResolution();
		}, 10f, 50f, true, true, false);
		uicontextMenu.AddSlider("image", (float a) => a.ToString() ?? "", (float)this.extra.idImage, delegate(float a)
		{
			this.extra.idImage = (int)a;
			this.OnChangeResolution();
		}, 0f, (float)(this.sprites.Length - 1), true, true, false);
		uicontextMenu.AddButton("colorBG", delegate()
		{
			EMono.ui.AddLayer<LayerColorPicker>().SetColor(this.bgColor, Color.white, delegate(PickerState state, Color _c)
			{
				this.bgColor = _c;
				this.OnChangeResolution();
			});
		}, true);
		base.SetBaseContextMenu(m);
	}

	public void Refresh()
	{
		EMono.scene.cam.rect = (this.extra.reverse ? new Rect(0.01f * (float)this.extra.width, 0f, 1f + 0.01f * (float)this.extra.width, 1f) : new Rect(-0.01f * (float)this.extra.width, 0f, 1f, 1f));
	}

	public override void OnChangeResolution()
	{
		base.OnChangeResolution();
		RectTransform rectTransform = this.Rect();
		float x = 0.01f * (float)Screen.width * (float)this.extra.width / EMono.core.uiScale + 50f;
		float num = (float)Screen.height / EMono.core.uiScale + 50f;
		rectTransform.sizeDelta = new Vector2(x, num);
		if (this.extra.reverse)
		{
			rectTransform.SetAnchor(0f, 0.5f, 0f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * 0.5f, 0f);
		}
		else
		{
			rectTransform.SetAnchor(1f, 0.5f, 1f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * -0.5f, 0f);
		}
		this.Refresh();
		this.imagePic.SetActive(this.extra.idImage > 0);
		Sprite sprite = this.sprites[this.extra.idImage];
		if (sprite)
		{
			this.imagePic.sprite = sprite;
			this.imagePic.rectTransform.sizeDelta = new Vector2(num * (float)sprite.texture.width / (float)sprite.texture.height, num);
			this.imagePic.color = this.bgColor;
		}
		if (!this.extra.alignUI)
		{
			EMono.ui.rectLayers.anchoredPosition = Vector2.zero;
			EMono.ui.rectLayers.sizeDelta = Vector2.zero;
			this.SlideMiniGame(0f);
			return;
		}
		if (this.extra.reverse)
		{
			EMono.ui.rectLayers.anchoredPosition = new Vector2(rectTransform.sizeDelta.x / 2f, 0f);
			EMono.ui.rectLayers.sizeDelta = new Vector2(-rectTransform.sizeDelta.x, 0f);
			this.SlideMiniGame(rectTransform.sizeDelta.x);
			return;
		}
		EMono.ui.rectLayers.anchoredPosition = new Vector2(-rectTransform.sizeDelta.x / 2f, 0f);
		EMono.ui.rectLayers.sizeDelta = new Vector2(-rectTransform.sizeDelta.x, 0f);
		this.SlideMiniGame(-rectTransform.sizeDelta.x);
	}

	public void SlideMiniGame(float w)
	{
		if (LayerMiniGame.Instance)
		{
			LayerMiniGame.Instance.mini.SlidePosition(w * EMono.core.uiScale);
		}
	}

	private void OnEnable()
	{
		this.Refresh();
	}

	private void OnDisable()
	{
		EMono.scene.cam.rect = new Rect(0f, 0f, 1f, 1f);
		EMono.ui.rectLayers.anchoredPosition = Vector2.zero;
		EMono.ui.rectLayers.sizeDelta = Vector2.zero;
		this.SlideMiniGame(0f);
	}

	public static WidgetSideScreen Instance;

	public Sprite[] sprites;

	public Image imagePic;

	public class Extra
	{
		public int width;

		public int height;

		public int idImage;

		public int bgColor;

		public bool reverse;

		public bool alignUI;
	}
}
