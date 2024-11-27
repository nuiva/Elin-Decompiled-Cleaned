using System;
using UnityEngine;

public class MiniGame_Basket : ModMinigame<Shooter>
{
	public override string id
	{
		get
		{
			return "Basket";
		}
	}

	public override void OnActivate()
	{
		if (!this.game)
		{
			if (!this.prefab)
			{
				this.prefab = Resources.Load<Shooter>("Basket");
			}
			Debug.Log(this.prefab);
			this.go = UnityEngine.Object.Instantiate<GameObject>(this.prefab.gameObject);
			Debug.Log(this.go);
			this.game = this.go.GetComponentInChildren<Shooter>();
		}
		base.SetAudioMixer(this.go);
		Shooter.game = new Game_Basket
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
		EClass.scene.audioListener.enabled = false;
	}

	public override void OnDeactivate()
	{
		base.Kill();
		EClass.scene.audioListener.enabled = true;
	}

	public Shooter prefab;
}
