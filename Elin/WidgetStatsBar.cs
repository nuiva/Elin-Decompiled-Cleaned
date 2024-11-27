using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetStatsBar : Widget
{
	public override object CreateExtra()
	{
		return new WidgetStatsBar.Extra();
	}

	public WidgetStatsBar.Extra extra
	{
		get
		{
			return base.config.extra as WidgetStatsBar.Extra;
		}
	}

	public Color colorDefault
	{
		get
		{
			return base.config.skin.BG.colors.textDefault;
		}
	}

	public Color colorBad
	{
		get
		{
			return base.config.skin.BG.colors.textBad;
		}
	}

	public Color colorVeryBad
	{
		get
		{
			return base.config.skin.BG.colors.textBad;
		}
	}

	public override void OnActivate()
	{
		WidgetStatsBar.Instance = this;
		this.mold = this.layout.CreateMold(null);
		this.Build();
		base.InvokeRepeating("Refresh", 0.2f, 0.2f);
	}

	public void Build()
	{
		this.items.Clear();
		this.layout.DestroyChildren(false, true);
		if (this.extra.attributes)
		{
			foreach (Element ele in EMono.pc.elements.ListElements((Element e) => e.HasTag("primary"), null))
			{
				this.Add(ele, "", null, null, null, null);
			}
			this.Add(EMono.pc.elements.GetElement(79), "", null, null, null, null);
		}
		if (this.extra.maxAlly)
		{
			this.Add(null, "maxAlly", this.iconMaxAlly, () => (EMono.pc.party.members.Count - 1).ToString() + "/" + EMono.player.MaxAlly.ToString(), delegate
			{
				if (EMono.player.lastEmptyAlly >= 0)
				{
					return FontColor.Default;
				}
				return FontColor.Bad;
			}, null);
		}
		if (this.extra.money)
		{
			this.Add(null, "money", this.iconMoney, () => EMono.pc.GetCurrency("money").ToString("#,0") ?? "", null, null);
		}
		if (this.extra.money2)
		{
			this.Add(null, "money2", this.iconMoney2, () => EMono.pc.GetCurrency("money2").ToString("#,0") ?? "", null, null);
		}
		if (this.extra.plat)
		{
			this.Add(null, "plat", this.iconPlat, () => EMono.pc.GetCurrency("plat").ToString("#,0") ?? "", null, null);
		}
		if (this.extra.medal)
		{
			this.Add(null, "medal", this.iconMedal, () => EMono.pc.GetCurrency("medal").ToString("#,0") ?? "", null, null);
		}
		if (this.extra.karma)
		{
			this.Add(null, "karma", this.iconKarma, () => EMono.player.karma.ToString() ?? "", null, null);
		}
		if (this.extra.mood)
		{
			this.Add(null, "mood", this.iconGodMood, () => EMono.pc.faith.TextMood ?? "", null, null);
		}
		if (this.extra.fame)
		{
			this.Add(null, "fame", this.iconFame, () => EMono.player.fame.ToString() ?? "", null, null);
		}
		if (this.extra.influence)
		{
			this.Add(null, "influence", this.iconInfluence, () => EMono._zone.influence.ToString() ?? "", null, () => EMono._zone.influence != 0);
		}
		if (this.extra.tourism_value)
		{
			this.Add(null, "tourism_value", this.iconTourismValue, delegate
			{
				if (EMono.Branch != null)
				{
					return EMono.Branch.tourism.ToFormat();
				}
				return "";
			}, null, () => EMono._zone.IsPCFaction);
		}
		if (this.extra.hearthLv)
		{
			this.Add(null, "hearth_lv", this.iconHearth, delegate
			{
				if (EMono.Branch != null)
				{
					return EMono.Branch.TextLv;
				}
				return "";
			}, delegate
			{
				if (EMono.Branch != null && EMono.Branch.exp < EMono.Branch.GetNextExp(-1))
				{
					return FontColor.Default;
				}
				return FontColor.Good;
			}, () => EMono._zone.IsPCFaction);
		}
		if (this.extra.weight)
		{
			this.Add(null, "weight", this.iconWeight, () => ((float)EMono.pc.ChildrenWeight / 1000f).ToString("F1") + " / " + ((float)EMono.pc.WeightLimit / 1000f).ToString("F1"), delegate
			{
				int phase = EMono.pc.burden.GetPhase();
				if (phase == 0)
				{
					return FontColor.Default;
				}
				if (phase != 1)
				{
					return FontColor.Bad;
				}
				return FontColor.Warning;
			}, null);
		}
		this.Refresh();
	}

	public UIItem Add(Element ele, string id = "", Sprite sprite = null, Func<string> func = null, Func<FontColor> funcCol = null, Func<bool> funcShow = null)
	{
		UIItem uiitem = Util.Instantiate<UIItem>(this.mold, this.layout);
		WidgetStatsBar.Item item = new WidgetStatsBar.Item
		{
			ele = ele,
			component = uiitem,
			id = id,
			func = func,
			funcCol = funcCol,
			funcShow = funcShow
		};
		if (sprite)
		{
			uiitem.image1.sprite = sprite;
		}
		else
		{
			uiitem.image1.sprite = ele.GetIcon("_m");
		}
		uiitem.image1.SetNativeSize();
		uiitem.text1.skinRoot = base.skinRoot;
		Shadow component = uiitem.text1.GetComponent<Shadow>();
		if (component)
		{
			component.enabled = base.config.skin.BG.textShadow;
		}
		this.items.Add(item);
		return uiitem;
	}

	public void Refresh()
	{
		bool flag = false;
		foreach (WidgetStatsBar.Item item in this.items)
		{
			string text = item.component.text1.text;
			FontColor fontColor = FontColor.Default;
			string text2;
			if (!item.id.IsEmpty())
			{
				text2 = item.func();
				if (item.funcCol != null)
				{
					fontColor = item.funcCol();
				}
			}
			else
			{
				Element ele = item.ele;
				text2 = (ele.DisplayValue.ToString() ?? "");
				if (ele.DisplayValue < ele.ValueWithoutLink)
				{
					text2 = text2 + " (" + ele.ValueWithoutLink.ToString() + ")";
				}
				if (ele.DisplayValue < ele.ValueWithoutLink)
				{
					fontColor = FontColor.Bad;
				}
				else if (EMono.pc.tempElements != null)
				{
					Element element = EMono.pc.tempElements.GetElement(ele.id);
					if (element != null && element.vBase > 0)
					{
						fontColor = FontColor.Good;
					}
				}
			}
			if (text2 != text || item.lastColor != fontColor)
			{
				item.component.text1.SetText(text2, fontColor);
				item.component.RebuildLayout(false);
				flag = true;
				item.lastColor = fontColor;
			}
			if (item.funcShow != null)
			{
				item.component.SetActive(item.funcShow());
			}
		}
		if (flag)
		{
			this.layout.RebuildLayout(false);
			this.RebuildLayout(false);
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uicontextMenu = m.AddChild("setting");
		uicontextMenu.AddToggle("attributes", this.extra.attributes, delegate(bool a)
		{
			this.extra.attributes = a;
			this.Build();
		});
		uicontextMenu.AddToggle("maxAlly", this.extra.maxAlly, delegate(bool a)
		{
			this.extra.maxAlly = a;
			this.Build();
		});
		uicontextMenu.AddToggle("money", this.extra.money, delegate(bool a)
		{
			this.extra.money = a;
			this.Build();
		});
		uicontextMenu.AddToggle("money2", this.extra.money2, delegate(bool a)
		{
			this.extra.money2 = a;
			this.Build();
		});
		uicontextMenu.AddToggle("plat", this.extra.plat, delegate(bool a)
		{
			this.extra.plat = a;
			this.Build();
		});
		uicontextMenu.AddToggle("medal", this.extra.medal, delegate(bool a)
		{
			this.extra.medal = a;
			this.Build();
		});
		uicontextMenu.AddToggle("influence", this.extra.influence, delegate(bool a)
		{
			this.extra.influence = a;
			this.Build();
		});
		uicontextMenu.AddToggle("karma", this.extra.karma, delegate(bool a)
		{
			this.extra.karma = a;
			this.Build();
		});
		uicontextMenu.AddToggle("fame", this.extra.fame, delegate(bool a)
		{
			this.extra.fame = a;
			this.Build();
		});
		if (EMono.debug)
		{
			uicontextMenu.AddToggle("godMood", this.extra.mood, delegate(bool a)
			{
				this.extra.mood = a;
				this.Build();
			});
		}
		uicontextMenu.AddToggle("tourism_value", this.extra.tourism_value, delegate(bool a)
		{
			this.extra.tourism_value = a;
			this.Build();
		});
		uicontextMenu.AddToggle("hearth_lv", this.extra.hearthLv, delegate(bool a)
		{
			this.extra.hearthLv = a;
			this.Build();
		});
		uicontextMenu.AddToggle("invWeight", this.extra.weight, delegate(bool a)
		{
			this.extra.weight = a;
			this.Build();
		});
		base.SetBaseContextMenu(m);
	}

	public override void OnApplySkin()
	{
		base.OnApplySkin();
		this.Build();
	}

	public static WidgetStatsBar Instance;

	public LayoutGroup layout;

	public List<WidgetStatsBar.Item> items = new List<WidgetStatsBar.Item>();

	public Sprite iconMoney;

	public Sprite iconMoney2;

	public Sprite iconPlat;

	public Sprite iconMedal;

	public Sprite iconKarma;

	public Sprite iconWeight;

	public Sprite iconInfluence;

	public Sprite iconMaxAlly;

	public Sprite iconGodMood;

	public Sprite iconHearth;

	public Sprite iconTourismValue;

	public Sprite iconFame;

	private UIItem mold;

	public class Item
	{
		public UIItem component;

		public Element ele;

		public string id;

		public Sprite sprite;

		public Func<string> func;

		public Func<FontColor> funcCol;

		public Func<bool> funcShow;

		public FontColor lastColor;
	}

	public class Extra
	{
		public bool attributes;

		public bool money;

		public bool money2;

		public bool plat;

		public bool medal;

		public bool karma;

		public bool mood;

		public bool weight;

		public bool influence;

		public bool maxAlly;

		public bool hearthLv;

		public bool tourism_value;

		public bool fame;
	}
}
