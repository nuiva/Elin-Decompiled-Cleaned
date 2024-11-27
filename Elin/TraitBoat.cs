using System;

public class TraitBoat : Trait
{
	public override bool IsFloating
	{
		get
		{
			return true;
		}
	}

	public override bool IsGround
	{
		get
		{
			return true;
		}
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		if (state == PlaceState.installed)
		{
			this.ForeachDeck(delegate(Point p)
			{
				p.SetFloor(1, 57);
				p.cell.floorDir = ((this.owner.dir % 2 == 0) ? 0 : 1);
			});
		}
	}

	public override void OnRemovedFromZone()
	{
		if (this.owner.placeState == PlaceState.installed)
		{
			SourceMaterial.Row mat = this.GetWaterMat();
			this.ForeachDeck(delegate(Point p)
			{
				p.SetFloor(mat.id, 43);
			});
		}
	}

	public void ForeachDeck(Action<Point> action)
	{
		int num = 1 + ((this.owner.dir % 2 == 1 && this.owner.id != "boat3") ? 1 : 0);
		int num2 = 1 + ((this.owner.dir % 2 == 0 && this.owner.id != "boat3") ? 1 : 0);
		int x = this.owner.pos.x - this.owner.W + num;
		int z = this.owner.pos.z + num2;
		int mx = this.owner.pos.x - num;
		int mz = this.owner.pos.z + this.owner.H - num2;
		this.owner.pos.ForeachMultiSize(this.owner.W, this.owner.H, delegate(Point p, bool main)
		{
			if (p.x > x && p.z >= z && p.x <= mx && p.z < mz)
			{
				action(p);
			}
		});
	}

	public SourceMaterial.Row GetWaterMat()
	{
		int mat = 67;
		this.owner.ForeachPoint(delegate(Point p, bool main)
		{
			if (p.cell.IsFloorWater)
			{
				mat = p.matFloor.id;
			}
		});
		return EClass.sources.materials.rows[mat];
	}
}
