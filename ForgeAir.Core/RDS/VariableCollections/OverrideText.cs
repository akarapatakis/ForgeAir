using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.RDS.VariableCollections
{
    public static class OverrideText
    {
        public static string OverrideTextWithVariable(string inText) {

            foreach (var placeholder in TextVariables.Variables)
            {
                if (inText.Contains(placeholder.Key))
                {
                    inText.Replace(placeholder.Key, placeholder.Value());
                }
            }
            return inText;

        }
    }
}
