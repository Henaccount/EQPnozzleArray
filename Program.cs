using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.ProcessPower.DataLinks;
using Autodesk.ProcessPower.DataObjects;
using Autodesk.ProcessPower.PartsRepository;
using Autodesk.ProcessPower.PnP3dDataLinks;
using Autodesk.ProcessPower.PnP3dEquipment;
using Autodesk.ProcessPower.PnP3dObjects;
using Autodesk.ProcessPower.ProjectManager;
using System;
using System.Collections.Generic;
using PlantApp = Autodesk.ProcessPower.PlantInstance.PlantApplication;
using System.Windows.Forms;

namespace EQPnozzleArray
{
    public class Program
    {
        [CommandMethod("EQPnozzleArray")]
        public void EQPnozzleArray()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptEntityResult equipResult = ed.GetEntity("\nSelect an equipment object: ");
            if (equipResult.Status != PromptStatus.OK || !(equipResult.ObjectId.IsValid))
            {
                ed.WriteMessage("\nInvalid selection. Please select an equipment object.");
                return;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                if (equipResult.ObjectId.ObjectClass.Name.Equals("AcPpDb3dEquipment"))
                {
                    Equipment Eqp = tr.GetObject(equipResult.ObjectId, OpenMode.ForRead) as Equipment;
                    ObjectId eqpid = equipResult.ObjectId;
                    // Create and add our message filter

                    MyMessageFilter filter = new MyMessageFilter();

                    System.Windows.Forms.Application.AddMessageFilter(filter);


                    // Start the loop

                    while (true)

                    {

                        // Check for user input events

                        System.Windows.Forms.Application.DoEvents();


                        // Check whether the filter has set the flag

                        if (filter.bCanceled == true)

                        {

                            ed.WriteMessage("\nLoop cancelled.");

                            break;

                        }

                        ed.WriteMessage("\nInside while loop...");
                        ed.Command("PLANTNOZZLEADD", eqpid, "LOCation", "LIne");
                        
                    }

                    // We're done - remove the message filter

                    System.Windows.Forms.Application.RemoveMessageFilter(filter);

                    //first nozzle: user selects type, type information will be saved in variable
                    //following nozzles: only select line and use default type

                    tr.Commit();
                }
                
            }
        }

        public class MyMessageFilter : IMessageFilter

        {

            public const int WM_KEYDOWN = 0x0100;


            public bool bCanceled = false;


            public bool PreFilterMessage(ref Message m)

            {

                if (m.Msg == WM_KEYDOWN)

                {

                    // Check for the Escape keypress

                    Keys kc = (Keys)(int)m.WParam & Keys.KeyCode;

                    if (m.Msg == WM_KEYDOWN && kc == Keys.Escape)

                    {

                        bCanceled = true;

                    }

                    // Return true to filter all keypresses

                    return true;

                }

                // Return false to let other messages through

                return false;

            }

        }

    }


}
