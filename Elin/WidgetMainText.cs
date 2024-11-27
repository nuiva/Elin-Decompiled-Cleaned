using System;
using UnityEngine;

public class WidgetMainText : Widget
{
	public override object CreateExtra()
	{
		return new WidgetMainText.Extra();
	}

	public WidgetMainText.Extra extra
	{
		get
		{
			return base.config.extra as WidgetMainText.Extra;
		}
	}

	public override bool ShowStyleMenu
	{
		get
		{
			return false;
		}
	}

	public override bool AlwaysTop
	{
		get
		{
			return true;
		}
	}

	public override void OnActivate()
	{
		bool flag = true;
		if (WidgetMainText.boxBk)
		{
			UnityEngine.Object.DestroyImmediate(this.box.gameObject);
			this.box = WidgetMainText.boxBk;
			WidgetMainText.boxBk.transform.SetParent(base.transform, false);
			WidgetMainText.boxBk.GetComponentInChildren<UIDragPanel>(true).SetTarget(this.Rect());
			WidgetMainText.boxBk = null;
			flag = false;
		}
		this.dragPanel = this.box.dragPanel;
		this.imageBG = this.box.imageBg;
		WidgetMainText.Instance = this;
		this.box.maxBlock = this.extra.maxLine;
		this.box.fadeLines = this.extra.fadeLines;
		if (base.config.skin.bgColor.Equals(new Color(0f, 0f, 0f, 0f)))
		{
			base.config.skin.bgColor = EMono.core.refs.bg_msg[this.extra.bg].color;
		}
		this.RefreshBG();
		this.OnChangeResolution();
		this.box.Init();
		if (flag)
		{
			this.Append("welcome_intro".langGame(EMono.core.version.GetText(), null, null, null), null);
			this.Append(this.spriteElin);
			this.NewLine();
			this.Append("welcome".langGame(EInput.keys.report.key.ToString() ?? "", null, null, null), null);
			this.NewLine();
		}
		this._Refresh();
	}

	public void RefreshBG()
	{
		this.box.SetBG(EMono.core.refs.bg_msg[this.extra.bg], base.config.skin.bgColor);
	}

	public void Append(string s, Point pos = null)
	{
		this.Append(s, Msg.colors.Default, pos);
	}

	public void Append(string s, Color col, Point pos = null)
	{
		if (s.IsEmpty() || s == " ")
		{
			return;
		}
		if (pos != null)
		{
			this.box.Load("MsgFocus").button1.onClick.AddListener(delegate()
			{
				EMono.screen.Focus(pos);
			});
		}
		if (s.StartsWith("*"))
		{
			s = " " + s;
		}
		if (Lang.setting.useSpace)
		{
			s += " ";
			if (s[0] == ' ')
			{
				s = s.TrimStart(' ');
			}
		}
		else if (s.EndsWith("*"))
		{
			s += " ";
		}
		if (MsgBlock.lastBlock != null && MsgBlock.lastText == s)
		{
			MsgBlock lastBlock = MsgBlock.lastBlock;
			if (lastBlock.txt != null)
			{
				UIText txt = lastBlock.txt;
				lastBlock.repeat++;
				string text = txt.text;
				if (text.EndsWith(")  ") && text.Contains("(x"))
				{
					text = text.Substring(0, text.IndexOf("(x"));
				}
				txt.text = text + "(x" + (lastBlock.repeat + 1).ToString() + ")  ";
				if (lastBlock.repeat == 1)
				{
					txt.RebuildLayout(false);
					this.box.RebuildLayout(true);
				}
				return;
			}
		}
		this.box.Append(s, col);
		this._Refresh();
	}

	public void Append(Sprite sprite)
	{
		this.box.Append(sprite, false);
		this._Refresh();
	}

	public static void HideLog()
	{
		if (!WidgetMainText.Instance || !WidgetMainText.Instance.box.isShowingLog)
		{
			return;
		}
		WidgetMainText.Instance._ToggleLog();
	}

	public static void ToggleLog()
	{
		if (!WidgetMainText.Instance)
		{
			EMono.ui.widgets.Activate("MainText");
		}
		WidgetMainText.Instance._ToggleLog();
		SE.ClickGeneral();
	}

	public void _ToggleLog()
	{
		this.box.ToggleLog();
		this._Refresh();
		if (this.box.isShowingLog)
		{
			if (WidgetFeed.Instance)
			{
				WidgetFeed.Instance.pop.KillAll(false);
			}
			this.box.RebuildLayout(true);
		}
	}

	public void NewLine()
	{
		this.box.MarkNewBlock();
	}

	public static void Refresh()
	{
		if (WidgetMainText.Instance)
		{
			WidgetMainText.Instance._Refresh();
		}
	}

	private void _Refresh()
	{
		this.box.SetActive(this.box.isShowingLog || !WidgetFeed.Intercept, delegate(bool enabled)
		{
			if (enabled)
			{
				this.box.RebuildLayout(true);
			}
		});
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		SkinConfig cfg = base.config.skin;
		UIContextMenu uicontextMenu = m.AddChild("setting");
		uicontextMenu.AddSlider("msgLines", (float n) => n.ToString() ?? "", (float)this.box.maxBlock, delegate(float a)
		{
			this.box.maxBlock = (int)a;
			this.extra.maxLine = (int)a;
		}, 2f, 10f, true, false, false);
		uicontextMenu.AddSlider("width", (float a) => a.ToString() ?? "", (float)this.extra.width, delegate(float a)
		{
			this.extra.width = (int)a;
			this.OnChangeResolution();
		}, 30f, 100f, true, true, false);
		UIContextMenu uicontextMenu2 = m.AddChild("style");
		uicontextMenu2.AddToggle("fadeLines", this.extra.fadeLines, delegate(bool a)
		{
			WidgetMainText.Extra extra = this.extra;
			this.box.fadeLines = a;
			extra.fadeLines = a;
			this.box.RefreshAlpha();
		});
		uicontextMenu2.AddSlider("changeBG", (float n) => n.ToString() + "/" + (EMono.core.refs.bg_msg.Count - 1).ToString(), (float)this.extra.bg, delegate(float a)
		{
			this.extra.bg = (int)a;
			cfg.bgColor = EMono.core.refs.bg_msg[this.extra.bg].color;
			this.RefreshBG();
		}, 0f, (float)(EMono.core.refs.bg_msg.Count - 1), true, true, false);
		Action<PickerState, Color> <>9__8;
		uicontextMenu2.AddButton("colorBG", delegate()
		{
			LayerColorPicker layerColorPicker = EMono.ui.AddLayer<LayerColorPicker>();
			Color bgColor = cfg.bgColor;
			Color color = EMono.ui.skins.skinSets[cfg.id].bgs[cfg.bg].color;
			Action<PickerState, Color> onChangeColor;
			if ((onChangeColor = <>9__8) == null)
			{
				onChangeColor = (<>9__8 = delegate(PickerState state, Color _c)
				{
					cfg.bgColor = _c;
					this.RefreshBG();
				});
			}
			layerColorPicker.SetColor(bgColor, color, onChangeColor);
		}, true);
		base.SetBaseContextMenu(m);
	}

	public override void OnChangeResolution()
	{
		base.OnChangeResolution();
		RectTransform rectTransform = this.Rect();
		rectTransform.sizeDelta = new Vector2(0.01f * (float)Screen.width * (float)this.extra.width, rectTransform.sizeDelta.y);
	}

	public static WidgetMainText Instance;

	public static MsgBox boxBk;

	public MsgBox box;

	private UIItem currentItem;

	public UIItem moldLine;

	public int visibleLines;

	public Transform layout;

	public Sprite spriteElin;

	public Sprite spriteDestroy;

	public Sprite spriteEther;

	[NonSerialized]
	public bool newLine = true;

	public class Extra
	{
		public int maxLine;

		public int bg;

		public int width;

		public bool fadeLines;
	}
}
