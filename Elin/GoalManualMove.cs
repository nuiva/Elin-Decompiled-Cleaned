using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalManualMove : Goal
{
	public override bool CancelWhenDamaged
	{
		get
		{
			return false;
		}
	}

	public override bool UseTurbo
	{
		get
		{
			return false;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		GoalManualMove.hasMoved = false;
		bool willEnd = false;
		for (;;)
		{
			if (EClass.player.TooHeavyToMove())
			{
				yield return this.Cancel();
			}
			if (EClass.player.nextMove == Vector2.zero)
			{
				if (!EClass.core.config.test.extraMoveCancel)
				{
					willEnd = true;
					EClass.player.nextMove = GoalManualMove.lastMove;
				}
				else
				{
					yield return base.Success(null);
				}
			}
			if (EClass.player.nextMove != GoalManualMove.lastMove)
			{
				GoalManualMove.lastMove = EClass.player.nextMove;
				GoalManualMove.lastPoint.Set(Point.Invalid);
				GoalManualMove.lastlastPoint.Set(Point.Invalid);
			}
			GoalManualMove.dest.Set(EClass.pc.pos);
			GoalManualMove.dest.x += (int)EClass.player.nextMove.x;
			GoalManualMove.dest.z += (int)EClass.player.nextMove.y;
			if (!GoalManualMove.dest.IsValid)
			{
				EClass.player.nextMove = Vector2.zero;
				EClass.pc.ai.Cancel();
				yield return base.Success(null);
			}
			if (!GoalManualMove.TryMove((int)EClass.player.nextMove.x, (int)EClass.player.nextMove.y) && !GoalManualMove.TryAltMove())
			{
				EClass.player.nextMove = Vector2.zero;
				EClass.pc.ai.Cancel();
				yield return base.Success(null);
			}
			GoalManualMove.dest.Set(EClass.pc.pos);
			GoalManualMove.dest.x += (int)EClass.player.nextMove.x;
			GoalManualMove.dest.z += (int)EClass.player.nextMove.y;
			GoalManualMove.dest.Set(EClass.pc.GetFirstStep(GoalManualMove.dest, PathManager.MoveType.Default));
			if (GoalManualMove.dest.IsInBounds && !GoalManualMove.dest.Equals(EClass.pc.pos))
			{
				EClass.pc.TryMove(GoalManualMove.dest, true);
				if (willEnd)
				{
					yield return base.Success(null);
				}
				GoalManualMove.hasMoved = true;
			}
			else
			{
				EClass.player.nextMove = Vector2.zero;
				EClass.pc.ai.Cancel();
				yield return base.Success(null);
			}
			if (!EClass.pc.pos.Equals(GoalManualMove.lastPoint))
			{
				GoalManualMove.lastlastPoint.Set(GoalManualMove.lastPoint);
				GoalManualMove.lastPoint.Set(EClass.pc.pos);
			}
			yield return AIAct.Status.Running;
		}
		yield break;
	}

	public static bool CanMove()
	{
		Vector2 nextMove = EClass.player.nextMove;
		Vector2 vector = GoalManualMove.lastMove;
		Point point = GoalManualMove.lastPoint.Copy();
		Point point2 = GoalManualMove.lastlastPoint.Copy();
		if (EClass.player.nextMove != GoalManualMove.lastMove)
		{
			GoalManualMove.lastMove = EClass.player.nextMove;
			GoalManualMove.lastPoint.Set(Point.Invalid);
			GoalManualMove.lastlastPoint.Set(Point.Invalid);
		}
		if (!GoalManualMove.TryMove((int)EClass.player.nextMove.x, (int)EClass.player.nextMove.y) && !GoalManualMove.TryAltMove())
		{
			GoalManualMove.lastMove = vector;
			GoalManualMove.lastPoint.Set(point);
			GoalManualMove.lastlastPoint.Set(point2);
			EClass.player.nextMove = nextMove;
			return false;
		}
		GoalManualMove.dest.Set(EClass.pc.pos);
		GoalManualMove.dest.x += (int)EClass.player.nextMove.x;
		GoalManualMove.dest.z += (int)EClass.player.nextMove.y;
		GoalManualMove.dest.Set(EClass.pc.GetFirstStep(GoalManualMove.dest, PathManager.MoveType.Default));
		GoalManualMove.lastMove = vector;
		GoalManualMove.lastPoint.Set(point);
		GoalManualMove.lastlastPoint.Set(point2);
		EClass.player.nextMove = nextMove;
		return GoalManualMove.dest.IsInBounds && !GoalManualMove.dest.Equals(EClass.pc.pos) && !EClass.pc.IsEnemyOnPath(GoalManualMove.dest, false);
	}

	public static bool TryAltMove()
	{
		float x = EClass.player.nextMove.x;
		float y = EClass.player.nextMove.y;
		bool extraTurnaround = EClass.core.config.test.extraTurnaround;
		if (y == 1f)
		{
			if (x == 1f)
			{
				return GoalManualMove.TryMove(1, 0) || GoalManualMove.TryMove(0, 1) || (extraTurnaround && (GoalManualMove.TryMove(1, -1) || GoalManualMove.TryMove(-1, 1)));
			}
			if (x == 0f)
			{
				return GoalManualMove.TryMove(1, 1) || GoalManualMove.TryMove(-1, 1) || (extraTurnaround && (GoalManualMove.TryMove(1, 0) || GoalManualMove.TryMove(-1, 0)));
			}
			return GoalManualMove.TryMove(0, 1) || GoalManualMove.TryMove(-1, 0) || (extraTurnaround && (GoalManualMove.TryMove(1, 1) || GoalManualMove.TryMove(-1, -1)));
		}
		else if (y == 0f)
		{
			if (x == 1f)
			{
				return GoalManualMove.TryMove(1, -1) || GoalManualMove.TryMove(1, 1) || (extraTurnaround && (GoalManualMove.TryMove(0, -1) || GoalManualMove.TryMove(0, 1)));
			}
			return GoalManualMove.TryMove(-1, 1) || GoalManualMove.TryMove(-1, -1) || (extraTurnaround && (GoalManualMove.TryMove(0, 1) || GoalManualMove.TryMove(0, -1)));
		}
		else
		{
			if (x == 1f)
			{
				return GoalManualMove.TryMove(0, -1) || GoalManualMove.TryMove(1, 0) || (extraTurnaround && (GoalManualMove.TryMove(-1, -1) || GoalManualMove.TryMove(1, 1)));
			}
			if (x == 0f)
			{
				return GoalManualMove.TryMove(-1, -1) || GoalManualMove.TryMove(1, -1) || (extraTurnaround && (GoalManualMove.TryMove(-1, 0) || GoalManualMove.TryMove(1, 0)));
			}
			return GoalManualMove.TryMove(-1, 0) || GoalManualMove.TryMove(0, -1) || (extraTurnaround && (GoalManualMove.TryMove(-1, 1) || GoalManualMove.TryMove(1, -1)));
		}
	}

	public static bool TryMove(int x, int z)
	{
		Point.shared.Set(EClass.pc.pos);
		Point.shared.x += x;
		Point.shared.z += z;
		Point.shared.Set(EClass.pc.GetFirstStep(Point.shared, PathManager.MoveType.Default));
		if (Point.shared.IsInBounds && !Point.shared.Equals(GoalManualMove.lastlastPoint))
		{
			EClass.player.nextMove.x = (float)x;
			EClass.player.nextMove.y = (float)z;
			return true;
		}
		return false;
	}

	public static Point dest = new Point();

	public static Point lastPoint = new Point();

	public static Point lastlastPoint = new Point();

	public static Vector2 lastMove;

	public static bool hasMoved;
}
