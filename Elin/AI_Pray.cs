using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Pray : AIAct
{
	public static TraitAltar GetAltar(Chara c)
	{
		if (c.faith.IsEyth)
		{
			return null;
		}
		List<TraitAltar> list = EClass._map.props.installed.traits.List<TraitAltar>((TraitAltar t) => t.idDeity == c.faith.id && c.HasAccess(t.owner.pos));
		if (list.Count == 0)
		{
			return null;
		}
		return list.RandomItem<TraitAltar>();
	}

	public bool IsValid()
	{
		return this.altar != null && this.altar.ExistsOnMap;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		yield return base.DoGoto(this.altar.owner, 1, null);
		Progress_Custom seq = new Progress_Custom
		{
			cancelWhenMoved = false,
			canProgress = (() => this.IsValid()),
			onProgressBegin = delegate()
			{
				this.owner.Say("pray2", this.owner, this.owner.faith.Name, null);
			},
			onProgress = delegate(Progress_Custom p)
			{
				this.owner.PlayAnime(AnimeID.Shiver, false);
			},
			onProgressComplete = delegate()
			{
				this.owner.PlaySound("pray", 1f, true);
				this.owner.PlayEffect("revive", true, 0f, default(Vector3));
				AI_Pray.Pray(this.owner);
			}
		}.SetDuration(30, 5);
		yield return base.Do(seq, null);
		yield break;
	}

	public static void Pray(Chara c)
	{
		c.ModExp(306, 200);
	}

	public TraitAltar altar;
}
