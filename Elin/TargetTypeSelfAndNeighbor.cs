using System;

public class TargetTypeSelfAndNeighbor : TargetType
{
	public override TargetRange Range
	{
		get
		{
			return TargetRange.Neighbor;
		}
	}

	public override int LimitDist
	{
		get
		{
			return 2;
		}
	}

	public override bool ShowMapHighlight
	{
		get
		{
			return false;
		}
	}

	public override bool RequireLos
	{
		get
		{
			return false;
		}
	}
}
