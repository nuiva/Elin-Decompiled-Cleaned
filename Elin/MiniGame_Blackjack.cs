using System;
using BJ;
using UnityEngine;

public class MiniGame_Blackjack : ModMinigame<Blackjack>
{
	public override string id
	{
		get
		{
			return "Blackjack";
		}
	}

	public override bool CanExit()
	{
		return this.game == null || this.game.btnExit.gameObject.activeSelf;
	}

	public override void OnActivate()
	{
		if (!this.game)
		{
			if (!this.prefab)
			{
				this.prefab = Resources.Load<Blackjack>("BlackJack");
			}
			Debug.Log(this.prefab);
			this.go = UnityEngine.Object.Instantiate<GameObject>(this.prefab.gameObject);
			Debug.Log(this.go);
			this.game = this.go.GetComponentInChildren<Blackjack>();
		}
		base.SetAudioMixer(this.go);
		Blackjack.game = new Game_Blackjack
		{
			Deactivate = new Action(base.Deactivate),
			OnPlay = new Func<int, bool>(base.OnPlay),
			ModChangeCoin = delegate(int a)
			{
				this.balance.changeCoin += a;
			},
			ModLastCoin = delegate(int a)
			{
				this.balance.lastCoin += a;
			},
			LastCoin = (() => this.balance.lastCoin)
		};
		this.game.btnExit.SetOnClick(new Action(base.Exit));
		this.game.Money = this.balance.lastCoin;
	}

	public override void SlidePosition(float w)
	{
		this.game.transCanvas.anchoredPosition = new Vector2(w / 2f, 75f);
	}

	public override void OnDeactivate()
	{
		this.balance.changeCoin = this.game.Money - this.balance.lastCoin;
		if (!this.game.isGameStarted)
		{
			this.balance.changeCoin += this.game.chips;
		}
		base.Kill();
	}

	public Blackjack prefab;
}
