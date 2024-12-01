public class TraitFireworkLauncher : TraitFirework
{
	public override bool OnUse(Chara c)
	{
		Toggle(!owner.isOn);
		return true;
	}
}
