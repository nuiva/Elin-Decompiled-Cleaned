using System;
using UnityEngine;

public class Critter : EClass
{
	public virtual int[] animeTiles
	{
		get
		{
			return null;
		}
	}

	public virtual int[] idleTiles
	{
		get
		{
			return null;
		}
	}

	public virtual int AnimeTile
	{
		get
		{
			return this.animeTiles[this.index % this.animeTiles.Length];
		}
	}

	public virtual int IdleTile
	{
		get
		{
			return this.idleTiles[this.index % this.idleTiles.Length];
		}
	}

	public virtual int SnowTile
	{
		get
		{
			return 0;
		}
	}

	public virtual int Interval
	{
		get
		{
			return 5;
		}
	}

	public virtual float BaseSpeed
	{
		get
		{
			return 1f;
		}
	}

	public static Critter Create(Cell cell)
	{
		BiomeProfile biomeProfile = cell.sourceFloor.biome ?? EClass.core.refs.biomes.Plain;
		if (biomeProfile == null)
		{
			return null;
		}
		if (cell.IsTopWater)
		{
			if (EClass.rnd(12) == 0)
			{
				return new CritterFish();
			}
			return null;
		}
		else
		{
			if (biomeProfile.id == BiomeID.Snow)
			{
				return null;
			}
			if (EClass.rnd(12) == 0)
			{
				return new CritterFrog();
			}
			if (EClass.rnd(6) == 0)
			{
				return new CritterFrogSmall();
			}
			if (EClass.rnd(12) == 0)
			{
				return new CritterCancer();
			}
			if (EClass.rnd(6) == 0)
			{
				return new CritterCancerSmall();
			}
			if (EClass.rnd(12) == 0)
			{
				return new CritterRat();
			}
			if (EClass.rnd(6) == 0)
			{
				return new CritterRatSmall();
			}
			if (EClass.rnd(12) == 0)
			{
				return new CritterRoach();
			}
			if (EClass.rnd(6) == 0)
			{
				return new CritterRoachSmall();
			}
			return new CritterRandom();
		}
	}

	public static void RebuildCritter(Cell cell)
	{
		cell.critter = null;
		if (cell.sourceObj.id == 68)
		{
			cell.critter = new CritterFish();
		}
	}

	public void Update()
	{
		float gameDelta = Core.gameDelta;
		this.animeTimer -= gameDelta * this.speed * 100f;
		if (this.animeTimer < 0f)
		{
			this.index++;
			this.animeTimer = 2f;
		}
		if (this.dir.x != 0f || this.dir.y != 0f)
		{
			this.tile = this.AnimeTile;
		}
		else
		{
			this.tile = this.IdleTile;
		}
		this.dirTimer -= gameDelta;
		if (this.dirTimer < 0f)
		{
			this.dirTimer = (float)(this.Interval + EClass.rnd(this.Interval) / 2);
			this.dir.x = (float)(EClass.rnd(3) - 1);
			this.dir.y = (float)(EClass.rnd(3) - 1);
			this.speed = 0.01f * (float)(EClass.rnd(20) + 1) * this.BaseSpeed;
			this.reverse = (this.dir.x > 0f);
		}
		this.x += this.dir.x * gameDelta * this.speed;
		if (this.x > 0.12f)
		{
			this.x = 0.12f;
			this.dir.x = 0f;
		}
		else if (this.x < -0.12f)
		{
			this.x = -0.12f;
			this.dir.x = 0f;
		}
		this.y += this.dir.y * gameDelta * this.speed;
		if (this.y > 0.12f)
		{
			this.y = 0.12f;
			this.dir.y = 0f;
			return;
		}
		if (this.y < -0.12f)
		{
			this.y = -0.12f;
			this.dir.y = 0f;
		}
	}

	public const float R = 0.12f;

	public int tile;

	public int index;

	public Vector2 dir;

	public float x;

	public float y;

	public float dirTimer;

	public float animeTimer;

	public float speed = 0.1f;

	public bool reverse;
}
