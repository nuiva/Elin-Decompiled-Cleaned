public class TraitBookResident : TraitItem
{
	public override string LangUse => "actRead";

	public override bool OnUse(Chara c)
	{
		if (!EClass._zone.IsPCFaction)
		{
			Msg.SayNothingHappen();
			return false;
		}
		EClass.ui.AddLayer<LayerPeople>();
		return false;
	}
}
