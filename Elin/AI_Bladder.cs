using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Bladder : AIAct
{
	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.toilet == null)
		{
			this.toilet = (EClass._map.Installed.traits.GetTraitSet<TraitBath>().GetRandom() as TraitBath);
		}
		if (this.toilet == null)
		{
			yield return this.Cancel();
		}
		yield return base.DoGoto(this.toilet.owner, null);
		Progress_Custom seq = new Progress_Custom
		{
			onProgressBegin = delegate()
			{
				this.owner.SetTempHand(-1, -1);
				this.owner.SetPCCState(PCCState.Naked);
				this.owner.SetCensored(true);
				this.owner.PlaySound("water", 1f, true);
				this.owner.Kick(this.owner.pos, true);
				this.owner.pos.TalkWitnesses(this.owner, "disgust", 4, WitnessType.everyone, null, 3);
			},
			onProgress = delegate(Progress_Custom p)
			{
				this.owner.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
			},
			onProgressComplete = delegate()
			{
				if (this.toilet == null || !this.toilet.ExistsOnMap || !this.toilet.owner.pos.Equals(this.owner.pos))
				{
					EClass._map.SetLiquid(this.owner.pos.x, this.owner.pos.z, 1, 3);
				}
				this.owner.ShowEmo(Emo.happy, 0f, true);
			}
		}.SetDuration(15, 5);
		yield return base.Do(seq, null);
		yield break;
	}

	public override void OnReset()
	{
		this.owner.SetPCCState(PCCState.Normal);
		this.owner.SetCensored(false);
	}

	public TraitBath toilet;
}
