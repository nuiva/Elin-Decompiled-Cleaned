using System.Collections.Generic;
using UnityEngine;

public class AI_Massage : AIAct
{
	public Chara target;

	public override bool PushChara => false;

	public override bool IsAutoTurn => true;

	public override TargetType TargetType => TargetType.Chara;

	public override IEnumerable<Status> Run()
	{
		target.Say("massage_start", target, owner);
		isFail = () => !target.IsAliveInCurrentZone || owner.Dist(target) > 3;
		for (int i = 0; i < 30; i++)
		{
			_ = i;
			target.AddCondition<ConWait>(30, force: true);
			yield return DoGoto(target.pos, 1);
			owner.LookAt(target);
			target.LookAt(owner);
			if (i % 3 == 0)
			{
				target.renderer.PlayAnime(AnimeID.Attack, owner);
				owner.renderer.PlayAnime(AnimeID.Shiver);
				if (EClass.rnd(5) == 0)
				{
					target.Talk("goodBoy");
				}
			}
		}
		target.Say("massage_end", target);
		Finish(target, owner, 20);
	}

	public void Finish(Chara cc, Chara tc, int stamina)
	{
		cc.Talk("ticket_finish");
		cc.ShowEmo(Emo.love, 0f, skipSame: false);
		tc.ShowEmo(Emo.love, 0f, skipSame: false);
		tc.PlaySound("heal");
		tc.PlayEffect("heal_stamina");
		tc.stamina.Mod(tc.stamina.max * stamina / 100);
		tc.Say("feelgood", tc);
	}

	public override Status Cancel()
	{
		Debug.Log("Canceled massage");
		return base.Cancel();
	}
}
