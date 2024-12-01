using System.Collections.Generic;
using UnityEngine;

public class PropsManager : EClass
{
	public PropsStocked stocked = new PropsStocked();

	public PropsInstalled installed = new PropsInstalled();

	public PropsRoaming roaming = new PropsRoaming();

	public List<Card> deconstructing = new List<Card>();

	public List<Card> sales = new List<Card>();

	private bool dbg => EClass.debug.debugProps;

	public void Init()
	{
		stocked.Init();
		installed.Init();
		roaming.Init();
	}

	public void OnCardAddedToZone(Card c)
	{
		if (!c.isThing)
		{
			if (c.isSale)
			{
				sales.Add(c);
			}
			return;
		}
		switch (c.placeState)
		{
		case PlaceState.installed:
			installed.Add(c);
			if (c.isSale)
			{
				sales.Add(c);
			}
			break;
		case PlaceState.roaming:
			roaming.Add(c);
			break;
		case PlaceState.stocked:
			stocked.Add(c);
			break;
		}
	}

	public void OnSetPlaceState(Card c, PlaceState? newType, PlaceState? oldType = null)
	{
		if (!c.isThing || c.parent != EClass._zone)
		{
			return;
		}
		if (oldType.HasValue)
		{
			switch (oldType)
			{
			case PlaceState.roaming:
				if (dbg && !roaming.all.Contains(c))
				{
					Debug.LogError("remove roaming" + c);
				}
				roaming.Remove(c);
				break;
			case PlaceState.stocked:
				if (dbg && !stocked.all.Contains(c))
				{
					Debug.LogError("remove stocked" + c);
				}
				stocked.Remove(c);
				break;
			case PlaceState.installed:
				if (dbg && !installed.all.Contains(c))
				{
					Debug.LogError("remove installed" + c);
				}
				installed.Remove(c);
				break;
			}
			if (c.isSale)
			{
				sales.Remove(c);
			}
		}
		if (!newType.HasValue || !newType.HasValue)
		{
			return;
		}
		switch (newType.GetValueOrDefault())
		{
		case PlaceState.roaming:
			if (dbg && roaming.all.Contains(c))
			{
				Debug.LogError("add roaming" + c);
			}
			roaming.Add(c);
			break;
		case PlaceState.stocked:
			if (dbg && stocked.all.Contains(c))
			{
				Debug.LogError("add stocked" + c);
			}
			stocked.Add(c);
			break;
		case PlaceState.installed:
			if (dbg && installed.all.Contains(c))
			{
				Debug.LogError("add installed" + c);
			}
			installed.Add(c);
			if (c.isSale)
			{
				sales.Add(c);
			}
			break;
		}
	}
}
