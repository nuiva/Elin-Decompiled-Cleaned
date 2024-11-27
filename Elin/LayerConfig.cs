using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LayerConfig : ELayer
{
	public override string GetTextHeader(Window window)
	{
		return "_configOf".lang(base.GetTextHeader(window), null, null, null, null);
	}

	public override void OnBeforeAddLayer()
	{
		this.windows[0].setting.tabs[5].disable = !ELayer.core.config.other.showTestOptions;
	}

	public override void OnInit()
	{
		this.inputW.text = (ELayer.config.graphic.w.ToString() ?? "");
		this.inputH.text = (ELayer.config.graphic.h.ToString() ?? "");
		ELayer.config.graphic.fullScreen = Screen.fullScreen;
		this.windows[0].AddBottomSpace(20);
		this.buttonReset = this.windows[0].AddBottomButton("", delegate
		{
			Dialog.YesNo("dialogResetConfigTab".lang(this.windows[0].CurrentTab.idLang.lang(), null, null, null, null), delegate
			{
				this.Close();
				if (this.windows[0].idTab == 0)
				{
					CoreConfig.ResetGeneral();
				}
				else if (this.windows[0].idTab == 1)
				{
					CoreConfig.ResetGraphics();
				}
				else if (this.windows[0].idTab == 2)
				{
					CoreConfig.ResetGame();
				}
				else if (this.windows[0].idTab == 3)
				{
					CoreConfig.ResetInput();
				}
				else if (this.windows[0].idTab == 4)
				{
					CoreConfig.ResetOther();
				}
				else
				{
					CoreConfig.ResetTest();
				}
				ELayer.ui.AddLayer<LayerConfig>();
			}, null, "yes", "no");
		}, false);
		this.windows[0].AddBottomButton("resetConfig", delegate
		{
			Dialog.YesNo("dialogResetConfig", delegate
			{
				this.Close();
				CoreConfig.Reset();
				ELayer.ui.AddLayer<LayerConfig>();
			}, null, "yes", "no");
		}, false);
		List<PostEffectProfile> list = (from p in Resources.LoadAll<PostEffectProfile>("Scene/Profile/PostEffect/")
		where !p.disable
		select p).ToList<PostEffectProfile>();
		this.ddPostEffect.SetList<PostEffectProfile>(ELayer.config.graphic.idPostProfile, list, (PostEffectProfile a, int b) => a.name, delegate(int a, PostEffectProfile b)
		{
			ELayer.config.graphic.idPostProfile = b.name;
			b.OnChangeProfile();
			this.sliderSharpen.value = (float)ELayer.config.graphic.sharpen;
			this.sliderSharpen2.value = (float)ELayer.config.graphic.sharpen2;
			this.sliderBlur.value = (float)ELayer.config.graphic.blur;
			this.toggleKuwahara.SetCheck(ELayer.config.graphic.kuwahara);
			ELayer.config.ApplyGrading();
		}, false);
		this.Refresh();
	}

	public override void OnSwitchContent(Window window)
	{
		this.buttonReset.mainText.SetText("resetConfigTab".lang(this.windows[0].CurrentTab.idLang.lang(), null, null, null, null));
		this.windows[0].rectBottom.RebuildLayout(true);
		this.windows[0].CurrentContent.RebuildLayout(true);
	}

	private void Update()
	{
		if (!this.inputW.isFocused && !this.inputH.isFocused)
		{
			this.ValidateResolution();
		}
	}

	public void ValidateResolution()
	{
		int num = Mathf.Clamp(this.inputW.text.ToInt(), 800, Display.main.systemWidth);
		int num2 = Mathf.Clamp(this.inputH.text.ToInt(), 600, Display.main.systemHeight);
		this.inputW.text = (num.ToString() ?? "");
		this.inputH.text = (num2.ToString() ?? "");
		ELayer.core.config.graphic.w = this.inputW.text.ToInt();
		ELayer.core.config.graphic.h = this.inputH.text.ToInt();
	}

	public override void OnKill()
	{
		ELayer.core.config.graphic.w = this.inputW.text.ToInt();
		ELayer.core.config.graphic.h = this.inputH.text.ToInt();
		ELayer.core.config.Save();
		ELayer.core.config.Apply();
	}

	public void Refresh()
	{
		ELayer.config.ignoreApply = true;
		this.SetSlider(this.sliderMaster, ELayer.config.sound.volumeMaster, delegate(float a)
		{
			ELayer.config.sound.volumeMaster = a;
			ELayer.config.ApplyVolume();
			return Lang.Get("MASTER") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderBGM, ELayer.config.sound.volumeBGM, delegate(float a)
		{
			ELayer.config.sound.volumeBGM = a;
			ELayer.config.ApplyVolume();
			return Lang.Get("BGM") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderSFX, ELayer.config.sound.volumeSFX, delegate(float a)
		{
			ELayer.config.sound.volumeSFX = a;
			ELayer.config.ApplyVolume();
			return Lang.Get("SFX") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderAMB, ELayer.config.sound.volumeAMB, delegate(float a)
		{
			ELayer.config.sound.volumeAMB = a;
			ELayer.config.ApplyVolume();
			return Lang.Get("AMB") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderBrightness, ELayer.config.graphic.brightness, delegate(float a)
		{
			ELayer.config.graphic.brightness = a;
			ELayer.config.ApplyGrading();
			return Lang.Get("brightness") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderContrast, ELayer.config.graphic.contrast, delegate(float a)
		{
			ELayer.config.graphic.contrast = a;
			ELayer.config.ApplyGrading();
			return Lang.Get("contrast") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderSaturation, ELayer.config.graphic.saturation, delegate(float a)
		{
			ELayer.config.graphic.saturation = a;
			ELayer.config.ApplyGrading();
			return Lang.Get("saturation") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderVibrance, ELayer.config.graphic.vibrance, delegate(float a)
		{
			ELayer.config.graphic.vibrance = a;
			ELayer.config.ApplyGrading();
			return Lang.Get("vibrance") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderSharpen, (float)ELayer.config.graphic.sharpen, delegate(float a)
		{
			ELayer.config.graphic.sharpen = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("sharpen") + "(" + ((int)a).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderSharpen2, (float)ELayer.config.graphic.sharpen2, delegate(float a)
		{
			ELayer.config.graphic.sharpen2 = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("sharpen2") + "(" + ((int)a).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderBlur, (float)ELayer.config.graphic.blur, delegate(float a)
		{
			ELayer.config.graphic.blur = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("blur") + "(" + ((int)a).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderScrollSense, ELayer.config.ui.scrollSens, delegate(float a)
		{
			ELayer.config.ui.scrollSens = a;
			UIScrollView.sensitivity = ELayer.config.ui.ScrollSensitivity;
			return Lang.Get("scrollSens") + "(" + ((int)(a * 100f)).ToString() + ")";
		}, false);
		this.SetSlider(this.sliderScale, (float)ELayer.config.ui.scale, delegate(float a)
		{
			ELayer.config.ui.scale = (int)a;
			ELayer.config.ApplyScale();
			ELayer.core.OnChangeResolution();
			return Lang.Get("uiScale") + "(" + (a * 5f).ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderUIBrightness, (float)ELayer.config.ui.brightness, delegate(float a)
		{
			ELayer.config.ui.brightness = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("uiBrightness") + "(" + a.ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderUIContrast, (float)ELayer.config.ui.contrast, delegate(float a)
		{
			ELayer.config.ui.contrast = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("uiContrast") + "(" + a.ToString() + "%)";
		}, false);
		this.SetSlider(this.sliderBalloon, (float)ELayer.config.ui.outlineAlpha, delegate(float a)
		{
			ELayer.config.ui.outlineAlpha = (int)a;
			ELayer.config.Apply();
			return Lang.Get("outlineAlpha") + "(" + a.ToString() + "%)";
		}, false);
		this.SetGroup(this.groupVSync, ELayer.config.graphic.fps, delegate(int a)
		{
			ELayer.config.graphic.fps = a;
			ELayer.config.Apply();
		});
		if (!this.moldLanguage)
		{
			this.moldLanguage = this.layoutLanguage.CreateMold(null);
		}
		this.layoutLanguage.DestroyChildren(false, true);
		int value = 0;
		List<LangSetting> list = new List<LangSetting>();
		foreach (LangSetting langSetting in MOD.langs.Values)
		{
			Util.Instantiate<UIButton>(this.moldLanguage, this.layoutLanguage).mainText.text = langSetting.name + "(" + langSetting.name_en + ")";
			if (ELayer.config.lang == langSetting.id)
			{
				value = list.Count;
			}
			list.Add(langSetting);
		}
		this.layoutLanguage.RebuildLayout(false);
		this.SetGroup(this.groupLang, value, delegate(int a)
		{
			if (ELayer.config.lang != list[a].id)
			{
				ELayer.config.lang = list[a].id;
				ELayer.core.SetLang(ELayer.config.lang, false);
				this.Close();
				IChangeLanguage[] componentsInChildren = ELayer.ui.GetComponentsInChildren<IChangeLanguage>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnChangeLanguage();
				}
				ELayer.ui.AddLayer<LayerConfig>();
				Dialog.Ok("dialogChangeLang", null);
				ELayer.config.Save();
			}
		});
		this.toggleGradientWater.SetToggle(ELayer.config.graphic.gradientWater, delegate(bool on)
		{
			ELayer.config.graphic.gradientWater = on;
			ELayer.scene.ApplyZoneConfig();
		});
		this.toggleVsync.SetToggle(ELayer.config.graphic.vsync, delegate(bool on)
		{
			ELayer.config.graphic.vsync = on;
			ELayer.config.Apply();
		});
		this.toggleAutoscale.SetToggle(ELayer.config.ui.autoscale, delegate(bool on)
		{
			ELayer.config.ui.autoscale = on;
			ELayer.core.OnChangeResolution();
			ELayer.config.ApplyScale();
		});
		this.toggleOpenLastTab.SetToggle(ELayer.config.ui.openLastTab, delegate(bool on)
		{
			ELayer.config.ui.openLastTab = on;
			ELayer.config.Apply();
		});
		this.toggleBalloon.SetToggle(ELayer.config.ui.balloonBG, delegate(bool on)
		{
			ELayer.config.ui.balloonBG = on;
			ELayer.config.Apply();
		});
		this.toggleRightClickClose.SetToggle(ELayer.config.ui.rightClickClose, delegate(bool on)
		{
			ELayer.config.ui.rightClickClose = on;
			ELayer.config.Apply();
		});
		this.toggleFullscreen.SetToggle(ELayer.config.graphic.fullScreen, delegate(bool on)
		{
			ELayer.config.graphic.fullScreen = on;
			ELayer.config.Apply();
		});
		this.togglePixelPerfectUI.SetToggle(ELayer.config.graphic.pixelperfectUI, delegate(bool on)
		{
			ELayer.config.graphic.pixelperfectUI = on;
			ELayer.config.Apply();
		});
		this.toggleFixedResolution.SetToggle(ELayer.config.graphic.fixedResolution, delegate(bool on)
		{
			ELayer.config.graphic.fixedResolution = on;
			ELayer.config.Apply();
		});
		this.toggleBloom.SetToggle(ELayer.config.graphic.bloom, delegate(bool on)
		{
			ELayer.config.graphic.bloom = on;
			ELayer.config.Apply();
		});
		this.toggleHDR.SetToggle(ELayer.config.graphic.hdr, delegate(bool on)
		{
			ELayer.config.graphic.hdr = on;
			ELayer.config.Apply();
		});
		this.toggleKuwahara.SetToggle(ELayer.config.graphic.kuwahara, delegate(bool on)
		{
			ELayer.config.graphic.kuwahara = on;
			ELayer.config.ApplyGrading();
		});
		this.toggleDisableShake.SetToggle(ELayer.config.graphic.disableShake, delegate(bool on)
		{
			ELayer.config.graphic.disableShake = on;
		});
		this.toggleFirefly.SetToggle(ELayer.config.graphic.firefly, delegate(bool on)
		{
			ELayer.config.graphic.firefly = on;
			ELayer.screen.RefreshSky();
		});
		this.toggleGodray.SetToggle(ELayer.config.graphic.godray, delegate(bool on)
		{
			ELayer.config.graphic.godray = on;
			ELayer.screen.RefreshSky();
		});
		this.toggleBlizzard.SetToggle(ELayer.config.graphic.blizzard, delegate(bool on)
		{
			ELayer.config.graphic.blizzard = on;
			ELayer.screen.RefreshSky();
		});
		this.toggleAllyLight.SetToggle(ELayer.config.graphic.drawAllyLight, delegate(bool on)
		{
			ELayer.config.graphic.drawAllyLight = on;
			if (ELayer.core.IsGameStarted)
			{
				ELayer._map.RefreshFOVAll();
			}
		});
		this.toggleBlur.SetToggle(ELayer.config.ui.blur, delegate(bool on)
		{
			ELayer.config.ui.blur = on;
		});
		this.toggleDynamicBrightness.SetToggle(ELayer.config.ui.dynamicBrightness, delegate(bool on)
		{
			ELayer.config.ui.dynamicBrightness = on;
			ELayer.config.RefreshUIBrightness();
		});
		this.toggleSecureWidth.SetToggle(ELayer.config.ui.secureMinWidth, delegate(bool on)
		{
			ELayer.config.ui.secureMinWidth = on;
		});
		this.toggleShowFloatButtons.SetToggle(ELayer.config.ui.showFloatButtons, delegate(bool on)
		{
			ELayer.config.ui.showFloatButtons = on;
		});
		this.buttonApplyScreenSize.SetOnClick(delegate
		{
			this.ValidateResolution();
			ELayer.config.ApplyResolution(true);
		});
		List<string> list3 = new List<string>();
		list3.Add("none");
		list3.Add("focus");
		list3.Add("focusPause");
		List<string> list2 = new List<string>();
		list2.Add("ani0");
		list2.Add("ani1");
		list2.Add("ani2");
		list2.Add("ani3");
		list2.Add("ani4");
		this.SetFont(ELayer.config.font.fontUI, this.fontUI, "fontUI");
		this.SetFont(ELayer.config.font.fontChatbox, this.fontChatbox, "fontChatbox");
		this.SetFont(ELayer.config.font.fontBalloon, this.fontBalloon, "fontBalloon");
		this.SetFont(ELayer.config.font.fontDialog, this.fontDialog, "fontDialog");
		this.SetFont(ELayer.config.font.fontWidget, this.fontWidget, "widget");
		this.SetFont(ELayer.config.font.fontNews, this.fontNews, "fontNews");
		ELayer.config.ignoreApply = false;
	}

	public void SetFont(SkinManager.FontSaveData data, UIItem item, string lang)
	{
		item.text1.SetLang(lang);
		UIDropdown componentInChildren = item.GetComponentInChildren<UIDropdown>();
		UIButtonLR uibuttonLR = item.button1 as UIButtonLR;
		List<string> langs = new List<string>
		{
			"sizeS",
			"sizeDefault",
			"sizeL",
			"sizeLL",
			"sizeLLL",
			"sizeLLLL"
		};
		SkinManager skins = ELayer.ui.skins;
		uibuttonLR.SetOptions(data.size, langs, delegate(int i)
		{
			data.size = i;
			this.ApplyFont();
		}, false, null);
		componentInChildren.options.Clear();
		for (int j = 0; j < skins.FontList.Count; j++)
		{
			FontSource fontSource = skins.FontList[j];
			componentInChildren.options.Add(new Dropdown.OptionData
			{
				text = fontSource._name
			});
			if (j == data.index)
			{
				componentInChildren.value = j;
			}
		}
		componentInChildren.onValueChanged.RemoveAllListeners();
		componentInChildren.onValueChanged.AddListener(delegate(int i)
		{
			data.index = i;
			this.ApplyFont();
		});
	}

	public void ApplyFont()
	{
		ELayer.config.ApplyFont();
		this.RebuildLayout(true);
	}

	public void SetSlider(Slider slider, float value, Func<float, string> action, bool notify = false)
	{
		slider.onValueChanged.RemoveAllListeners();
		slider.onValueChanged.AddListener(delegate(float a)
		{
			slider.GetComponentInChildren<UIText>(true).text = action(a);
		});
		if (notify)
		{
			slider.value = value;
		}
		else
		{
			slider.SetValueWithoutNotify(value);
		}
		slider.GetComponentInChildren<UIText>(true).text = action(value);
	}

	public void SetGroup(UISelectableGroup group, int value, UnityAction<int> action)
	{
		group.Init(value, action, false);
	}

	public Slider sliderMaster;

	public Slider sliderBGM;

	public Slider sliderSFX;

	public Slider sliderAMB;

	public Slider sliderBrightness;

	public Slider sliderContrast;

	public Slider sliderSaturation;

	public Slider sliderVibrance;

	public Slider sliderScrollSense;

	public Slider sliderScale;

	public Slider sliderSharpen;

	public Slider sliderSharpen2;

	public Slider sliderBlur;

	public UISelectableGroup groupVSync;

	public UISelectableGroup groupLang;

	public UIButton toggleVsync;

	public UIButton toggleFixedResolution;

	public UIButton toggleOpenLastTab;

	public UIButton toggleRightClickClose;

	public UIButton toggleFullscreen;

	public UIButton togglePixelPerfectUI;

	public UIButton toggleBloom;

	public UIButton toggleHDR;

	public UIButton toggleBlur;

	public UIButton toggleAutoscale;

	public UIButton toggleDynamicBrightness;

	public UIButton toggleKuwahara;

	public UIButton toggleBlizzard;

	public UIButton toggleDisableShake;

	public UIButton toggleFirefly;

	public UIButton toggleSecureWidth;

	public UIButton toggleGradientWater;

	public UIButton toggleGodray;

	public UIButton toggleShowFloatButtons;

	public UIButton toggleAllyLight;

	public UIButton toggleBalloon;

	public UIItem fontUI;

	public UIItem fontChatbox;

	public UIItem fontBalloon;

	public UIItem fontDialog;

	public UIItem fontWidget;

	public UIItem fontNews;

	public UIDropdown ddPostEffect;

	public UIButton buttonReset;

	public Slider sliderUIBrightness;

	public Slider sliderUIContrast;

	public Slider sliderBalloon;

	public LayoutGroup layoutLanguage;

	public UIButton moldLanguage;

	public UIButton buttonApplyScreenSize;

	public InputField inputW;

	public InputField inputH;
}
