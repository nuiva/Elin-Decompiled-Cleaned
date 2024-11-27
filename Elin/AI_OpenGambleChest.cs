using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_OpenGambleChest : AIAct
{
	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Container;
		}
	}

	public bool IsValid()
	{
		return this.owner != null && !this.owner.isDead && !this.target.isDestroyed && (this.target.IsChildOf(this.owner) || this.owner.Dist(this.target) <= 1);
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		this.owner.Say("lockpick_start", this.owner, this.target, null, null);
		this.owner.PlaySound("lock_pick", 1f, true);
		while (this.target.Num > 0 && this.IsValid())
		{
			this.owner.PlaySound("lock_open_small", 1f, true);
			this.owner.LookAt(this.target);
			this.target.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
			yield return base.KeepRunning();
			EClass.player.stats.gambleChest++;
			Rand.SetSeed(EClass.game.seed + EClass.player.stats.gambleChest);
			bool flag = this.owner.Evalue(280) + 5 >= EClass.rnd(this.target.c_lockLv + 10);
			if (EClass.rnd(20) == 0)
			{
				flag = true;
			}
			if (EClass.rnd(20) == 0)
			{
				flag = false;
			}
			int num = 20 + this.target.c_lockLv / 3;
			if (flag)
			{
				num *= 3;
				EClass.player.stats.gambleChestOpen++;
				Rand.SetSeed(EClass.game.seed + EClass.player.stats.gambleChestOpen);
				bool flag2 = 100 + this.owner.LUC > EClass.rnd(10000);
				if (EClass.debug.enable && EClass.rnd(2) == 0)
				{
					flag2 = true;
				}
				if (flag2)
				{
					this.owner.PlaySound("money", 1f, true);
					this.owner.PlayAnime(AnimeID.Jump, false);
					Thing thing = ThingGen.Create("money", -1, -1).SetNum(EClass.rndHalf(50 * (100 + this.target.c_lockLv * 10)));
					this.owner.Pick(thing, false, true);
					this.owner.Say("gambleChest_win", thing, null, null);
				}
				else
				{
					this.owner.Say("gambleChest_loss", null, null);
				}
				Rand.SetSeed(-1);
			}
			else
			{
				this.owner.Say("gambleChest_broke", this.target.GetName(NameStyle.Full, 1), null);
				this.owner.PlaySound("rock_dead", 1f, true);
			}
			this.target.ModNum(-1, true);
			this.owner.ModExp(280, num);
			if (EClass.rnd(2) == 0)
			{
				this.owner.stamina.Mod(-1);
			}
		}
		yield break;
	}

	public static Thing MakeReward()
	{
		int num = 1;
		string text = "";
		if (text.IsEmpty())
		{
			text = "money";
		}
		return ThingGen.Create(text, -1, -1).SetNum(num);
	}

	public Thing target;
}
