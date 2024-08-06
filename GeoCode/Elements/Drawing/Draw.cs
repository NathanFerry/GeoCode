using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using GeoCode.Utils;

namespace GeoCode.Elements.Drawing
{
    public static class Draw
    {

        public static void DrawElement(Element e, LevelHandle level)
        {
            if (e != null)
            {


                new ElementPropertiesSetter()
                   .SetLevelChain(level.LevelId)
                   .SetColorChain(level.GetByLevelColor().Color)
                   .SetLineStyleChain(level.GetByLevelLineStyle())
                   .Apply(e);


                if (e.AddToModel() != StatusInt.Success) { Log.Write("L'élément " + e.ElementId + " n'a pas pu être ajouté au modèle."); }
            }
        }

        public static void DrawDynamicElement(Element e,DgnButtonEvent ev)
        {
            if (e != null)
            {
                var redraw = new RedrawElems();
                redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                redraw.DrawMode = DgnDrawMode.TempDraw;
                redraw.DrawPurpose = DrawPurpose.Dynamics;
                redraw.DoRedraw(e);
            }
        }

    }
}
