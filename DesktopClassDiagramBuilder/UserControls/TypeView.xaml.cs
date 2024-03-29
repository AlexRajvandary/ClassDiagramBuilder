﻿using ClassDiagramBuilder.Models.TypeAnalyzerModels;
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
        public static readonly DependencyProperty typeInfoProperty = DependencyProperty.Register("TypeInfo", typeof(TypeInfo), typeof(TypeView), new PropertyMetadata(null));


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
