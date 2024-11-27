using System;
using System.Collections.Generic;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class WidgetSceneCustomizer : Widget
{
	public override void OnActivate()
	{
		MapConfig conf = EMono._map.config;
		this.ddLiquid.SetList<LiquidProfile>(conf.idLiquid, this.liquids, (LiquidProfile a, int b) => a.name, delegate(int a, LiquidProfile b)
		{
			if (conf.idLiquid != b.name)
			{
				conf.idLiquid = b.name;
				conf.colorLiquid = null;
				EMono.scene.ApplyZoneConfig();
			}
			this.RefreshColor();
		}, true);
		this.ddRefraction.SetList<RefractionProfile>(conf.idRefraction, this.refractions, (RefractionProfile a, int b) => a.name, delegate(int a, RefractionProfile b)
		{
			if (conf.idRefraction != b.name)
			{
				conf.idRefraction = b.name;
				EMono.scene.ApplyZoneConfig();
			}
		}, true);
		this.ddLut.SetList<string>(conf.idLut, this.luts, (string a, int b) => a, delegate(int a, string b)
		{
			if (conf.idLut != b)
			{
				conf.idLut = b;
				EMono.scene.ApplyZoneConfig();
			}
		}, true);
		this.sliderLutBlend.SetSlider(conf.lutBlend, delegate(float a)
		{
			conf.lutBlend = a;
			EMono.scene.ApplyZoneConfig();
			return Lang.Get("lutBlend") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, -1, -1, true);
		this.sliderLutBrightness.SetSlider(conf.lutBrightness, delegate(float a)
		{
			conf.lutBrightness = a;
			EMono.scene.ApplyZoneConfig();
			return Lang.Get("lutBrightness") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, -1, -1, true);
		this.sliderLutContrast.SetSlider(conf.lutContrast, delegate(float a)
		{
			conf.lutContrast = a;
			EMono.scene.ApplyZoneConfig();
			return Lang.Get("lutContrast") + "(" + ((int)(a * 100f)).ToString() + "%)";
		}, -1, -1, true);
		List<Texture2D> list = new List<Texture2D>(Resources.LoadAll<Texture2D>("Scene/Profile/Lut/"));
		list.Insert(0, null);
		this.sliderLut.SetList<Texture2D>(list.Find(delegate(Texture2D a)
		{
			if (!(a == null))
			{
				return conf.idLut == a.name;
			}
			return conf.idLut == "None";
		}), list, delegate(int a, Texture2D b)
		{
			conf.idLut = ((b == null) ? "None" : b.name);
			EMono.scene.ApplyZoneConfig();
		}, delegate(Texture2D a)
		{
			if (!(a == null))
			{
				return a.name;
			}
			return "None";
		});
		this.RefreshColor();
	}

	public void ShowPicker()
	{
		MapConfig conf = EMono._map.config;
		LiquidProfile prof = LiquidProfile.Get(conf.idLiquid);
		LayerColorPicker layerColorPicker = EMono.ui.AddLayer<LayerColorPicker>();
		SerializableColor colorLiquid = conf.colorLiquid;
		layerColorPicker.SetColor((colorLiquid != null) ? colorLiquid.Get() : prof.modColor, prof.modColor, delegate(PickerState state, Color c)
		{
			prof.Apply(new Color?(c));
			if (state == PickerState.Confirm)
			{
				conf.colorLiquid = new SerializableColor(c);
				this.RefreshColor();
				return;
			}
			if (state == PickerState.Cancel)
			{
				EMono.scene.ApplyZoneConfig();
			}
		});
	}

	public void RefreshColor()
	{
		LiquidProfile liquidProfile = LiquidProfile.Get(EMono._map.config.idLiquid);
		SerializableColor colorLiquid = EMono._map.config.colorLiquid;
		Color color = (colorLiquid != null) ? colorLiquid.Get() : liquidProfile.modColor;
		color.a = 1f;
		this.buttonColorPicker.icon.color = color;
	}

	public void OnClickSave()
	{
		EMono.core.WaitForEndOfFrame(delegate
		{
			string text = StandaloneFileBrowser.SaveFilePanel("Save Zone Config", CorePath.SceneCustomizerSave, "new zone config", "json");
			if (!string.IsNullOrEmpty(text))
			{
				IO.SaveFile(text, EMono._map.config, false, null);
			}
		});
	}

	public void OnClickLoad()
	{
		EMono.core.WaitForEndOfFrame(delegate
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Load Zone Config", CorePath.SceneCustomizerSave, "json", false);
			if (array.Length != 0)
			{
				EMono._map.config = IO.LoadFile<MapConfig>(array[0], false, null);
				base.Reactivate();
			}
		});
	}

	public void OnClickReset()
	{
		EMono._map.config = new MapConfig();
		base.Reactivate();
	}

	public UIDropdown ddLiquid;

	public UIDropdown ddRefraction;

	public UIDropdown ddOverlay;

	public UIDropdown ddLut;

	public List<LiquidProfile> liquids;

	public List<RefractionProfile> refractions;

	public List<OverlayProfile> overlays;

	public List<string> luts;

	public UIButton toggleGradientWater;

	public UIButton buttonColorPicker;

	public Slider sliderLutBlend;

	public Slider sliderLutBrightness;

	public Slider sliderLutContrast;

	public UISlider sliderLut;
}
