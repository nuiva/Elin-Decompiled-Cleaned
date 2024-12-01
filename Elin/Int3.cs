public class Int3
{
	public int[] i = new int[3];

	public int x
	{
		get
		{
			return i[0];
		}
		set
		{
			i[0] = value;
		}
	}

	public int y
	{
		get
		{
			return i[1];
		}
		set
		{
			i[1] = value;
		}
	}

	public int z
	{
		get
		{
			return i[2];
		}
		set
		{
			i[2] = value;
		}
	}

	public Int3()
	{
	}

	public Int3(int _x, int _y, int _z)
	{
		Set(_x, _y, _z);
	}

	public Int3(float _x, float _y, float _z)
	{
		Set(_x, _y, _z);
	}

	public void Set(int _x, int _y, int _z)
	{
		i[0] = _x;
		i[1] = _y;
		i[2] = _z;
	}

	public void Set(float _x, float _y, float _z)
	{
		i[0] = (int)_x;
		i[1] = (int)_y;
		i[2] = (int)_z;
	}
}
