using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Shear : AI_TargetCard
{
	public override string GetText(string str = "")
	{
		string[] list = Lang.GetList("fur");
		string str2 = list[Mathf.Clamp(this.target.c_fur / 10, 0, list.Length - 1)];
		return "AI_Shear".lang() + "(" + str2 + ")";
	}

	public override bool IsValidTC(Card c)
	{
		return c != null && c.CanBeSheared();
	}

	public override bool Perform()
	{
		this.target = Act.TC;
		return base.Perform();
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		yield return base.DoGoto(this.target, null);
		int furLv = Mathf.Clamp(this.target.c_fur / 10 + 1, 1, 5);
		Progress_Custom seq = new Progress_Custom
		{
			canProgress = (() => this.IsValidTC(this.target)),
			onProgressBegin = delegate()
			{
				this.owner.Say("shear_start", this.owner, this.target, null, null);
				if (EClass.rnd(5) == 0)
				{
					this.owner.Talk("goodBoy", null, null, false);
				}
			},
			onProgress = delegate(Progress_Custom p)
			{
				this.owner.LookAt(this.target);
				this.owner.PlaySound("shear", 1f, true);
				this.target.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
				if (this.owner.Dist(this.target) > 1)
				{
					EClass.pc.TryMoveTowards(this.target.pos);
					if (this.owner == null)
					{
						p.Cancel();
						return;
					}
					if (this.owner.Dist(this.target) > 1)
					{
						EClass.pc.Say("targetTooFar", null, null);
						p.Cancel();
						return;
					}
				}
			},
			onProgressComplete = delegate()
			{
				string id = "fiber";
				string idMat = "wool";
				string id2 = this.target.id;
				if (!(id2 == "putty_snow"))
				{
					if (!(id2 == "putty_snow_gold"))
					{
						if (!this.target.Chara.race.fur.IsEmpty())
						{
							string[] array = this.target.Chara.race.fur.Split('/', StringSplitOptions.None);
							id = array[0];
							idMat = array[1];
						}
					}
					else
					{
						idMat = "gold";
					}
				}
				else
				{
					idMat = "cashmere";
				}
				Thing thing = ThingGen.Create(id, idMat);
				int num = 100 * furLv + furLv * furLv * 10;
				int num2 = this.target.LV;
				if (this.target.Chara.IsInCombat || this.target.Chara.IsMinion)
				{
					this.owner.Say("shear_penalty", null, null);
					num /= 2;
					num2 /= 2;
				}
				int num3 = 20 + thing.material.tier * 20;
				thing.SetNum(Mathf.Max(num / num3, 1) + EClass.rnd(furLv + 1));
				thing.SetEncLv(EClass.curve(num2, 30, 10, 75) / 10);
				thing.elements.ModBase(2, EClass.curve(num2 / 10 * 10, 30, 10, 75));
				this.target.c_fur = -5;
				this.owner.Say("shear_end", this.owner, this.target, thing.Name, null);
				this.owner.Pick(thing, false, true);
				this.owner.elements.ModExp(237, 50 * furLv, false);
				EClass.pc.stamina.Mod(-1);
				this.target.Chara.ModAffinity(this.owner, 1, true);
			}
		}.SetDuration((6 + furLv * 6) * 100 / (100 + this.owner.Tool.material.hardness * 2), 3);
		yield return base.Do(seq, null);
		yield break;
	}
}
