using UnityEngine;

public class TraitRoadSign : Trait
{
	public override bool CanBeMasked => true;

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		int num = ((dir == -1) ? owner.dir : dir);
		Cell cell = point.cell;
		Vector3 vector = (num switch
		{
			2 => cell.Back, 
			1 => cell.Right, 
			0 => cell.Front, 
			_ => cell.Left, 
		}).GetPoint().Position();
		EClass.screen.guide.passGuideFloor.Add(vector.x, vector.y, vector.z, 10f, 0.3f);
	}
}
