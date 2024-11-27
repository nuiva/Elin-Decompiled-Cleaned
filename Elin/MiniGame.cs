using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MiniGame
{
	public static void RegisterMiniGame(string id, MiniGame g, string _path)
	{
		g.path = new FileInfo(_path).DirectoryName;
		MiniGame.minigames[id] = g;
		Debug.Log("Registered:" + ((g != null) ? g.ToString() : null) + " at " + g.path);
	}

	public static void Activate(MiniGame.Type type)
	{
		if (!MiniGame.minigames.ContainsKey("Basket"))
		{
			MiniGame.minigames.Add("Basket", new MiniGame_Basket());
		}
		if (!MiniGame.minigames.ContainsKey("Blackjack"))
		{
			MiniGame.minigames.Add("Blackjack", new MiniGame_Blackjack());
		}
		Debug.Log("Activating:" + type.ToString());
		MiniGame miniGame = MiniGame.minigames.TryGetValue(type.ToString(), null);
		if (miniGame == null)
		{
			Msg.Say("minigame_notSupported");
			EClass.scene.TryWarnLinuxMod();
			return;
		}
		LayerMiniGame layerMiniGame = EClass.ui.AddLayer<LayerMiniGame>();
		Debug.Log(layerMiniGame);
		Debug.Log(miniGame);
		miniGame.balance = new MiniGame.Balance();
		layerMiniGame.mini = miniGame;
		layerMiniGame.type = type;
		Debug.Log("aaa");
		layerMiniGame.Run();
	}

	public virtual string id
	{
		get
		{
			return "";
		}
	}

	public virtual void OnActivate()
	{
	}

	public virtual void OnDeactivate()
	{
	}

	public void Deactivate()
	{
		this.OnDeactivate();
		Debug.Log(this.balance.lastCoin);
		Debug.Log(this.balance.changeCoin);
		if (this.balance.changeCoin != 0)
		{
			EClass.pc.ModCurrency(this.balance.changeCoin, "casino_coin");
		}
		LayerMiniGame layer = EClass.ui.GetLayer<LayerMiniGame>(false);
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
		LayerMiniGame layer = EClass.ui.GetLayer<LayerMiniGame>(false);
		if (layer == null)
		{
			return;
		}
		layer.Close();
	}

	public bool OnPlay(int a)
	{
		LayerMiniGame layer = EClass.ui.GetLayer<LayerMiniGame>(false);
		if (layer)
		{
			switch (layer.type)
			{
			case MiniGame.Type.Slot:
				EClass.pc.ModExp(134, 10);
				break;
			case MiniGame.Type.Blackjack:
				EClass.pc.ModExp(135, 10);
				break;
			case MiniGame.Type.Basket:
				EClass.pc.ModExp(108, 15);
				break;
			}
		}
		EClass.pc.stamina.Mod(-a);
		EClass.player.EndTurn(true);
		return !EClass.pc.isDead && !EClass.pc.IsDisabled;
	}

	public void GetSlotReward(string id, int pay = 1, int bet = 1)
	{
		if (id == null)
		{
			id = new string[]
			{
				"ehe",
				"wild",
				"cat",
				"larnneire",
				"lomias",
				"bread"
			}.RandomItem<string>();
			Debug.Log(id);
		}
		Thing c = ThingGen.Create("casino_coin", -1, -1).SetNum(pay * bet);
		Msg.Say("slot_win", c, null, null, null);
	}

	public static Dictionary<string, MiniGame> minigames = new Dictionary<string, MiniGame>();

	public MiniGame.Balance balance = new MiniGame.Balance();

	public GameObject go;

	public AssetBundle asset;

	public string path;

	public bool isActive;

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
}
