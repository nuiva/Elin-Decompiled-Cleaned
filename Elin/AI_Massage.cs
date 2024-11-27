using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Massage : AIAct
{
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

	public override IEnumerable<AIAct.Status> Run()
	{
		this.target.Say("massage_start", this.target, this.owner, null, null);
		this.isFail = (() => !this.target.IsAliveInCurrentZone || this.owner.Dist(this.target) > 3);
		int num;
		for (int i = 0; i < 30; i = num + 1)
		{
			this.target.AddCondition<ConWait>(30, true);
			yield return base.DoGoto(this.target.pos, 1, false, null);
			this.owner.LookAt(this.target);
			this.target.LookAt(this.owner);
			if (i % 3 == 0)
			{
				this.target.renderer.PlayAnime(AnimeID.Attack, this.owner);
				this.owner.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
				if (EClass.rnd(5) == 0)
				{
					this.target.Talk("goodBoy", null, null, false);
				}
			}
			num = i;
		}
		this.target.Say("massage_end", this.target, null, null);
		this.Finish(this.target, this.owner, 20);
		yield break;
	}

	public void Finish(Chara cc, Chara tc, int stamina)
	{
		cc.Talk("ticket_finish", null, null, false);
		cc.ShowEmo(Emo.love, 0f, false);
		tc.ShowEmo(Emo.love, 0f, false);
		tc.PlaySound("heal", 1f, true);
		tc.PlayEffect("heal_stamina", true, 0f, default(Vector3));
		tc.stamina.Mod(tc.stamina.max * stamina / 100);
		tc.Say("feelgood", tc, null, null);
	}

	public override AIAct.Status Cancel()
	{
		Debug.Log("Canceled massage");
		return base.Cancel();
	}

	public Chara target;
}
