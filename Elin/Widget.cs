using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Widget : EMono, IChangeResolution, ISkinRoot
{
	public void Test()
	{
	}

	public string ID
	{
		get
		{
			if (this._ID == null)
			{
				return this._ID = base.name.Replace("Widget", "").Replace("(Clone)", "");
			}
			return this._ID;
		}
	}

	public Widget.Config config
	{
		get
		{
			Widget.Config result;
			if ((result = this._config) == null)
			{
				result = (this._config = EMono.player.widgets.dict[this.ID]);
			}
			return result;
		}
	}

	public SkinRoot skinRoot
	{
		get
		{
			SkinRoot result;
			if ((result = this._skinRoot) == null)
			{
				result = (this._skinRoot = base.GetComponent<SkinRoot>());
			}
			return result;
		}
	}

	public bool IsSealed
	{
		get
		{
			return this.config.meta.system && !EMono.core.config.test.unsealWidgets;
		}
	}

	public virtual bool ShowStyleMenu
	{
		get
		{
			return true;
		}
	}

	public virtual bool AlwaysTop
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsStaticPosition
	{
		get
		{
			return false;
		}
	}

	public virtual bool AlwaysBottom
	{
		get
		{
			return false;
		}
	}

	public virtual Type SetSiblingAfter
	{
		get
		{
			return null;
		}
	}

	public virtual bool ShowInBuildMode
	{
		get
		{
			return this.config.mode.HasFlag(GameMode.Build);
		}
	}

	public virtual bool RightClickToClose
	{
		get
		{
			return this.config.RC;
		}
	}

	public virtual bool AllowRightClickToClose
	{
		get
		{
			return false;
		}
	}

	public bool IsInRightMode()
	{
		return (EMono.scene.actionMode.ShowBuildWidgets && this.ShowInBuildMode) || this.config.IsInRightMode();
	}

	public void Activate()
	{
		this._rect = this.Rect();
		this.config.state = Widget.State.Active;
		if (this.config.extra == null)
		{
			this.config.extra = this.CreateExtra();
		}
		if (!this.dragPanel)
		{
			this.dragPanel = base.GetComponentInChildren<UIDragPanel>();
		}
		if (this.IsStaticPosition)
		{
			this.dragPanel.enable = false;
		}
		else
		{
			this.dragPanel.autoAnchor = (this.config.userAnchor == RectPosition.Auto);
			if (this.config.userAnchor != RectPosition.Auto)
			{
				this._rect.SetAnchor(this.config.userAnchor);
			}
			else
			{
				this._rect.SetAnchor(this.config.anchor);
			}
			this._rect.anchoredPosition = new Vector2(this.config.x, this.config.y);
			this.dragPanel.onDrag = new Action(this.OnChangePosition);
		}
		this.SetPivot(this.config.pivot);
		this.OnActivate();
		if (this == null)
		{
			return;
		}
		this.flip = (this._rect.position.x > (float)Screen.width * 0.5f);
		this.OnFlip();
		this.OnChangePosition();
		this.ClampToScreen();
		foreach (SkinDeco deco in this.config.skin.decos)
		{
			this.InstantiateDeco(deco);
		}
	}

	public virtual void OnActivate()
	{
	}

	public virtual object CreateExtra()
	{
		return null;
	}

	public void Deactivate()
	{
		this.OnDeactivate();
		this.config.state = Widget.State.Inactive;
		this.UpdateConfig();
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	public virtual void OnDeactivate()
	{
	}

	public void Close()
	{
		EMono.ui.widgets.DeactivateWidget(this);
	}

	public void Reactivate()
	{
		EMono.ui.widgets.DeactivateWidget(this);
		EMono.ui.widgets.ActivateWidget(this.config);
	}

	public virtual void OnManagerActivate()
	{
		this.goCover = Util.Instantiate("UI/Widget/CoverWidget", base.transform).gameObject;
		RectTransform rectTransform = this.goCover.transform.Rect();
		RectTransform rectTransform2 = this.dragPanel.Rect();
		rectTransform.pivot = rectTransform2.pivot;
		rectTransform.anchorMin = rectTransform2.anchorMin;
		rectTransform.anchorMax = rectTransform2.anchorMax;
		rectTransform.anchoredPosition = rectTransform2.anchoredPosition;
		rectTransform.sizeDelta = rectTransform2.sizeDelta;
		UIDragPanel componentInChildren = this.goCover.GetComponentInChildren<UIDragPanel>();
		componentInChildren.autoAnchor = (this.config.userAnchor == RectPosition.Auto);
		componentInChildren.onDrag = new Action(this.OnChangePosition);
		componentInChildren.container = this.dragPanel.container;
		componentInChildren.target = this.dragPanel.target;
		componentInChildren.bound = this.dragPanel.bound;
		if (this is WidgetSideScreen)
		{
			componentInChildren.enable = false;
		}
		this.goCover.GetComponentInChildren<UIText>().SetText(("Widget" + this.ID).lang());
	}

	public virtual void OnManagerDeactivate()
	{
		UnityEngine.Object.DestroyImmediate(this.goCover);
	}

	public void SetAnchor(RectPosition p)
	{
		this.config.userAnchor = p;
		this.dragPanel.autoAnchor = (p == RectPosition.Auto);
		this._rect.SetAnchor(p);
	}

	public void SetPivot(RectPosition p)
	{
		this.config.pivot = p;
		RectTransform rectTransform = this.Rect();
		Vector3 position = base.transform.position;
		switch (p)
		{
		case RectPosition.TopLEFT:
			rectTransform.pivot = new Vector2(0f, 1f);
			goto IL_126;
		case RectPosition.TopCenter:
			rectTransform.pivot = new Vector2(0.5f, 1f);
			goto IL_126;
		case RectPosition.TopRIGHT:
			rectTransform.pivot = new Vector2(1f, 1f);
			goto IL_126;
		case RectPosition.Left:
			rectTransform.pivot = new Vector2(0f, 0.5f);
			goto IL_126;
		case RectPosition.Right:
			rectTransform.pivot = new Vector2(1f, 0.5f);
			goto IL_126;
		case RectPosition.BottomLEFT:
			rectTransform.pivot = new Vector2(0f, 0f);
			goto IL_126;
		case RectPosition.BottomCenter:
			rectTransform.pivot = new Vector2(0.5f, 0f);
			goto IL_126;
		case RectPosition.BottomRIGHT:
			rectTransform.pivot = new Vector2(1f, 0f);
			goto IL_126;
		}
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		IL_126:
		base.transform.position = position;
		this.OnChangePivot();
		this.ClampToScreen();
	}

	public virtual void OnChangePivot()
	{
	}

	public void UpdateConfig()
	{
		this.config.anchor = this._rect.GetAnchor();
		this.config.x = this._rect.anchoredPosition.x;
		this.config.y = this._rect.anchoredPosition.y;
		this.OnUpdateConfig();
	}

	public virtual void OnUpdateConfig()
	{
	}

	protected void ClampToScreenEnsured(Component c, Vector2 anchoredPos)
	{
		RectTransform rectTransform = c.Rect();
		rectTransform.anchoredPosition = anchoredPos;
		rectTransform.RebuildLayout(false);
		rectTransform.ForceUpdateRectTransforms();
		this.ClampToScreen(rectTransform, 10f);
		rectTransform.anchoredPosition = anchoredPos;
		rectTransform.RebuildLayout(false);
		rectTransform.ForceUpdateRectTransforms();
		this.ClampToScreen(rectTransform, 10f);
	}

	public void ClampToScreen()
	{
		this.ClampToScreen(this.Rect(), 10f);
	}

	protected void ClampToScreen(RectTransform rect, float margin = 10f)
	{
		if (this.IsStaticPosition || this.config.userAnchor != RectPosition.Auto)
		{
			return;
		}
		Vector3 position = rect.position;
		Vector2 a = new Vector2(margin, margin);
		Vector2 a2 = new Vector2((float)Screen.width - margin, (float)Screen.height - margin);
		Vector3 vector = a - rect.rect.min;
		Vector3 vector2 = a2 - rect.rect.max;
		position.x = (float)((int)Mathf.Clamp(position.x, vector.x - 20f, vector2.x + 20f));
		position.y = (float)((int)Mathf.Clamp(position.y, vector.y - 20f, vector2.y + 20f));
		rect.position = position;
	}

	public virtual void OnChangeResolution()
	{
		this.ClampToScreen(this.Rect(), 10f);
	}

	public void OnChangePosition()
	{
		bool flag = this.Rect().position.x > (float)Screen.width * 0.5f;
		if (this.flip != flag)
		{
			this.flip = flag;
			this.OnFlip();
		}
		this.RefreshTipPivotPosition();
	}

	public virtual void OnChangeActionMode()
	{
		this.SetActive(this.IsInRightMode());
	}

	public virtual void OnFlip()
	{
	}

	public bool IsAlignTop()
	{
		RectPosition anchor = this.Rect().GetAnchor();
		return anchor == RectPosition.TopCenter || anchor == RectPosition.TopLEFT || anchor == RectPosition.TopRIGHT;
	}

	public Layer AddLayer(Layer l, Transform trans)
	{
		l.OnBeforeAddLayer();
		l.gameObject.SetActive(true);
		l.transform.SetParent(trans, false);
		l.Init();
		l.OnAfterAddLayer();
		Window window = l.windows[0];
		RectTransform rectTransform = l.Rect();
		RectTransform rectTransform2 = window.Rect();
		rectTransform.sizeDelta = window.Rect().sizeDelta;
		rectTransform.SetPivot(0.5f, 0.5f);
		rectTransform.SetAnchor(0.5f, 0.5f, 0.5f, 0.5f);
		rectTransform2.SetPivot(0.5f, 0.5f);
		rectTransform2.SetAnchor(0.5f, 0.5f, 0.5f, 0.5f);
		rectTransform2.anchoredPosition = Vector2.zero;
		window.setting.allowMove = (window.setting.allowResize = false);
		window.setting.saveWindow = false;
		window.bgCollider.SetActive(false);
		return l;
	}

	public void RefreshTipPivotPosition()
	{
		bool flag = (float)(Screen.width / 2 - 40) > base.transform.position.x;
		if (this.tipPivotLeft)
		{
			this.tipPivotLeft.SetActive(!flag || !this.tipPivotRight);
		}
		if (this.tipPivotRight)
		{
			this.tipPivotRight.SetActive(flag || !this.tipPivotLeft);
		}
	}

	public SkinSet GetSkin()
	{
		return SkinManager.Instance.skinSets[this.config.skin.id];
	}

	public SkinConfig GetSkinConfig()
	{
		return this.config.skin;
	}

	public void SetSkin(int id, int v = 0)
	{
		SkinConfig skin = this.config.skin;
		if (skin.id != id)
		{
			skin.SetID(id);
		}
		skin.id = id;
		skin.bg = v;
		skin.bgColor = skin.BG.color;
		this.ApplySkin();
	}

	public void TestSkin()
	{
		this.config.skin.SetID();
		this.ApplySkin();
	}

	public virtual void ApplySkin()
	{
		SkinRoot component = base.GetComponent<SkinRoot>();
		if (!component)
		{
			return;
		}
		component.Reset();
		IUISkin[] componentsInChildren = base.GetComponentsInChildren<IUISkin>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ApplySkin();
		}
		this.OnApplySkin();
	}

	public virtual void OnApplySkin()
	{
	}

	public void InstantiateDeco(SkinDeco deco)
	{
		SkinDecoActor skinDecoActor = Util.Instantiate<SkinDecoActor>("UI/Widget/SkinDecoActor", base.transform);
		deco.actor = skinDecoActor;
		skinDecoActor.owner = deco;
		skinDecoActor.image.sprite = ResourceCache.Load<Sprite>("Media/Graphics/Deco/deco" + deco.id.ToString());
		skinDecoActor.Rect().anchoredPosition = new Vector2((float)deco.x, (float)deco.y);
		skinDecoActor.Refresh();
	}

	public void AddDeco(SkinDeco deco)
	{
		this.config.skin.decos.Add(deco);
		this.InstantiateDeco(deco);
	}

	public void RemoveDeco(SkinDeco deco)
	{
		this.config.skin.decos.Remove(deco);
		UnityEngine.Object.DestroyImmediate(deco.actor.gameObject);
	}

	public virtual bool CanShowContextMenu()
	{
		return true;
	}

	public void ShowContextMenu()
	{
		UIContextMenu uicontextMenu = EMono.ui.CreateContextMenu("ContextMenu");
		this.OnSetContextMenu(uicontextMenu);
		uicontextMenu.Show();
	}

	public void SetBaseContextMenu(UIContextMenu m)
	{
		UIContextMenu uicontextMenu = m.AddChild("anchor");
		SkinConfig cfg = this.config.skin;
		using (List<RectPosition>.Enumerator enumerator = Util.EnumToList<RectPosition>().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				RectPosition p = enumerator.Current;
				uicontextMenu.AddButton(((this.config.userAnchor == p) ? "★ " : "") + p.ToString().lang(), delegate()
				{
					this.SetAnchor(p);
					SE.ClickGeneral();
					m.Hide();
				}, true);
			}
		}
		uicontextMenu = m.AddChild("pivot");
		using (List<RectPosition>.Enumerator enumerator = Util.EnumToList<RectPosition>().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				RectPosition p = enumerator.Current;
				uicontextMenu.AddButton(((this.config.pivot == p) ? "★ " : "") + p.ToString().lang(), delegate()
				{
					this.SetPivot(p);
					SE.ClickGeneral();
					m.Hide();
				}, true);
			}
		}
		UIContextMenuItem sliderB = null;
		Action Refresh = delegate()
		{
			int num = EMono.ui.skins.skinSets[cfg.id].bgs.Count - 1;
			sliderB.slider.maxValue = (float)num;
			sliderB.slider.value = (float)cfg.bg;
			sliderB.textSlider.text = cfg.bg.ToString() + "/" + (EMono.ui.skins.skinSets[cfg.id].bgs.Count - 1).ToString();
		};
		if (this.ShowStyleMenu)
		{
			UIContextMenu uicontextMenu2 = m.AddOrGetChild("style");
			sliderB = uicontextMenu2.AddSlider("changeBG", (float n) => n.ToString() + "/" + (EMono.ui.skins.skinSets[cfg.id].bgs.Count - 1).ToString(), (float)cfg.bg, delegate(float a)
			{
				this.SetSkin(cfg.id, (int)a);
				Refresh();
			}, 0f, (float)(EMono.ui.skins.skinSets[cfg.id].bgs.Count - 1), true, true, false);
			uicontextMenu2.AddSlider("bgSize", (float n) => n.ToString() ?? "", (float)cfg.bgSize, delegate(float a)
			{
				cfg.bgSize = (int)a;
				this.ApplySkin();
			}, -50f, 50f, true, true, false);
			Action<PickerState, Color> <>9__13;
			uicontextMenu2.AddButton("colorBG", delegate()
			{
				if (EMono.ui.skins.skinSets[cfg.id].bgs[cfg.bg].redirect != SkinAssetRedirect.None)
				{
					SE.Beep();
					return;
				}
				LayerColorPicker layerColorPicker = EMono.ui.AddLayer<LayerColorPicker>();
				Color bgColor = cfg.bgColor;
				Color color = EMono.ui.skins.skinSets[cfg.id].bgs[cfg.bg].color;
				Action<PickerState, Color> onChangeColor;
				if ((onChangeColor = <>9__13) == null)
				{
					onChangeColor = (<>9__13 = delegate(PickerState state, Color _c)
					{
						cfg.bgColor = _c;
						this.ApplySkin();
					});
				}
				layerColorPicker.SetColor(bgColor, color, onChangeColor);
			}, true);
			uicontextMenu2.AddButton("editDeco", delegate()
			{
				EMono.ui.AddLayer<LayerSkinDeco>().SetWidget(this);
			}, true);
		}
		if (this.AllowRightClickToClose)
		{
			m.AddToggle("RC_widget", this.config.RC, delegate(bool a)
			{
				this.config.RC = a;
			});
		}
		m.AddButton(() => (this.config.locked ? "unlockWidget" : "lockWidget").lang(), delegate()
		{
			EMono.ui.widgets.ToggleLock(this.config);
			SE.ClickGeneral();
			m.Hide();
			LayerWidget layer = EMono.ui.GetLayer<LayerWidget>(false);
			if (layer == null)
			{
				return;
			}
			layer.Refresh();
		});
		if (!this.IsSealed)
		{
			m.AddButton("closeWidget", delegate()
			{
				if (this is WidgetQuestTracker)
				{
					EMono.player.questTracker = false;
				}
				EMono.ui.widgets.DeactivateWidget(this);
				LayerWidget layer = EMono.ui.GetLayer<LayerWidget>(false);
				if (layer == null)
				{
					return;
				}
				layer.Refresh();
			}, true);
		}
	}

	public virtual void OnSetContextMenu(UIContextMenu m)
	{
		this.SetBaseContextMenu(m);
	}

	public void SetGridContextMenu(UIContextMenu m)
	{
		SkinConfig cfg = this.config.skin;
		m.AddSlider("skinGrid", (float n) => n.ToString() ?? "", (float)cfg.grid, delegate(float a)
		{
			cfg.grid = (int)a;
			cfg.gridColor = cfg.Grid.color;
			this.ApplySkin();
		}, 0f, (float)(cfg.Skin.bgGrid.Count - 1), true, true, false);
		Action<PickerState, Color> <>9__3;
		m.AddButton("colorGrid", delegate()
		{
			LayerColorPicker layerColorPicker = EMono.ui.AddLayer<LayerColorPicker>();
			Color gridColor = cfg.gridColor;
			Color color = cfg.Grid.color;
			Action<PickerState, Color> onChangeColor;
			if ((onChangeColor = <>9__3) == null)
			{
				onChangeColor = (<>9__3 = delegate(PickerState state, Color _c)
				{
					cfg.gridColor = _c;
					this.ApplySkin();
				});
			}
			layerColorPicker.SetColor(gridColor, color, onChangeColor);
		}, true);
	}

	public void SoundActivate()
	{
		EMono.Sound.Play(this.soundActivate);
	}

	private string _ID;

	private Widget.Config _config;

	public Image imageBG;

	public SoundData soundActivate;

	public UIDragPanel dragPanel;

	private RectTransform _rect;

	public RectTransform tipPivotLeft;

	public RectTransform tipPivotRight;

	private SkinRoot _skinRoot;

	protected bool flip;

	private GameObject goCover;

	[Serializable]
	public class Meta
	{
		public override string ToString()
		{
			return this.id;
		}

		public string id;

		public bool enabled;

		public bool locked;

		public bool debugOnly;

		public bool system;

		public Widget.WidgetType type;

		public GameMode mode;
	}

	public enum WidgetType
	{
		Default,
		ZoomMenu
	}

	public class Config
	{
		[JsonIgnore]
		public GameMode mode
		{
			get
			{
				return this.meta.mode;
			}
		}

		[JsonIgnore]
		public bool IsSealed
		{
			get
			{
				return this.IsSystem && !EMono.core.config.test.unsealWidgets;
			}
		}

		[JsonIgnore]
		public bool IsSystem
		{
			get
			{
				return this.meta.system || this.annoyPlayer;
			}
		}

		public bool IsInRightMode()
		{
			ActionMode actionMode = EMono.scene.actionMode;
			if (actionMode.ShowBuildWidgets)
			{
				return this.mode.HasFlag(GameMode.Build);
			}
			if (actionMode == ActionMode.NoMap || actionMode == ActionMode.NewZone || actionMode == ActionMode.ViewMap)
			{
				return this.mode.HasFlag(GameMode.NoMap);
			}
			if (actionMode == ActionMode.Bird)
			{
				return this.mode.HasFlag(GameMode.Bird);
			}
			if (ActionMode.DefaultMode == ActionMode.Sim)
			{
				return this.mode.HasFlag(GameMode.Sim);
			}
			if (ActionMode.DefaultMode == ActionMode.EloMap)
			{
				return this.mode.HasFlag(GameMode.EloMap);
			}
			return (ActionMode.IsAdv || ActionMode.DefaultMode == ActionMode.View) && (actionMode != ActionMode.Region || this.mode.HasFlag(GameMode.EloMap)) && (ActionMode.Adv.zoomOut || this.meta.type != Widget.WidgetType.ZoomMenu) && this.mode.HasFlag(GameMode.Adv);
		}

		public Widget.State state;

		public RectPosition anchor;

		public RectPosition userAnchor;

		public RectPosition pivot = RectPosition.Center;

		public string id;

		public string titleLang;

		public float x;

		public float y;

		public bool locked;

		public bool RC;

		public object extra;

		public SkinConfig skin = new SkinConfig();

		[JsonIgnore]
		public bool valid;

		[JsonIgnore]
		public bool annoyPlayer;

		[JsonIgnore]
		public Widget.Meta meta;
	}

	public enum State
	{
		Active,
		Inactive
	}
}
