using System;

namespace Algorithms
{
	public class WeightCell
	{
		public virtual bool IsPathBlocked(PathManager.MoveType moveType)
		{
			return this.blocked;
		}

		public bool blocked;

		public byte[] weights = new byte[4];

		public byte baseWeight;
	}
}
