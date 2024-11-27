using System;
using Newtonsoft.Json;

public class ConfigAutoCombat : EClass
{
	public bool enable
	{
		get
		{
			return this.idType != "_";
		}
	}

	[JsonProperty]
	public string idType = "archer";

	[JsonProperty]
	public bool abortOnAllyDying;

	[JsonProperty]
	public bool abortOnAllyDead;

	[JsonProperty]
	public bool abortOnEnemyDead;

	[JsonProperty]
	public bool abortOnHalfHP;

	[JsonProperty]
	public bool abortOnKill;

	[JsonProperty]
	public bool abortOnItemLoss;

	[JsonProperty]
	public bool bUseHotBar;

	[JsonProperty]
	public bool bUseFav;

	[JsonProperty]
	public bool bUseInventory;

	[JsonProperty]
	public bool bCastParty;

	[JsonProperty]
	public bool bDontChangeTarget;

	[JsonProperty]
	public bool bDontAutoAttackNeutral;

	[JsonProperty]
	public bool bDontChase;

	[JsonProperty]
	public bool turbo;

	[JsonProperty]
	public bool detail;
}
