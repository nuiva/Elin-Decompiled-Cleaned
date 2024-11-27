using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ContentConfigTest : ContentConfig
{
	public override void OnInstantiate()
	{
		List<SkinRootStatic> mainSkins = EClass.ui.skins.mainSkins;
		this.ddSkin.SetList<SkinRootStatic>(base.config.test.idSkin, mainSkins, (SkinRootStatic a, int b) => a.Name, delegate(int a, SkinRootStatic b)
		{
			base.config.test.idSkin = a;
			base.config.ApplySkin();
		}, true);
		List<string> langs = new List<string>
		{
			"ani0",
			"ani1",
			"ani2",
			"ani3",
			"ani4"
		};
		this.toggleRefIcon.SetToggle(base.config.test.showRefIcon, delegate(bool on)
		{
			base.config.test.showRefIcon = on;
			if (EClass.core.IsGameStarted)
			{
				LayerInventory.SetDirtyAll(false);
			}
		});
		this.buttonAnimeFrame.SetOptions(base.config.test.animeFrame, langs, delegate(int i)
		{
			base.config.test.animeFrame = i;
			base.config.Apply();
		}, false, "animeFrame");
		this.buttonAnimeFramePCC.SetOptions(base.config.test.animeFramePCC, langs, delegate(int i)
		{
			base.config.test.animeFramePCC = i;
			base.config.Apply();
		}, false, "animeFramePCC");
		this.toggleShowNumber.SetToggle(base.config.test.showNumbers, delegate(bool on)
		{
			base.config.test.showNumbers = on;
		});
		this.toggleToolNoPick.SetToggle(base.config.test.toolNoPick, delegate(bool on)
		{
			base.config.test.toolNoPick = on;
		});
		this.toggleAlwaysRun.SetToggle(base.config.test.alwaysRun, delegate(bool on)
		{
			base.config.test.alwaysRun = on;
		});
		this.toggleExTurn.SetToggle(base.config.test.extraTurnaround, delegate(bool on)
		{
			base.config.test.extraTurnaround = on;
		});
		this.toggleExtraRace.SetToggle(base.config.test.extraRace, delegate(bool on)
		{
			base.config.test.extraRace = on;
		});
		this.toggleExtraCancelMove.SetToggle(base.config.test.extraMoveCancel, delegate(bool on)
		{
			base.config.test.extraMoveCancel = on;
		});
		this.toggleAAPortrait.SetToggle(base.config.test.aaPortrait, delegate(bool on)
		{
			base.config.test.aaPortrait = on;
		});
		this.toggleBloom2.SetToggle(base.config.test.bloom2, delegate(bool on)
		{
			base.config.test.bloom2 = on;
			base.config.Apply();
		});
		this.toggleBlockOnItem.SetToggle(base.config.test.allowBlockOnItem, delegate(bool on)
		{
			base.config.test.allowBlockOnItem = on;
			base.config.Apply();
		});
		this.toggleAlwaysFixCamera.SetToggle(base.config.test.alwaysFixCamera, delegate(bool on)
		{
			base.config.test.alwaysFixCamera = on;
		});
		this.toggleIgnoreBackerFlag.SetToggle(base.config.test.ignoreBackerDestoryFlag, delegate(bool on)
		{
			base.config.test.ignoreBackerDestoryFlag = on;
		});
		this.toggleSeal.SetToggle(base.config.test.unsealWidgets, delegate(bool on)
		{
			base.config.test.unsealWidgets = on;
			base.config.Apply();
			WidgetHotbar.RebuildPages();
		});
		base.SetSlider(this.sliderBrightness, base.config.test.brightnessNight, delegate(float a)
		{
			base.config.test.brightnessNight = a;
			base.config.ApplyGrading();
			if (EClass.core.IsGameStarted)
			{
				EClass.screen.RefreshGrading();
			}
			return Lang.Get("brightnessNight") + "(" + ((int)(a + 100f)).ToString() + "%)";
		});
	}

	public UIDropdown ddSkin;

	public UIButtonLR buttonAnimeFrame;

	public UIButtonLR buttonAnimeFramePCC;

	public UIButton toggleAlwaysRun;

	public UIButton toggleShowNumber;

	public UIButton toggleAAPortrait;

	public UIButton toggleExTurn;

	public UIButton toggleBloom2;

	public UIButton toggleExtraRace;

	public UIButton toggleSeal;

	public UIButton toggleExtraCancelMove;

	public UIButton toggleBlockOnItem;

	public UIButton toggleAlwaysFixCamera;

	public UIButton toggleIgnoreBackerFlag;

	public UIButton toggleRefIcon;

	public UIButton toggleToolNoPick;

	public Slider sliderBrightness;
}
