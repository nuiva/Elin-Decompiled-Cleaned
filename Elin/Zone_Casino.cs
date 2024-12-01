using System.Collections.Generic;

public class Zone_Casino : Zone_Civilized
{
	public override bool IsSkyLevel => true;

	public override bool RevealRoom => true;

	public override bool AllowCriminal => false;

	public override void OnActivate()
	{
		base.OnActivate();
		HashSet<int> hashSet = new HashSet<int>();
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.IsGlobal || chara.id != "snail" || chara.IsMinion || (chara.c_idBacker != 0 && EClass.sources.backers.map[chara.c_idBacker].isStatic != 0))
			{
				continue;
			}
			chara.idSkin = 5;
			if (!chara.HasElement(1231))
			{
				chara.SetFeat(1231);
			}
			if (EClass.rnd(4) == 0)
			{
				SourceBacker.Row row = EClass.sources.backers.listSnail.NextItem(ref BackerContent.indexSnail);
				if (hashSet.Contains(row.id))
				{
					chara.RemoveBacker();
					continue;
				}
				hashSet.Add(row.id);
				chara.ApplyBacker(row.id);
			}
		}
	}
}
