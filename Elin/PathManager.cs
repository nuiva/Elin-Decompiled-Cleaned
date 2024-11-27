using System;
using System.Collections.Generic;
using System.Threading;
using Algorithms;
using UnityEngine;

public class PathManager : MonoBehaviour
{
	public IPathfinder pathfinder
	{
		get
		{
			return this._pathfinder;
		}
	}

	private void Awake()
	{
		PathManager.Instance = this;
	}

	public void RequestPath(PathProgress progress)
	{
		PathManager.requestCount++;
		progress.state = PathProgress.State.Searching;
		ThreadPool.QueueUserWorkItem(delegate(object a)
		{
			new PathThread().Start(progress);
		});
	}

	public void RequestPathImmediate(PathProgress progress)
	{
		PathManager.requestCount++;
		this.pathfinder.FindPath(progress);
	}

	public bool IsPathClear(Point origin, Point dest, IPathfindWalker walker, int radius)
	{
		PathManager.tempProgress.walker = walker;
		PathManager.tempProgress.moveType = PathManager.MoveType.Default;
		PathManager.tempProgress.RequestPathImmediate(origin, dest, 0, false, -1);
		return PathManager.tempProgress.nodes.Count > 0 && PathManager.tempProgress.nodes.Count < radius;
	}

	public PathProgress RequestPathImmediate(Point origin, Point dest, IPathfindWalker walker, PathManager.MoveType moveType = PathManager.MoveType.Default, int searchLimit = -1, int destDist = 0)
	{
		PathManager.tempProgress.walker = walker;
		PathManager.tempProgress.moveType = moveType;
		PathManager.tempProgress.RequestPathImmediate(origin, dest, destDist, false, searchLimit);
		return PathManager.tempProgress;
	}

	public Point GetFirstStep(Point origin, Point _dest, IPathfindWalker walker, int maxDist = 20, PathManager.MoveType moveType = PathManager.MoveType.Default)
	{
		Point dest = _dest.Copy();
		Point point = this._GetFirstStep(origin, dest, walker, maxDist, moveType);
		if (point.IsValid)
		{
			return point;
		}
		return Point.Invalid;
	}

	public Point _GetFirstStep(Point origin, Point dest, IPathfindWalker walker, int maxDist = 20, PathManager.MoveType moveType = PathManager.MoveType.Default)
	{
		if (!dest.IsValid || (dest.cell.blocked && origin.Distance(dest) <= 1))
		{
			return Point.Invalid;
		}
		PathManager.tempProgress.walker = walker;
		PathManager.tempProgress.moveType = moveType;
		PathManager.tempProgress.RequestPathImmediate(origin, dest, (moveType == PathManager.MoveType.Combat) ? 1 : 0, false, maxDist);
		if (!PathManager.tempProgress.HasPath)
		{
			return Point.Invalid;
		}
		List<PathFinderNode> nodes = PathManager.tempProgress.nodes;
		if (nodes.Count <= 0)
		{
			return Point.Invalid;
		}
		PathFinderNode pathFinderNode = nodes[nodes.Count - 1];
		if (pathFinderNode.X == origin.x && pathFinderNode.Z == origin.z && nodes.Count > 1)
		{
			pathFinderNode = nodes[nodes.Count - 2];
		}
		if (Mathf.Abs(pathFinderNode.X - origin.x) > 1 || Mathf.Abs(pathFinderNode.Z - origin.z) > 1)
		{
			return Point.Invalid;
		}
		Point point = new Point(pathFinderNode.X, pathFinderNode.Z);
		if (point.x == origin.x && point.z == origin.z)
		{
			return Point.Invalid;
		}
		return point;
	}

	public void OnGridModified()
	{
	}

	public static int requestCount;

	public static PathManager Instance;

	public static PathProgress tempProgress = new PathProgress();

	public PathFinder _pathfinder;

	public int searchLimit = 1000000;

	public enum MoveType
	{
		Default,
		Combat
	}
}
