using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MemoryCards
{
	public class MemoryCardsView : MonoBehaviour
	{
		[SerializeField] private MemoryCardsHandler memoryCardsHandler;
		[SerializeField] private DataHandler dataHandler;

		[SerializeField] private GameObject container;
		[SerializeField] private Tile tilePrefab;

		[SerializeField] private Button buttonStart;
		[SerializeField] private Button buttonRestart;

		[Header("Leaderboard")]
		[SerializeField] private RectTransform panelLeaderboard;
		[SerializeField] private RectTransform panelLeaderboardContainer;
		[SerializeField] private LeaderboardItemView leaderboardItemPrefab;
		[SerializeField] private List<LeaderboardItemView> leaderboardItems;

		[SerializeField] private RectTransform panelSaveScore;
		[SerializeField] private TMP_InputField tMP_InputField;
		[SerializeField] private Button buttonSaveScore;

		private void Start()
		{
			memoryCardsHandler.OnStartedGame.AddListener(OnStartedGame);
			memoryCardsHandler.OnCompleteGame.AddListener(OnCompleteGame);

			buttonStart.onClick.AddListener(StartGame);
			buttonRestart.onClick.AddListener(StartGame);

			buttonSaveScore.onClick.AddListener(SaveScore);

			buttonStart.interactable = true;
			buttonRestart.interactable = false;

			ShowLeaderboard();
		}

		public void StartGame()
		{
			memoryCardsHandler.StartGame();
		}

		private void OnStartedGame()
		{
			DrawBoard();

			buttonStart.interactable = false;
			buttonRestart.interactable = true;

			ShowLeaderboard();
		}

		private void DrawBoard()
		{
			foreach (var slot in memoryCardsHandler.Slots)
			{
				Tile tile = Instantiate(tilePrefab, container.transform);

				SetTileSizeAndPosition(tile, slot.R, slot.C);

				if (slot != null)
				{
					slot.SetTile(tile);
					tile.SetValues(slot.number, memoryCardsHandler.Settings.GetSpriteByNumber(slot.number), memoryCardsHandler.Settings.colorHide, memoryCardsHandler.Settings.colorSelected, memoryCardsHandler.Settings.colorShow);
					tile.Button.onClick.AddListener(() => { memoryCardsHandler.CheckSlotTile(slot); });
				}

				slot.state = SlotState.Hide;
			}
		}

		private void SetTileSizeAndPosition(Tile tile, int row, int col)
		{
			RectTransform rectContainer = container.GetComponent<RectTransform>();

			Vector2 boardSize = rectContainer.sizeDelta;
			int space = 10;
			float marginX = (memoryCardsHandler.Columns + 1) * space;
			float marginY = (memoryCardsHandler.Rows + 1) * space;

			float sizeX = (rectContainer.sizeDelta.x - ((memoryCardsHandler.Columns + 1) * space)) / memoryCardsHandler.Columns;
			float sizeY = (rectContainer.sizeDelta.y - ((memoryCardsHandler.Rows + 1) * space)) / memoryCardsHandler.Rows;

			RectTransform rectTile = tile.GetComponent<RectTransform>();
			rectTile.sizeDelta = new Vector2(sizeX, sizeY);

			float posY = ((row * sizeY) + (sizeY / 2)) + ((marginY / (memoryCardsHandler.Rows + 1) * row) + space);
			float posX = ((col * sizeX) + (sizeX / 2)) + ((marginX / (memoryCardsHandler.Columns + 1) * col) + space);
			rectTile.anchoredPosition =new Vector2(posX, posY);
		}

		private void OnCompleteGame()
		{
			panelLeaderboard.gameObject.SetActive(false);
			panelSaveScore.gameObject.SetActive(true);
			tMP_InputField.text = string.Empty;
		}

		private void SaveScore()
		{
			if (string.IsNullOrEmpty(tMP_InputField.text))
				return;

			memoryCardsHandler.PlayerData.playerName = tMP_InputField.text;
			dataHandler.SaveData(memoryCardsHandler.PlayerData);

			ShowLeaderboard();
		}

		private void ShowLeaderboard()
		{
			ClearLeaderboard();
			panelLeaderboard.gameObject.SetActive(true);
			panelSaveScore.gameObject.SetActive(false);

			Leaderboard leaderboard = new Leaderboard();

			leaderboard = dataHandler.LoadData();

			for (int i = 0; i < leaderboard.scores.Count; i++)
			{
				LeaderboardItemView item = Instantiate(leaderboardItemPrefab, panelLeaderboardContainer.transform);
				item.textName.text = leaderboard.scores[i].playerName;
				item.textScore.text = leaderboard.scores[i].gameData.score.ToString();

				leaderboardItems.Add(item);
			}
		}

		private void ClearLeaderboard()
		{
			while (leaderboardItems.Count > 0)
			{
				Destroy(leaderboardItems[leaderboardItems.Count - 1].gameObject);
				leaderboardItems.RemoveAt(leaderboardItems.Count - 1);
			}

			leaderboardItems.Clear();
			leaderboardItems = new List<LeaderboardItemView>();
		}
	}
}