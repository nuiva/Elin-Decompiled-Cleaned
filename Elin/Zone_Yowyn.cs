using System;

public class Zone_Yowyn : Zone_Town
{
	public override string IDPlaylistOverwrite
	{
		get
		{
			if (!this.IsFestival)
			{
				return null;
			}
			return "Festival_Yowyn";
		}
	}

	public override bool IsFestival
	{
		get
		{
			return base.lv == 0 && EClass.world.date.month == 9;
		}
	}

	public override string IDHat
	{
		get
		{
			if (!this.IsFestival)
			{
				return null;
			}
			return "hat_mushroom";
		}
	}

	public override void OnVisitNewMapOrRegenerate()
	{
		base.OnVisitNewMapOrRegenerate();
		if (base.lv == -1)
		{
			base.ApplyBackerPet(EClass.pc.HasCondition<ConDrawBacker>());
		}
	}
}
