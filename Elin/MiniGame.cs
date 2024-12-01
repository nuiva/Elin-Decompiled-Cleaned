using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MiniGame
{
	public class Balance
	{
		public int lastCoin;

		public int changeCoin;
	}

	public enum Type
	{
		Slot,
		Blackjack,
		Scratch,
		Basket,
		CoinDrop
	}

	public static Dictionary<string, MiniGame> minigames = new Dictionary<string, MiniGame>();

	public Balance balance = new Balance();

	public GameObject go;

	public AssetBundle asset;

	public string path;

	public bool isActive;

	public virtual string id => "";

	public static void RegisterMiniGame(string id, MiniGame g, string _path)
	{
		g.path = new FileInfo(_path).DirectoryName;
		minigames[id] = g;
		Debug.Log("Registered:" + g?.ToString() + " at " + g.path);
	}

	public static void Activate(Type type)
	{
		if (!minigames.ContainsKey("Basket"))
		{
			minigames.Add("Basket", new MiniGame_Basket());
		}
		if (!minigames.ContainsKey("Blackjack"))
		{
			minigames.Add("Blackjack", new MiniGame_Blackjack());
		}
		Debug.Log("Activating:" + type);
		MiniGame miniGame = minigames.TryGetValue(type.ToString());
		if (miniGame == null)
		{
			Msg.Say("minigame_notSupported");
			EClass.scene.TryWarnLinuxMod();
			return;
		}
		LayerMiniGame layerMiniGame = EClass.ui.AddLayer<LayerMiniGame>();
		Debug.Log(layerMiniGame);
		Debug.Log(miniGame);
		miniGame.balance = new Balance();
		layerMiniGame.mini = miniGame;
		layerMiniGame.type = type;
		Debug.Log("aaa");
		layerMiniGame.Run();
	}

	public virtual void OnActivate()
	{
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual void SlidePosition(float w)
	{
	}

	public void Deactivate()
	{
		OnDeactivate();
		Debug.Log(balance.lastCoin);
		Debug.Log(balance.changeCoin);
		if (balance.changeCoin != 0)
		{
			EClass.pc.ModCurrency(balance.changeCoin, "casino_coin");
		}
		LayerMiniGame layer = EClass.ui.GetLayer<LayerMiniGame>();
		if (layer != null)
		{
			layer.mini = null;
			layer.Close();
		}
	}

	public void SetAudioMixer(GameObject go)
	{
		AudioSource[] componentsInChildren = go.GetComponentsInChildren<AudioSource>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].outputAudioMixerGroup = SoundManager.current.mixer.FindMatchingGroups("SFX")[0];
		}
	}

	public void Say(string lang)
	{
		Msg.Say(lang);
	}

	public virtual bool CanExit()
	{
		return true;
	}

	public void Exit()
	{
		EClass.ui.GetLayer<LayerMiniGame>()?.Close();
	}

	public bool OnPlay(int a)
	{
		LayerMiniGame layer = EClass.ui.GetLayer<LayerMiniGame>();
		if ((bool)layer)
		{
			switch (layer.type)
			{
			case Type.Basket:
				EClass.pc.ModExp(108, 15);
				break;
			case Type.Blackjack:
				EClass.pc.ModExp(135, 10);
				break;
			case Type.Slot:
				EClass.pc.ModExp(134, 10);
				break;
			}
		}
		EClass.pc.stamina.Mod(-a);
		EClass.player.EndTurn();
		if (EClass.pc.isDead || EClass.pc.IsDisabled)
		{
			return false;
		}
		return true;
	}

	public void GetSlotReward(string id, int pay = 1, int bet = 1)
	{
		if (id == null)
		{
			id = new string[6] { "ehe", "wild", "cat", "larnneire", "lomias", "bread" }.RandomItem();
			Debug.Log(id);
		}
		Thing c = ThingGen.Create("casino_coin").SetNum(pay * bet);
		Msg.Say("slot_win", c);
	}
}
