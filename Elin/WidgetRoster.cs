using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class WidgetRoster : Widget
{
	public override object CreateExtra()
	{
		return new WidgetRoster.Extra();
	}

	public WidgetRoster.Extra extra
	{
		get
		{
			return base.config.extra as WidgetRoster.Extra;
		}
	}

	public override bool AlwaysBottom
	{
		get
		{
			return true;
		}
	}

	public override Type SetSiblingAfter
	{
		get
		{
			return typeof(WidgetBottomBar);
		}
	}

	public override void OnActivate()
	{
		if (Mathf.Abs(this.extra.margin) > 100)
		{
			this.extra.margin = 0;
			this.extra.width = 40;
			this.extra.portrait = false;
			this.extra.onlyName = true;
		}
		WidgetRoster.Instance = this;
		this.mold = this.layout.CreateMold(null);
		this.Build();
	}

	private void OnEnable()
	{
		base.InvokeRepeating("Refresh", 0.2f, 0.2f);
	}

	private void OnDisable()
	{
		base.CancelInvoke();
	}

	public void OnMoveZone()
	{
		this.Build();
	}

	public void Build()
	{
		this.buttons.Clear();
		this.layout.DestroyChildren(false, true);
		this.layout.constraint = (this.extra.vertical ? GridLayoutGroup.Constraint.FixedColumnCount : GridLayoutGroup.Constraint.FixedRowCount);
		this.layout.startCorner = (this.extra.reverse ? GridLayoutGroup.Corner.LowerRight : GridLayoutGroup.Corner.UpperLeft);
		this.layout.cellSize = new Vector2((float)(this.extra.width * 4 / 5), (float)(this.extra.onlyName ? 32 : this.extra.width));
		this.layout.spacing = new Vector2((float)this.extra.margin, (float)this.extra.margin);
		foreach (Chara chara in EMono.pc.party.members)
		{
			if (chara != EMono.pc || this.extra.pc)
			{
				this.Add(chara);
			}
		}
		if (this.buttons.Count == 0)
		{
			this.Add(EMono.pc);
		}
		this.layout.RebuildLayout(false);
		this.RebuildLayout(false);
		this.OnChangeResolution();
		base.ClampToScreen();
	}

	public static void SetDirty()
	{
		if (WidgetRoster.Instance)
		{
			WidgetRoster.Instance.dirty = true;
		}
	}

	public void Refresh()
	{
		if (this.dirty)
		{
			this.Build();
		}
		foreach (ButtonRoster buttonRoster in this.buttons.Values)
		{
			buttonRoster.Refresh();
		}
		this.dirty = false;
	}

	public void Add(Chara c)
	{
		ButtonRoster buttonRoster = Util.Instantiate<ButtonRoster>(this.mold, this.layout);
		buttonRoster.SetChara(c);
		this.buttons.Add(c, buttonRoster);
	}

	public void Remove(Chara c)
	{
		UnityEngine.Object.DestroyImmediate(this.buttons[c].gameObject);
		this.buttons.Remove(c);
	}

	public void OnAddMember(Chara c)
	{
		if (this.buttons.ContainsKey(c))
		{
			return;
		}
		this.Add(c);
	}

	public void OnRemoveMember(Chara c)
	{
		if (!this.buttons.ContainsKey(c))
		{
			return;
		}
		this.Remove(c);
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		ButtonRoster b = InputModuleEX.GetComponentOf<ButtonRoster>();
		if (b)
		{
			int index = EMono.pc.party.members.IndexOf(b.chara);
			int count = EMono.pc.party.members.Count;
			if (index >= 1 && index < count - 1)
			{
				m.AddButton("next", delegate()
				{
					base.<OnSetContextMenu>g__Move|11(1);
				}, true);
			}
			if (index >= 2)
			{
				m.AddButton("prev", delegate()
				{
					base.<OnSetContextMenu>g__Move|11(-1);
				}, true);
			}
		}
		UIContextMenu uicontextMenu = m.AddChild("setting");
		uicontextMenu.AddSlider("width", (float a) => a.ToString() ?? "", (float)this.extra.width, delegate(float a)
		{
			this.extra.width = (int)a;
			this.Build();
		}, 30f, 160f, true, true, false);
		uicontextMenu.AddSlider("margine", (float a) => a.ToString() ?? "", (float)this.extra.margin, delegate(float a)
		{
			this.extra.margin = (int)a;
			this.Build();
		}, -50f, 50f, true, true, false);
		uicontextMenu.AddToggle("roster_pc", this.extra.pc, delegate(bool a)
		{
			this.extra.pc = a;
			this.Build();
		});
		uicontextMenu.AddToggle("vertical", this.extra.vertical, delegate(bool a)
		{
			this.extra.vertical = a;
			this.Build();
		});
		uicontextMenu.AddToggle("roster_portrait", this.extra.portrait, delegate(bool a)
		{
			this.extra.portrait = a;
			this.Build();
		});
		uicontextMenu.AddToggle("roster_onlyname", this.extra.onlyName, delegate(bool a)
		{
			this.extra.onlyName = a;
			this.Build();
		});
		uicontextMenu.AddToggle("reverseOrder", this.extra.reverse, delegate(bool a)
		{
			this.extra.reverse = a;
			this.Build();
		});
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", (float)base.config.skin.button, delegate(float a)
		{
			this.config.skin.button = (int)a;
			this.ApplySkin();
		}, 0f, (float)(base.config.skin.Skin.buttons.Count - 1), true, true, false);
		base.SetBaseContextMenu(m);
	}

	public static WidgetRoster Instance;

	public GridLayoutGroup layout;

	public ButtonRoster mold;

	public RawImage imageGrid;

	public Dictionary<Chara, ButtonRoster> buttons = new Dictionary<Chara, ButtonRoster>();

	public UIButton moldDropperLeft;

	public UIButton moldDropperRight;

	public bool showName;

	public bool dirty;

	public int maxWidth;

	public class Extra
	{
		[JsonProperty]
		public int width = 40;

		[JsonProperty]
		public int margin;

		[JsonProperty]
		public bool vertical;

		[JsonProperty]
		public bool pc;

		[JsonProperty]
		public bool portrait;

		[JsonProperty]
		public bool reverse;

		[JsonProperty]
		public bool onlyName;

		[JsonProperty]
		public bool showHP;
	}
}
