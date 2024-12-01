using System.Linq;

public class ConDisease : BadCondition
{
	public override bool PreventRegen
	{
		get
		{
			if (GetPhase() <= 0)
			{
				return EClass.rnd(2) == 0;
			}
			return true;
		}
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		elements = new ElementContainer();
		elements.SetParent(owner);
	}

	public override void Tick()
	{
		if (EClass.rnd(20) == 0)
		{
			Mod((EClass.rnd(2) == 0) ? 1 : (-1));
		}
		if (EClass.rnd(200) == 0)
		{
			SourceElement.Row row = EClass.sources.elements.rows.Where((SourceElement.Row e) => e.tag.Contains("primary")).RandomItem();
			elements.ModBase(row.id, -1);
		}
	}
}
