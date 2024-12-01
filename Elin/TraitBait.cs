public class TraitBait : TraitEquipItem
{
	public override int DefaultStock => 2 + EClass.rnd(10);

	public override Thing EQ
	{
		get
		{
			return EClass.player.eqBait;
		}
		set
		{
			EClass.player.eqBait = value;
		}
	}
}
