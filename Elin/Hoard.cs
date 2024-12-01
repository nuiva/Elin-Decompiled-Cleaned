using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class Hoard : EClass
{
	public enum Mode
	{
		all,
		lux,
		junk
	}

	public class Item : EClass
	{
		[JsonProperty]
		public string id;

		[JsonProperty]
		public int[] ints = new int[5];

		public BitArray32 bits;

		public int num
		{
			get
			{
				return ints[1];
			}
			set
			{
				ints[1] = value;
			}
		}

		public int show
		{
			get
			{
				return ints[2];
			}
			set
			{
				ints[2] = value;
			}
		}

		public bool random
		{
			get
			{
				return bits[0];
			}
			set
			{
				bits[0] = value;
			}
		}

		public bool floating
		{
			get
			{
				return bits[1];
			}
			set
			{
				bits[1] = value;
			}
		}

		public SourceCollectible.Row Source
		{
			get
			{
				if (!EClass.sources.collectibles.initialized)
				{
					EClass.sources.collectibles.Init();
				}
				return EClass.sources.collectibles.map[id];
			}
		}

		public bool IsUnique => Source.tag.Contains("unique");

		[OnSerializing]
		internal void OnSerializing(StreamingContext context)
		{
			ints[0] = (int)bits.Bits;
		}

		[OnDeserialized]
		internal void _OnDeserialized(StreamingContext context)
		{
			bits.Bits = (uint)ints[0];
		}

		public string Name(int n)
		{
			return "(" + "collectible".lang() + ") [" + Lang._rarity(Source.rarity) + "] " + Source.GetName() + " x " + n;
		}
	}

	public Dictionary<string, Item> items = new Dictionary<string, Item>();

	[JsonProperty]
	public List<Item> list = new List<Item>();

	[JsonProperty]
	public Mode mode;

	[JsonProperty]
	public int bg;

	[JsonProperty]
	public int reflection = 100;

	[JsonProperty]
	public int maxSounds = 28;

	[JsonProperty]
	public int volume = 40;

	[JsonProperty]
	public int hiScore;

	[JsonProperty]
	public int score;

	public bool shadow = true;

	public bool voice = true;

	public bool pixelPerfect;

	public bool hentai;

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		foreach (Item item in list)
		{
			items.Add(item.id, item);
		}
	}

	public Item AddRandom(int r, bool msg = true)
	{
		IEnumerable<SourceCollectible.Row> ie = EClass.sources.collectibles.rows.Where((SourceCollectible.Row a) => a.rarity == r);
		return Add(ie.RandomItem().id, 1, msg);
	}

	public Item AddRandom(bool msg = true)
	{
		string id = EClass.sources.collectibles.rows.RandomItem().id;
		return Add(id, 1, msg);
	}

	public Item Add(string id)
	{
		SourceCollectible.Row row = EClass.sources.collectibles.map[id];
		int num = ((row.num == 0) ? 10 : row.num);
		if (num != 1)
		{
			num *= 2;
		}
		Item item = Add(row.id, num);
		item.random = true;
		return item;
	}

	public Item Add(string id, int num, bool msg = false)
	{
		if (num == 0)
		{
			return items.TryGetValue(id);
		}
		Item value = null;
		if (!items.TryGetValue(id, out value))
		{
			value = new Item
			{
				id = id
			};
			list.Add(value);
			items[id] = value;
		}
		if (value.num == value.show)
		{
			value.show += num;
		}
		value.num += num;
		if (value.IsUnique && value.show > 1)
		{
			value.show = 1;
		}
		if (msg)
		{
			Msg.SetColor(EClass.Colors.GetRarityColor(value.Source.rarity, light: true));
			Msg.AquireItem(value.Name(num));
		}
		if (hentai)
		{
			value.show = value.num;
		}
		return value;
	}

	public Sprite GetSprite(string id)
	{
		SpriteSheet.Add("UI/Layer/Hoard/Molds/_sprites_hoard");
		return SpriteSheet.Get("_sprites_hoard_" + id);
	}

	public GameObject GetActor(string id)
	{
		EClass.sources.collectibles.Init();
		SourceCollectible.Row row = EClass.sources.collectibles.map[id];
		SpriteRenderer spriteRenderer = Object.Instantiate(ResourceCache.Load<SpriteRenderer>("UI/Layer/Hoard/Molds/" + row.prefab.IsEmpty("default")));
		spriteRenderer.sprite = GetSprite(id);
		return spriteRenderer.gameObject;
	}

	public void Clear()
	{
		items.Clear();
		list.Clear();
	}
}
