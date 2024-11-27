using System;
using System.Collections.Generic;

public class Zone_Casino : Zone_Civilized
{
	public override bool IsSkyLevel
	{
		get
		{
			return true;
		}
	}

	public override bool RevealRoom
	{
		get
		{
			return true;
		}
	}

	public override bool AllowCriminal
	{
		get
		{
			return false;
		}
	}

	public override void OnActivate()
	{
		base.OnActivate();
		HashSet<int> hashSet = new HashSet<int>();
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsGlobal && !(chara.id != "snail") && !chara.IsMinion && (chara.c_idBacker == 0 || EClass.sources.backers.map[chara.c_idBacker].isStatic == 0))
			{
				chara.idSkin = 5;
				if (!chara.HasElement(1231, 1))
				{
					chara.SetFeat(1231, 1, false);
				}
				if (EClass.rnd(4) == 0)
				{
					SourceBacker.Row row = EClass.sources.backers.listSnail.NextItem(ref BackerContent.indexSnail);
					if (hashSet.Contains(row.id))
					{
						chara.RemoveBacker();
					}
					else
					{
						hashSet.Add(row.id);
						chara.ApplyBacker(row.id);
					}
				}
			}
		}
	}
}
