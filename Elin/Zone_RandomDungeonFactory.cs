public class Zone_RandomDungeonFactory : Zone_RandomDungeon
{
	public override int BaseElectricity => 1000;

	public override void OnGenerateMap()
	{
		PlaceRail(RailType.Factoy);
		EClass._map.ForeachCell(delegate(Cell c)
		{
			if (EClass.rnd(5) != 0 && c._block != 0 && !c.HasObj && !c.isSurrounded && !c.hasDoor)
			{
				c.GetSharedPoint().SetObj(106, 1, EClass.rnd(4));
			}
		});
		base.OnGenerateMap();
	}
}
