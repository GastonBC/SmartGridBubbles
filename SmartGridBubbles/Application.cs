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
    [Autodesk.Revit.DB.Macros.AddInId(GlobalVars.APP_GUID)]
    public class ThisApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication uiApp)
        {
            #region GAS ADDIN BOILERPLATE

            // Finds and creates the tab, finds and creates the panel



            string ThisDllPath = Assembly.GetExecutingAssembly().Location;
            Assembly ThisAssembly = Assembly.GetExecutingAssembly();

            // Assembly that contains the invoke method
            String exeConfigPath = Path.GetDirectoryName(ThisDllPath) + "\\SmartGridBubbles.dll";

            RibbonPanel DefaultPanel = null;


            // Create the panel in the addins tab
            try
            {
                DefaultPanel = uiApp.CreateRibbonPanel(GlobalVars.PANEL_NAME);
            }

            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                DefaultPanel = uiApp.GetRibbonPanels().FirstOrDefault(n => n.Name.Equals(GlobalVars.PANEL_NAME, StringComparison.InvariantCulture));
            }
            #endregion

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
