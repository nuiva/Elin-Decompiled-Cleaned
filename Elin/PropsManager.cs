using System;
using System.Collections.Generic;
using UnityEngine;

public class PropsManager : EClass
{
	private bool dbg
	{
		get
		{
			return EClass.debug.debugProps;
		}
	}

	public void Init()
	{
		this.stocked.Init();
		this.installed.Init();
		this.roaming.Init();
	}

	public void OnCardAddedToZone(Card c)
	{
		if (!c.isThing)
		{
			if (c.isSale)
			{
				this.sales.Add(c);
			}
			return;
		}
		switch (c.placeState)
		{
		case PlaceState.roaming:
			this.roaming.Add(c);
			return;
		case PlaceState.stocked:
			this.stocked.Add(c);
			break;
		case PlaceState.installed:
			this.installed.Add(c);
			if (c.isSale)
			{
				this.sales.Add(c);
				return;
			}
			break;
		default:
			return;
		}
	}

	public void OnSetPlaceState(Card c, PlaceState? newType, PlaceState? oldType = null)
	{
		if (!c.isThing)
		{
			return;
		}
		if (c.parent != EClass._zone)
		{
			return;
		}
		if (oldType != null)
		{
			if (oldType != null)
			{
				switch (oldType.GetValueOrDefault())
				{
				case PlaceState.roaming:
					if (this.dbg && !this.roaming.all.Contains(c))
					{
						Debug.LogError("remove roaming" + ((c != null) ? c.ToString() : null));
					}
					this.roaming.Remove(c);
					break;
				case PlaceState.stocked:
					if (this.dbg && !this.stocked.all.Contains(c))
					{
						Debug.LogError("remove stocked" + ((c != null) ? c.ToString() : null));
					}
					this.stocked.Remove(c);
					break;
				case PlaceState.installed:
					if (this.dbg && !this.installed.all.Contains(c))
					{
						Debug.LogError("remove installed" + ((c != null) ? c.ToString() : null));
					}
					this.installed.Remove(c);
					break;
				}
			}
			if (c.isSale)
			{
				this.sales.Remove(c);
			}
		}
		if (newType != null && newType != null)
		{
			switch (newType.GetValueOrDefault())
			{
			case PlaceState.roaming:
				if (this.dbg && this.roaming.all.Contains(c))
				{
					Debug.LogError("add roaming" + ((c != null) ? c.ToString() : null));
				}
				this.roaming.Add(c);
				return;
			case PlaceState.stocked:
				if (this.dbg && this.stocked.all.Contains(c))
				{
					Debug.LogError("add stocked" + ((c != null) ? c.ToString() : null));
				}
				this.stocked.Add(c);
				return;
			case PlaceState.installed:
				if (this.dbg && this.installed.all.Contains(c))
				{
					Debug.LogError("add installed" + ((c != null) ? c.ToString() : null));
				}
				this.installed.Add(c);
				if (c.isSale)
				{
					this.sales.Add(c);
				}
				break;
			default:
				return;
			}
		}
	}

	public PropsStocked stocked = new PropsStocked();

	public PropsInstalled installed = new PropsInstalled();

	public PropsRoaming roaming = new PropsRoaming();

	public List<Card> deconstructing = new List<Card>();

	public List<Card> sales = new List<Card>();
}
