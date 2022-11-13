namespace ClassDiagramBuilder.Models.TypeAnalyzerModels
{
    public class PropertyInfo : MemberInfo
    {
        public PropertyInfo(AcsessModifiers acsessModifier,
                            bool isAbstract,
                            bool isAuto,
                            bool isStatic,
                            string typeName,
                            string name,
                            string nameSpace,
                            AcsessModifiers getterAcsessModifier = AcsessModifiers.Private,
                            AcsessModifiers setterAcsessModifier = AcsessModifiers.Private) : base(acsessModifier, isAbstract, isStatic, name, nameSpace)
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