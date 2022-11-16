using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class EventInfo : MemberInfo
    {
        public EventInfo(AcsessModifiers acsessModifier,
            string delegateType,
            bool isAbstract,
            bool isStatic,
            string name,
            string nameSpace) : base(acsessModifier, isAbstract, isStatic, name, nameSpace)
        {
            DelegateType = delegateType;
        }

        public string DelegateType { get; set; }

        public override string ToString()
        {
            return $"{DelegateType} {Name}";
        }
    }
}
