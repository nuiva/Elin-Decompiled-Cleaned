using System;

public class TraitFireworkLauncher : TraitFirework
{
	public override bool OnUse(Chara c)
	{
		this.Toggle(!this.owner.isOn, false);
		return true;
	}
}
