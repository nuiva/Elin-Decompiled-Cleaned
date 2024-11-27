using System;

public static class CheckExtension
{
	public static bool IsFail(this Check.Result r)
	{
		return r == Check.Result.CriticalFail || r == Check.Result.Fail;
	}

	public static bool IsPass(this Check.Result r)
	{
		return r == Check.Result.CriticalPass || r == Check.Result.Pass;
	}
}
