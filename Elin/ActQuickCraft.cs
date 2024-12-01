public class ActQuickCraft : Ability
{
	public override bool Perform()
	{
		EClass.ui.AddLayer<LayerCraft>().SetFactory(null);
		return false;
	}
}
