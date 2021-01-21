using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGridBubbles
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("A0C20AC6-2839-4F3F-BA16-B50605359F4C")]
    class ThisCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)

        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {

                using (Transaction t = new Transaction(doc, "Correct grid bubbles"))
                {
                    t.Start();
                    foreach (ElementId elemId in uidoc.Selection.GetElementIds())
                    {
                        Element SelectedElem = doc.GetElement(elemId);
                        if (SelectedElem is DatumPlane)
                        {
                            DatumPlane xGrid = SelectedElem as DatumPlane;

                            IList<Curve> CurvesInGrid = xGrid.GetCurvesInView(DatumExtentType.ViewSpecific, uidoc.ActiveView);

                            Curve cv = CurvesInGrid.First(); // only one segment grids allowed

                            XYZ cvStart = cv.GetEndPoint(0);
                            XYZ cvEnd = cv.GetEndPoint(1);

                            XYZ cvTop = null;
                            XYZ cvLeft = null;

                            #region FIND TOP AND LEFT
                            if (Convert.ToInt32(cvStart.Y) == Convert.ToInt32(cvEnd.Y)) // then its a horizontal line
                            {
                                if (cvStart.X < cvEnd.X) // drawn left to right, keep start    X--------->
                                {
                                    cvLeft = cvStart;
                                }

                                else if (cvStart.X > cvEnd.X) // drawn right to left, keep end    <---------X
                                {
                                    cvLeft = cvEnd;
                                }
                            }

                            else if (Convert.ToInt32(cvStart.X) == Convert.ToInt32(cvEnd.X)) // then its a vertical
                            {

                                if (cvStart.Y < cvEnd.Y) // drawn bottom to top, keep end
                                {
                                    cvTop = cvEnd;
                                }

                                else if (cvStart.Y > cvEnd.Y) // drawn top to bottom, keep start
                                {
                                    cvTop = cvStart;
                                }
                            }
                            #endregion

                            // Use top and left, and start and end to find which endpoint to turn on

                            if ((cvLeft != null && cvLeft.IsAlmostEqualTo(cvStart)) || (cvTop != null && cvTop.IsAlmostEqualTo(cvStart))) // the grid was drawn correctly
                            {
                                xGrid.ShowBubbleInView(DatumEnds.End0, uidoc.ActiveView);
                                xGrid.HideBubbleInView(DatumEnds.End1, uidoc.ActiveView);

                            }

                            else if ((cvLeft != null && cvLeft.IsAlmostEqualTo(cvEnd)) || cvTop != null && cvTop.IsAlmostEqualTo(cvEnd)) // the grid was drawn the other way around
                            {
                                xGrid.ShowBubbleInView(DatumEnds.End1, uidoc.ActiveView);
                                xGrid.HideBubbleInView(DatumEnds.End0, uidoc.ActiveView);
                            }


                        }
                    }

                    t.Commit();
                }


                return Result.Succeeded;
            }

            catch (Exception ex)
            {
                Utils.CatchDialog(ex);
                return Result.Failed;
            }
        }
    }
}
