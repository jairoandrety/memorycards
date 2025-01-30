using System.Collections.Generic;

namespace MemoryCards
{
	[System.Serializable]
	public class Leaderboard
	{
		public List<PlayerData> scores = new List<PlayerData>();
	}
}