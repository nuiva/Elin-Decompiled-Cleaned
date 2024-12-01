using System.Collections.Generic;

public class AI_Deconstruct : AIAct
{
	public Card target;

	public bool IsValidTarget()
	{
		if (target != null && target.ExistsOnMap)
		{
			return target.isDeconstructing;
		}
		return false;
	}

	public override IEnumerable<Status> Run()
	{
		if (target != null)
		{
			target.SetDeconstruct(deconstruct: true);
		}
		else
		{
			target = EClass._map.props.deconstructing.RandomItem();
		}
		if (!IsValidTarget())
		{
			yield return Cancel();
		}
		yield return DoGoto(target);
		Progress_Custom seq = new Progress_Custom
		{
			canProgress = () => IsValidTarget(),
			onProgressBegin = delegate
			{
			},
			onProgress = delegate
			{
				owner.PlaySound(target.material.GetSoundImpact());
				target.renderer.PlayAnime(AnimeID.Shiver);
			},
			onProgressComplete = delegate
			{
				target.Deconstruct();
			}
		}.SetDuration(30);
		yield return Do(seq);
	}
}
