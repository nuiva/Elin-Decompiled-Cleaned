using System.Collections.Generic;

public class NoGoal : Goal
{
	public override bool IsIdle => true;

	public override bool IsNoGoal => true;

	public override int MaxRestart => 99999999;

	public override bool CancelWhenDamaged => false;

	public override IEnumerable<Status> Run()
	{
		if (EClass.scene.actionMode == ActionMode.Sim && EClass.rnd(3) == 0)
		{
			owner.MoveRandom();
			if (EClass.rnd(10) == 0)
			{
				owner.renderer.PlayAnime(AnimeID.Jump);
			}
		}
		yield return Restart();
	}
}
