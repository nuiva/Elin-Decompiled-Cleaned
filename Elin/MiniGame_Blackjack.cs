using BJ;
using UnityEngine;

public class MiniGame_Blackjack : ModMinigame<Blackjack>
{
	public Blackjack prefab;

	public override string id => "Blackjack";

	public override bool CanExit()
	{
		if (!(game == null))
		{
			return game.btnExit.gameObject.activeSelf;
		}
		return true;
	}

	public override void OnActivate()
	{
		if (!game)
		{
			if (!prefab)
			{
				prefab = Resources.Load<Blackjack>("BlackJack");
			}
			Debug.Log(prefab);
			go = Object.Instantiate(prefab.gameObject);
			Debug.Log(go);
			game = go.GetComponentInChildren<Blackjack>();
		}
		SetAudioMixer(go);
		Blackjack.game = new Game_Blackjack
		{
			Deactivate = base.Deactivate,
			OnPlay = base.OnPlay,
			ModChangeCoin = delegate(int a)
			{
				balance.changeCoin += a;
			},
			ModLastCoin = delegate(int a)
			{
				balance.lastCoin += a;
			},
			LastCoin = () => balance.lastCoin
		};
		game.btnExit.SetOnClick(base.Exit);
		game.Money = balance.lastCoin;
	}

	public override void SlidePosition(float w)
	{
		game.transCanvas.anchoredPosition = new Vector2(w / 2f, 75f);
	}

	public override void OnDeactivate()
	{
		balance.changeCoin = game.Money - balance.lastCoin;
		if (!game.isGameStarted)
		{
			balance.changeCoin += game.chips;
		}
		Kill();
	}
}
