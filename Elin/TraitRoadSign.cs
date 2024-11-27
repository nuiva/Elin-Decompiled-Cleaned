using System;
using UnityEngine;

public class TraitRoadSign : Trait
{
	public override bool CanBeMasked
	{
		get
		{
			return true;
		}
	}

	public unsafe override void OnRenderTile(Point point, HitResult result, int dir)
	{
		int num = (dir == -1) ? this.owner.dir : dir;
		Cell cell = point.cell;
		cell = ((num == 0) ? cell.Front : ((num == 1) ? cell.Right : ((num == 2) ? cell.Back : cell.Left)));
		Vector3 vector = *cell.GetPoint().Position();
		EClass.screen.guide.passGuideFloor.Add(vector.x, vector.y, vector.z, 10f, 0.3f);
	}
}
