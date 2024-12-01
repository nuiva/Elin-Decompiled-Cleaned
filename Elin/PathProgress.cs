using System.Collections.Generic;
using Algorithms;

public class PathProgress
{
	public enum State
	{
		Idle,
		Searching,
		PathReady,
		Fail
	}

	public IPathfindWalker walker;

	public List<PathFinderNode> nodes = new List<PathFinderNode>();

	public Point startPoint = new Point();

	public Point destPoint = new Point();

	public int nodeIndex;

	public State state;

	public int destDist;

	public int searchLimit;

	public bool ignoreConnection;

	public PathManager.MoveType moveType;

	public bool HasPath
	{
		get
		{
			if (state == State.PathReady)
			{
				return nodes.Count >= 1;
			}
			return false;
		}
	}

	public bool IsDestinationReached(Point pos)
	{
		return Util.Distance(pos.x, pos.z, destPoint.x, destPoint.z) <= destDist;
	}

	public void RequestPath(Point _startPoint, Point _destPoint, int _destDist, bool _ignoreConnection, int _searchLimit = -1)
	{
		startPoint.Set(_startPoint);
		destPoint.Set(_destPoint);
		destDist = _destDist;
		ignoreConnection = _ignoreConnection;
		searchLimit = ((_searchLimit == -1) ? PathManager.Instance.searchLimit : _searchLimit);
		PathManager.Instance.RequestPath(this);
	}

	public void RequestPathImmediate(Point _startPoint, Point _destPoint, int _destDist, bool _ignoreConnection, int _searchLimit = -1)
	{
		startPoint.Set(_startPoint);
		destPoint.Set(_destPoint);
		destDist = _destDist;
		ignoreConnection = _ignoreConnection;
		searchLimit = ((_searchLimit == -1) ? PathManager.Instance.searchLimit : _searchLimit);
		PathManager.Instance.RequestPathImmediate(this);
	}
}
