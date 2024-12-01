using System.Collections.Generic;

public class AI_Pray : AIAct
{
	public TraitAltar altar;

	public static TraitAltar GetAltar(Chara c)
	{
		if (c.faith.IsEyth)
		{
			return null;
		}
		List<TraitAltar> list = EClass._map.props.installed.traits.List((TraitAltar t) => t.idDeity == c.faith.id && c.HasAccess(t.owner.pos));
		if (list.Count == 0)
		{
			return null;
		}
		return list.RandomItem();
	}

	public bool IsValid()
	{
		if (altar != null)
		{
			return altar.ExistsOnMap;
		}
		return false;
	}

	public override IEnumerable<Status> Run()
	{
		yield return DoGoto(altar.owner, 1);
		Progress_Custom seq = new Progress_Custom
		{
			cancelWhenMoved = false,
			canProgress = () => IsValid(),
			onProgressBegin = delegate
			{
			},
			onProgress = delegate
			{
				owner.PlayAnime(AnimeID.Shiver);
			},
			onProgressComplete = delegate
			{
				Pray(owner);
			}
		}.SetDuration(30, 5);
		yield return Do(seq);
	}

	public static void Pray(Chara c, bool silent = false)
	{
		if (!silent)
		{
			c.Say("pray2", c, c.faith.Name);
			c.PlaySound("pray");
			c.PlayEffect("revive");
		}
		c.ModExp(306, 200);
	}
}
