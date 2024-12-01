using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public class LayerEditBio : ELayer
{
	public UICharaMaker maker;

	public Chara chara;

	public EmbarkActor moldActor;

	public Image imageBG;

	private bool started;

	public override void OnAfterAddLayer()
	{
		if (ELayer.game == null)
		{
			Game.Create();
		}
		if (!LayerTitle.actor)
		{
			LayerTitle.actor = Util.Instantiate(moldActor);
		}
		SetChara(ELayer.pc);
		ELayer.ui.hud.hint.Show("hintEmbarkTop".lang(), icon: false);
		imageBG.SetAlpha(0f);
		imageBG.DOFade(0.15f, 10f);
	}

	public void SetChara(Chara c, UnityAction _onKill = null)
	{
		chara = c ?? ELayer.player.chara;
		maker.SetChara(chara);
		if (_onKill != null)
		{
			onKill.AddListener(_onKill);
		}
	}

	public void OnClickStart()
	{
		if (!ELayer.game.Difficulty.allowManualSave)
		{
			Dialog.YesNo("dialog_warnManualSave".lang(ELayer.game.Difficulty.Name), delegate
			{
				Start();
			});
		}
		else
		{
			Start();
		}
		void Start()
		{
			if (!started)
			{
				started = true;
				if (!ELayer.debug.skipInitialQuest)
				{
					if (!LayerDrama.Instance)
					{
						LayerDrama.ActivateMain("mono", "1-1");
					}
					else
					{
						LayerTitle.KillActor();
						ELayer.game.StartNewGame();
					}
				}
			}
		}
	}

	public void OnClickHelp()
	{
		LayerHelp.Toggle("general", "3");
	}
}
