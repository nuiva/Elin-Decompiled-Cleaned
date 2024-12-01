using UnityEngine;

public class MiniGame_Basket : ModMinigame<Shooter>
{
	public Shooter prefab;

	public override string id => "Basket";

	public override void OnActivate()
	{
		if (!game)
		{
			if (!prefab)
			{
				prefab = Resources.Load<Shooter>("Basket");
			}
			Debug.Log(prefab);
			go = Object.Instantiate(prefab.gameObject);
			Debug.Log(go);
			game = go.GetComponentInChildren<Shooter>();
		}
		SetAudioMixer(go);
		Shooter.game = new Game_Basket
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
		EClass.scene.audioListener.enabled = false;
	}

	public override void SlidePosition(float w)
	{
		game.transCanvas.Rect().anchoredPosition = new Vector2(w / 2f, 0f);
	}

	public override void OnDeactivate()
	{
		Kill();
		EClass.scene.audioListener.enabled = true;
	}
}
