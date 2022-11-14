using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClassDiagramBuilder;
using ClassDiagramBuilder.Models;
using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace DesktopClassDiagramBuilder.ViewModels
{
    public class MainViewModel : PropertyChangedNotifier
    {
        ICommand calculate;
        private string path;
        private Graph<TypeInfo> types;
        private TypeInfo typeInfo;
        private static ProjectAnalyzer projectAnalyzer;

        public MainViewModel()
        {
            projectAnalyzer = new ProjectAnalyzer();
            calculate = new RelayCommand(GetTypeInfos);
        }

        public ICommand Calculate
        {
            get => calculate;
            set
            {
                calculate = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => path;
            set
            {
                path = value;
                OnPropertyChanged();
            }
        }

        public Graph<TypeInfo> Types
        {
            get => types;
            set
            {
                types = value;
                OnPropertyChanged();
            }
        }

        public TypeInfo TypeInfo
        {
            get => typeInfo;
            set
            {
                typeInfo = value;
                OnPropertyChanged();
            }
        }

        public void GetTypeInfos()
        {
            TypeInfo = projectAnalyzer?.AnalyzeFile(Path)?.FirstOrDefault();
        }
    }
}
