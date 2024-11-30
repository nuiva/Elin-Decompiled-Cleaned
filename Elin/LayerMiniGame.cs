using System;
using UnityEngine;

public class LayerMiniGame : ELayer
{
	public override void OnAfterInit()
	{
		LayerMiniGame.Instance = this;
		ELayer.pc.SetNoGoal();
		EInput.Consume(true, 1);
		ELayer.ui.layerFloat.SetActive(false);
		if (WidgetCurrentTool.Instance)
		{
			WidgetCurrentTool.Instance.SetActive(false);
		}
		if (WidgetEquip.Instance)
		{
			WidgetEquip.Instance.SetActive(false);
		}
	}

	public override void OnUpdateInput()
	{
		if (this.mini == null && EInput.leftMouse.clicked)
		{
			SE.Click();
			ELayer.pc.stamina.Mod(-10);
			ELayer.player.EndTurn(true);
			Debug.Log(ELayer.ui.IsPauseGame);
			string str = ELayer.scene.actionMode.ShouldPauseGame.ToString();
			string str2 = "/";
			ActionMode actionMode = ELayer.scene.actionMode;
			Debug.Log(str + str2 + ((actionMode != null) ? actionMode.ToString() : null));
			Debug.Log(ELayer.scene.actionMode.gameSpeed);
			Debug.Log(ELayer.pc.turn);
			if (ELayer.pc.isDead || ELayer.pc.IsDisabled)
			{
				this.Close();
			}
		}
	}

	public override bool OnBack()
	{
		if (this.mini != null && !this.mini.CanExit())
		{
			SE.BeepSmall();
			return false;
		}
		return base.OnBack();
	}

	public void Run()
	{
		if (this.mini == null)
		{
			return;
		}
		this.mini.balance.lastCoin = ELayer.pc.GetCurrency("casino_coin");
		this.mini.balance.changeCoin = 0;
		this.mini.OnActivate();
		if (WidgetSideScreen.Instance)
		{
			WidgetSideScreen.Instance.OnChangeResolution();
		}
	}

	public override void OnKill()
	{
		if (this.mini != null)
		{
			this.mini.Deactivate();
		}
		ELayer.ui.layerFloat.SetActive(true);
		if (WidgetCurrentTool.Instance)
		{
			WidgetCurrentTool.Instance.SetActive(true);
		}
		if (WidgetEquip.Instance)
		{
			WidgetEquip.Instance.SetActive(true);
		}
	}

	public static LayerMiniGame Instance;

	public MiniGame mini;

	public MiniGame.Type type;
}
