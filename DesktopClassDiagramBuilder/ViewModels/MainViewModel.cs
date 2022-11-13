using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassDiagramBuilder;
using ClassDiagramBuilder.Models;
using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace DesktopClassDiagramBuilder.ViewModels
{
    public class MainViewModel : PropertyChangedNotifier
    {
        private TypeInfo typeInfo;
        private static ProjectAnalyzer projectAnalyzer;

        public MainViewModel()
        {
            projectAnalyzer = new ProjectAnalyzer();
            TypeInfo = projectAnalyzer.AnalyzeFile(@"C:\Users\Alex Raj\source\repos\ClassDiagramBuilder\ClassDiagramBuilder\Program.cs").FirstOrDefault();
        }

        public Graph<TypeInfo> Types { get; set; }

        public TypeInfo TypeInfo 
        {
            get => typeInfo;
            set
            {
                typeInfo = value;
                OnPropertyChanged();
            }
        }
    }
}
