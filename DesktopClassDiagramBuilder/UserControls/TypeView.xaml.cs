using ClassDiagramBuilder.Models.TypeAnalyzerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopClassDiagramBuilder.UserControls
{
    /// <summary>
    /// Interaction logic for TypeView.xaml
    /// </summary>
    public partial class TypeView : UserControl
    {
        public static readonly DependencyProperty typeInfoProperty = DependencyProperty.Register("TypeInfo", typeof(TypeInfo), typeof(TypeView), new PropertyMetadata(null, ResizeUserControl));
        private double listItemHeight = 22;
        private static void ResizeUserControl(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is TypeView typeView)
            {
                typeView.FieldList.Height = typeView.TypeInfo.Fields.Count * typeView.listItemHeight;
                typeView.ConstructorsList.Height = typeView.TypeInfo.Constructors.Count * typeView.listItemHeight;
                typeView.PropertiesList.Height = typeView.TypeInfo.Properties.Count * typeView.listItemHeight;
                typeView.MethodsList.Height = typeView.TypeInfo.Methods.Count * typeView.listItemHeight;
            }
        }

        public TypeView()
        {
            InitializeComponent();
        }

        public TypeInfo TypeInfo
        {
            get
            {
                return (TypeInfo)GetValue(typeInfoProperty);
            }
            set
            {
                SetValue(typeInfoProperty, value);
            }
        }
    }
}
