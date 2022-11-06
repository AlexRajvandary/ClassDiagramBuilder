namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class PropertyInfo : MemberInfo
    {
        public PropertyInfo(AcsessModifiers acsessModifier,
                            AcsessModifiers getterAcsessModifier,
                            AcsessModifiers setterAcsessModifier,
                            bool isAbstract,
                            bool isAuto,
                            bool isStatic,
                            string name,
                            string nameSpace) : base(acsessModifier, isAbstract, isStatic, name, nameSpace)
        {
            GetterAcsessModifier = getterAcsessModifier;
            IsAuto = isAuto;
            SetterAcsessModifier = setterAcsessModifier;
        }

        public AcsessModifiers GetterAcsessModifier { get; private set; }

        public bool IsAuto { get; private set; }

        public AcsessModifiers SetterAcsessModifier { get; private set; }
    }
}