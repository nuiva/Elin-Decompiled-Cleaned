public class ElementContainerFaction : ElementContainer
{
	public bool isDirty;

	public bool IsEffective(Thing t)
	{
		if (t.c_idDeity.IsEmpty())
		{
			return true;
		}
		return t.c_idDeity == EClass.pc.idFaith;
	}

	public void OnEquip(Chara c, Thing t)
	{
		if (c.IsPCFaction)
		{
			OnEquip(t);
		}
	}

	public void OnUnequip(Chara c, Thing t)
	{
		if (c.IsPCFaction)
		{
			OnUnequip(t);
		}
	}

	public void OnEquip(Thing t)
	{
		if (!IsEffective(t))
		{
			return;
		}
		foreach (Element value in t.elements.dict.Values)
		{
			if (value.IsGlobalElement)
			{
				ModBase(value.id, value.Value).vExp = value.vExp;
				isDirty = true;
			}
		}
		CheckDirty();
	}

	public void OnUnequip(Thing t)
	{
		if (!IsEffective(t))
		{
			return;
		}
		foreach (Element value in t.elements.dict.Values)
		{
			if (value.IsGlobalElement)
			{
				ModBase(value.id, -value.Value);
				isDirty = true;
			}
		}
		CheckDirty();
	}

	public void CheckDirty()
	{
		if (!isDirty)
		{
			return;
		}
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.IsPCFaction)
			{
				value.Refresh();
			}
		}
		if (EClass.core.IsGameStarted)
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.IsPCFactionMinion)
				{
					chara.Refresh();
				}
			}
		}
		isDirty = false;
	}

	public void OnLeaveFaith()
	{
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.IsPCFaction)
			{
				OnRemoveMember(value);
			}
		}
	}

	public void OnJoinFaith()
	{
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.IsPCFaction)
			{
				OnAddMemeber(value);
			}
		}
	}

	public void OnAddMemeber(Chara c)
	{
		foreach (BodySlot slot in c.body.slots)
		{
			if (slot.thing != null)
			{
				OnEquip(slot.thing);
			}
		}
	}

	public void OnRemoveMember(Chara c)
	{
		foreach (BodySlot slot in c.body.slots)
		{
			if (slot.thing != null)
			{
				OnUnequip(slot.thing);
			}
		}
	}
}
