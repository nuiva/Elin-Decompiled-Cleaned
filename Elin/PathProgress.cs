using System;
using System.Collections.Generic;
using Algorithms;

public class PathProgress
{
	public bool HasPath
	{
		get
		{
			return this.state == PathProgress.State.PathReady && this.nodes.Count >= 1;
		}
	}

	public bool IsDestinationReached(Point pos)
	{
		return Util.Distance(pos.x, pos.z, this.destPoint.x, this.destPoint.z) <= this.destDist;
	}

	public void RequestPath(Point _startPoint, Point _destPoint, int _destDist, bool _ignoreConnection, int _searchLimit = -1)
	{
		if (_startPoint.cell.isSurrounded4d || _destPoint.cell.isSurrounded4d)
		{
			this.state = PathProgress.State.Fail;
			this.nodeIndex = 0;
			this.nodes.Clear();
			return;
		}
		this.startPoint.Set(_startPoint);
		this.destPoint.Set(_destPoint);
		this.destDist = _destDist;
		this.ignoreConnection = _ignoreConnection;
		this.searchLimit = ((_searchLimit == -1) ? PathManager.Instance.searchLimit : _searchLimit);
		PathManager.Instance.RequestPath(this);
	}

	public void RequestPathImmediate(Point _startPoint, Point _destPoint, int _destDist, bool _ignoreConnection, int _searchLimit = -1)
	{
		if (_startPoint.cell.isSurrounded4d || _destPoint.cell.isSurrounded4d)
		{
			this.state = PathProgress.State.Fail;
			this.nodeIndex = 0;
			this.nodes.Clear();
			return;
		}
		this.startPoint.Set(_startPoint);
		this.destPoint.Set(_destPoint);
		this.destDist = _destDist;
		this.ignoreConnection = _ignoreConnection;
		this.searchLimit = ((_searchLimit == -1) ? PathManager.Instance.searchLimit : _searchLimit);
		PathManager.Instance.RequestPathImmediate(this);
	}

	public IPathfindWalker walker;

	public List<PathFinderNode> nodes = new List<PathFinderNode>();

	public Point startPoint = new Point();

	public Point destPoint = new Point();

	public int nodeIndex;

	public PathProgress.State state;

	public int destDist;

	public int searchLimit;

	public bool ignoreConnection;

	public PathManager.MoveType moveType;

	public enum State
	{
		Idle,
		Searching,
		PathReady,
		Fail
	}
}
