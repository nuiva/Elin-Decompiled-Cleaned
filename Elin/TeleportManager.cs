using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TeleportManager : EClass
{
	public void SetID(TraitTeleporter t, int uidZone)
	{
		string id = t.id;
		int uid = t.owner.uid;
		if (id.IsEmpty())
		{
			this.Remove(uid);
			return;
		}
		TeleportManager.Item item = this.items.TryGetValue(uid, null);
		if (item == null)
		{
			item = new TeleportManager.Item();
			this.items.Add(uid, item);
		}
		item.uidZone = uidZone;
		item.id = id;
	}

	public Zone GetTeleportZone(TraitTeleporter t)
	{
		string id = t.id;
		int uid = t.owner.uid;
		if (id.IsEmpty())
		{
			return null;
		}
		List<Zone> list = new List<Zone>();
		foreach (KeyValuePair<int, TeleportManager.Item> keyValuePair in this.items)
		{
			if (keyValuePair.Key != uid && keyValuePair.Value.id == id)
			{
				Zone zone = EClass.game.spatials.Find(keyValuePair.Value.uidZone);
				if (zone != null && zone != EClass._zone)
				{
					list.Add(zone);
				}
			}
		}
		return list.RandomItem<Zone>();
	}

	public void Remove(int uidThing)
	{
		this.items.Remove(uidThing);
	}

	[JsonProperty]
	public Dictionary<int, TeleportManager.Item> items = new Dictionary<int, TeleportManager.Item>();

	public class Item : EClass
	{
		[JsonProperty]
		public string id;

		[JsonProperty]
		public int uidZone;
	}
}
