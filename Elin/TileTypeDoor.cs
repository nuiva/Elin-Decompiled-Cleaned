using System;

public class TileTypeDoor : TileTypeObj
{
	public override string LangPlaceType
	{
		get
		{
			return "place_Door";
		}
	}

	public override bool FreeStyle
	{
		get
		{
			return false;
		}
	}

	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool CanBuiltOnBlock
	{
		get
		{
			return true;
		}
	}

	public override bool IsDoor
	{
		get
		{
			return true;
		}
	}

	public override bool CanBeHeld
	{
		get
		{
			return EClass._zone.IsPCFaction || EClass._zone is Zone_Tent;
		}
	}
}
