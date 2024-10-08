﻿using BH.oM.Computing;
using BH.oM.Computing.RhinoCompute;
using Grasshopper.Kernel;
using System.Collections.Generic;

namespace BH.Engine.Computing.RhinoCompute
{
    public static partial class Query
    {
        public static RuntimeMessages RuntimeMessages(this GH_Document ghDoc)
        {
            List<string> errors = new List<string>();
            List<string> warnings = new List<string>();
            List<string> remarks = new List<string>();

            if (ghDoc != null)
                ghDoc.RuntimeMessages(out errors, out warnings, out remarks);

            return new RuntimeMessages() { Errors = errors, Warnings = warnings, Remarks = remarks };
        }

        public static void RuntimeMessages(this GH_Document gh_document, out List<string> errors, out List<string> warnings, out List<string> remarks)
        {
            List<IGH_ActiveObject> activeObjects = gh_document.ActiveObjects();

            errors = new List<string>();
            warnings = new List<string>();
            remarks = new List<string>();

            if (activeObjects == null)
                return;

            errors = RuntimeMessages(activeObjects, GH_RuntimeMessageLevel.Error);
            warnings = RuntimeMessages(activeObjects, GH_RuntimeMessageLevel.Warning);
            remarks = RuntimeMessages(activeObjects, GH_RuntimeMessageLevel.Remark);
        }

        public static List<string> RuntimeMessages(IEnumerable<IGH_ActiveObject> objs, GH_RuntimeMessageLevel messageLevel)
        {
            List<string> allObjsMessages = new List<string>();

            foreach (var obj in objs)
                allObjsMessages.AddRange(RuntimeMessages(obj, messageLevel));

            return allObjsMessages;
        }

        public static List<string> RuntimeMessages(IGH_ActiveObject obj, GH_RuntimeMessageLevel messageLevel, bool skipFirstNullErrorForBHoMComponents = true, bool printComponentType = false, bool printComponentGuid = false)
        {
            List<string> runtimeMessages = new List<string>();

            IList<string> messages = obj.RuntimeMessages(messageLevel);

            // For some reason, all BHoM components return the following error, even if their computation works.
            // For now, we avoid reporting the first occurrence of this error on BHoM components. 
            // This obviously brings risks as we could miss to report true positives.
            // TODO: discover why.
            string nullErrorText = "Solution exception:Object reference not set to an instance of an object.";

            foreach (var msg in messages)
            {
                if (skipFirstNullErrorForBHoMComponents && obj.GetType().FullName.StartsWith("BH.UI") && msg == nullErrorText)
                {
                    skipFirstNullErrorForBHoMComponents = false;
                    continue;
                }

                // BHoM enums also show irrelevant error message, even if they work. We skip those.
                if (obj.GetType().FullName == "BH.UI.Grasshopper.Components.CreateEnumComponent" 
                    && msg == "Object reference not set to an instance of an object.")
                {
                    continue;
                }

                string msgToAdd = $"{messageLevel} message from component named `{obj.Name}`:";
                msgToAdd += $"\n\t{msg}`";

                if (printComponentType)
                    msgToAdd += $"\nComponent type: {obj.GetType().FullName}";
                if (printComponentGuid)
                    msgToAdd += $"\nComponent GUID: `{obj.InstanceGuid}`";

                runtimeMessages.Add(msgToAdd);
            }

            return runtimeMessages;
        }
    }
}
