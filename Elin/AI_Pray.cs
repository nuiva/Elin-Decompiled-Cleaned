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
		Progress_Custom progress_Custom = new Progress_Custom();
		progress_Custom.cancelWhenMoved = false;
		progress_Custom.canProgress = (() => this.IsValid());
		progress_Custom.onProgressBegin = delegate()
		{
		};
		progress_Custom.onProgress = delegate(Progress_Custom p)
		{
			this.owner.PlayAnime(AnimeID.Shiver, false);
		};
		progress_Custom.onProgressComplete = delegate()
		{
			AI_Pray.Pray(this.owner, false);
		};
		Progress_Custom seq = progress_Custom.SetDuration(30, 5);
		yield return base.Do(seq, null);
		yield break;
	}

	public static void Pray(Chara c, bool silent = false)
	{
		if (!silent)
		{
			c.Say("pray2", c, c.faith.Name, null);
			c.PlaySound("pray", 1f, true);
			c.PlayEffect("revive", true, 0f, default(Vector3));
		}
		c.ModExp(306, 200);
	}

	public TraitAltar altar;
}
