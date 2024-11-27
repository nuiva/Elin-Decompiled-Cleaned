using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerTextureViewer : ELayer
{
	public override void OnInit()
	{
		this.windows[0].AddBottomSpace(20);
		this.windows[0].AddBottomButton("openUser", delegate
		{
			Util.ShowExplorer(CorePath.user + "Texture Replace/dummy.txt", false);
		}, false);
		this.windows[0].AddBottomButton("toggleSnowTexture", delegate
		{
			SE.Tab();
			this.snow = !this.snow;
			this.OnSwitchContent(this.windows[0]);
		}, false);
	}

	public override void OnSwitchContent(Window window)
	{
		this.data = (ELayer.core.textures.texMap.TryGetValue(window.CurrentTab.idLang + (this.snow ? "Snow" : ""), null) ?? ELayer.core.textures.texMap[window.CurrentTab.idLang]);
		this.RefreshPage();
		this.scrollbarH.value = 0f;
		this.scrollvarV.value = 1f;
	}

	public void RefreshPage()
	{
		this.scale = 0.5f;
		string id = this.data.id;
		if (id == "objS" || id == "objSS" || id == "objSSnow" || id == "objSSSnow")
		{
			this.scale = 1f;
		}
		if (this.zoom)
		{
			this.scale *= 2f;
		}
		if (this.fixZoom)
		{
			this.scale /= ELayer.ui.canvasScaler.scaleFactor;
		}
		this.imageTex.texture = null;
		this.imageTex.texture = this.data.tex;
		this.imageTex.rectTransform.sizeDelta = new Vector2((float)this.data.tex.width * this.scale, (float)this.data.tex.height * this.scale);
		this.imageTex.RebuildLayoutTo<Layer>();
		foreach (Image image in this.markers)
		{
			UnityEngine.Object.Destroy(image.gameObject);
		}
		this.markers.Clear();
		foreach (TextureReplace textureReplace in this.data.dictReplace.Values)
		{
			Image image2 = Util.Instantiate<Image>(this.moldMarker, this.imageTex.transform.parent);
			image2.rectTransform.sizeDelta = new Vector2(((float)Mathf.Max(textureReplace.w, this.data.tileW) + this.highlightSize * 2f) * this.scale, ((float)Mathf.Max(textureReplace.h, this.data.tileH) + this.highlightSize * 2f) * this.scale);
			this.SetPos(image2, textureReplace.index % 100, textureReplace.index / 100 * -1, this.highlightSize, this.markerFix);
			image2.color = ((textureReplace.source == TextureReplace.Source.Local) ? this.colorLocal : ((textureReplace.source == TextureReplace.Source.User) ? this.colorUser : this.colorMod));
			this.markers.Add(image2);
		}
	}

	public void SetPos(Component r, int x, int y, float size = 0f, Vector2 posFix = default(Vector2))
	{
		r.Rect().anchoredPosition = new Vector2((float)(x * this.data.tileW) * this.scale * this.test.x - this.highlightSize * this.scale, (float)(y * this.data.tileH) * this.scale * this.test.y + this.highlightSize * this.scale) + posFix;
	}

	private void Update()
	{
		RectTransform rectTransform = this.imageTex.rectTransform;
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.imageTex.rectTransform, Input.mousePosition, ELayer.ui.canvas.worldCamera, out vector);
		int num = (int)vector.x / (int)((float)this.data.tileW * this.scale);
		int num2 = (int)vector.y / (int)((float)this.data.tileH * this.scale);
		if (EInput.axis != Vector2.zero)
		{
			this.sizeX = Mathf.Clamp(this.sizeX + (int)EInput.axis.x, 1, 5);
			this.sizeY = Mathf.Clamp(this.sizeY + (int)EInput.axis.y * -1, 1, 5);
			EInput.requireAxisReset = true;
		}
		if (EInput.middleMouse.clicked)
		{
			SE.Tab();
			this.zoom = !this.zoom;
			this.RefreshPage();
		}
		bool flag = InputModuleEX.IsPointerOver(this.transMask);
		this.highlight.SetActive(flag);
		if (flag)
		{
			this.highlight.rectTransform.sizeDelta = new Vector2(((float)(this.data.tileW * this.sizeX) + this.highlightSize * 2f) * this.scale, ((float)(this.data.tileH * this.sizeY) + this.highlightSize * 2f) * this.scale);
			this.SetPos(this.highlight, num, num2, this.highlightSize, default(Vector2));
			string str = this.data.id + "_";
			int index = Mathf.Abs(num2) * 100 + num;
			str += index.ToString();
			this.textHint.text = str + ((this.sizeX == 1 && this.sizeY == 1) ? "" : string.Concat(new string[]
			{
				"(",
				this.sizeX.ToString(),
				"x",
				this.sizeY.ToString(),
				")"
			}));
			if (EInput.leftMouse.clicked)
			{
				UIContextMenu uicontextMenu = ELayer.ui.CreateContextMenuInteraction();
				TextureReplace replace = this.data.dictReplace.TryGetValue(index, null);
				if (replace != null)
				{
					uicontextMenu.AddButton("open_replace", delegate()
					{
						Util.Run(replace.file.FullName);
					}, true);
					if (replace.source != TextureReplace.Source.Mod)
					{
						uicontextMenu.AddButton("delete_replace", delegate()
						{
							try
							{
								this.data.DeleteReplace(replace);
								this.data.ForceRefresh();
							}
							catch
							{
							}
							SE.Trash();
							this.RefreshPage();
						}, true);
					}
				}
				else
				{
					uicontextMenu.AddButton("create_replace", delegate()
					{
						this.data.CreateReplace(index, CorePath.user + "Texture Replace/", TextureReplace.Source.User, this.sizeX, this.sizeY);
						SE.Change();
						this.RefreshPage();
					}, true);
				}
				if (ELayer._zone.isMapSaved && (replace == null || replace.source != TextureReplace.Source.Local))
				{
					uicontextMenu.AddButton("create_replaceLocal", delegate()
					{
						string text = ELayer._zone.pathSave + "Texture Replace";
						IO.CreateDirectory(text);
						this.data.CreateReplace(index, text + "/", TextureReplace.Source.Local, this.sizeX, this.sizeY);
						SE.Change();
						this.RefreshPage();
					}, true);
				}
				uicontextMenu.Show();
			}
		}
	}

	public RawImage imageTex;

	public Image highlight;

	public Image moldMarker;

	public TextureData data;

	public UIScrollView view;

	public Scrollbar scrollbarH;

	public Scrollbar scrollvarV;

	public UIText textHint;

	public Transform transMask;

	public float scale;

	public float highlightSize;

	public List<Image> markers = new List<Image>();

	public Vector2 markerFix;

	public Color colorUser;

	public Color colorMod;

	public Color colorLocal;

	public int sizeX;

	public int sizeY;

	public bool fixZoom;

	public bool snow;

	public Vector2 test;

	private bool zoom;
}
