using System;
using System.Collections.Generic;
using Algorithms;

public class AI_Goto : AIAct
{
	public override bool UseTurbo
	{
		get
		{
			return false;
		}
	}

	public override bool CancelWhenDamaged
	{
		get
		{
			return !EClass._zone.IsRegion;
		}
	}

	public override bool CancelWhenMoved
	{
		get
		{
			return true;
		}
	}

	public override bool InformCancel
	{
		get
		{
			return false;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override bool PushChara
	{
		get
		{
			AIAct parent = this.parent;
			return parent == null || parent.PushChara;
		}
	}

	public override Point GetDestination()
	{
		if (this.destCard != null)
		{
			return this.destCard.pos;
		}
		return this.dest;
	}

	public AI_Goto(Point _dest, int dist, bool _ignoreConnection = false, bool _interaction = false)
	{
		this.dest.Set(_dest);
		this.destDist = dist;
		this.ignoreConnection = _ignoreConnection;
		this.interaction = _interaction;
	}

	public AI_Goto(Card _card, int dist, bool _ignoreConnection = false, bool _interaction = false)
	{
		this.destCard = _card;
		this.destDist = dist;
		this.ignoreConnection = _ignoreConnection;
		this.interaction = _interaction;
	}

	public override void OnReset()
	{
		this.owner.path.state = PathProgress.State.Idle;
	}

	public AIAct.Status TryGoTo()
	{
		PathProgress path = this.owner.path;
		Point pos = this.owner.pos;
		if (this.owner.IsPC)
		{
			if (this.dest.x > EClass._map.bounds.maxX + 1)
			{
				this.dest.x = EClass._map.bounds.maxX + 1;
			}
			else if (this.dest.x < EClass._map.bounds.x - 1)
			{
				this.dest.x = EClass._map.bounds.x - 1;
			}
			if (this.dest.z > EClass._map.bounds.maxZ + 1)
			{
				this.dest.z = EClass._map.bounds.maxZ + 1;
			}
			else if (this.dest.z < EClass._map.bounds.z - 1)
			{
				this.dest.z = EClass._map.bounds.z - 1;
			}
		}
		if ((this.repath || (this.owner.IsPC && ActionMode.IsAdv)) && path.state == PathProgress.State.Idle)
		{
			path.RequestPathImmediate(pos, this.dest, this.destDist, this.ignoreConnection, -1);
			this.repath = false;
		}
		switch (path.state)
		{
		case PathProgress.State.Idle:
			path.RequestPath(pos, this.dest, this.destDist, this.ignoreConnection, -1);
			break;
		case PathProgress.State.PathReady:
		{
			if (path.nodeIndex < 0)
			{
				return this.Cancel();
			}
			PathFinderNode pathFinderNode = path.nodes[path.nodeIndex];
			if (this.owner.IsPC && ActionMode.IsAdv && path.nodeIndex > 2)
			{
				ActionMode.Adv.SetTurbo(-1);
			}
			Point shared = Point.GetShared(pathFinderNode.X, pathFinderNode.Z);
			if (shared.x == this.owner.pos.x && shared.z == this.owner.pos.z && path.nodeIndex > 0 && !shared.cell.HasLadder && !this.owner.Cell.HasLadder)
			{
				path.nodeIndex--;
				pathFinderNode = path.nodes[path.nodeIndex];
				shared.Set(pathFinderNode.X, pathFinderNode.Z);
			}
			if (shared.HasChara && !this.owner.IsPC)
			{
				this.waitCount++;
				if (this.waitCount < 3 || EClass.rnd(5) != 0)
				{
					return AIAct.Status.Running;
				}
			}
			this.waitCount = 0;
			Card.MoveResult moveResult = this.owner.TryMove(shared, true);
			if (this.owner == null)
			{
				return AIAct.Status.Running;
			}
			if (moveResult == Card.MoveResult.Fail || moveResult == Card.MoveResult.Door)
			{
				if (this.owner.IsPC && ActionMode.IsAdv)
				{
					return this.Cancel();
				}
				return base.Restart();
			}
			else
			{
				if (!this.owner.pos.Equals(shared))
				{
					path.state = PathProgress.State.Idle;
					this.repath = true;
				}
				path.nodeIndex--;
				if (this.IsDestinationReached())
				{
					return base.Success(null);
				}
			}
			break;
		}
		case PathProgress.State.Fail:
			return this.Cancel();
		}
		return AIAct.Status.Running;
	}

	public bool IsDestinationReached()
	{
		if (this.destCard != null)
		{
			int num = this.destCard.pos.Distance(this.owner.pos);
			return (this.interaction && num == 1 && this.owner.CanInteractTo(this.owner.pos)) || num <= this.destDist;
		}
		int num2 = Util.Distance(this.owner.pos.x, this.owner.pos.z, this.dest.x, this.dest.z);
		return (this.interaction && num2 == 1 && this.owner.CanInteractTo(this.dest)) || num2 <= this.destDist;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.owner.host != null)
		{
			yield return this.Cancel();
		}
		for (;;)
		{
			if (this.destCard != null)
			{
				this.dest.Set(this.GetDestination());
			}
			while (this.owner.path.state == PathProgress.State.Searching)
			{
				yield return AIAct.Status.Running;
			}
			this.owner.path.state = PathProgress.State.Idle;
			if (this.IsDestinationReached())
			{
				yield return base.Success(null);
			}
			EClass.player.enemySpotted = false;
			for (;;)
			{
				if (this.owner.IsPC)
				{
					if (EClass.player.TooHeavyToMove())
					{
						yield return this.Cancel();
					}
					if (EClass.player.enemySpotted)
					{
						EClass.player.enemySpotted = false;
						Msg.Say("enemy_spotted");
						yield return this.Cancel();
					}
				}
				yield return this.TryGoTo();
				if (this.destCard != null)
				{
					Point destination = this.GetDestination();
					int num = this.dest.Distance(destination);
					if (num > 3 && num < 20)
					{
						break;
					}
				}
			}
			this.repath = true;
			this.owner.path.state = PathProgress.State.Idle;
		}
		yield break;
	}

	public Card destCard;

	public Point dest = new Point();

	public int destDist;

	public int waitCount;

	public bool ignoreConnection;

	public bool interaction;

	public bool repath;
}
