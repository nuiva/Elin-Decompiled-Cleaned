using System;
using UnityEngine;

public class ELayer : Layer
{
	public static Core core
	{
		get
		{
			return Core.Instance;
		}
	}

	public static Game game
	{
		get
		{
			return ELayer.core.game;
		}
	}

	public static bool AdvMode
	{
		get
		{
			return ELayer.scene.actionMode == ActionMode.Adv;
		}
	}

	public static Player player
	{
		get
		{
			return ELayer.core.game.player;
		}
	}

	public static Chara pc
	{
		get
		{
			return ELayer.core.game.player.chara;
		}
	}

	public static FactionBranch Branch
	{
		get
		{
			return ELayer.core.game.activeZone.branch;
		}
	}

	public static Faction Home
	{
		get
		{
			return ELayer.core.game.factions.Home;
		}
	}

	public static UI ui
	{
		get
		{
			return ELayer.core.ui;
		}
	}

	public static Map _map
	{
		get
		{
			return ELayer.core.game.activeZone.map;
		}
	}

	public static Zone _zone
	{
		get
		{
			return ELayer.core.game.activeZone;
		}
	}

	public static Scene scene
	{
		get
		{
			return ELayer.core.scene;
		}
	}

	public static BaseGameScreen screen
	{
		get
		{
			return ELayer.core.screen;
		}
	}

	public static CoreConfig config
	{
		get
		{
			return ELayer.core.config;
		}
	}

	public static GameSetting setting
	{
		get
		{
			return ELayer.core.gameSetting;
		}
	}

	public static ColorProfile Colors
	{
		get
		{
			return ELayer.core.Colors;
		}
	}

	public static SourceManager sources
	{
		get
		{
			return ELayer.core.sources;
		}
	}

	public static World world
	{
		get
		{
			return ELayer.core.game.world;
		}
	}

	public static SoundManager Sound
	{
		get
		{
			return SoundManager.current;
		}
	}

	public static CoreDebug debug
	{
		get
		{
			return ELayer.core.debug;
		}
	}

	public static int rnd(int a)
	{
		return Rand.Range(0, a);
	}

	public bool IsFloat
	{
		get
		{
			return ELayer.ui.layerFloat.layers.Contains(this);
		}
	}

	public virtual string IdHelp
	{
		get
		{
			return base.GetType().Name;
		}
	}

	public override void OnBeforeAddLayer()
	{
		if (ELayer.core.IsGameStarted && this.defaultActionMode)
		{
			ActionMode.DefaultMode.Activate(true, false);
		}
	}

	public void InitInspector()
	{
		base.Init();
	}

	public sealed override void Init()
	{
		EInput.Consume(0);
		if (this.closeOthers)
		{
			ELayer.ui.RemoveLayers(false);
		}
		base.Init();
	}

	public override void OnAfterAddLayer()
	{
		this.TryShowHelp();
		WidgetMainText.HideLog();
	}

	public void TryShowHelp()
	{
		bool flag = Resources.Load<TextAsset>(CorePath.Text_DialogHelp + this.IdHelp);
		if (flag && !ELayer.core.config.helpFlags.Contains(this.IdHelp))
		{
			LayerDrama.ActivateNerun(this.IdHelp);
			ELayer.core.config.helpFlags.Add(this.IdHelp);
		}
		if (this.windows.Count > 0 && this.windows[0].buttonHelp)
		{
			this.windows[0].buttonHelp.SetActive(flag);
			if (flag)
			{
				this.windows[0].buttonHelp.onClick.AddListener(delegate()
				{
					LayerDrama.ActivateNerun(this.IdHelp);
				});
			}
		}
	}

	public void AddLayerToUI(string id)
	{
		ELayer.ui.AddLayer(id);
	}

	protected sealed override void _Close()
	{
		base._Close();
		EInput.Consume(false, 1);
		if (!this.option.consumeInput)
		{
			EInput.rightMouse.consumed = false;
			EInput.rightMouse.ignoreClick = true;
		}
	}

	protected sealed override void Kill()
	{
		if (this.isDestroyed || base.gameObject == null)
		{
			return;
		}
		if (this.itemTalk != null)
		{
			this.itemTalk.important = false;
			WidgetFeed.Instance.pop.Kill(this.itemTalk, false);
		}
		BaseCore.Instance.eventSystem.SetSelectedGameObject(null);
		EInput.DisableIME();
		base.Kill();
		if (this.option.playlist && ELayer.core.IsGameStarted)
		{
			ELayer._zone.RefreshBGM();
		}
		if (ELayer.debug.alwaysResetWindow)
		{
			Window.dictData.Clear();
		}
		if (this.option.hideFloatUI)
		{
			ELayer.ui.ShowFloats();
		}
		if (this.option.hideWidgets)
		{
			ELayer.ui.widgets.Show();
		}
	}

	public void TryShowHint(string _langHint = null)
	{
		if (!_langHint.IsEmpty() && Lang.Has(_langHint))
		{
			this.langHint = _langHint;
		}
		string text = "h_" + base.GetType().Name;
		if (!this.langHint.IsEmpty())
		{
			ELayer.ui.hud.hint.Show(this.langHint.lang(), true);
			return;
		}
		if (Lang.Has(text))
		{
			ELayer.ui.hud.hint.Show(text, true);
			return;
		}
		if (!this.option.dontRefreshHint)
		{
			ELayer.ui.hud.hint.Refresh();
		}
	}

	public void TalkHomeMemeber(string id)
	{
		Msg.TalkHomeMemeber(id);
	}

	public void TalkMaid(string id)
	{
		Msg.TalkMaid(id);
	}

	public override void OnRightClick()
	{
		ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
		if (componentOf && componentOf.invOwner != null)
		{
			return;
		}
		base.OnRightClick();
	}

	private PopItem itemTalk;
}
