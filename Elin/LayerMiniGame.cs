using UnityEngine;

public class LayerMiniGame : ELayer
{
	public static LayerMiniGame Instance;

	public MiniGame mini;

	public MiniGame.Type type;

	public override void OnAfterInit()
	{
		Instance = this;
		ELayer.pc.SetNoGoal();
		EInput.Consume(consumeAxis: true);
		ELayer.ui.layerFloat.SetActive(enable: false);
		if ((bool)WidgetCurrentTool.Instance)
		{
			WidgetCurrentTool.Instance.SetActive(enable: false);
		}
		if ((bool)WidgetEquip.Instance)
		{
			WidgetEquip.Instance.SetActive(enable: false);
		}
	}

	public override void OnUpdateInput()
	{
		if (mini == null && EInput.leftMouse.clicked)
		{
			SE.Click();
			ELayer.pc.stamina.Mod(-10);
			ELayer.player.EndTurn();
			Debug.Log(ELayer.ui.IsPauseGame);
			Debug.Log(ELayer.scene.actionMode.ShouldPauseGame + "/" + ELayer.scene.actionMode);
			Debug.Log(ELayer.scene.actionMode.gameSpeed);
			Debug.Log(ELayer.pc.turn);
			if (ELayer.pc.isDead || ELayer.pc.IsDisabled)
			{
				Close();
			}
		}
	}

	public override bool OnBack()
	{
		if (mini != null && !mini.CanExit())
		{
			SE.BeepSmall();
			return false;
		}
		return base.OnBack();
	}

	public void Run()
	{
		if (mini != null)
		{
			mini.balance.lastCoin = ELayer.pc.GetCurrency("casino_coin");
			mini.balance.changeCoin = 0;
			mini.OnActivate();
			if ((bool)WidgetSideScreen.Instance)
			{
				WidgetSideScreen.Instance.OnChangeResolution();
			}
		}
	}

	public override void OnKill()
	{
		if (mini != null)
		{
			mini.Deactivate();
		}
		ELayer.ui.layerFloat.SetActive(enable: true);
		if ((bool)WidgetCurrentTool.Instance)
		{
			WidgetCurrentTool.Instance.SetActive(enable: true);
		}
		if ((bool)WidgetEquip.Instance)
		{
			WidgetEquip.Instance.SetActive(enable: true);
		}
	}
}
