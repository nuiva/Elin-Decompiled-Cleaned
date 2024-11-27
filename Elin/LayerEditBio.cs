using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public class LayerEditBio : ELayer
{
	public override void OnAfterAddLayer()
	{
		if (ELayer.game == null)
		{
			Game.Create(null);
		}
		if (!LayerTitle.actor)
		{
			LayerTitle.actor = Util.Instantiate<EmbarkActor>(this.moldActor, null);
		}
		this.SetChara(ELayer.pc, null);
		ELayer.ui.hud.hint.Show("hintEmbarkTop".lang(), false);
		this.imageBG.SetAlpha(0f);
		this.imageBG.DOFade(0.15f, 10f);
	}

	public void SetChara(Chara c, UnityAction _onKill = null)
	{
		this.chara = (c ?? ELayer.player.chara);
		this.maker.SetChara(this.chara);
		if (_onKill != null)
		{
			this.onKill.AddListener(_onKill);
		}
	}

	public void OnClickStart()
	{
		if (!ELayer.game.Difficulty.allowManualSave)
		{
			Dialog.YesNo("dialog_warnManualSave".lang(ELayer.game.Difficulty.Name, null, null, null, null), delegate
			{
				this.<OnClickStart>g__Start|7_0();
			}, null, "yes", "no");
			return;
		}
		this.<OnClickStart>g__Start|7_0();
	}

	public void OnClickHelp()
	{
		LayerHelp.Toggle("general", "3");
	}

	[CompilerGenerated]
	private void <OnClickStart>g__Start|7_0()
	{
		if (this.started)
		{
			return;
		}
		this.started = true;
		if (!ELayer.debug.skipInitialQuest)
		{
			if (!LayerDrama.Instance)
			{
				LayerDrama.ActivateMain("mono", "1-1", null, null, "");
				return;
			}
			LayerTitle.KillActor();
			ELayer.game.StartNewGame();
		}
	}

	public UICharaMaker maker;

	public Chara chara;

	public EmbarkActor moldActor;

	public Image imageBG;

	private bool started;
}
