using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace ModSettingsHelper
{
    // reference: https://github.com/erdelf/GodsOfRimworld/blob/master/Source/Ankh/ModControl.cs
    // reference: https://github.com/erdelf/PrisonerRansom/
    public static class ModWindowHelper
    {
        static float topPad = 30f;
        static float leftPad = 0f;
        static float vspacing = 30f;
        static float textFieldPadding = 10f;
        static float horizontalSliderPadding = 10f;
        static float curY = topPad;
        static float curX = leftPad;

        static float horizontalSliderHeight = 60f;
        static float radioListItemHeight = 30f;

        // NOTE: could get away from this if went with an instance...
        public static void Reset()
        {
            curY = topPad;
            curX = leftPad;
        }

        public static float HorizontalSlider(Rect rect, float value, float leftValue, float rightValue, bool middleAlignment = false, string label = null, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f)
        {
            curY += horizontalSliderPadding;
            Rect r = GetRect(horizontalSliderHeight, rect.width);
            float val = Widgets.HorizontalSlider(r, value, leftValue, rightValue, middleAlignment, label, leftAlignedLabel, rightAlignedLabel, roundTo);
            curY += horizontalSliderPadding;
            return val;
        }

        public static void MakeLabel(Rect inRect, string label)
        {
            MakeLabel(inRect.width - 64f, label);
        }

        public static void MakeLabeledTextField(Rect inRect, string label, ref string val)
        {
            curY += textFieldPadding;
            Widgets.Label(new Rect(0f, curY + 5f, inRect.width - 16f, 40f), label);
            val = Widgets.TextField(new Rect(50f, curY + 6f, inRect.width - 16f, 40f), val);
            curY += vspacing + textFieldPadding;
        }

        // TODO: rewrite this -> it's ugly.
        public static void MakeTextFieldNumericLabeled<T>(Rect inRect, string label, ref T val, ref string buffer, float min, float max) where T : struct
        {
            curY += textFieldPadding;
            Widgets.TextFieldNumericLabeled<T>(new Rect(0f, curY + 5f, inRect.width - 16f, 40f), label, ref val, ref buffer, min, max);
            curY += vspacing + textFieldPadding;
        }

        private static void MakeLabel(float width, string label)
        {
            Widgets.Label(new Rect(0f, curY + 5f, width - 16f, 40f), label);
            curY += vspacing;
        }

        public static void MakeLabeledCheckbox(Rect inRect, string label, ref bool val)
        {
            MakeLabeledCheckbox(inRect.width, label, ref val);
        }

        private static void MakeLabeledCheckbox(float width, string label, ref bool val)
        {
            // NOTE: consider breaking out more of these numbers
            Widgets.Label(new Rect(0f, curY + 5f, width - 16f, 40f), label);
            Widgets.Checkbox(width - 64f, curY + 6f, ref val);
            curY += vspacing;
        }

        public static void MakeLabeledRadioList<T>(Rect inRect, List<LabeledRadioValue<T>> items, ref T val)
        {
            foreach (LabeledRadioValue<T> item in items)
            {
                Rect r = GetRect(radioListItemHeight, inRect.width);

                if (Widgets.RadioButtonLabeled(r, item.Label, EqualityComparer<T>.Default.Equals(item.Value, val)))
                {
                    val = item.Value;
                }
            }
            curY += vspacing;
        }

        // NOTE: consider using an enum...
        public static void MakeLabeledRadioList(Rect inRect, string[] labels, ref string val)
        {
            MakeLabeledRadioList<string>(inRect, GenerateLabeledRadioValues(labels), ref val);
        }

        public static List<LabeledRadioValue<string>> GenerateLabeledRadioValues(string[] labels)
        {
            List<LabeledRadioValue<string>> list = new List<LabeledRadioValue<string>>();
            foreach (string label in labels)
            {
                list.Add(new LabeledRadioValue<string>(label, label));
            }
            return list;
        }

        public static void MakeLabeledRadioList<T>(Rect inRect, Dictionary<string, T> dict, ref T val)
        {
            MakeLabeledRadioList<T>(inRect, GenerateLabeledRadioValues<T>(dict), ref val);
        }

        // (label, value) => (key, value)
        public static List<LabeledRadioValue<T>> GenerateLabeledRadioValues<T>(Dictionary<string, T> dict)
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

        public static Rect GetRect(float height, float width)
        {
            // NOTE: come back to the concept of `ColumnWidth`
            Rect result = new Rect(curX, curY, width, height);
            curY += height;
            return result;
        }

    }
}