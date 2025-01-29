using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryCards
{
	[System.Serializable]
    public class BoardLoader
    {
        [TextArea(1, 20)] public string jsonToInject = string.Empty;
        [SerializeField] private BlocksData data = new BlocksData();

        public BlocksData Data => data;

        public void LoadData(string value)
        { 
            data = JsonUtility.FromJson<BlocksData>(value);
        }

        public void LoadDataFromLocalJson()
        {
            data = new BlocksData();

            if (string.IsNullOrEmpty(jsonToInject))
                return;
            LoadData(jsonToInject);
        }

        public bool ValidateData(int minNumCards, int maxNumCards)
        {
            return data.blocks.Count > 0 ? data.blocks.Count > minNumCards && data.blocks.Count < maxNumCards ? true : false : false;
        }

  //      public bool HasDuplicatePosition()
  //      {
		//	foreach (var block in data.blocks)
		//	{
  //              return data.blocks.FindAll(i => i.R == block.R && i.C == block.C).Count > 1;
		//	}

  //          return false;
		//}
    }

    [System.Serializable]
    public class TwiceTile
    {
        [SerializeField] private List<Block> blocks = new List<Block>();
        [SerializeField] private List<Tile> tiles = new List<Tile>();

        public List<Block> Blocks => blocks;
        //public List<Tile> Tiles => tiles;

        public void AddBlock(Block block)
        {
            blocks.Add(block);
        }
    }

	public class MemoryCardsHandler : MonoBehaviour
    {
        [SerializeField] private MemoryCardsSettings settings;

        [SerializeField] private BoardLoader boardLoader = new BoardLoader();

        [SerializeField] private List<TwiceTile> twiceTiles = new List<TwiceTile>();

		//private Slot[,] slots = new Slot[0,0];
		private List<Slot> slots = new List<Slot>();

		private int xBound = 0;
        private int yBound = 0;

		private List<Slot> slotsSelected = new List<Slot>();

		public MemoryCardsSettings Settings => settings;
		public BoardLoader BoardLoader => boardLoader;

		//public Slot[,] Slots => slots;
		public List<Slot> Slots => slots;

		//public Slot GetSlot(int row,  int col)
		//{
		//	return slots[row, col];
		//}

		#region Create And Validate Board
		public bool CreateBoard()
        {
			boardLoader.LoadDataFromLocalJson();
			if (!boardLoader.ValidateData(settings.minNumCards, settings.maxNumCards))
			{
				Debug.LogError("there is not a data valid for start the game.");
				return false;
			}

			foreach (var block in boardLoader.Data.blocks)
			{
				if (block.R > yBound)
					yBound = block.R;

				if (block.C > xBound)
					xBound = block.C;
			}

			//slots = new Slot[xBound, yBound];


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
				Debug.LogFormat("{0} - {1}", block.R , block.C);


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

				//if (slots[block.C - 1, block.R - 1].number != 0)
				//{
				//	Debug.LogError("Slot has value, this slot is repeat");
    //                return true;
				//}
				//else
				//{
				//	slots[block.C - 1, block.R - 1].number = block.number;
				//}
			}

			Debug.Log(slots.Count);
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

			//for (int row = 0; row < slots.GetLength(0); row++)
			//{
			//	for (int col = 0; col < slots.GetLength(1); col++)
			//	{
			//		if (slots[row, col].number == valueToFind)
			//		{
			//			count++;
			//		}
			//	}
			//}
			//return count;
		}
		#endregion

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

			slotsSelected.Add(slot);

			if(slotsSelected.Count == 2)
			{
				bool isMatched = slotsSelected[0].number.Equals(slotsSelected[1].number);
				
				if(isMatched)
				{
					slotsSelected.ForEach(i => i.SetMatched());
				}
				else
				{
					slotsSelected.ForEach(i => i.DeselectSlot());
				}

				slotsSelected.Clear();
			}

			CheckGameState();
		}


		private void CheckGameState()
		{
			bool win = slots.TrueForAll(i => i.IsMatched);

			if (win)
			{
				Debug.Log("win");
			}
		}
	}
}