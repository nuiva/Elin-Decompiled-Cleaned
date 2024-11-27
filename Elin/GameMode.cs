using System;

[Flags]
public enum GameMode
{
	Reserved = 1,
	Adv = 2,
	Sim = 4,
	Build = 8,
	NoMap = 16,
	EloMap = 32,
	Bird = 64
}
