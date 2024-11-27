using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Deconstruct : AIAct
{
	public bool IsValidTarget()
	{
		return this.target != null && this.target.ExistsOnMap && this.target.isDeconstructing;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.target != null)
		{
			this.target.SetDeconstruct(true);
		}
		else
		{
			this.target = EClass._map.props.deconstructing.RandomItem<Card>();
		}
		if (!this.IsValidTarget())
		{
			yield return this.Cancel();
		}
		yield return base.DoGoto(this.target, null);
		Progress_Custom progress_Custom = new Progress_Custom();
		progress_Custom.canProgress = (() => this.IsValidTarget());
		progress_Custom.onProgressBegin = delegate()
		{
		};
		progress_Custom.onProgress = delegate(Progress_Custom p)
		{
			this.owner.PlaySound(this.target.material.GetSoundImpact(null), 1f, true);
			this.target.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
		};
		progress_Custom.onProgressComplete = delegate()
		{
			this.target.Deconstruct();
		};
		Progress_Custom seq = progress_Custom.SetDuration(30, 2);
		yield return base.Do(seq, null);
		yield break;
	}

	public Card target;
}
