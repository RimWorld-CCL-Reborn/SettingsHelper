using UnityEngine;

namespace ColorPicker.Dialog
{
    public class ColorPresets
    {
        public int Count { get { return this.Colors.Length; } }

        private Color[] Colors { get; set; }
        private int SelectedIndex { get; set; }
        public bool IsModified { get; set; }

        public ColorPresets()
        {
            this.Colors = new Color[6];
            this.Deselect();
            this.IsModified = false;
        }

        public void Deselect() => this.SelectedIndex = -1;
        public bool HasSelected() => this.SelectedIndex != -1;
        public bool IsSelected(int i) => this.SelectedIndex == i;
        public void SetSelected(int i) => this.SelectedIndex = i;

        public Color GetSelectedColor() => this.Colors[this.SelectedIndex];

        public void SetColor(int i, Color c)
        {
            if (!this.Colors[i].Equals(c))
            {
                this.Colors[i] = c;
                this.IsModified = true;
            }
        }

        internal void SetSelectedColor(Color c)
        {
            this.Colors[this.SelectedIndex] = c;
            this.IsModified = true;
        }

        public Color this[int i]
        {
            get => this.Colors[i];
            set => this.SetColor(i, value);
        }
    }
}