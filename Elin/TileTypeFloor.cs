public class TileTypeFloor : TileType
{
	public override bool IsOccupyCell => false;

	public override string LangPlaceType => "place_Floor";

	public override BaseTileSelector.SelectType SelectType => BaseTileSelector.SelectType.Multiple;

	public override BaseTileSelector.HitType HitType => BaseTileSelector.HitType.Floor;

	public override bool IsBlockLiquid => true;

	public override bool CanBuiltOnThing => true;

	public override bool CanBuiltOnBlock => true;

	public override bool IsFloorOrBridge => true;

	public override bool IsFloor => true;

	public override bool IsPlayFootSound => true;

	public override bool CastShadowSelf => true;
}
