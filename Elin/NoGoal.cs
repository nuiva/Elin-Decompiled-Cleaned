using System;
using System.Collections.Generic;
using UnityEngine;

public class NoGoal : Goal
{
	public override bool IsIdle
	{
		get
		{
			return true;
		}
	}

	public override bool IsNoGoal
	{
		get
		{
			return true;
		}
	}

	public override int MaxRestart
	{
		get
		{
			return 99999999;
		}
	}

	public override bool CancelWhenDamaged
	{
		get
		{
			return false;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (EClass.scene.actionMode == ActionMode.Sim && EClass.rnd(3) == 0)
		{
			this.owner.MoveRandom();
			if (EClass.rnd(10) == 0)
			{
				this.owner.renderer.PlayAnime(AnimeID.Jump, default(Vector3), false);
			}
		}
		yield return base.Restart();
		yield break;
	}
}
