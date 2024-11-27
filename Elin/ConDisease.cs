using System;
using System.Linq;

public class ConDisease : BadCondition
{
	public override bool PreventRegen
	{
		get
		{
			return this.GetPhase() > 0 || EClass.rnd(2) == 0;
		}
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.elements = new ElementContainer();
		this.elements.SetParent(this.owner);
	}

	public override void Tick()
	{
		if (EClass.rnd(20) == 0)
		{
			base.Mod((EClass.rnd(2) == 0) ? 1 : -1, false);
		}
		if (EClass.rnd(200) == 0)
		{
			SourceElement.Row row = (from e in EClass.sources.elements.rows
			where e.tag.Contains("primary")
			select e).RandomItem<SourceElement.Row>();
			this.elements.ModBase(row.id, -1);
		}
	}
}
