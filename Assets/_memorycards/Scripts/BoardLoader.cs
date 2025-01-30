using UnityEngine;
using UnityEngine.UIElements;

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

        public bool ValidateData(int min, int max, int maxNumber)
        {
            int rows = 0;
            int columns = 0;

			foreach (var block in data.blocks)
			{
				if (block.R > rows)
					rows = block.R;

				if (block.C > columns)
					columns = block.C;
			}

			bool checkRows = rows < min || rows > max;
            bool checkColumns = columns < min || columns > max;
            bool checkNumber = data.blocks.Exists(i => i.number > maxNumber);
            return checkRows || checkColumns || checkNumber;
        }
    }
}