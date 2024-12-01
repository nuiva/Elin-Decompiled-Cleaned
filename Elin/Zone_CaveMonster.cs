public class Zone_CaveMonster : Zone_RandomDungeon
{
	public override int LvBoss => -1;

	public override void OnGenerateRooms(BaseMapGen gen)
	{
		for (int i = 0; i < 1 + EClass.rnd(3); i++)
		{
			gen.TryAddMapPiece((i == 0) ? MapPiece.Type.Treasure : MapPiece.Type.Any, 0f);
		}
	}
}
