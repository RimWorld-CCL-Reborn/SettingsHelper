using System.IO;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ColorPicker.Dialog
{
    [StaticConstructorOnStartup]
    public class ColorSelectDialog : Window
    {
        private static readonly Texture2D colorPresetTexture = new Texture2D(20, 20);
        private static readonly ColorPresets colorPresets = IOUtil.LoadColorPresets();
        private static readonly Texture2D colorPickerTexture;

        private readonly SelectionColorWidget selectionColorWidget;
        private readonly string label;
        private readonly bool allowAlpha;

        static ColorSelectDialog()
        {
            // REFERENCE: https://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
            // REFERENCE: https://stackoverflow.com/questions/7542059/most-efficient-way-of-reading-data-from-a-stream
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourcePath = "SettingsHelper.ColorPicker.colorpicker.png";

            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[2048]; // read in chunks of 2KB
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    memoryStream.Write(buffer, 0, bytesRead);
                byte[] result = memoryStream.ToArray();
                // do something with the result
                colorPickerTexture = new Texture2D(2, 2, TextureFormat.Alpha8, true);
                colorPickerTexture.LoadImage(result);
            }

        }

        public ColorSelectDialog(string label, Color color, SelectionChange selectionChange = null, bool allowAlpha = false) : base()
        {
            this.doCloseX = true;
            this.absorbInputAroundWindow = true; ;
            this.label = label;
            this.allowAlpha = allowAlpha;
            this.selectionColorWidget = new SelectionColorWidget(color) { selectionChange = selectionChange };
        }

        public override Vector2 InitialSize { get => new Vector2(400, 550);  }

        public override void PreClose()
        {
            base.PreClose();
            if (colorPresets.IsModified)
            {
                IOUtil.SaveColorPresets(colorPresets);
                colorPresets.IsModified = false;
            }
            colorPresets.Deselect();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.Label(new Rect(5, 5, inRect.width - 10, 25), this.label);
            this.AddColorSelectorWidget(15, 10, inRect.width - 30, this.selectionColorWidget);

            //Text.Font = GameFont.Small;
            if (Widgets.ButtonText(new Rect(inRect.width * 0.5f + 30, inRect.height - 30, 60, 30), "AcceptButton".Translate()))
                base.Close();

            if (Widgets.ButtonText(new Rect(inRect.width * 0.5f - 90, inRect.height - 30, 60, 30), "ResetButton".Translate()))
                this.selectionColorWidget.ResetToDefault();
        }

        void AddColorSelectorWidget(float left, float top, float width, SelectionColorWidget selectionColorWidget)
        {
            Rect colorPickerRect = new Rect(0, 25f, width, colorPickerTexture.height * width / colorPickerTexture.width);

            GUI.BeginGroup(new Rect(left, top, width, colorPickerRect.height + 20));
            GUI.color = Color.white;
            if (GUI.RepeatButton(colorPickerRect, colorPickerTexture, GUI.skin.label))
            {
                //SetColorToSelected(selectionColorWidget, presetsDto, GetColorFromTexture(Event.current.mousePosition, colorPickerRect, colorPickerTexture));
                Color color = GetColorFromTexture(Event.current.mousePosition, colorPickerRect, colorPickerTexture);
                if (colorPresets.HasSelected())
                    colorPresets.SetSelectedColor(color);
                else
                    selectionColorWidget.SelectedColor = color;
            }
            GUI.EndGroup();

            Color rgbColor = Color.white;
            if (colorPresets.HasSelected())
                rgbColor = colorPresets.GetSelectedColor();
            else
                rgbColor = selectionColorWidget.SelectedColor;

            GUI.BeginGroup(new Rect(0, colorPickerRect.height + 90f, width, 30f));

            bool colorChanged = false;

            GUI.Label(new Rect(0f, 0f, 10f, 20f), "R");
            string rText = GUI.TextField(new Rect(12f, 1f, 30f, 20f), this.selectionColorWidget.rBuffer, 3);
            if (rText?.Length > 0 && rText != this.selectionColorWidget.rBuffer)
                colorChanged = true;

            GUI.Label(new Rect(52f, 0f, 10f, 20f), "G");
            string gText = GUI.TextField(new Rect(64f, 1f, 30f, 20f), this.selectionColorWidget.gBuffer, 3);
            if (gText?.Length > 0 && gText != this.selectionColorWidget.gBuffer)
                colorChanged = true;

            GUI.Label(new Rect(104f, 0f, 10f, 20f), "B");
            string bText = GUI.TextField(new Rect(116f, 1f, 30f, 20f), this.selectionColorWidget.bBuffer, 3);
            if (bText?.Length > 0 && bText != this.selectionColorWidget.bBuffer)
                colorChanged = true;

            GUI.color = rgbColor;
            Rect colorRect = new Rect(156f, 1f, colorPresetTexture.width, colorPresetTexture.height);
            if (allowAlpha) colorRect.x += 52f;
            GUI.DrawTexture(colorRect, BaseContent.WhiteTex);
            GUI.color = Color.white;

            string aText = "";
            if (allowAlpha)
            {
                GUI.Label(new Rect(156f, 0f, 10f, 20f), "A");
                aText = GUI.TextField(new Rect(168f, 1f, 30f, 20f), ((int)(rgbColor.a*255)).ToString(), 3);
                if (aText?.Length > 0 && bText != this.selectionColorWidget.aBuffer)
                    colorChanged = true;
            }
            GUI.EndGroup();

            /*if (allowAlpha)
            {
                GUI.BeginGroup(new Rect(0, colorPickerRect.height + 50f, width, 30f));
                GUI.Label(new Rect(0, 0, 75, 40), "ReColorStockpile.Alpha".Translate());
                rgbColor.a = Widgets.HorizontalSlider(new Rect(90, 0, 150, 20), rgbColor.a, 0, 1);
                GUI.EndGroup();
            }*/

            // Handle presets.
            GUI.BeginGroup(new Rect(0, colorPickerRect.height + 130, width, 120));
            GUI.Label(new Rect(0, 0, 100, 30), "Presets:");
            bool skipRGB = false;
            float l = 0;
            for (int i = 0; i < colorPresets.Count; ++i)
            {
                GUI.color = colorPresets[i];
                l += colorPresetTexture.width + 4;
                Rect presetRect = new Rect(l, 32, colorPresetTexture.width, colorPresetTexture.height);
                GUI.Label(presetRect, new GUIContent(colorPresetTexture, "SH_ColorPresetHelp".Translate()));
                if (Widgets.ButtonInvisible(presetRect, false))
                {
                    if (Event.current.shift)
                    {
                        if (colorPresets.IsSelected(i))
                            colorPresets.Deselect();
                        else
                        {
                            if (!colorPresets.HasSelected())
                                colorPresets.SetColor(i, selectionColorWidget.SelectedColor);
                            colorPresets.SetSelected(i);
                        }
                    }
                    else
                        selectionColorWidget.SelectedColor = colorPresets[i];
                    skipRGB = true;
                }
                GUI.color = Color.white;
                if (colorPresets.IsSelected(i))
                    Widgets.DrawBox(new Rect(presetRect.x-2,presetRect.y+1,presetRect.width-2,presetRect.height-2));
            }
            GUI.Label(new Rect(0, 30 + colorPresetTexture.height + 2, width, 60), GUI.tooltip);
            GUI.EndGroup();

            if (!skipRGB && colorChanged)
            {
                selectionColorWidget.SelectedColor = new Color(ColorConvert(rText), ColorConvert(gText), ColorConvert(bText), ColorConvert(aText));
            }

        }

        private Color CurrentColor
        {
            get => colorPresets.HasSelected() ? colorPresets.GetSelectedColor() : selectionColorWidget.SelectedColor;
        }

        private string ColorConvert(float f)
        {
            int i = (int)(f * 255);
            // clamp [0,255]
            if (i > 255) i = 255;
            else if (i < 0) i = 0;
            return i.ToString();
        }

        private float ColorConvert(string intText)
        {
            float f = int.Parse(intText) / 255f;
            // clamp [0,1]
            if (f > 1) f = 1;
            else if (f < 0) f = 0;
            return f;
        }

        private Color GetColorFromTexture(Vector2 mousePosition, Rect rect, Texture2D texture)
        {
            float localMouseX = mousePosition.x - rect.x;
            float localMouseY = mousePosition.y - rect.y;
            int imageX = (int)(localMouseX * ((float)colorPickerTexture.width / (rect.width + 0f)));
            int imageY = (int)((rect.height - localMouseY) * ((float)colorPickerTexture.height / (rect.height + 0f)));
            Color pixel = texture.GetPixel(imageX, imageY);
            pixel.a = (colorPresets.HasSelected()) ? colorPresets.GetSelectedColor().a : this.selectionColorWidget.SelectedColor.a;
            return pixel;
        }
    }
}