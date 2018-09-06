using UnityEngine;

namespace ColorPicker.Dialog
{
    public delegate void SelectionChange(SelectionColorWidget selectionColorWidget);

    public class SelectionColorWidget
    {
        public SelectionChange selectionChange;

        public readonly Color originalColor;

        private Color selectedColor;

        public string rBuffer;
        public string gBuffer;
        public string bBuffer;

        public Color SelectedColor
        {
            get { return this.selectedColor; }
            set
            {
                if (!this.selectedColor.Equals(value))
                {
                    this.selectedColor = value;
                    this.selectionChange?.Invoke(this);
                    this.SetColorBuffers();
                }
            }
        }

        public SelectionColorWidget(Color color)
        {
            this.originalColor = color;
            this.selectedColor = color;
            this.SetColorBuffers();
        }

        public void ResetToDefault()
        {
            this.selectedColor = this.originalColor;
            this.selectionChange?.Invoke(this);
            this.SetColorBuffers();
        }

        private void SetColorBuffers()
        {
            this.rBuffer = this.ColorConverter(this.selectedColor.r).ToString();
            this.gBuffer = this.ColorConverter(this.selectedColor.g).ToString();
            this.bBuffer = this.ColorConverter(this.selectedColor.b).ToString();
        }

        private int ColorConverter(float f) => (int)(f * 255.999f);
    }
}