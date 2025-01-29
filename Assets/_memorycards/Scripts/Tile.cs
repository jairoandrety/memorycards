using UnityEngine;
using UnityEngine.UI;

namespace MemoryCards
{
	public class Tile : MonoBehaviour
    {
		[SerializeField] private Image background;
		[SerializeField] private Image graphic;
        [SerializeField] private Button button;

        private int number = 0;

		public Color colorHide;
		public Color colorSelected;
		public Color colorShow;

		public Button Button => button;
        public int Number => number;

        public void SetValues(int value, Sprite sprite, Color hide, Color selected, Color show)
        {
            number = value;
            graphic.sprite = sprite;

			colorHide = hide;
			colorSelected = selected;
			colorShow = show;
        }

		public void Hide()
		{
			button.enabled = true;
            background.color = colorHide;
		}

		public void select()
        {
            button.enabled = false;
			background.color = colorSelected;
		}

       public void Show()
        {
			button.enabled = false;
			background.color = colorShow;
		}
    }
}