using System;

public class TileTypeFloor : TileType
{
	public override bool IsOccupyCell
	{
		get
		{
			return false;
		}
	}

	public override string LangPlaceType
	{
		get
		{
			return "place_Floor";
		}
	}

	public override BaseTileSelector.SelectType SelectType
	{
		get
		{
			return BaseTileSelector.SelectType.Multiple;
		}
	}

	public override BaseTileSelector.HitType HitType
	{
		get
		{
			return BaseTileSelector.HitType.Floor;
		}
	}

	public override bool IsBlockLiquid
	{
		get
		{
			return true;
		}
	}

	public override bool CanBuiltOnThing
	{
		get
		{
			return true;
		}
	}

	public override bool CanBuiltOnBlock
	{
		get
		{
			return true;
		}
	}

	public override bool IsFloorOrBridge
	{
		get
		{
			return true;
		}
	}

	public override bool IsFloor
	{
		get
		{
			return true;
		}
	}

	public override bool IsPlayFootSound
	{
		get
		{
			return true;
		}
	}

	public override bool CastShadowSelf
	{
		get
		{
			return true;
		}
	}
}
