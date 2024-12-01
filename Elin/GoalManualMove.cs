using System.Collections.Generic;
using UnityEngine;

public class GoalManualMove : Goal
{
	public static Point dest = new Point();

	public static Point lastPoint = new Point();

	public static Point lastlastPoint = new Point();

	public static Vector2 lastMove;

	public static bool hasMoved;

	public override bool CancelWhenDamaged => false;

	public override bool UseTurbo => false;

	public override IEnumerable<Status> Run()
	{
		hasMoved = false;
		bool willEnd = false;
		while (true)
		{
			if (EClass.player.TooHeavyToMove())
			{
				yield return Cancel();
			}
			if (EClass.player.nextMove == Vector2.zero)
			{
				if (!EClass.core.config.test.extraMoveCancel)
				{
					willEnd = true;
					EClass.player.nextMove = lastMove;
				}
				else
				{
					yield return Success();
				}
			}
			if (EClass.player.nextMove != lastMove)
			{
				lastMove = EClass.player.nextMove;
				lastPoint.Set(Point.Invalid);
				lastlastPoint.Set(Point.Invalid);
			}
			dest.Set(EClass.pc.pos);
			dest.x += (int)EClass.player.nextMove.x;
			dest.z += (int)EClass.player.nextMove.y;
			if (!dest.IsValid)
			{
				EClass.player.nextMove = Vector2.zero;
				EClass.pc.ai.Cancel();
				yield return Success();
			}
			if (!TryMove((int)EClass.player.nextMove.x, (int)EClass.player.nextMove.y) && !TryAltMove())
			{
				EClass.player.nextMove = Vector2.zero;
				EClass.pc.ai.Cancel();
				yield return Success();
			}
			dest.Set(EClass.pc.pos);
			dest.x += (int)EClass.player.nextMove.x;
			dest.z += (int)EClass.player.nextMove.y;
			dest.Set(EClass.pc.GetFirstStep(dest));
			if (dest.IsInBounds && !dest.Equals(EClass.pc.pos))
			{
				EClass.pc.TryMove(dest);
				if (willEnd)
				{
					yield return Success();
				}
				hasMoved = true;
			}
			else
			{
				EClass.player.nextMove = Vector2.zero;
				EClass.pc.ai.Cancel();
				yield return Success();
			}
			if (!EClass.pc.pos.Equals(lastPoint))
			{
				lastlastPoint.Set(lastPoint);
				lastPoint.Set(EClass.pc.pos);
			}
			yield return Status.Running;
		}
	}

	public static bool CanMove()
	{
		Vector2 nextMove = EClass.player.nextMove;
		Vector2 vector = lastMove;
		Point point = lastPoint.Copy();
		Point point2 = lastlastPoint.Copy();
		if (EClass.player.nextMove != lastMove)
		{
			lastMove = EClass.player.nextMove;
			lastPoint.Set(Point.Invalid);
			lastlastPoint.Set(Point.Invalid);
		}
		if (!TryMove((int)EClass.player.nextMove.x, (int)EClass.player.nextMove.y) && !TryAltMove())
		{
			lastMove = vector;
			lastPoint.Set(point);
			lastlastPoint.Set(point2);
			EClass.player.nextMove = nextMove;
			return false;
		}
		dest.Set(EClass.pc.pos);
		dest.x += (int)EClass.player.nextMove.x;
		dest.z += (int)EClass.player.nextMove.y;
		dest.Set(EClass.pc.GetFirstStep(dest));
		lastMove = vector;
		lastPoint.Set(point);
		lastlastPoint.Set(point2);
		EClass.player.nextMove = nextMove;
		if (dest.IsInBounds && !dest.Equals(EClass.pc.pos) && !EClass.pc.IsEnemyOnPath(dest, cancelAI: false))
		{
			return true;
		}
		return false;
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
				if (!TryMove(1, 0) && !TryMove(0, 1))
				{
					if (extraTurnaround)
					{
						if (!TryMove(1, -1))
						{
							return TryMove(-1, 1);
						}
						return true;
					}
					return false;
				}
				return true;
			}
			if (x == 0f)
			{
				if (!TryMove(1, 1) && !TryMove(-1, 1))
				{
					if (extraTurnaround)
					{
						if (!TryMove(1, 0))
						{
							return TryMove(-1, 0);
						}
						return true;
					}
					return false;
				}
				return true;
			}
			if (!TryMove(0, 1) && !TryMove(-1, 0))
			{
				if (extraTurnaround)
				{
					if (!TryMove(1, 1))
					{
						return TryMove(-1, -1);
					}
					return true;
				}
				return false;
			}
			return true;
		}
		if (y == 0f)
		{
			if (x == 1f)
			{
				if (!TryMove(1, -1) && !TryMove(1, 1))
				{
					if (extraTurnaround)
					{
						if (!TryMove(0, -1))
						{
							return TryMove(0, 1);
						}
						return true;
					}
					return false;
				}
				return true;
			}
			if (!TryMove(-1, 1) && !TryMove(-1, -1))
			{
				if (extraTurnaround)
				{
					if (!TryMove(0, 1))
					{
						return TryMove(0, -1);
					}
					return true;
				}
				return false;
			}
			return true;
		}
		if (x == 1f)
		{
			if (!TryMove(0, -1) && !TryMove(1, 0))
			{
				if (extraTurnaround)
				{
					if (!TryMove(-1, -1))
					{
						return TryMove(1, 1);
					}
					return true;
				}
				return false;
			}
			return true;
		}
		if (x == 0f)
		{
			if (!TryMove(-1, -1) && !TryMove(1, -1))
			{
				if (extraTurnaround)
				{
					if (!TryMove(-1, 0))
					{
						return TryMove(1, 0);
					}
					return true;
				}
				return false;
			}
			return true;
		}
		if (!TryMove(-1, 0) && !TryMove(0, -1))
		{
			if (extraTurnaround)
			{
				if (!TryMove(-1, 1))
				{
					return TryMove(1, -1);
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public static bool TryMove(int x, int z)
	{
		Point.shared.Set(EClass.pc.pos);
		Point.shared.x += x;
		Point.shared.z += z;
		Point.shared.Set(EClass.pc.GetFirstStep(Point.shared));
		if (Point.shared.IsInBounds && !Point.shared.Equals(lastlastPoint))
		{
			EClass.player.nextMove.x = x;
			EClass.player.nextMove.y = z;
			return true;
		}
		return false;
	}
}
