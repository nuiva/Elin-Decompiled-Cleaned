using System.Collections.Generic;
using UnityEngine;

public class GoalSearch : Goal
{
	public override Sprite actionIcon => EClass.core.refs.orbitIcons.Search;

	public override int MaxRestart => 10;

	public override IEnumerable<Status> Run()
	{
		yield return Success();
	}
}
