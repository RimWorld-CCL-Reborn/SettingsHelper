using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using ColourPicker;

namespace SettingsHelper
{
    // REFERENCE: https://github.com/erdelf/GodsOfRimworld/blob/master/Source/Ankh/ModControl.cs
    // REFERENCE: https://github.com/erdelf/PrisonerRansom/
    public static class ListingStandardHelper
    {
        private static float gap = 12f;
        private static float lineGap = 3f;

        public static float Gap { get => gap; set => gap = value; }
        public static float LineGap { get => lineGap; set => lineGap = value; }

        public static Listing_Standard BeginListingStandard(this Rect rect, int columns = 1)
        {
            Listing_Standard listing_Standard = new Listing_Standard() { ColumnWidth = (rect.width / (float)(columns)) - ((float)(columns) * 5f) };
            listing_Standard.Begin(rect);
            return listing_Standard;
        }

        public static void AddHorizontalLine(this Listing_Standard listing_Standard, float? gap = null)
        {
            listing_Standard.Gap(gap ?? lineGap);
            listing_Standard.GapLine(gap ?? lineGap);
        }

        public static void AddLabelLine(this Listing_Standard listing_Standard, string label, float? height = null)
        {
            listing_Standard.Gap(Gap);
            Rect lineRect = listing_Standard.GetRect(height);

            // TODO: tooltips
            //Widgets.DrawHighlightIfMouseover(lineRect);
            //TooltipHandler.TipRegion(lineRect, "TODO: TIP GOES HERE");

            Widgets.Label(lineRect, label);
        }

        public static Rect GetRect(this Listing_Standard listing_Standard, float? height = null)
        {
            return listing_Standard.GetRect(height ?? Text.LineHeight);
        }

        public static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, float leftPartPct = 0.5f, float? height = null)
        {
            Rect lineRect = listing_Standard.GetRect(height);
            leftHalf = lineRect.LeftPart(leftPartPct).Rounded();
            return lineRect;
        }

        public static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, out Rect rightHalf, float leftPartPct = 0.5f, float? height = null)
        {
            Rect lineRect = listing_Standard.LineRectSpilter(out leftHalf, leftPartPct, height);
            rightHalf = lineRect.RightPart(1f - leftPartPct).Rounded();
            return lineRect;
        }

        // TODO: these could be simplified or better formalized...

        public static void AddLabeledRadioList(this Listing_Standard listing_Standard, string header, string[] labels, ref string val, float? headerHeight = null)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.AddLabelLine(header, headerHeight);
            listing_Standard.AddRadioList<string>(GenerateLabeledRadioValues(labels), ref val);
        }

        public static void AddLabeledRadioList<T>(this Listing_Standard listing_Standard, string header, Dictionary<string, T> dict, ref T val, float? headerHeight = null)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.AddLabelLine(header, headerHeight);
            listing_Standard.AddRadioList<T>(GenerateLabeledRadioValues<T>(dict), ref val);
        }

        private static void AddRadioList<T>(this Listing_Standard listing_Standard, List<LabeledRadioValue<T>> items, ref T val, float? height = null)
        {
            foreach (LabeledRadioValue<T> item in items)
            {
                listing_Standard.Gap(Gap);
                Rect lineRect = listing_Standard.GetRect(height);
                if (Widgets.RadioButtonLabeled(lineRect, item.Label, EqualityComparer<T>.Default.Equals(item.Value, val)))
                    val = item.Value;
            }
        }

        private static List<LabeledRadioValue<string>> GenerateLabeledRadioValues(string[] labels)
        {
            List<LabeledRadioValue<string>> list = new List<LabeledRadioValue<string>>();
            foreach (string label in labels)
            {
                list.Add(new LabeledRadioValue<string>(label, label));
            }
            return list;
        }

        // (label, value) => (key, value)
        private static List<LabeledRadioValue<T>> GenerateLabeledRadioValues<T>(Dictionary<string, T> dict)
        {
            List<LabeledRadioValue<T>> list = new List<LabeledRadioValue<T>>();
            foreach (KeyValuePair<string, T> entry in dict)
            {
                list.Add(new LabeledRadioValue<T>(entry.Key, entry.Value));
            }
            return list;
        }

        public class LabeledRadioValue<T>
        {
            private string label;
            private T val;

            public LabeledRadioValue(string label, T val)
            {
                Label = label;
                Value = val;
            }

            public string Label
            {
                get { return label; }
                set { label = value; }
            }

            public T Value
            {
                get { return val; }
                set { val = value; }
            }
        }

        public static void AddLabeledTextField(this Listing_Standard listing_Standard, string label, ref string settingsValue, float leftPartPct = 0.5f)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.LineRectSpilter(out Rect leftHalf, out Rect rightHalf, leftPartPct);

            // TODO: tooltips
            //Widgets.DrawHighlightIfMouseover(lineRect);
            //TooltipHandler.TipRegion(lineRect, "TODO: TIP GOES HERE");

            Widgets.Label(leftHalf, label);

            string buffer = settingsValue.ToString();
            settingsValue = Widgets.TextField(rightHalf, buffer);
        }

        public static void AddLabeledNumericalTextField<T>(this Listing_Standard listing_Standard, string label, ref T settingsValue, float leftPartPct = 0.5f, float minValue = 1f, float maxValue = 100000f) where T : struct
        {
            listing_Standard.Gap(Gap);
            listing_Standard.LineRectSpilter(out Rect leftHalf, out Rect rightHalf, leftPartPct);

            // TODO: tooltips
            //Widgets.DrawHighlightIfMouseover(lineRect);
            //TooltipHandler.TipRegion(lineRect, "TODO: TIP GOES HERE");

            Widgets.Label(leftHalf, label);

            string buffer = settingsValue.ToString();
            Widgets.TextFieldNumeric<T>(rightHalf, ref settingsValue, ref buffer, minValue, maxValue);
        }

        public static void AddLabeledCheckbox(this Listing_Standard listing_Standard, string label, ref bool settingsValue)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.CheckboxLabeled(label, ref settingsValue);
        }

        public static void AddLabeledSlider(this Listing_Standard listing_Standard, string label, ref float value, float leftValue, float rightValue, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f, bool middleAlignment = false)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.LineRectSpilter(out Rect leftHalf, out Rect rightHalf);

            Widgets.Label(leftHalf, label);

            float bufferVal = value;
            // NOTE: this BottomPart will probably need some reworking if the height of rect is greater than a line
            value = Widgets.HorizontalSlider(rightHalf.BottomPart(0.70f), bufferVal, leftValue, rightValue, middleAlignment, null, leftAlignedLabel, rightAlignedLabel, roundTo);
        }

        public static void AddColorPickerButton(this Listing_Standard listing_Standard, string label, Color color, Action<Color> callback, string buttonText = "Change")
        {
            listing_Standard.Gap(ListingStandardHelper.Gap);
            Rect lineRect = listing_Standard.GetRect();

            float textSize = Text.CalcSize(buttonText).x + 10f;
            float rightSize = textSize + 5f + lineRect.height;
            Rect rightPart = lineRect.RightPartPixels(textSize + 5f + lineRect.height);

            // draw button leaving room for color rect in rightHalf rect (plus some padding)
            if (Widgets.ButtonText(rightPart.LeftPartPixels(textSize), buttonText))
                //Find.WindowStack.Add(new ColorSelectDialog(buttonText, color, selectionChange));
                Find.WindowStack.Add(new Dialog_ColourPicker(color, callback));
            GUI.color = color;
            // draw square with color in rightHalf rect
            GUI.DrawTexture(rightPart.RightPartPixels(rightPart.height), BaseContent.WhiteTex);
            GUI.color = Color.white;

            Rect leftPart = lineRect.LeftPartPixels(lineRect.width - rightSize);
            Widgets.Label(leftPart, label);
        }

        public static void AddColorPickerButton(this Listing_Standard listing_Standard, string label, Color color, String fieldName, object colorContainer, string buttonText = "Change")
        {
            ListingStandardHelper.AddColorPickerButton(listing_Standard, label, color, (Color c) => colorContainer.GetType().GetField(fieldName).SetValue(colorContainer, color), buttonText);
        }

        // Verse.Listing_Standard
        public static float Slider(this Listing_Standard listing_Standard, float val, float min, float max, string label = null, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f, bool middleAlignment = false)
        {
            Rect rect = listing_Standard.GetRect(22f);
            float result = Widgets.HorizontalSlider(rect, val, min, max, middleAlignment, label, leftAlignedLabel, rightAlignedLabel, roundTo);
            listing_Standard.Gap(listing_Standard.verticalSpacing);
            return result;
        }

        public static void AddLabeledSlider<T>(this Listing_Standard listing_Standard, string label, ref T value) where T : Enum
        {
            Enum enu = value as Enum;

            //listing_Standard.Gap(Gap);
            listing_Standard.Gap(10);
            listing_Standard.LineRectSpilter(out Rect leftHalf, out Rect rightHalf);

            Widgets.Label(leftHalf, label);

            float bufferVal = Convert.ToInt32(enu);
            // NOTE: this BottomPart will probably need some reworking if the height of rect is greater than a line
            float tempVal = Widgets.HorizontalSlider(rightHalf.BottomPart(0.70f), bufferVal, 0f, Enum.GetValues(typeof(T)).Length - 1, true, Enum.GetName(typeof(T), value), roundTo: 1);

            value = (T)Enum.ToObject(typeof(T), (int)tempVal);
        }

    }

    public static class SubSettingsWindowHelper
    {
        public delegate void WindowContentsHandler(Rect inRect);

        public class SettingsWindow : Window
        {
            private readonly WindowContentsHandler doWindowContents;

            public SettingsWindow(WindowContentsHandler handler) : base()
            {
                this.doCloseButton = true;
                this.doCloseX = true;
                this.forcePause = true;
                this.absorbInputAroundWindow = true;
                this.doWindowContents = handler;
            }

            public override Vector2 InitialSize
            {
                get => new Vector2(900f, 700f);
            }

            public override void DoWindowContents(Rect inRect) => this.doWindowContents(inRect);
        }

        public static void AddSubSettingsButton(this Listing_Standard listing_Standard, string label, WindowContentsHandler handler)
        {
            listing_Standard.Gap(ListingStandardHelper.Gap);
            Rect lineRect = listing_Standard.GetRect();

            // TODO: button sizing...
            if (Widgets.ButtonText(lineRect, label))
                Find.WindowStack.Add(new SettingsWindow(handler));
        }
    }

}