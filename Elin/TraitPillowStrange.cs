public class TraitPillowStrange : TraitPillow
{
	public override bool IsOn
	{
		get
		{
			if (owner.dir != 0)
			{
				return owner.dir == 2;
			}
			return true;
		}
	}

	public override int radius
	{
		get
		{
			if (!IsOn)
			{
				return 5;
			}
			return 0;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (owner.IsInstalled)
		{
			p.TrySetAct("actUse", delegate
			{
				owner.SetDir((owner.dir == 0) ? 1 : ((owner.dir != 1) ? ((owner.dir == 2) ? 3 : 2) : 0));
				SE.Rotate();
				return false;
			}, owner);
		}
	}
}
