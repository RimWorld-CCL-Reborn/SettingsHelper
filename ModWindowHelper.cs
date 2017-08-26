using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace ModWindowHelper
{
    // reference: https://github.com/erdelf/GodsOfRimworld/blob/master/Source/Ankh/ModControl.cs
    // reference: https://github.com/erdelf/PrisonerRansom/
    public static class ModWindowHelper
    {
        static float topPad = 30f;
        static float leftPad = 0f;
        static float vspacing = 30f; // same as radioListItemHeight
        static float curY = topPad;
        static float curX = leftPad;

        // NOTE: could get away from this if went with an instance...
        static public void Reset()
        {
            curY = topPad;
            curX = leftPad;
        }

        static public void MakeLabel(Rect inRect, string label)
        {
            MakeLabel(inRect.width - 64f, label);
        }

        static private void MakeLabel(float width, string label)
        {
            Widgets.Label(new Rect(0f, curY + 5f, width - 16f, 40f), label);
            curY += vspacing;
        }

        static public void MakeLabeledCheckbox(Rect inRect, string label, ref bool val)
        {
            MakeLabeledCheckbox(inRect.width, label, ref val);
        }

        static private void MakeLabeledCheckbox(float width, string label, ref bool val)
        {
            // NOTE: consider breaking out more of these numbers
            Widgets.Label(new Rect(0f, curY + 5f, width - 16f, 40f), label);
            Widgets.Checkbox(width - 64f, curY + 6f, ref val);
            curY += vspacing;
        }

        static float radioListItemHeight = 30f;
        static public void MakeLabeledRadioList<T>(Rect inRect, List<LabeledRadioValue<T>> items, ref T val)
        {
            foreach (LabeledRadioValue<T> item in items)
            {
                Rect r = GetRect(radioListItemHeight, inRect.width);

                if (Widgets.RadioButtonLabeled(r, item.Label, EqualityComparer<T>.Default.Equals(item.Value, val)))
                {
                    val = item.Value;
                }
            }
        }

        static public void MakeLabeledRadioList<T>(Rect inRect, Dictionary<string, T> dict, ref T val)
        {
            MakeLabeledRadioList<T>(inRect, GenerateLabeledRadioValues<T>(dict), ref val);
        }

        // (label, value) => (key, value)
        static public List<LabeledRadioValue<T>> GenerateLabeledRadioValues<T>(Dictionary<string, T> dict)
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

        static public Rect GetRect(float height, float width)
        {
            // NOTE: come back to the concept of `ColumnWidth`
            Rect result = new Rect(curX, curY, width, height);
            curY += height;
            return result;
        }

    }
}