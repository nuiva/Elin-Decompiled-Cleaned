public class AttbMain : Element
{
	public override bool CanGainExp => true;

	public override int DisplayValue
	{
		get
		{
			if (id != 79 || owner.Chara == null)
			{
				return base.DisplayValue;
			}
			return owner.Chara.Speed;
		}
	}

	public override int MinPotential => 80;

	public override bool CanLink(ElementContainer owner)
	{
		return !base.IsGlobalElement;
	}

	public override void OnChangeValue()
	{
		Chara chara = owner.Chara;
		switch (id)
		{
		case 70:
		case 71:
			chara?.SetDirtyWeight();
			break;
		case 79:
			chara?.SetDirtySpeed();
			break;
		}
	}
}
