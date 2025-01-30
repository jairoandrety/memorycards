using System;
using System.IO;
using UnityEngine;

namespace MemoryCards
{
	public class DataHandler : MonoBehaviour
	{
		private string filePath;

		void Start()
		{
			filePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
		}

		public void SaveData(PlayerData playerData)
		{
			Leaderboard leaderboard = LoadData();
			leaderboard.scores.Add(playerData);
			leaderboard.scores.Sort((a, b) => b.gameData.score.CompareTo(a.gameData.score));

			string json = JsonUtility.ToJson(leaderboard, true);
			File.WriteAllText(filePath, json);
			PlayerPrefs.SetString("LastPlayer", playerData.playerName);
			PlayerPrefs.Save();
			Debug.Log("Datos guardados en: " + filePath);
		}

		public Leaderboard LoadData()
		{
			if (File.Exists(filePath))
			{
				string json = File.ReadAllText(filePath);
				return JsonUtility.FromJson<Leaderboard>(json);
			}
			else
			{
				Debug.LogWarning("No se encontró archivo de datos, creando nuevo.");
				return new Leaderboard();
			}
		}

		public string GetLastPlayer()
		{
			return PlayerPrefs.GetString("LastPlayer", "");
		}
	}
}