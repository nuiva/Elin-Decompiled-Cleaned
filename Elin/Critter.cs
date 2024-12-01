using UnityEngine;

public class Critter : EClass
{
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

	public virtual int[] animeTiles => null;

	public virtual int[] idleTiles => null;

	public virtual int AnimeTile => animeTiles[index % animeTiles.Length];

	public virtual int IdleTile => idleTiles[index % idleTiles.Length];

	public virtual int SnowTile => 0;

	public virtual int Interval => 5;

	public virtual float BaseSpeed => 1f;

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
		animeTimer -= gameDelta * speed * 100f;
		if (animeTimer < 0f)
		{
			index++;
			animeTimer = 2f;
		}
		if (dir.x != 0f || dir.y != 0f)
		{
			tile = AnimeTile;
		}
		else
		{
			tile = IdleTile;
		}
		dirTimer -= gameDelta;
		if (dirTimer < 0f)
		{
			dirTimer = Interval + EClass.rnd(Interval) / 2;
			dir.x = EClass.rnd(3) - 1;
			dir.y = EClass.rnd(3) - 1;
			speed = 0.01f * (float)(EClass.rnd(20) + 1) * BaseSpeed;
			reverse = dir.x > 0f;
		}
		x += dir.x * gameDelta * speed;
		if (x > 0.12f)
		{
			x = 0.12f;
			dir.x = 0f;
		}
		else if (x < -0.12f)
		{
			x = -0.12f;
			dir.x = 0f;
		}
		y += dir.y * gameDelta * speed;
		if (y > 0.12f)
		{
			y = 0.12f;
			dir.y = 0f;
		}
		else if (y < -0.12f)
		{
			y = -0.12f;
			dir.y = 0f;
		}
	}
}
