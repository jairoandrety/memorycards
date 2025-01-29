using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using MemoryCards;

namespace Tests
{
	public class JsonValidationTests
	{
		private const string jsonString = "{\"blocks\":[{\"R\":1,\"C\":1,\"number\":1},{\"R\":2,\"C\":1,\"number\":5},{\"R\":3,\"C\":1,\"number\":2},{\"R\":4,\"C\":1,\"number\":6},{\"R\":1,\"C\":2,\"number\":6},{\"R\":2,\"C\":2,\"number\":2},{\"R\":3,\"C\":2,\"number\":3},{\"R\":4,\"C\":2,\"number\":5},{\"R\":1,\"C\":3,\"number\":3},{\"R\":2,\"C\":3,\"number\":4},{\"R\":3,\"C\":3,\"number\":4},{\"R\":4,\"C\":3,\"number\":1}]}";

		//private string jsonString = "";

		private BlocksData data = new BlocksData() { };

		[SetUp]
		public void Setup()
		{
			data = new BlocksData();
			data.blocks = new List<Block>();
		}

		[Test]
		public void ValidateJsonStructure()
		{
			//Arrange

			// Act
			data = JsonUtility.FromJson<BlocksData>(jsonString);

			// Assert
			Assert.IsNotNull(data, "JSON deserialization failed.");
			Assert.IsNotNull(data.blocks, "Blocks array is missing in JSON.");
		}

		[Test]
		public void ValidateBlockValues()
		{
			//Arrange

			// Act
			data = JsonUtility.FromJson<BlocksData>(jsonString);

			// Validate each block
			foreach (var block in data.blocks)
			{
				Assert.IsTrue(block.R > 0, $"Invalid row value: {block.R}");
				Assert.IsTrue(block.C > 0, $"Invalid column value: {block.C}");
				Assert.IsTrue(block.number >= 0 && block.number <= 9, $"Invalid number value: {block.number}");
			}
		}

		[Test]
		public void ValidateNumberOccurrences()
		{
			//Arrange
			Dictionary<int, int> numberCounts = new Dictionary<int, int>();

			// Act
			data = JsonUtility.FromJson<BlocksData>(jsonString);
					
			foreach (var block in data.blocks)
			{
				if (!numberCounts.ContainsKey(block.number))
				{
					numberCounts[block.number] = 0;
				}

				numberCounts[block.number]++;
				Debug.Log(block.number.ToString());

				// Assert if any number appears more than twice
				Assert.IsFalse(numberCounts[block.number] > 2, $"Number {block.number} appears more than twice.");
			}
		}
	}
}