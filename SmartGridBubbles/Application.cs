using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using System.Reflection;

namespace SmartGridBubbles
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("A0C20AC6-2839-4F3F-BA16-B50605359F4C")]
    public class ThisApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication uiApp)
        {
            string ThisDllPath = Assembly.GetExecutingAssembly().Location;
            Assembly ThisAssembly = Assembly.GetExecutingAssembly();

            // Assembly that contains the invoke method
            String exeConfigPath = Path.GetDirectoryName(ThisDllPath) + "\\SmartGridBubbles.dll";

            RibbonPanel DefaultPanel = null;

            // Try creating the panel in Pyrevit Tab
            try
            {
                DefaultPanel = uiApp.CreateRibbonPanel("Gas Tools", "Quick Tools");
            }

            // Tab doesn't exist, create it
            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                uiApp.CreateRibbonTab("Gas Tools");
                DefaultPanel = uiApp.CreateRibbonPanel("Gas Tools", "Quick Tools");
            }

            try
            {

                string SmartGridBubblesName = "Smart Grid Bubbles";
                PushButtonData SmartGridBubblesData = new PushButtonData(SmartGridBubblesName, SmartGridBubblesName, exeConfigPath, "SmartGridBubbles.ThisCommand"); // Invoke class, pushbutton data
                SmartGridBubblesData.LargeImage = Utils.RetriveImage("SmartGridBubbles.Resources.GridsIcons32x32.ico"); // Pushbutton image
                SmartGridBubblesData.ToolTip = "Automatically turn on or off grid bubbles correctly";
                PushButton SmartGridsButton = DefaultPanel.AddItem(SmartGridBubblesData) as PushButton;

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
