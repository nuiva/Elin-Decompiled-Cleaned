using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContentConfigGame : ContentConfig
{
	public override void OnInstantiate()
	{
		this.toggleDisableAutoStairs.SetToggle(base.config.game.disableAutoStairs, delegate(bool on)
		{
			base.config.game.disableAutoStairs = on;
		});
		this.toggleConfirmGive.SetToggle(base.config.game.confirmGive, delegate(bool on)
		{
			base.config.game.confirmGive = on;
		});
		this.toggleWarnCrime.SetToggle(base.config.game.warnCrime, delegate(bool on)
		{
			base.config.game.warnCrime = on;
		});
		this.toggleWarnMana.SetToggle(base.config.game.warnMana, delegate(bool on)
		{
			base.config.game.warnMana = on;
		});
		this.toggleWarnDisassemble.SetToggle(base.config.game.warnDisassemble, delegate(bool on)
		{
			base.config.game.warnDisassemble = on;
		});
		this.toggleFogBounds.SetToggle(base.config.game.dontRenderOutsideMap, delegate(bool on)
		{
			base.config.game.dontRenderOutsideMap = on;
		});
		this.toggleShowInvBG.SetToggle(base.config.game.showInvBG, delegate(bool on)
		{
			base.config.game.showInvBG = on;
		});
		this.toggleHaltOnSpotEnemy.SetToggle(base.config.game.haltOnSpotEnemy, delegate(bool on)
		{
			base.config.game.haltOnSpotEnemy = on;
		});
		this.toggleHaltOnSpotTrap.SetToggle(base.config.game.haltOnSpotTrap, delegate(bool on)
		{
			base.config.game.haltOnSpotTrap = on;
		});
		this.toggleHideWeapon.SetToggle(base.config.game.hideWeapons, delegate(bool on)
		{
			base.config.game.hideWeapons = on;
		});
		this.toggleSmoothPick.SetToggle(base.config.game.smoothPick, delegate(bool on)
		{
			base.config.game.smoothPick = on;
		});
		this.toggleMarkStack.SetToggle(base.config.game.markStack, delegate(bool on)
		{
			base.config.game.markStack = on;
		});
		this.toggleWaitOnDebuff.SetToggle(base.config.game.waitOnDebuff, delegate(bool on)
		{
			base.config.game.waitOnDebuff = on;
		});
		this.toggleWaitOnRange.SetToggle(base.config.game.waitOnRange, delegate(bool on)
		{
			base.config.game.waitOnRange = on;
		});
		this.toggleWaitOnMelee.SetToggle(base.config.game.waitOnMelee, delegate(bool on)
		{
			base.config.game.waitOnMelee = on;
		});
		this.toggleTutorial.SetToggle(base.config.game.tutorial, delegate(bool on)
		{
			base.config.game.tutorial = on;
		});
		this.toggleShippingResult.SetToggle(base.config.game.showShippingResult, delegate(bool on)
		{
			base.config.game.showShippingResult = on;
		});
		this.toggleAdvanceMenu.SetToggle(base.config.game.advancedMenu, delegate(bool on)
		{
			base.config.game.advancedMenu = on;
		});
		this.toggleOffhand.SetToggle(base.config.game.showOffhand, delegate(bool on)
		{
			base.config.game.showOffhand = on;
			EClass.pc.SetTempHand(0, 0);
		});
		this.toggleHoldMiddle.SetToggle(base.config.game.holdMiddleButtonToHold, delegate(bool on)
		{
			base.config.game.holdMiddleButtonToHold = on;
		});
		this.toggleShiftToUseNegativeAbility.SetToggle(base.config.game.shiftToUseNegativeAbilityOnSelf, delegate(bool on)
		{
			base.config.game.shiftToUseNegativeAbilityOnSelf = on;
		});
		this.toggleHoldDoubleClick.SetToggle(base.config.game.doubleClickToHold, delegate(bool on)
		{
			base.config.game.doubleClickToHold = on;
		});
		this.toggleConfirmExit.SetToggle(base.config.game.confirmMapExit, delegate(bool on)
		{
			base.config.game.confirmMapExit = on;
		});
		this.toggleUseAbilityOnHotkey.SetToggle(base.config.game.useAbilityOnHotkey, delegate(bool on)
		{
			base.config.game.useAbilityOnHotkey = on;
		});
		this.toggleNetSend.SetToggle(base.config.net.sendEvent, delegate(bool on)
		{
			base.config.net.sendEvent = on;
		});
		base.SetSlider(this.sliderWait, (float)base.config.game.waiter, delegate(float a)
		{
			base.config.game.waiter = (int)a;
			return Lang.Get("gameWait_duration") + "(" + (25f * a).ToString() + "%)";
		});
		base.SetSlider(this.sliderBackupNum, (float)base.config.game.numBackup, delegate(float a)
		{
			base.config.game.numBackup = (int)a;
			return "backupNum".lang(a.ToString() ?? "", null, null, null, null);
		});
		base.SetSlider(this.sliderBackupInterval, (float)base.config.game.backupInterval, delegate(float a)
		{
			base.config.game.backupInterval = (int)a;
			return "backupInterval".lang((a * 30f).ToString() ?? "", null, null, null, null);
		});
		this.toggleAutoBackup.SetToggle(base.config.game.autoBackup, delegate(bool on)
		{
			base.config.game.autoBackup = on;
			this.sliderBackupInterval.SetActive(on);
		});
		this.sliderBackupInterval.SetActive(base.config.game.autoBackup);
		this.SetGroup(this.groupRide, base.config.game.showRide, delegate(int a)
		{
			base.config.game.showRide = a;
			base.config.Apply();
		});
		this.SetGroup(this.groupBackerFilter, base.config.backer.filter, delegate(int a)
		{
			base.config.backer.filter = a;
			base.config.Apply();
		});
		this.SetGroup(this.groupBorder, base.config.game.showBorder, delegate(int a)
		{
			base.config.game.showBorder = a;
			base.config.Apply();
		});
	}

	public void SetGroup(UISelectableGroup group, int value, UnityAction<int> action)
	{
		group.Init(value, action, false);
	}

	public UIButton toggleFogBounds;

	public UIButton toggleOffhand;

	public UIButton toggleShowInvBG;

	public UIButton toggleConfirmGive;

	public UIButton toggleWarnCrime;

	public UIButton toggleWarnMana;

	public UIButton toggleWarnDisassemble;

	public UIButton toggleHoldMiddle;

	public UIButton toggleHoldDoubleClick;

	public UIButton toggleConfirmExit;

	public UIButton toggleUseAbilityOnHotkey;

	public UIButton toggleHideWeapon;

	public UIButton toggleNetSend;

	public UIButton toggleNet;

	public UIButton toggleDisableAutoStairs;

	public UIButton toggleSmoothPick;

	public UIButton toggleMarkStack;

	public UIButton toggleRefIcon;

	public UIButton toggleShippingResult;

	public UIButton toggleShiftToUseNegativeAbility;

	public UIButton toggleAutoBackup;

	public UISelectableGroup groupBackerFilter;

	public UISelectableGroup groupBorder;

	public UISelectableGroup groupRide;

	public Slider sliderWait;

	public Slider sliderBackupNum;

	public Slider sliderBackupInterval;

	public UIButton toggleWaitOnDebuff;

	public UIButton toggleWaitOnRange;

	public UIButton toggleWaitOnMelee;

	public UIButton toggleTutorial;

	public UIButton toggleAdvanceMenu;

	public UIButton toggleHaltOnSpotEnemy;

	public UIButton toggleHaltOnSpotTrap;
}
