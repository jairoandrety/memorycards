using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MemoryCards
{
	[System.Serializable]
	public class PlayerData
	{
		public string playerName = string.Empty;
		public GameData gameData = new GameData();
	}

	[System.Serializable]
	public class GameData
	{
		public int total_clicks =0;
		public int total_time =0;
		public int pairs = 0;
		public int score = 0;
	}

	public class MemoryCardsHandler : MonoBehaviour
    {
		public UnityEvent OnStartedGame;
		public UnityEvent OnCompleteGame;

        [SerializeField] private MemoryCardsSettings settings;

        [SerializeField] private BoardLoader boardLoader = new BoardLoader();

		[SerializeField] private AudioClip audioClipMatch;
		[SerializeField] private AudioSource audioSourceMatch;

		private List<Slot> slots = new List<Slot>();
		private int columns = 0;
        private int rows = 0;
		private List<Slot> slotsSelected = new List<Slot>();

		private PlayerData playerData = new PlayerData();

		private DateTime timeStartGame;
		private DateTime timeEndGame;

		public MemoryCardsSettings Settings => settings;
		public BoardLoader BoardLoader => boardLoader;
		public List<Slot> Slots => slots;
		public PlayerData PlayerData => playerData;

		public int Columns => columns;
		public int Rows => rows;

		#region Create And Validate Board
		public bool CreateBoard()
        {
			boardLoader.LoadDataFromLocalJson();

			if (boardLoader.ValidateData(settings.minNumCards, settings.maxNumCards, settings.sprites.Count - 1))
			{
				Debug.LogError("there is not a data valid for start the game.");
				return false;
			}

			foreach (var block in boardLoader.Data.blocks)
			{
				if (block.R > rows)
					rows = block.R;

				if (block.C > columns)
					columns = block.C;
			}

			//Verify Number Repeat
			if (AssignNumberAndCheckRepeatPosition())
			{
				return false;
			}

			//Verify Occurrences
			if (VerifyOccurrences())
			{
				return false;
			}

			return true;
		}

        private bool AssignNumberAndCheckRepeatPosition()
        {
			foreach (var block in boardLoader.Data.blocks)
			{
				Slot slot = new Slot();

				slot.R = block.R - 1;
				slot.C = block.C - 1;

				bool exitsSlot = slots.Exists(i => i.R == block.R && i.C == slot.C);
				if (exitsSlot)
				{
					Debug.LogError("Slot has value, this slot is repeat");
					return true;
				}
				else
				{
					slot.number = block.number;
				}

				slots.Add(slot);
			}

			return false;
		}

        private bool VerifyOccurrences()
        {
			foreach (var block in boardLoader.Data.blocks)
			{
				int occurrences = CountOccurrences(block.number);

				if (occurrences < 2)
				{
					Debug.LogError("Exists a single block");
                    return true;
				}
				if (occurrences > 2)
				{
					Debug.LogError("The block number is being used more than twice");
                    return true;
                }
			}

            return false;
		}

		int CountOccurrences(int valueToFind)
		{
			return slots.FindAll(i => i.number == valueToFind).Count;
		}
		#endregion

		private void ClearBoard()
		{
			while (slots.Count > 0)
			{
				Destroy(slots[slots.Count -1].tile.gameObject);
				slots.RemoveAt(slots.Count - 1);
			}

			slots.Clear();
			slots = new List<Slot>();
		}

		public void StartGame()
		{
			ClearBoard();
			if(CreateBoard())
			{
				timeStartGame = DateTime.Now;
				playerData = new PlayerData();
				OnStartedGame?.Invoke();
			}
		}

		public void CheckSlotTile(int row, int col)
		{
			Slot slot = slots.Find(i=> i.R == row && i.C == col);
			CheckSlotTile(slot);
		}

		public void CheckSlotTile(Slot slot)
		{
			if (slot == null) return;

			if (slotsSelected.Contains(slot))
				return;

			playerData.gameData.total_clicks++;
			slotsSelected.Add(slot);

			if(slotsSelected.Count == 2)
			{
				StartCoroutine(CheckSlotsSelected());
			}

			CheckGameState();
		}

		private IEnumerator	CheckSlotsSelected()
		{
			EnableSlots(false);
			bool isMatched = slotsSelected[0].number.Equals(slotsSelected[1].number);

			if (isMatched)
			{
				slotsSelected.ForEach(i => i.SetMatched());
				playerData.gameData.pairs++;

				audioSourceMatch.clip = audioClipMatch;
				if (audioSourceMatch != null)
					audioSourceMatch.Play();
			}
			else
			{
				yield return new WaitForSeconds(0.25f);
				slotsSelected.ForEach(i => i.DeselectSlot());
			}

			slotsSelected.Clear();
			EnableSlots(true);
		}

		private void EnableSlots(bool value)
		{
			slots.ForEach(i => i.tile.Button.enabled = value);
		}

		private void CheckGameState()
		{
			bool win = slots.TrueForAll(i => i.IsMatched);

			if (win)
			{
				timeEndGame = DateTime.Now;

				TimeSpan difference = timeEndGame - timeStartGame;
				int seconds = (int)difference.TotalSeconds;
				playerData.gameData.total_time = seconds;

				int pairsScore = 500 /(playerData.gameData.total_clicks / playerData.gameData.pairs);
				int timeScore = 300 / (playerData.gameData.total_time / playerData.gameData.pairs);
				int totalScore = pairsScore + timeScore;

				if (totalScore < 0)
					totalScore = 0;

				playerData.gameData.score = totalScore;

				OnCompleteGame?.Invoke();
			}
		}
	}
}