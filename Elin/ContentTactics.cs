using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContentTactics : ContentConfig
{
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

	public override void OnInstantiate()
	{
		Refresh();
	}

	public void Refresh()
	{
		ConfigAutoCombat at = EClass.game.config.autoCombat;
		ConfigPreference pref = EClass.game.config.preference;
		List<SourceTactics.Row> list = EClass.sources.tactics.rows.Where((SourceTactics.Row t) => t.tag.Contains("pc")).ToList();
		toggleDetail.SetToggle(at.detail, delegate
		{
			at.detail = !at.detail;
			Refresh();
		});
		transDetail.SetActive(at.detail);
		ddTactics.SetList(list.FindIndex((SourceTactics.Row t) => t.id == at.idType), list, (SourceTactics.Row a, int b) => a.GetName(), delegate(int a, SourceTactics.Row b)
		{
			at.idType = b.id;
			EClass.pc._tactics = null;
		});
		togglePrefKeepPlaying.SetToggle(pref.keepPlayingMusic, delegate(bool on)
		{
			pref.keepPlayingMusic = on;
		});
		togglePickFish.SetToggle(pref.pickFish, delegate(bool on)
		{
			pref.pickFish = on;
		});
		toggleAutoEat.SetToggle(pref.autoEat, delegate(bool on)
		{
			pref.autoEat = on;
		});
		toggleAbortOnKill.SetToggle(at.abortOnKill, delegate(bool on)
		{
			at.abortOnKill = on;
		});
		toggleAbortOnItemLoss.SetToggle(at.abortOnItemLoss, delegate(bool on)
		{
			at.abortOnItemLoss = on;
		});
		toggleAbortOnAllyDying.SetToggle(at.abortOnAllyDying, delegate(bool on)
		{
			at.abortOnAllyDying = on;
		});
		toggleAbortOnAllyDead.SetToggle(at.abortOnAllyDead, delegate(bool on)
		{
			at.abortOnAllyDead = on;
		});
		toggleOnEnemyDead.SetToggle(at.abortOnEnemyDead, delegate(bool on)
		{
			at.abortOnEnemyDead = on;
		});
		toggleOnHalfHP.SetToggle(at.abortOnHalfHP, delegate(bool on)
		{
			at.abortOnHalfHP = on;
		});
		toggleBUseHotbar.SetToggle(at.bUseHotBar, delegate(bool on)
		{
			at.bUseHotBar = on;
		});
		toggleBUseFav.SetToggle(at.bUseFav, delegate(bool on)
		{
			at.bUseFav = on;
		});
		toggleBCastParty.SetToggle(at.bCastParty, delegate(bool on)
		{
			at.bCastParty = on;
		});
		toggleBUseInventory.SetToggle(at.bUseInventory, delegate(bool on)
		{
			at.bUseInventory = on;
		});
		toggleBDontAutoAttackNeutral.SetToggle(at.bDontAutoAttackNeutral, delegate(bool on)
		{
			at.bDontAutoAttackNeutral = on;
		});
		toggleBTurbo.SetToggle(at.turbo, delegate(bool on)
		{
			at.turbo = on;
		});
		toggleBDontChangeTarget.SetToggle(at.bDontChangeTarget, delegate(bool on)
		{
			at.bDontChangeTarget = on;
		});
		toggleBDontChase.SetToggle(at.bDontChase, delegate(bool on)
		{
			at.bDontChase = on;
		});
		toggleFollowDist.SetToggle(EClass.game.config.tactics.allyKeepDistance, delegate(bool on)
		{
			EClass.game.config.tactics.allyKeepDistance = on;
		});
		toggleDontWander.SetToggle(EClass.game.config.tactics.dontWander, delegate(bool on)
		{
			EClass.game.config.tactics.dontWander = on;
		});
		base.transform.RebuildLayout(recursive: true);
		base.transform.RebuildLayoutTo<Layer>();
	}
}
