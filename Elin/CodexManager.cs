using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CodexManager : EClass
{
	public CodexCreature GetOrCreate(string id)
	{
		CodexCreature codexCreature = this.creatures.TryGetValue(id, null);
		if (codexCreature == null)
		{
			codexCreature = new CodexCreature
			{
				id = id
			};
			this.creatures[id] = codexCreature;
		}
		return codexCreature;
	}

	public void OnLoad()
	{
		foreach (KeyValuePair<string, CodexCreature> keyValuePair in this.creatures)
		{
			keyValuePair.Value.id = keyValuePair.Key;
		}
	}

	public void AddCard(string id, int num = 1)
	{
		this.GetOrCreate(id).numCard += num;
	}

	public bool Has(string id)
	{
		return this.creatures.ContainsKey(id);
	}

	public void MarkCardDrop(string id)
	{
		this.GetOrCreate(id).droppedCard = true;
	}

	public bool DroppedCard(string id)
	{
		return this.creatures.ContainsKey(id) && this.creatures[id].droppedCard;
	}

	public void AddKill(string id)
	{
		CodexCreature orCreate = this.GetOrCreate(id);
		int kills = orCreate.kills;
		orCreate.kills = kills + 1;
	}

	public void AddWeakspot(string id)
	{
		CodexCreature orCreate = this.GetOrCreate(id);
		int weakspot = orCreate.weakspot;
		orCreate.weakspot = weakspot + 1;
	}

	public void AddSpawn(string id)
	{
		CodexCreature orCreate = this.GetOrCreate(id);
		int spawns = orCreate.spawns;
		orCreate.spawns = spawns + 1;
	}

	public List<CardRow> ListKills()
	{
		List<CardRow> list = new List<CardRow>();
		foreach (KeyValuePair<string, CodexCreature> keyValuePair in this.creatures)
		{
			if (keyValuePair.Value.kills > 0)
			{
				CardRow cardRow = EClass.sources.cards.map.TryGetValue(keyValuePair.Key, null);
				if (cardRow != null && !cardRow.HasTag(CTAG.noRandomProduct))
				{
					list.Add(cardRow);
				}
			}
		}
		return list;
	}

	[JsonProperty]
	public Dictionary<string, CodexCreature> creatures = new Dictionary<string, CodexCreature>();
}
