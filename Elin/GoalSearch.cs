using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalSearch : Goal
{
	public override Sprite actionIcon
	{
		get
		{
			return EClass.core.refs.orbitIcons.Search;
		}
	}

	public override int MaxRestart
	{
		get
		{
			return 10;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		yield return base.Success(null);
		yield break;
	}
}
