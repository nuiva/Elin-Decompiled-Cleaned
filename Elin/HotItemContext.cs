using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class HotItemContext : HotItem
{
	public override string Name
	{
		get
		{
			return ("m_" + this.id).lang();
		}
	}

	public override string TextTip
	{
		get
		{
			return this.id.lang();
		}
	}

	public override string pathSprite
	{
		get
		{
			if (!(this.id == "system"))
			{
				return "icon_" + this.id;
			}
			return "icon_m_system";
		}
	}

	public override Selectable.Transition Transition
	{
		get
		{
			if (!this.AutoExpand)
			{
				return base.Transition;
			}
			return Selectable.Transition.None;
		}
	}

	public bool AutoExpand
	{
		get
		{
			return this.autoExpand;
		}
	}

	public override void OnHover(UIButton b)
	{
		if (!this.AutoExpand || EClass.ui.BlockInput)
		{
			return;
		}
		this.OnClick(null, null);
	}

	public override void OnClick(UIButton b)
	{
		if (EClass.ui.contextMenu.isActive)
		{
			return;
		}
		HotItemContext.Show(this.id, UIButton.buttonPos);
	}

	public override void OnShowContextMenu(UIContextMenu m)
	{
		m.AddToggle("autoExpand", this.autoExpand, delegate(bool on)
		{
			this.autoExpand = on;
		});
	}

	public static void Show(string id, Vector3 pos)
	{
		HotItemContext.<>c__DisplayClass15_0 CS$<>8__locals1 = new HotItemContext.<>c__DisplayClass15_0();
		string menuName = (id == "system") ? "ContextSystem" : "ContextMenu";
		CS$<>8__locals1.m = EClass.ui.contextMenu.Create(menuName, true);
		CS$<>8__locals1.d = EClass.core.game.world.date;
		CS$<>8__locals1.conf = EClass.game.config;
		CS$<>8__locals1.isRegion = EClass._zone.IsRegion;
		if (!(id == "mapTool"))
		{
			if (id == "system")
			{
				UIContextMenu uicontextMenu = CS$<>8__locals1.m.AddChild("etc");
				uicontextMenu.AddButton("LayerFeedback".lang() + "(" + EInput.keys.report.key.ToString() + ")", delegate()
				{
					EClass.ui.ToggleFeedback();
				}, true);
				uicontextMenu.AddButton("LayerConsole", delegate()
				{
					EClass.ui.AddLayer<LayerConsole>();
				}, true);
				uicontextMenu.AddButton("LayerCredit", delegate()
				{
					EClass.ui.AddLayer<LayerCredit>();
				}, true);
				uicontextMenu.AddButton("announce", delegate()
				{
					EClass.ui.AddLayer("LayerAnnounce");
				}, true);
				uicontextMenu.AddButton("about", delegate()
				{
					EClass.ui.AddLayer("LayerAbout");
				}, true);
				uicontextMenu.AddButton("hideUI", delegate()
				{
					SE.ClickGeneral();
					EClass.ui.canvas.enabled = !EClass.ui.canvas.enabled;
				}, true);
				UIContextMenu uicontextMenu2 = CS$<>8__locals1.m.AddChild("tool");
				uicontextMenu2.AddButton("LayerMod", delegate()
				{
					EClass.ui.AddLayer<LayerMod>();
				}, true);
				uicontextMenu2.AddButton("LayerTextureViewer", delegate()
				{
					EClass.ui.AddLayer<LayerTextureViewer>();
				}, true);
				CS$<>8__locals1.m.AddSeparator(0);
				CS$<>8__locals1.m.AddButton("help", delegate()
				{
					LayerHelp.Toggle("general", "1");
				}, true);
				CS$<>8__locals1.m.AddButton("widget", delegate()
				{
					EClass.ui.AddLayer<LayerWidget>();
				}, true);
				CS$<>8__locals1.m.AddButton("config", delegate()
				{
					EClass.ui.AddLayer<LayerConfig>();
				}, true);
				CS$<>8__locals1.m.AddSeparator(0);
				CS$<>8__locals1.m.AddButton("LayerHoard", delegate()
				{
					EClass.ui.AddLayer<LayerHoard>();
				}, true);
				CS$<>8__locals1.m.AddSeparator(0);
				if (EClass.game.Difficulty.allowManualSave || EClass.debug.enable)
				{
					CS$<>8__locals1.m.AddButton("save", delegate()
					{
						EClass.game.Save(false, null, false);
					}, true);
					CS$<>8__locals1.m.AddButton("load", delegate()
					{
						EClass.ui.AddLayer<LayerLoadGame>().Init(false, "", "");
					}, true);
				}
				CS$<>8__locals1.m.AddSeparator(0);
				CS$<>8__locals1.m.AddButton("title", delegate()
				{
					EClass.game.GotoTitle(true);
				}, true);
				CS$<>8__locals1.m.AddButton("quit", new Action(EClass.game.Quit), true);
				CS$<>8__locals1.m.GetComponent<Image>().SetAlpha(1f);
			}
		}
		else if (EClass.scene.actionMode.IsBuildMode)
		{
			if (EClass.debug.enable)
			{
				CS$<>8__locals1.m.AddButton("Reset Map", delegate()
				{
					Zone.forceRegenerate = true;
					EClass._zone.Activate();
				}, true);
				CS$<>8__locals1.m.AddChild("Map Subset");
				CS$<>8__locals1.m.AddSeparator(0);
				CS$<>8__locals1.<Show>g__AddSliderMonth|1();
				CS$<>8__locals1.<Show>g__AddSliderHour|2();
				CS$<>8__locals1.<Show>g__AddSliderWeather|3();
			}
		}
		else
		{
			if (!CS$<>8__locals1.isRegion && EClass.scene.flock.gameObject.activeSelf)
			{
				CS$<>8__locals1.m.AddButton("birdView", delegate()
				{
					EClass.scene.ToggleBirdView(true);
				}, true);
			}
			CS$<>8__locals1.<Show>g__AddTilt|4();
			CS$<>8__locals1.m.AddToggle("highlightArea", CS$<>8__locals1.conf.highlightArea, delegate(bool a)
			{
				EClass.scene.ToggleHighlightArea();
			});
			CS$<>8__locals1.m.AddToggle("noRoof", CS$<>8__locals1.conf.noRoof, delegate(bool a)
			{
				EClass.scene.ToggleRoof();
			});
			if (EClass._zone.IsRegion)
			{
				CS$<>8__locals1.m.AddSlider("zoomRegion", (float a) => (a * (float)CoreConfig.ZoomStep).ToString() + "%", (float)(CS$<>8__locals1.conf.regionZoom / CoreConfig.ZoomStep), delegate(float b)
				{
					CS$<>8__locals1.conf.regionZoom = (int)b * CoreConfig.ZoomStep;
				}, (float)(100 / CoreConfig.ZoomStep), (float)(200 / CoreConfig.ZoomStep), true, false, false);
			}
			else if (ActionMode.Adv.zoomOut2)
			{
				CS$<>8__locals1.m.AddSlider("zoomAlt", (float a) => (a * (float)CoreConfig.ZoomStep).ToString() + "%", (float)(CS$<>8__locals1.conf.zoomedZoom / CoreConfig.ZoomStep), delegate(float b)
				{
					CS$<>8__locals1.conf.zoomedZoom = (int)b * CoreConfig.ZoomStep;
				}, (float)(50 / CoreConfig.ZoomStep), (float)(200 / CoreConfig.ZoomStep), true, false, false);
			}
			else
			{
				CS$<>8__locals1.m.AddSlider("zoom", (float a) => (a * (float)CoreConfig.ZoomStep).ToString() + "%", (float)(CS$<>8__locals1.conf.defaultZoom / CoreConfig.ZoomStep), delegate(float b)
				{
					CS$<>8__locals1.conf.defaultZoom = (int)b * CoreConfig.ZoomStep;
				}, (float)(50 / CoreConfig.ZoomStep), (float)(200 / CoreConfig.ZoomStep), true, false, false);
			}
			CS$<>8__locals1.m.AddSlider("backDrawAlpha", (float a) => a.ToString() + "%", (float)EClass.game.config.backDrawAlpha, delegate(float b)
			{
				EClass.game.config.backDrawAlpha = (int)b;
			}, 0f, 50f, true, false, false);
			if (EClass.debug.enable)
			{
				CS$<>8__locals1.m.AddSeparator(0);
				CS$<>8__locals1.<Show>g__AddSliderMonth|1();
				CS$<>8__locals1.m.AddSlider("sliderDay", (float a) => a.ToString() ?? "", (float)EClass.world.date.day, delegate(float b)
				{
					if ((int)b != EClass.world.date.day)
					{
						EClass.world.date.day = (int)b - 1;
						EClass.world.date.AdvanceDay();
					}
					EClass._map.RefreshAllTiles();
					EClass.screen.RefreshAll();
				}, 1f, 30f, true, false, false);
				CS$<>8__locals1.<Show>g__AddSliderHour|2();
				CS$<>8__locals1.<Show>g__AddSliderWeather|3();
				CS$<>8__locals1.m.AddSlider("sliderAnimeSpeed", (float a) => EClass.game.config.animeSpeed.ToString() + "%", (float)EClass.game.config.animeSpeed, delegate(float b)
				{
					EClass.game.config.animeSpeed = (int)b;
				}, 0f, 100f, true, false, false);
				UIContextMenu uicontextMenu3 = CS$<>8__locals1.m.AddChild("debug");
				uicontextMenu3.AddToggle("reveal_map", EClass.debug.revealMap, delegate(bool a)
				{
					EClass.debug.ToggleRevealMap();
				});
				uicontextMenu3.AddToggle("test_los", EClass.debug.testLOS, delegate(bool a)
				{
					HotItemContext.<Show>g__Toggle|15_0(ref EClass.debug.testLOS);
				});
				uicontextMenu3.AddToggle("test_los2", EClass.debug.testLOS2, delegate(bool a)
				{
					HotItemContext.<Show>g__Toggle|15_0(ref EClass.debug.testLOS2);
				});
				uicontextMenu3.AddToggle("godBuild", EClass.debug.godBuild, delegate(bool a)
				{
					HotItemContext.<Show>g__Toggle|15_0(ref EClass.debug._godBuild);
				});
				uicontextMenu3.AddToggle("godMode", EClass.debug.godMode, delegate(bool a)
				{
					HotItemContext.<Show>g__Toggle|15_0(ref EClass.debug.godMode);
				});
				uicontextMenu3.AddToggle("autoAdvanceQuest", EClass.debug.autoAdvanceQuest, delegate(bool a)
				{
					HotItemContext.<Show>g__Toggle|15_0(ref EClass.debug.autoAdvanceQuest);
				});
				uicontextMenu3.AddSlider("slopeMod", (float a) => a.ToString() ?? "", (float)CS$<>8__locals1.conf.slopeMod, delegate(float b)
				{
					EClass.game.config.slopeMod = (int)b;
					(EClass.pc.renderer as CharaRenderer).first = true;
				}, 0f, 500f, true, false, false);
			}
		}
		CS$<>8__locals1.m.Show(pos);
	}

	[CompilerGenerated]
	internal static void <Show>g__Toggle|15_0(ref bool flag)
	{
		flag = !flag;
		WidgetMenuPanel.OnChangeMode();
		EClass.player.hotbars.ResetHotbar(2);
		SE.ClickGeneral();
	}

	[JsonProperty]
	public string id;

	[JsonProperty]
	public bool autoExpand;
}
