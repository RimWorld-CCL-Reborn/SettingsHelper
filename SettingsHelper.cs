using Verse;
using UnityEngine;

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

        //NOTE: are these Text.Anchor necessary??

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

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(lineRect, label);
            Text.Anchor = anchor;
        }

        private static Rect GetRect(this Listing_Standard listing_Standard,  float? height = null)
        {
            return listing_Standard.GetRect(height ?? Text.LineHeight);
        }

        private static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, float? height = null)
        {
            Rect lineRect = listing_Standard.GetRect(height);
            leftHalf = lineRect.LeftHalf().Rounded();
            return lineRect;
        }

        private static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, out Rect rightHalf, float? height = null)
        {
            Rect lineRect = listing_Standard.LineRectSpilter(out leftHalf, height);

            rightHalf = lineRect.RightHalf().Rounded();
            // TODO: What the?
            rightHalf = rightHalf.LeftPartPixels(rightHalf.width - Text.LineHeight);

            return lineRect;
        }

        public static void AddSettingsLine<T>(this Listing_Standard listing_Standard, string label, ref T settingsValue) where T : struct
        {
            listing_Standard.Gap(Gap);
            listing_Standard.LineRectSpilter(out Rect leftHalf, out Rect rightHalf);

            // TODO: tooltips
            //Widgets.DrawHighlightIfMouseover(lineRect);
            //TooltipHandler.TipRegion(lineRect, "TODO: TIP GOES HERE");

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(leftHalf, label);
            Text.Anchor = anchor;

            string buffer = settingsValue.ToString();
            Widgets.TextFieldNumeric<T>(rightHalf, ref settingsValue, ref buffer, 1f, 100000f);
        }

        public static void AddLabeledCheckbox(this Listing_Standard listing_Standard, string label, ref bool settingsValue)
        {
            listing_Standard.Gap(Gap);
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            listing_Standard.CheckboxLabeled(label, ref settingsValue);
            Text.Anchor = anchor;
        }

        public static void AddLabeledSlider(this Listing_Standard listing_Standard, string label, ref float value, float leftValue, float rightValue)
        {
            listing_Standard.Gap(Gap);
            listing_Standard.LineRectSpilter(out Rect leftHalf, out Rect rightHalf);

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(leftHalf, label);
            Text.Anchor = anchor;

            float bufferVal = value;
            // NOTE: this BottomPart will probably need some reworking if the height of rect is greater than a line
            value = Widgets.HorizontalSlider(rightHalf.BottomPart(0.70f), bufferVal, leftValue, rightValue);
        }
    }
}