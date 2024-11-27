using System;

public class TraitAltarAncient : TraitAltar
{
	public override string idDeity
	{
		get
		{
			FactionBranch branch = EClass.Branch;
			return ((branch != null) ? branch.faith.id : null) ?? EClass.game.religions.Eyth.id;
		}
	}

	public override Religion Deity
	{
		get
		{
			FactionBranch branch = EClass.Branch;
			return ((branch != null) ? branch.faith : null) ?? EClass.game.religions.Eyth;
		}
	}
}
