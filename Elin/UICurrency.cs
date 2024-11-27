using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICurrency : EMono
{
	private void Awake()
	{
		if (this.autoBuild)
		{
			this.Build();
		}
		base.InvokeRepeating("Refresh", 0.1f, 0.1f);
	}

	private void OnEnable()
	{
		this.Refresh();
	}

	public void Build(UICurrency.Options _options)
	{
		this.options = _options;
		this.Build();
	}

	public void Build()
	{
		this.items.Clear();
		this.mold = this.layout.CreateMold(null);
		if (this.options.plat)
		{
			this.Add(this.icons[1], "plat", () => EMono.pc.GetCurrency("plat").ToString("#,0") ?? "");
		}
		if (this.options.money)
		{
			this.Add(this.icons[0], "money", () => EMono.pc.GetCurrency("money").ToString("#,0") ?? "");
		}
		if (this.options.money2)
		{
			this.Add(this.icons[5], "money2", () => EMono.pc.GetCurrency("money2").ToString("#,0") ?? "");
		}
		if (this.options.medal)
		{
			this.Add(this.icons[4], "medal", () => EMono.pc.GetCurrency("medal").ToString("#,0") ?? "");
		}
		if (this.options.ecopo)
		{
			this.Add(EMono.sources.cards.map["ecopo"].GetSprite(0, 0, false), "ecopo", () => EMono.pc.GetCurrency("ecopo").ToString("#,0") ?? "");
		}
		if (this.options.influence)
		{
			this.Add(this.icons[3], "influence", () => EMono._zone.influence.ToString() ?? "");
		}
		if (this.options.casino)
		{
			this.Add(this.icons[10], "casino_coin", () => EMono.pc.GetCurrency("casino_coin").ToString("#,0") ?? "");
		}
		if (this.options.weight)
		{
			this.Add(this.icons[11], "weightInv", () => Lang._weight(this.target.ChildrenWeight, false, 0) + " / " + Lang._weight(this.target.WeightLimit, true, 0));
		}
		if (EMono.BranchOrHomeBranch != null)
		{
			HomeResourceManager resources = EMono.BranchOrHomeBranch.resources;
			if (this.options.branchMoney)
			{
				this.Add(this.icons[5], resources.money.Name, () => resources.money.value.ToString("#,0") ?? "");
			}
			if (this.options.branchFood)
			{
				this.Add(this.icons[6], resources.food.Name, () => resources.food.value.ToString("#,0") ?? "");
			}
			if (this.options.branchKnowledge)
			{
				this.Add(this.icons[7], resources.knowledge.Name, () => resources.knowledge.value.ToString("#,0") ?? "");
			}
			if (this.options.admin)
			{
				this.Add(this.icons[9], "ap", () => EMono.Branch.policies.CurrentAP().ToString() + "/" + EMono.Branch.MaxAP.ToString());
			}
			if (this.options.resources)
			{
				using (List<BaseHomeResource>.Enumerator enumerator = EMono.BranchOrHomeBranch.resources.list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BaseHomeResource r = enumerator.Current;
						if (r.IsAvailable)
						{
							this.Add(r.Sprite, r.Name, () => r.value.ToString("#,0") ?? "");
						}
					}
				}
			}
		}
		this.layout.RebuildLayout(false);
	}

	public void Add(Sprite icon, string lang, Func<string> func)
	{
		UIButton uibutton = Util.Instantiate<UIButton>(this.mold, this.layout);
		uibutton.icon.sprite = icon;
		uibutton.icon.SetNativeSize();
		uibutton.tooltip.lang = lang.lang().ToTitleCase(true);
		this.items.Add(new UICurrency.Item
		{
			func = func,
			text = uibutton.mainText
		});
		uibutton.mainText.SetText(func());
		uibutton.mainText.RebuildLayout(false);
		uibutton.RebuildLayout(false);
	}

	public void Refresh()
	{
		foreach (UICurrency.Item item in this.items)
		{
			string text = item.func();
			if (item.text.text != (text ?? ""))
			{
				item.text.SetText(text);
				item.text.RebuildLayout(false);
				item.text.transform.parent.RebuildLayout(false);
			}
		}
	}

	public List<UICurrency.Item> items = new List<UICurrency.Item>();

	public bool autoBuild;

	public bool disable;

	public UICurrency.Options options;

	public Sprite[] icons;

	public LayoutGroup layout;

	public Card target;

	private UIButton mold;

	[Serializable]
	public class Options
	{
		public bool money;

		public bool plat;

		public bool ecopo;

		public bool knowledge;

		public bool influence;

		public bool medal;

		public bool money2;

		public bool resources;

		public bool branchMoney;

		public bool branchFood;

		public bool branchMaterial;

		public bool branchKnowledge;

		public bool admin;

		public bool casino;

		public bool weight;
	}

	public class Item
	{
		public Func<string> func;

		public UIText text;
	}
}
