namespace MemoryCards
{
	public enum SlotState
	{
		None = 0,
		Hide = 1,
		Showed = 2,
	}

	[System.Serializable]
	public class Slot
	{
		public int R = 0;
		public int C = 0;
		public int number = 0;

		public SlotState state = SlotState.None;
		public Tile tile;
	
		private bool isMatched = false;

		public bool IsMatched => isMatched;

		public void SetTile(Tile value)
		{
			tile = value;
			tile.Button.onClick.AddListener(() => { OnSelectSlot(); });
		}

		public void OnSelectSlot()
		{
			switch (state)
			{
				case SlotState.None:

				break;
				case SlotState.Hide:
					tile.select();
				break;
				case SlotState.Showed:
					if (isMatched)
					{
						tile.Show();
					}
					else
					{
						tile.Hide();
					}
					break;
				default:

				break;
			}
		}

		public void DeselectSlot()
		{
			tile.Hide();
		}

		public void SetMatched()
		{
			tile.Show();
			isMatched = true;
		}
	}
}