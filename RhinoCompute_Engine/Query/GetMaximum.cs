﻿using BH.oM.RemoteCompute.RhinoCompute;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;


namespace BH.Engine.RemoteCompute.RhinoCompute
{
    public static partial class Query
    {
        public static double? GetMaximum(this IGH_Param param)
        {
            if (param is IGH_ContextualParameter contextualParameter && param.Sources.Count == 1)
                return GetMaximum(param.Sources[0]);

            if (param is GH_NumberSlider paramSlider)
                return System.Convert.ChangeType(paramSlider.Slider.Maximum, typeof(double)) as double?;

            return null;
        }
    }
}
