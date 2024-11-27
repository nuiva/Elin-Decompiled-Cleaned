using System;

public class TraitNote : Trait
{
	public override void OnCreate(int lv)
	{
		if (base.GetParam(1, null) != null)
		{
			this.owner.c_note = base.GetParam(1, null);
		}
	}

	public override void OnImportMap()
	{
		this.OnCreate(EClass._zone.DangerLv);
	}

	public override void TrySetAct(ActPlan p)
	{
		if (p.altAction)
		{
			p.TrySetAct("actWrite", delegate()
			{
				Dialog.InputName("dialogWriteNote", this.owner.c_note, delegate(bool cancel, string text)
				{
					if (!cancel)
					{
						this.owner.c_note = text;
					}
				}, Dialog.InputType.Default);
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}
}
