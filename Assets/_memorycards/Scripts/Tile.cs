using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace MemoryCards
{
	public class Tile : MonoBehaviour
    {
		[SerializeField] private Image background;
		[SerializeField] private Image graphic;
        [SerializeField] private Button button;

		[SerializeField] private Color colorHide;
		[SerializeField] private Color colorSelected;
		[SerializeField] private Color colorShow;

		[SerializeField] private AudioClip audioClipSelect;
		[SerializeField] private AudioSource audioSourceSelect;

		private int number = 0;

		public Button Button => button;
        public int Number => number;

        public void SetValues(int value, Sprite sprite, Color hide, Color selected, Color show)
        {
            number = value;
            graphic.sprite = sprite;

			colorHide = hide;
			colorSelected = selected;
			colorShow = show;

			Hide();
        }

		public void Hide()
		{
			button.enabled = true;
			background.rectTransform.DOScale(Vector3.one, 0.25f).OnComplete(()=> background.color = colorHide);
			ChangeBackgroundColor(colorHide);
			ShowGraphic(false);
		}

		public void select()
        {
            button.enabled = false;
			background.rectTransform.DOScale(Vector3.one * 0.95f, 0.25f).OnComplete(() => background.color = colorSelected);
			ChangeBackgroundColor(colorSelected);
			ShowGraphic(true);

			audioSourceSelect.clip = audioClipSelect;
			if (audioSourceSelect != null)
				audioSourceSelect.Play();
		}

		public void Show()
        {
			button.enabled = false;
			background.rectTransform.DOScale(Vector3.one * 0.95f, 0.25f).OnComplete(() => background.color = colorShow);
			ChangeBackgroundColor(colorShow);
		}

		private void ShowGraphic(bool value)
		{
			ChangeColor(graphic, new Color(1,1,1, value ? 1 : 0));
		}

		private void ChangeBackgroundColor(Color color)
		{
			ChangeColor(background, color);
		}

		private void ChangeColor(Image image, Color targetColor)
		{
			Color currentColor = image.color;
			Tweener colorTween = DOTween.To(() => currentColor, x => currentColor = x, targetColor, 0.25f)
			.OnUpdate(() => image.color = currentColor)
			.SetEase(Ease.InOutSine);
		}
    }
}