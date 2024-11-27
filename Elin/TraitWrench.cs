using System;
using UnityEngine;

public class TraitWrench : Trait
{
	public string ID
	{
		get
		{
			return base.GetParam(1, null);
		}
	}

	public override bool IsTool
	{
		get
		{
			return true;
		}
	}

	public bool IsValidTarget(Thing t)
	{
		if (!t.IsInstalled)
		{
			return false;
		}
		string id = this.ID;
		if (id == "tent_elec")
		{
			return t.trait is TraitTent && (t.trait as TraitTent).zone != null;
		}
		if (id == "bed")
		{
			return t.trait is TraitBed;
		}
		if (!(id == "storage") && !(id == "fridge"))
		{
			return (id == "extend_v" || id == "extend_h") && !(t.trait is TraitMagicChest) && !(t.trait is TraitDeliveryChest) && !t.trait.IsSpecialContainer && t.IsContainer && t.trait is TraitContainer;
		}
		return t.trait is TraitMagicChest;
	}

	public bool Upgrade(Thing t)
	{
		string id = this.ID;
		if (!(id == "tent_elec"))
		{
			if (!(id == "bed"))
			{
				if (!(id == "storage"))
				{
					if (!(id == "fridge"))
					{
						if (id == "extend_v" || id == "extend_h")
						{
							bool flag = this.ID == "extend_v";
							TraitContainer traitContainer = t.trait as TraitContainer;
							if (t.things.GridSize == 0)
							{
								return false;
							}
							if (flag)
							{
								if (t.things.height != traitContainer.Height)
								{
									return false;
								}
								t.things.SetSize(t.things.width, t.things.height + 1);
							}
							else
							{
								if (t.things.width != traitContainer.Width)
								{
									return false;
								}
								t.things.SetSize(t.things.width + 1, t.things.height);
							}
						}
					}
					else
					{
						if (t.c_containerUpgrade.cool != 0)
						{
							return false;
						}
						t.c_containerUpgrade.cool = 1;
						t.elements.SetBase(405, 50, 0);
					}
				}
				else
				{
					t.c_containerUpgrade.cap += 20;
				}
			}
			else
			{
				int c_containerSize = t.c_containerSize;
				t.c_containerSize = c_containerSize + 1;
				if (EClass.debug.enable)
				{
					t.c_containerSize += 1000;
				}
			}
		}
		else
		{
			(t.trait as TraitTent).zone.elements.ModBase(2201, 2);
		}
		if (EClass.Branch != null)
		{
			EClass.Branch.resources.SetDirty();
		}
		return true;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Things.ForEach(delegate(Thing t)
		{
			if (!this.IsValidTarget(t))
			{
				return;
			}
			p.TrySetAct("actWrench".lang(t.Name, null, null, null, null), delegate()
			{
				if (this.Upgrade(t))
				{
					Msg.Say("upgrade", t, this.owner.GetName(NameStyle.Full, 1), null, null);
					SE.Play("build_area");
					t.PlayEffect("buff", true, 0f, default(Vector3));
					this.owner.ModNum(-1, true);
					EClass._zone.RefreshElectricity();
				}
				else
				{
					Msg.Say("noMoreUpgrade", t, this.owner.GetName(NameStyle.Full, 1), null, null);
				}
				return false;
			}, null, 1);
		});
	}
}
