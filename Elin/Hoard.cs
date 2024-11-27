using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class Hoard : EClass
{
	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		foreach (Hoard.Item item in this.list)
		{
			this.items.Add(item.id, item);
		}
	}

	public Hoard.Item AddRandom(int r, bool msg = true)
	{
		IEnumerable<SourceCollectible.Row> ie = from a in EClass.sources.collectibles.rows
		where a.rarity == r
		select a;
		return this.Add(ie.RandomItem<SourceCollectible.Row>().id, 1, msg);
	}

	public Hoard.Item AddRandom(bool msg = true)
	{
		string id = EClass.sources.collectibles.rows.RandomItem<SourceCollectible.Row>().id;
		return this.Add(id, 1, msg);
	}

	public Hoard.Item Add(string id)
	{
		SourceCollectible.Row row = EClass.sources.collectibles.map[id];
		int num = (row.num == 0) ? 10 : row.num;
		if (num != 1)
		{
			num *= 2;
		}
		Hoard.Item item = this.Add(row.id, num, false);
		item.random = true;
		return item;
	}

	public Hoard.Item Add(string id, int num, bool msg = false)
	{
		if (num == 0)
		{
			return this.items.TryGetValue(id, null);
		}
		Hoard.Item item = null;
		if (!this.items.TryGetValue(id, out item))
		{
			item = new Hoard.Item
			{
				id = id
			};
			this.list.Add(item);
			this.items[id] = item;
		}
		if (item.num == item.show)
		{
			item.show += num;
		}
		item.num += num;
		if (item.IsUnique && item.show > 1)
		{
			item.show = 1;
		}
		if (msg)
		{
			Msg.SetColor(EClass.Colors.GetRarityColor(item.Source.rarity, true));
			Msg.AquireItem(item.Name(num));
		}
		if (this.hentai)
		{
			item.show = item.num;
		}
		return item;
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
		SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate<SpriteRenderer>(ResourceCache.Load<SpriteRenderer>("UI/Layer/Hoard/Molds/" + row.prefab.IsEmpty("default")));
		spriteRenderer.sprite = this.GetSprite(id);
		return spriteRenderer.gameObject;
	}

	public void Clear()
	{
		this.items.Clear();
		this.list.Clear();
	}

	public Dictionary<string, Hoard.Item> items = new Dictionary<string, Hoard.Item>();

	[JsonProperty]
	public List<Hoard.Item> list = new List<Hoard.Item>();

	[JsonProperty]
	public Hoard.Mode mode;

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

	public enum Mode
	{
		all,
		lux,
		junk
	}

	public class Item : EClass
	{
		public int num
		{
			get
			{
				return this.ints[1];
			}
			set
			{
				this.ints[1] = value;
			}
		}

		public int show
		{
			get
			{
				return this.ints[2];
			}
			set
			{
				this.ints[2] = value;
			}
		}

		public bool random
		{
			get
			{
				return this.bits[0];
			}
			set
			{
				this.bits[0] = value;
			}
		}

		public bool floating
		{
			get
			{
				return this.bits[1];
			}
			set
			{
				this.bits[1] = value;
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
				return EClass.sources.collectibles.map[this.id];
			}
		}

		public bool IsUnique
		{
			get
			{
				return this.Source.tag.Contains("unique");
			}
		}

		[OnSerializing]
		internal void OnSerializing(StreamingContext context)
		{
			this.ints[0] = (int)this.bits.Bits;
		}

		[OnDeserialized]
		internal void _OnDeserialized(StreamingContext context)
		{
			this.bits.Bits = (uint)this.ints[0];
		}

		public string Name(int n)
		{
			return string.Concat(new string[]
			{
				"(",
				"collectible".lang(),
				") [",
				Lang._rarity(this.Source.rarity),
				"] ",
				this.Source.GetName(),
				" x ",
				n.ToString()
			});
		}

		[JsonProperty]
		public string id;

		[JsonProperty]
		public int[] ints = new int[5];

		public BitArray32 bits;
	}
}
