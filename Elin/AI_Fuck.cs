using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AI_Fuck : AIAct
{
	public virtual AI_Fuck.FuckType Type
	{
		get
		{
			return AI_Fuck.FuckType.fuck;
		}
	}

	public override bool PushChara
	{
		get
		{
			return false;
		}
	}

	public override bool IsAutoTurn
	{
		get
		{
			return true;
		}
	}

	public override TargetType TargetType
	{
		get
		{
			return TargetType.Chara;
		}
	}

	public override int MaxProgress
	{
		get
		{
			return this.maxProgress;
		}
	}

	public override int CurrentProgress
	{
		get
		{
			return this.progress;
		}
	}

	public virtual bool CanTame()
	{
		return false;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.target == null)
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (!chara.IsHomeMember() && !chara.IsDeadOrSleeping && chara.Dist(this.owner) <= 5)
				{
					this.target = chara;
					break;
				}
			}
		}
		if (this.target == null)
		{
			yield return this.Cancel();
		}
		Chara cc = this.sell ? this.target : this.owner;
		Chara tc = this.sell ? this.owner : this.target;
		cc.Say(this.Type.ToString() + "_start", cc, tc, null, null);
		this.isFail = (() => !tc.IsAliveInCurrentZone || tc.Dist(this.owner) > 3);
		if (this.Type == AI_Fuck.FuckType.tame)
		{
			cc.SetTempHand(1104, -1);
		}
		int destDist = (this.Type == AI_Fuck.FuckType.fuck) ? 1 : 1;
		this.maxProgress = 25;
		if (this.succubus)
		{
			cc.Talk("seduce", null, null, false);
		}
		int num;
		for (int i = 0; i < this.maxProgress; i = num + 1)
		{
			this.progress = i;
			yield return base.DoGoto(this.target.pos, destDist, false, null);
			AI_Fuck.FuckType type = this.Type;
			if (type != AI_Fuck.FuckType.fuck)
			{
				if (type == AI_Fuck.FuckType.tame)
				{
					if (EClass.rnd(8) == 0)
					{
						tc.AddCondition<ConFear>(50, false);
					}
					if (i == 0 || i == 10)
					{
						cc.Talk("goodBoy", null, null, false);
					}
					cc.elements.ModExp(237, 10, false);
				}
			}
			else
			{
				cc.LookAt(tc);
				tc.LookAt(cc);
				num = i % 4;
				if (num != 0)
				{
					if (num == 2)
					{
						tc.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
						if (EClass.rnd(3) == 0)
						{
							tc.Talk("tailed", null, null, false);
						}
					}
				}
				else
				{
					cc.renderer.PlayAnime(AnimeID.Attack, tc);
					if (EClass.rnd(3) == 0 || this.sell)
					{
						cc.Talk("tail", null, null, false);
					}
				}
				if (EClass.rnd(3) == 0 || this.sell)
				{
					this.target.AddCondition<ConWait>(50, true);
				}
			}
			num = i;
		}
		this.Finish();
		yield break;
	}

	public void Finish()
	{
		Chara chara = this.sell ? this.target : this.owner;
		Chara chara2 = this.sell ? this.owner : this.target;
		if (chara.isDead || chara2.isDead)
		{
			return;
		}
		bool flag = EClass.rnd(2) == 0;
		AI_Fuck.FuckType type = this.Type;
		if (type != AI_Fuck.FuckType.fuck)
		{
			if (type == AI_Fuck.FuckType.tame)
			{
				if (this.CanTame())
				{
					if (flag)
					{
						chara.Say("tame_success", this.owner, this.target, null, null);
						chara2.MakeAlly(true);
						chara.elements.ModExp(237, 200, false);
					}
					else
					{
						chara.Say("tame_fail", chara, chara2, null, null);
					}
				}
				else
				{
					chara.Say("tame_invalid", chara2, null, null);
				}
			}
		}
		else
		{
			for (int i = 0; i < 2; i++)
			{
				Chara chara3 = (i == 0) ? chara : chara2;
				chara3.RemoveCondition<ConDrunk>();
				if (EClass.rnd(15) == 0 && !chara3.HasElement(1216, 1))
				{
					chara3.AddCondition<ConDisease>(200, false);
				}
				chara3.ModExp(77, 250);
				chara3.ModExp(71, 250);
				chara3.ModExp(75, 250);
				chara3.SAN.Mod(10);
			}
			if (!chara2.HasElement(1216, 1))
			{
				if (EClass.rnd(5) == 0)
				{
					chara2.AddCondition<ConParalyze>(500, false);
				}
				if (EClass.rnd(3) == 0)
				{
					chara2.AddCondition<ConInsane>(100 + EClass.rnd(100), false);
				}
			}
			int num = CalcMoney.Whore(chara2);
			chara.Talk("tail_after", null, null, false);
			bool flag2 = false;
			if (this.succubus)
			{
				chara.ShowEmo(Emo.love, 0f, true);
				chara2.ShowEmo(Emo.love, 0f, true);
				EClass.player.forceTalk = true;
				chara2.Talk("seduced", null, null, false);
			}
			else if (chara != EClass.pc)
			{
				Chara chara4 = chara;
				Chara chara5 = chara2;
				if (this.bitch)
				{
					chara = chara5;
					chara2 = chara4;
				}
				if (!chara.IsPCParty && chara2 == EClass.pc && EClass.rnd(4) != 0)
				{
					num = num / 5 + 1;
					chara.ModCurrency(num, "money");
				}
				if (chara.GetCurrency("money") >= num)
				{
					chara.Talk("tail_pay", null, null, false);
				}
				else
				{
					chara.Talk("tail_nomoney", null, null, false);
					num = chara.GetCurrency("money");
					chara2.Say("angry", chara2, null, null);
					chara2.Talk("angry", null, null, false);
					flag = this.sell;
					if (EClass.rnd(20) == 0)
					{
						flag2 = true;
					}
				}
				chara.ModCurrency(-num, "money");
				if (chara2 == EClass.pc)
				{
					if (num > 0)
					{
						EClass.player.DropReward(ThingGen.Create("money", -1, -1).SetNum(num), false);
						EClass.player.ModKarma(-1);
					}
				}
				else
				{
					chara2.ModCurrency(num, "money");
				}
				chara = chara4;
				chara2 = chara5;
			}
			if (flag2)
			{
				chara2.DoHostileAction(chara, false);
			}
			if (chara.IsPCParty || chara2.IsPCParty)
			{
				chara.stamina.Mod(-5 - EClass.rnd(chara.stamina.max / 10 + (this.succubus ? chara2.LV : 0) + 1));
				chara2.stamina.Mod(-5 - EClass.rnd(chara2.stamina.max / 20 + (this.succubus ? chara.LV : 0) + 1));
			}
			AI_Fuck.<Finish>g__SuccubusExp|21_0(chara, chara2);
			AI_Fuck.<Finish>g__SuccubusExp|21_0(chara2, chara);
		}
		chara2.ModAffinity(chara, flag ? 10 : -5, true);
	}

	[CompilerGenerated]
	internal static void <Finish>g__SuccubusExp|21_0(Chara c, Chara tg)
	{
		if (!c.HasElement(1216, 1))
		{
			return;
		}
		foreach (Element element in tg.elements.ListBestAttributes())
		{
			if (c.elements.ValueWithoutLink(element.id) < element.ValueWithoutLink)
			{
				c.elements.ModTempPotential(element.id, 1 + EClass.rnd(element.ValueWithoutLink - c.elements.ValueWithoutLink(element.id) / 5 + 1), 0);
				c.Say("succubus_exp", c, element.Name.ToLower(), null);
				break;
			}
		}
	}

	public Chara target;

	public bool sell;

	public bool bitch;

	public bool succubus;

	public int maxProgress;

	public int progress;

	public enum FuckType
	{
		fuck,
		tame
	}
}
