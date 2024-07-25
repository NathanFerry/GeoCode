using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.MstnPlatformNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCode.Utils
{
    public class Scan
    {
        public static List<Element> GetElements(List<MSElementType> elementTypes)
        {
            ScanCriteria criteria = new ScanCriteria();
            criteria.SetModelRef(Session.Instance.GetActiveDgnModelRef());
            criteria.AddElementTypes(elementTypes.ToArray());

            return criteria.Scan().ToList();
        }
}
}
