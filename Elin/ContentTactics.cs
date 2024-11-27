using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContentTactics : ContentConfig
{
	public override void OnInstantiate()
	{
		this.Refresh();
	}

	public void Refresh()
	{
		ConfigAutoCombat at = EClass.game.config.autoCombat;
		ConfigPreference pref = EClass.game.config.preference;
		List<SourceTactics.Row> list = (from t in EClass.sources.tactics.rows
		where t.tag.Contains("pc")
		select t).ToList<SourceTactics.Row>();
		this.toggleDetail.SetToggle(at.detail, delegate(bool a)
		{
			at.detail = !at.detail;
			this.Refresh();
		});
		this.transDetail.SetActive(at.detail);
		this.ddTactics.SetList<SourceTactics.Row>(list.FindIndex((SourceTactics.Row t) => t.id == at.idType), list, (SourceTactics.Row a, int b) => a.GetName(), delegate(int a, SourceTactics.Row b)
		{
			at.idType = b.id;
			EClass.pc._tactics = null;
		}, true);
		this.togglePrefKeepPlaying.SetToggle(pref.keepPlayingMusic, delegate(bool on)
		{
			pref.keepPlayingMusic = on;
		});
		this.togglePickFish.SetToggle(pref.pickFish, delegate(bool on)
		{
			pref.pickFish = on;
		});
		this.toggleAutoEat.SetToggle(pref.autoEat, delegate(bool on)
		{
			pref.autoEat = on;
		});
		this.toggleAbortOnKill.SetToggle(at.abortOnKill, delegate(bool on)
		{
			at.abortOnKill = on;
		});
		this.toggleAbortOnItemLoss.SetToggle(at.abortOnItemLoss, delegate(bool on)
		{
			at.abortOnItemLoss = on;
		});
		this.toggleAbortOnAllyDying.SetToggle(at.abortOnAllyDying, delegate(bool on)
		{
			at.abortOnAllyDying = on;
		});
		this.toggleAbortOnAllyDead.SetToggle(at.abortOnAllyDead, delegate(bool on)
		{
			at.abortOnAllyDead = on;
		});
		this.toggleOnEnemyDead.SetToggle(at.abortOnEnemyDead, delegate(bool on)
		{
			at.abortOnEnemyDead = on;
		});
		this.toggleOnHalfHP.SetToggle(at.abortOnHalfHP, delegate(bool on)
		{
			at.abortOnHalfHP = on;
		});
		this.toggleBUseHotbar.SetToggle(at.bUseHotBar, delegate(bool on)
		{
			at.bUseHotBar = on;
		});
		this.toggleBUseFav.SetToggle(at.bUseFav, delegate(bool on)
		{
			at.bUseFav = on;
		});
		this.toggleBCastParty.SetToggle(at.bCastParty, delegate(bool on)
		{
			at.bCastParty = on;
		});
		this.toggleBUseInventory.SetToggle(at.bUseInventory, delegate(bool on)
		{
			at.bUseInventory = on;
		});
		this.toggleBDontAutoAttackNeutral.SetToggle(at.bDontAutoAttackNeutral, delegate(bool on)
		{
			at.bDontAutoAttackNeutral = on;
		});
		this.toggleBTurbo.SetToggle(at.turbo, delegate(bool on)
		{
			at.turbo = on;
		});
		this.toggleBDontChangeTarget.SetToggle(at.bDontChangeTarget, delegate(bool on)
		{
			at.bDontChangeTarget = on;
		});
		this.toggleBDontChase.SetToggle(at.bDontChase, delegate(bool on)
		{
			at.bDontChase = on;
		});
		this.toggleFollowDist.SetToggle(EClass.game.config.tactics.allyKeepDistance, delegate(bool on)
		{
			EClass.game.config.tactics.allyKeepDistance = on;
		});
		this.toggleDontWander.SetToggle(EClass.game.config.tactics.dontWander, delegate(bool on)
		{
			EClass.game.config.tactics.dontWander = on;
		});
		base.transform.RebuildLayout(true);
		base.transform.RebuildLayoutTo<Layer>();
	}

	public UIButton toggleFollowDist;

	public UIButton toggleDontWander;

	public UIButton toggleAbortOnAllyDying;

	public UIButton toggleAbortOnAllyDead;

	public UIButton toggleAbortOnKill;

	public UIButton toggleOnEnemyDead;

	public UIButton toggleOnHalfHP;

	public UIButton toggleAbortOnItemLoss;

	public UIButton toggleBUseHotbar;

	public UIButton toggleBUseFav;

	public UIButton toggleBUseInventory;

	public UIButton toggleBTurbo;

	public UIButton toggleBCastParty;

	public UIButton toggleBDontChangeTarget;

	public UIButton toggleBDontAutoAttackNeutral;

	public UIButton toggleBDontChase;

	public UIButton toggleDetail;

	public UIButton togglePrefKeepPlaying;

	public UIButton togglePickFish;

	public UIButton toggleAutoEat;

	public UIDropdown ddTactics;

	public Transform transDetail;
}
