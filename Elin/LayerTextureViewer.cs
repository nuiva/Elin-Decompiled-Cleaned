using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerTextureViewer : ELayer
{
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

	public override void OnInit()
	{
		windows[0].AddBottomSpace();
		windows[0].AddBottomButton("openUser", delegate
		{
			Util.ShowExplorer(CorePath.user + "Texture Replace/dummy.txt");
		});
		windows[0].AddBottomButton("toggleSnowTexture", delegate
		{
			SE.Tab();
			snow = !snow;
			OnSwitchContent(windows[0]);
		});
	}

	public override void OnSwitchContent(Window window)
	{
		data = ELayer.core.textures.texMap.TryGetValue(window.CurrentTab.idLang + (snow ? "Snow" : "")) ?? ELayer.core.textures.texMap[window.CurrentTab.idLang];
		RefreshPage();
		scrollbarH.value = 0f;
		scrollvarV.value = 1f;
	}

	public void RefreshPage()
	{
		scale = 0.5f;
		switch (data.id)
		{
		case "objS":
		case "objSS":
		case "objSSnow":
		case "objSSSnow":
			scale = 1f;
			break;
		}
		if (zoom)
		{
			scale *= 2f;
		}
		if (fixZoom)
		{
			scale /= ELayer.ui.canvasScaler.scaleFactor;
		}
		imageTex.texture = null;
		imageTex.texture = data.tex;
		imageTex.rectTransform.sizeDelta = new Vector2((float)data.tex.width * scale, (float)data.tex.height * scale);
		imageTex.RebuildLayoutTo<Layer>();
		foreach (Image marker in markers)
		{
			Object.Destroy(marker.gameObject);
		}
		markers.Clear();
		foreach (TextureReplace value in data.dictReplace.Values)
		{
			Image image = Util.Instantiate(moldMarker, imageTex.transform.parent);
			image.rectTransform.sizeDelta = new Vector2(((float)Mathf.Max(value.w, data.tileW) + highlightSize * 2f) * scale, ((float)Mathf.Max(value.h, data.tileH) + highlightSize * 2f) * scale);
			SetPos(image, value.index % 100, value.index / 100 * -1, highlightSize, markerFix);
			image.color = ((value.source == TextureReplace.Source.Local) ? colorLocal : ((value.source == TextureReplace.Source.User) ? colorUser : colorMod));
			markers.Add(image);
		}
	}

	public void SetPos(Component r, int x, int y, float size = 0f, Vector2 posFix = default(Vector2))
	{
		r.Rect().anchoredPosition = new Vector2((float)(x * data.tileW) * scale * test.x - highlightSize * scale, (float)(y * data.tileH) * scale * test.y + highlightSize * scale) + posFix;
	}

	private void Update()
	{
		_ = imageTex.rectTransform;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(imageTex.rectTransform, Input.mousePosition, ELayer.ui.canvas.worldCamera, out var localPoint);
		int num = (int)localPoint.x / (int)((float)data.tileW * scale);
		int num2 = (int)localPoint.y / (int)((float)data.tileH * scale);
		if (EInput.axis != Vector2.zero)
		{
			sizeX = Mathf.Clamp(sizeX + (int)EInput.axis.x, 1, 5);
			sizeY = Mathf.Clamp(sizeY + (int)EInput.axis.y * -1, 1, 5);
			EInput.requireAxisReset = true;
		}
		if (EInput.middleMouse.clicked)
		{
			SE.Tab();
			zoom = !zoom;
			RefreshPage();
		}
		bool flag = InputModuleEX.IsPointerOver(transMask);
		highlight.SetActive(flag);
		if (!flag)
		{
			return;
		}
		highlight.rectTransform.sizeDelta = new Vector2(((float)(data.tileW * sizeX) + highlightSize * 2f) * scale, ((float)(data.tileH * sizeY) + highlightSize * 2f) * scale);
		SetPos(highlight, num, num2, highlightSize);
		string text = data.id + "_";
		int index = Mathf.Abs(num2) * 100 + num;
		text += index;
		textHint.text = text + ((sizeX == 1 && sizeY == 1) ? "" : ("(" + sizeX + "x" + sizeY + ")"));
		if (!EInput.leftMouse.clicked)
		{
			return;
		}
		UIContextMenu uIContextMenu = ELayer.ui.CreateContextMenuInteraction();
		TextureReplace replace = data.dictReplace.TryGetValue(index);
		if (replace != null)
		{
			uIContextMenu.AddButton("open_replace", delegate
			{
				Util.Run(replace.file.FullName);
			});
			if (replace.source != TextureReplace.Source.Mod)
			{
				uIContextMenu.AddButton("delete_replace", delegate
				{
					try
					{
						data.DeleteReplace(replace);
						data.ForceRefresh();
					}
					catch
					{
					}
					SE.Trash();
					RefreshPage();
				});
			}
		}
		else
		{
			uIContextMenu.AddButton("create_replace", delegate
			{
				data.CreateReplace(index, CorePath.user + "Texture Replace/", TextureReplace.Source.User, sizeX, sizeY);
				SE.Change();
				RefreshPage();
			});
		}
		if (ELayer._zone.isMapSaved && (replace == null || replace.source != TextureReplace.Source.Local))
		{
			uIContextMenu.AddButton("create_replaceLocal", delegate
			{
				string text2 = ELayer._zone.pathSave + "Texture Replace";
				IO.CreateDirectory(text2);
				data.CreateReplace(index, text2 + "/", TextureReplace.Source.Local, sizeX, sizeY);
				SE.Change();
				RefreshPage();
			});
		}
		uIContextMenu.Show();
	}
}
