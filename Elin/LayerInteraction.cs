using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerInteraction : ELayer
{
	public override void OnInit()
	{
		LayerInteraction.Instance = this;
		this.first = true;
	}

	public override void OnKill()
	{
		LayerInteraction.Target = null;
	}

	private unsafe void Update()
	{
		if (this.mode != LayerInteraction.Mode.Map)
		{
			return;
		}
		BaseTileSelector tileSelector = ELayer.screen.tileSelector;
		MeshPass meshPass = (this.point.HasBlock || this.point.cell.liquidLv > 0) ? ELayer.screen.guide.passGuideBlock : ELayer.screen.guide.passGuideFloor;
		int num = 0;
		Vector3 vector = *this.point.Position();
		meshPass.Add(vector.x, vector.y, vector.z, (float)num, 0.3f);
	}

	public override void OnUpdateInput()
	{
	}

	public static void Show(IInspect newTarget)
	{
		if (LayerInteraction.Target == newTarget)
		{
			return;
		}
		LayerInteraction.Target = newTarget;
		LayerInteraction.Page page = LayerInteraction.GetPage(LayerInteraction.Target);
		((LayerInteraction.Instance != null) ? LayerInteraction.Instance : ELayer.ui.AddLayer<LayerInteraction>()).ShowPage(page);
	}

	public static bool TryShow(bool quick)
	{
		if (!Scene.HitPoint.IsValid)
		{
			return false;
		}
		LayerInteraction.Show(ELayer.scene.mouseTarget.pos.ListInspectorTargets().NextItem(LayerInteraction.Target));
		return true;
	}

	public static LayerInteraction.Page GetPage(IInspect o)
	{
		LayerInteraction.Page page = new LayerInteraction.Page();
		List<LayerInteraction.Item> items = page.items;
		if (o is Area)
		{
			Area area = o as Area;
			page.area = area;
		}
		else if (o is Chara)
		{
			Chara t = o as Chara;
			string text = "charaInfo".lang();
			if (!t.IsHomeMember())
			{
				text = text + "(" + "unidentified".lang() + ")";
			}
			if (t.IsHomeMember())
			{
				page.Add(t, "tTalk".lang(), "", delegate()
				{
					t.ShowDialog();
				}, 0, false);
			}
			else
			{
				page.Add(t, text, "", delegate()
				{
					if (!t.IsHomeMember())
					{
						SE.Beep();
						return;
					}
				}, 0, false);
			}
		}
		else if (o is Thing)
		{
			Thing t = o as Thing;
			page.Add(t, "objInfo", "", delegate()
			{
				ELayer.ui.AddLayer<LayerInfo>().Set(t, false);
			}, 0, false);
			if (t.trait is TraitQuestBoard)
			{
				page.Add(t, "quest", "", delegate()
				{
					ELayer.ui.AddLayer<LayerQuestBoard>();
				}, 20, true);
				page.Add(t, "hire", "", delegate()
				{
					ELayer.ui.AddLayer<LayerHire>();
				}, 20, true);
			}
			if (t.trait is TraitGacha)
			{
				page.Add(t, "gacha", "", delegate()
				{
					ELayer.ui.AddLayer<LayerGacha>();
				}, 10, true);
			}
			if (t.trait.IsFactory)
			{
				page.Add(t, "craft", "icon_Inspect", delegate()
				{
					ELayer.ui.AddLayer<LayerCraft>().SetFactory(t);
				}, 100, true);
			}
			if (t.IsInstalled)
			{
				page.Add(t, "move", "", delegate()
				{
					ActionMode.Inspect.Activate(t);
				}, 0, false);
			}
		}
		return page;
	}

	public void Show(List<LayerInteraction.Page> _pages, LayerInteraction.Mode _mode)
	{
		this.pages = _pages;
		this.mode = _mode;
		LayerInteraction.Mode mode = this.mode;
		if (mode != LayerInteraction.Mode.Map)
		{
			if (mode == LayerInteraction.Mode.EloMap)
			{
				LayerInteraction.Page page = new LayerInteraction.Page();
				this.pages = new List<LayerInteraction.Page>();
				this.pages.Add(page);
				page.Add(null, "test1", "", delegate()
				{
				}, 0, false);
				page.Add(null, "test2", "", delegate()
				{
				}, 0, false);
			}
		}
		else
		{
			this.point.Set(Scene.HitPoint);
		}
		foreach (LayerInteraction.Page page2 in this.pages)
		{
			ButtonGrid buttonGrid = this.menu.Add() as ButtonGrid;
			page2.button = buttonGrid;
			buttonGrid.SetObject(page2.items[0].target);
			LayerInteraction.Page _page = page2;
			buttonGrid.onClick.AddListener(delegate()
			{
				this.ShowPage(_page);
				SE.Click();
			});
			buttonGrid.RebuildLayout(true);
		}
		this.menu.Show();
		this.ShowPage(0);
	}

	public void ShowPage(int index)
	{
		this.ShowPage(this.pages[index]);
	}

	public void ShowPage(LayerInteraction.Page page)
	{
		this.menu.Clear();
		using (List<LayerInteraction.Item>.Enumerator enumerator = page.items.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				LayerInteraction.Item item = enumerator.Current;
				UIButton uibutton = this.menu.Add();
				uibutton.icon.sprite = (SpriteSheet.Get(item.idSprite.IsEmpty("icon_" + item.text)) ?? uibutton.icon.sprite);
				uibutton.mainText.SetText((item.textFunc != null) ? item.textFunc() : item.text.lang());
				uibutton.onClick.AddListener(delegate()
				{
					item.action();
					if (item.reload)
					{
						this.Reload();
						this.ShowPage(page);
					}
				});
				uibutton.RebuildLayout(true);
			}
		}
		Chara chara = LayerInteraction.Target as Chara;
		bool flag = chara != null && chara.IsHomeMember();
		this.windowChara.SetActive(flag);
		if (flag)
		{
			this.windowChara.SetChara(chara);
		}
		this.menu.Show();
	}

	public void Reload()
	{
	}

	public static LayerInteraction Instance;

	public static IInspect Target;

	public LayerInteraction.Mode mode;

	public Point point;

	public List<LayerInteraction.Page> pages = new List<LayerInteraction.Page>();

	private bool first = true;

	public InteractionMenu menu;

	public WindowChara windowChara;

	public enum Mode
	{
		Map,
		Inventory,
		EloMap,
		Area,
		Custom
	}

	public class Page
	{
		public LayerInteraction.Item Add(object target, string text, string idSprite, Action action, int priority = 0, bool auto = false)
		{
			LayerInteraction.Item item = new LayerInteraction.Item
			{
				target = target,
				text = text,
				idSprite = idSprite,
				action = action,
				priority = priority,
				auto = auto
			};
			this.items.Add(item);
			return item;
		}

		public LayerInteraction.Item Add(object target, string text, Func<bool> valueFunc, Action<bool> action)
		{
			Action action2 = delegate()
			{
				SE.Click();
				action(!valueFunc());
			};
			LayerInteraction.Item item = new LayerInteraction.Item
			{
				target = target,
				textFunc = (() => text.lang() + " (" + (valueFunc() ? "on" : "off") + ")"),
				action = action2,
				reload = true
			};
			this.items.Add(item);
			return item;
		}

		public UIButton button;

		public Area area;

		public List<LayerInteraction.Item> items = new List<LayerInteraction.Item>();
	}

	public class Item
	{
		public bool IsArea
		{
			get
			{
				return this.target is Area;
			}
		}

		public object target;

		public string text;

		public string idSprite;

		public Action action;

		public int priority;

		public bool auto;

		public bool reload;

		public Func<string> textFunc;
	}
}
