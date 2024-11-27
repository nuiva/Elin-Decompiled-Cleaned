using System;

public class Int3
{
	public int x
	{
		get
		{
			return this.i[0];
		}
		set
		{
			this.i[0] = value;
		}
	}

	public int y
	{
		get
		{
			return this.i[1];
		}
		set
		{
			this.i[1] = value;
		}
	}

	public int z
	{
		get
		{
			return this.i[2];
		}
		set
		{
			this.i[2] = value;
		}
	}

	public Int3()
	{
	}

	public Int3(int _x, int _y, int _z)
	{
		this.Set(_x, _y, _z);
	}

	public Int3(float _x, float _y, float _z)
	{
		this.Set(_x, _y, _z);
	}

	public void Set(int _x, int _y, int _z)
	{
		this.i[0] = _x;
		this.i[1] = _y;
		this.i[2] = _z;
	}

	public void Set(float _x, float _y, float _z)
	{
		this.i[0] = (int)_x;
		this.i[1] = (int)_y;
		this.i[2] = (int)_z;
	}

	public int[] i = new int[3];
}
