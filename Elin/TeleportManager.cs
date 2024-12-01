using System.Collections.Generic;
using Newtonsoft.Json;

public class TeleportManager : EClass
{
	public class Item : EClass
	{
		[JsonProperty]
		public string id;

		[JsonProperty]
		public int uidZone;
	}

	[JsonProperty]
	public Dictionary<int, Item> items = new Dictionary<int, Item>();

	public void SetID(TraitTeleporter t, int uidZone)
	{
		string id = t.id;
		int uid = t.owner.uid;
		if (id.IsEmpty())
		{
			Remove(uid);
			return;
		}
		Item item = items.TryGetValue(uid);
		if (item == null)
		{
			item = new Item();
			items.Add(uid, item);
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
		foreach (KeyValuePair<int, Item> item in items)
		{
			if (item.Key != uid && item.Value.id == id)
			{
				Zone zone = EClass.game.spatials.Find(item.Value.uidZone);
				if (zone != null && zone != EClass._zone)
				{
					list.Add(zone);
				}
			}
		}
		return list.RandomItem();
	}

	public void Remove(int uidThing)
	{
		items.Remove(uidThing);
	}
}
