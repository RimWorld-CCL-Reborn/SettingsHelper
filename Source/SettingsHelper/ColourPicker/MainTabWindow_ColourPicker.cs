using RimWorld;
using Verse;
using UnityEngine;

#if DEBUG

namespace ColourPicker
{
    class MainTabWindow_ColourPicker : MainTabWindow
    {
        public Color    BGCol;
        public Texture2D        BGTex;

        public MainTabWindow_ColourPicker()
        {
            BGCol = Color.grey;
            BGTex = SolidColorMaterials.NewSolidColorTexture( BGCol );
        }

        public override void DoWindowContents( Rect inRect )
        {
            GUI.DrawTexture( inRect, BGTex );
            Rect button = new Rect(0f, 0f, 200f, 35f);
            button = button.CenteredOnXIn( inRect ).CenteredOnXIn( inRect );

            if (Widgets.ButtonText( button, "Change Colour" ) )
            {
                Find.WindowStack.Add( new Dialog_ColourPicker( BGCol, colour => BGTex = SolidColorMaterials.NewSolidColorTexture( colour ) ) );
            }
        }
    }
}

#endif