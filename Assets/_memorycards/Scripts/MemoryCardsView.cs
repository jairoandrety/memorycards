
using System.Collections.Generic;
using UnityEngine;

namespace MemoryCards
{
	public class MemoryCardsView : MonoBehaviour
	{
		[SerializeField] private MemoryCardsHandler memoryCardsHandler;

		[SerializeField] private GameObject container;
		[SerializeField] private Tile tilePrefab;
		[SerializeField] private List<Tile> tiles = new List<Tile>();

		private void Start()
		{
			memoryCardsHandler.CreateBoard();
			DrawBoard();
		}

		private void DrawBoard()
		{
			foreach (var slot in memoryCardsHandler.Slots)
			{
				Tile tile = Instantiate(tilePrefab, container.transform);

				if (slot != null)
				{
					slot.SetTile(tile);
					tile.SetValues(slot.number, memoryCardsHandler.Settings.GetSpriteByNumber(slot.number), memoryCardsHandler.Settings.colorHide, memoryCardsHandler.Settings.colorSelected, memoryCardsHandler.Settings.colorShow);
					tile.Button.onClick.AddListener(() => { memoryCardsHandler.CheckSlotTile(slot); });
				}

				slot.state = SlotState.Hide;

				tiles.Add(tile);
			}
		}
	}
}