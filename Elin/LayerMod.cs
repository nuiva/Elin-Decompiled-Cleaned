using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class LayerMod : ELayer
{
	public ModManager manager
	{
		get
		{
			return ELayer.core.mods;
		}
	}

	public override void OnInit()
	{
		this.textRestart.SetActive(false);
		this.toggleDisableMods.SetToggle(ELayer.config.other.disableMods, delegate(bool on)
		{
			ELayer.config.other.disableMods = on;
			ELayer.config.Save();
			this.textRestart.SetActive(true);
		});
		LayerMod.Instance = this;
		BaseList baseList = this.list;
		BaseList baseList2 = this.list2;
		UIList.Callback<BaseModPackage, ItemMod> callback = new UIList.Callback<BaseModPackage, ItemMod>();
		callback.onClick = delegate(BaseModPackage a, ItemMod b)
		{
		};
		callback.onInstantiate = delegate(BaseModPackage a, ItemMod b)
		{
			a.UpdateMeta(true);
			b.package = a;
			string s = (ELayer.core.mods.packages.IndexOf(a) + 1).ToString() + ". " + (a.isInPackages ? "[Private] " : "") + a.title;
			b.buttonActivate.mainText.SetText(s, (!a.IsValidVersion()) ? FontColor.Bad : (a.activated ? FontColor.ButtonGeneral : FontColor.Passive));
			b.buttonActivate.subText.text = a.version;
			b.buttonLock.mainText.text = a.author;
			b.buttonUp.SetActive(!a.builtin);
			b.buttonDown.SetActive(!a.builtin);
			b.buttonToggle.SetToggle(a.willActivate, null);
			b.buttonUp.SetOnClick(delegate
			{
				this.<OnInit>g__Move|7_1(a, b, -1);
			});
			b.buttonDown.SetOnClick(delegate
			{
				this.<OnInit>g__Move|7_1(a, b, 1);
			});
			UIButton bt = b.buttonToggle;
			bt.SetOnClick(delegate
			{
				a.willActivate = !a.willActivate;
				bt.SetToggle(a.willActivate, null);
				ELayer.core.mods.SaveLoadOrder();
				this.textRestart.SetActive(true);
			});
			bt.interactable = !a.builtin;
			Action <>9__13;
			Action <>9__10;
			Action <>9__11;
			Action <>9__12;
			b.buttonActivate.onClick.AddListener(delegate()
			{
				this.Refresh();
				UIContextMenu uicontextMenu = ELayer.ui.CreateContextMenuInteraction();
				if (ELayer.debug.enable || (!BaseCore.IsOffline && a.isInPackages && !a.builtin))
				{
					UIContextMenu uicontextMenu2 = uicontextMenu;
					string idLang = "mod_publish";
					Action action;
					if ((action = <>9__10) == null)
					{
						action = (<>9__10 = delegate()
						{
							string langDetail = "mod_publish_warn".lang(a.title, a.id, a.author, null, null);
							Action actionYes;
							if ((actionYes = <>9__13) == null)
							{
								actionYes = (<>9__13 = delegate()
								{
									ELayer.core.steam.CreateUserContent(a);
								});
							}
							Dialog.YesNo(langDetail, actionYes, null, "yes", "no");
						});
					}
					uicontextMenu2.AddButton(idLang, action, true);
				}
				if (!a.builtin)
				{
					UIContextMenu uicontextMenu3 = uicontextMenu;
					string idLang2 = a.willActivate ? "mod_deactivate" : "mod_activate";
					Action action2;
					if ((action2 = <>9__11) == null)
					{
						action2 = (<>9__11 = delegate()
						{
							SE.Click();
							a.willActivate = !a.willActivate;
							ELayer.core.mods.SaveLoadOrder();
							this.list.List(false);
							this.textRestart.SetActive(true);
						});
					}
					uicontextMenu3.AddButton(idLang2, action2, true);
				}
				UIContextMenu uicontextMenu4 = uicontextMenu;
				string idLang3 = "mod_info";
				Action action3;
				if ((action3 = <>9__12) == null)
				{
					action3 = (<>9__12 = delegate()
					{
						SE.Click();
						Util.ShowExplorer(a.dirInfo.FullName + "/package.xml", false);
					});
				}
				uicontextMenu4.AddButton(idLang3, action3, true);
				uicontextMenu.Show();
			});
			b.buttonLock.onClick.AddListener(delegate()
			{
				this.Refresh();
			});
		};
		callback.onList = delegate(UIList.SortMode a)
		{
			foreach (BaseModPackage baseModPackage in this.manager.packages)
			{
				if (baseModPackage.builtin)
				{
					this.list2.Add(baseModPackage);
				}
				else
				{
					this.list.Add(baseModPackage);
				}
			}
		};
		callback.onRefresh = new Action(this.Refresh);
		UIList.ICallback callbacks = callback;
		baseList2.callbacks = callback;
		baseList.callbacks = callbacks;
		this.list.List(false);
		this.list2.List(false);
	}

	public void Refresh()
	{
	}

	public override void OnKill()
	{
		ELayer.core.mods.SaveLoadOrder();
	}

	[CompilerGenerated]
	private void <OnInit>g__Move|7_1(BaseModPackage p, ItemMod b, int a)
	{
		List<BaseModPackage> packages = ELayer.core.mods.packages;
		int num = packages.IndexOf(p);
		if (num + a < 0 || num + a >= packages.Count || packages[num + a].builtin)
		{
			SE.BeepSmall();
			return;
		}
		packages.Move(p, a);
		SE.Tab();
		this.textRestart.SetActive(true);
		ELayer.core.mods.SaveLoadOrder();
		this.list.List(false);
	}

	public static LayerMod Instance;

	public UIList list;

	public UIList list2;

	public UIText textRestart;

	public UIButton toggleDisableMods;
}
