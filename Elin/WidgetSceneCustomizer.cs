using System.Collections.Generic;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class WidgetSceneCustomizer : Widget
{
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

	public override void OnActivate()
	{
		MapConfig conf = EMono._map.config;
		ddLiquid.SetList(conf.idLiquid, liquids, (LiquidProfile a, int b) => a.name, delegate(int a, LiquidProfile b)
		{
			if (conf.idLiquid != b.name)
			{
				conf.idLiquid = b.name;
				conf.colorLiquid = null;
				EMono.scene.ApplyZoneConfig();
			}
			RefreshColor();
		});
		ddRefraction.SetList(conf.idRefraction, refractions, (RefractionProfile a, int b) => a.name, delegate(int a, RefractionProfile b)
		{
			if (conf.idRefraction != b.name)
			{
				conf.idRefraction = b.name;
				EMono.scene.ApplyZoneConfig();
			}
		});
		ddLut.SetList(conf.idLut, luts, (string a, int b) => a, delegate(int a, string b)
		{
			if (conf.idLut != b)
			{
				conf.idLut = b;
				EMono.scene.ApplyZoneConfig();
			}
		});
		sliderLutBlend.SetSlider(conf.lutBlend, delegate(float a)
		{
			conf.lutBlend = a;
			EMono.scene.ApplyZoneConfig();
			return Lang.Get("lutBlend") + "(" + (int)(a * 100f) + "%)";
		});
		sliderLutBrightness.SetSlider(conf.lutBrightness, delegate(float a)
		{
			conf.lutBrightness = a;
			EMono.scene.ApplyZoneConfig();
			return Lang.Get("lutBrightness") + "(" + (int)(a * 100f) + "%)";
		});
		sliderLutContrast.SetSlider(conf.lutContrast, delegate(float a)
		{
			conf.lutContrast = a;
			EMono.scene.ApplyZoneConfig();
			return Lang.Get("lutContrast") + "(" + (int)(a * 100f) + "%)";
		});
		List<Texture2D> list = new List<Texture2D>(Resources.LoadAll<Texture2D>("Scene/Profile/Lut/"));
		list.Insert(0, null);
		sliderLut.SetList(list.Find((Texture2D a) => (!(a == null)) ? (conf.idLut == a.name) : (conf.idLut == "None")), list, delegate(int a, Texture2D b)
		{
			conf.idLut = ((b == null) ? "None" : b.name);
			EMono.scene.ApplyZoneConfig();
		}, (Texture2D a) => (!(a == null)) ? a.name : "None");
		RefreshColor();
	}

	public void ShowPicker()
	{
		MapConfig conf = EMono._map.config;
		LiquidProfile prof = LiquidProfile.Get(conf.idLiquid);
		EMono.ui.AddLayer<LayerColorPicker>().SetColor(conf.colorLiquid?.Get() ?? prof.modColor, prof.modColor, delegate(PickerState state, Color c)
		{
			prof.Apply(c);
			switch (state)
			{
			case PickerState.Confirm:
				conf.colorLiquid = new SerializableColor(c);
				RefreshColor();
				break;
			case PickerState.Cancel:
				EMono.scene.ApplyZoneConfig();
				break;
			}
		});
	}

	public void RefreshColor()
	{
		LiquidProfile liquidProfile = LiquidProfile.Get(EMono._map.config.idLiquid);
		Color color = EMono._map.config.colorLiquid?.Get() ?? liquidProfile.modColor;
		color.a = 1f;
		buttonColorPicker.icon.color = color;
	}

	public void OnClickSave()
	{
		EMono.core.WaitForEndOfFrame(delegate
		{
			string text = StandaloneFileBrowser.SaveFilePanel("Save Zone Config", CorePath.SceneCustomizerSave, "new zone config", "json");
			if (!string.IsNullOrEmpty(text))
			{
				IO.SaveFile(text, EMono._map.config);
			}
		});
	}

	public void OnClickLoad()
	{
		EMono.core.WaitForEndOfFrame(delegate
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Load Zone Config", CorePath.SceneCustomizerSave, "json", multiselect: false);
			if (array.Length != 0)
			{
				EMono._map.config = IO.LoadFile<MapConfig>(array[0]);
				Reactivate();
			}
		});
	}

	public void OnClickReset()
	{
		EMono._map.config = new MapConfig();
		Reactivate();
	}
}
