using System;
using UnityEngine.UI;

public class ContentConfigOther : ContentConfig
{
	public override void OnInstantiate()
	{
		this.ddSnap.SetList<CoreConfig.ScreenSnapType>((int)base.config.fix.snapType, Util.EnumToList<CoreConfig.ScreenSnapType>(), (CoreConfig.ScreenSnapType a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.ScreenSnapType b)
		{
			base.config.fix.snapType = b;
		}, true);
		this.ddDivider.SetList<CameraSupport.Divider>((int)base.config.fix.divider, Util.EnumToList<CameraSupport.Divider>(), (CameraSupport.Divider a, int b) => a.ToString().lang(), delegate(int a, CameraSupport.Divider b)
		{
			base.config.fix.divider = b;
			EClass.core.screen.RefreshScreenSize();
		}, true);
		base.SetSlider(this.sliderMissClick, (float)base.config.other.antiMissClick, delegate(float a)
		{
			base.config.other.antiMissClick = (int)a;
			EInput.antiMissClick = 0.1f * a;
			return Lang.Get("antiMissClick") + "(" + (0.1f * a).ToString() + "s)";
		});
		base.SetSlider(this.sliderBGMInterval, base.config.other.bgmInterval, delegate(float a)
		{
			base.config.other.bgmInterval = (float)((int)a);
			base.config.ApplyVolume();
			return Lang.Get("bgmInterval") + "(" + (a * 5f).ToString() + "sec)";
		});
		this.toggleSyncMod.SetToggle(base.config.other.syncMods, delegate(bool on)
		{
			base.config.other.syncMods = on;
		});
		this.toggleDisableMods.SetToggle(base.config.other.disableMods, delegate(bool on)
		{
			base.config.other.disableMods = on;
		});
		this.toggleNoCensor.SetToggle(base.config.other.noCensor, delegate(bool on)
		{
			base.config.other.noCensor = on;
			base.config.Apply();
		});
		this.toggleRunBackground.SetToggle(base.config.other.runBackground, delegate(bool on)
		{
			base.config.other.runBackground = on;
			base.config.Apply();
		});
		this.toggleMuteBackground.SetToggle(base.config.other.muteBackground, delegate(bool on)
		{
			base.config.other.muteBackground = on;
			base.config.Apply();
		});
		this.toggleAltInv.SetToggle(base.config.game.altInv, delegate(bool on)
		{
			base.config.game.altInv = on;
			EClass.ui.layerFloat.RemoveLayers(true);
		});
		this.toggleAltAbility.SetToggle(base.config.game.altAbility, delegate(bool on)
		{
			base.config.game.altAbility = on;
			EClass.ui.layerFloat.RemoveLayers(true);
		});
		this.toggleTest.SetToggle(base.config.other.showTestOptions, delegate(bool on)
		{
			base.config.other.showTestOptions = on;
			EClass.ui.GetLayer<LayerConfig>(false).Close();
			EClass.ui.AddLayer<LayerConfig>();
		});
		this.RefreshRewardCode();
	}

	public void OpenLog()
	{
		Util.ShowExplorer(CorePath.RootSave, false);
	}

	public void OoenConfigFolder()
	{
		Util.ShowExplorer(CorePath.RootSave + "config.txt", false);
	}

	public void OpenPackage()
	{
		Util.ShowExplorer(CorePath.rootMod + "_Elona", false);
	}

	public void OpenUser()
	{
		Util.ShowExplorer(CorePath.user + "PCC", false);
	}

	public void ResetWindows()
	{
		Dialog.YesNo("dialogResetWindow", delegate
		{
			Window.dictData.Clear();
		}, null, "yes", "no");
	}

	public void OpenBackerCodeInput()
	{
		Dialog.InputName("dialogBackerCode", "", delegate(bool cancel, string text)
		{
			if (!cancel)
			{
				if (ElinEncoder.IsValid(text))
				{
					base.config.rewardCode = text;
					SE.Change();
				}
				else
				{
					Dialog.Ok("invalidRewardCode".lang(text, null, null, null, null), null);
				}
				this.RefreshRewardCode();
			}
		}, Dialog.InputType.Default);
	}

	public void RefreshRewardCode()
	{
		this.buttonBackerCode.interactable = (!base.config.HasBackerRewardCode() || EClass.debug.enable);
		if (base.config.HasBackerRewardCode())
		{
			this.buttonBackerCode.mainText.SetText("rewardCodeActive".lang());
		}
	}

	public UIButton toggleSyncMod;

	public UIButton toggleNoCensor;

	public UIButton toggleRunBackground;

	public UIButton toggleMuteBackground;

	public UIButton toggleTest;

	public UIButton toggleAltInv;

	public UIButton toggleAltAbility;

	public UIButton buttonBackerCode;

	public UIButton toggleDisableMods;

	public UIDropdown ddSnap;

	public UIDropdown ddDivider;

	public Slider sliderMissClick;

	public Slider sliderBGMInterval;

	public UIText textBackerCode;
}
