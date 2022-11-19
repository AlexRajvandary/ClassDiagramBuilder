using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        IAsyncCommand calculate;
        private string path;
        private Graph<TypeInfo> types;
        private TypeInfo typeInfo;
        private ObservableCollection<TypeInfo> typeInfos;
        private static ProjectAnalyzer projectAnalyzer;

        public MainViewModel()
        {
            projectAnalyzer = new ProjectAnalyzer();
            calculate = new AsyncCommand(GetTypeInfos);
            TypeInfos = new ObservableCollection<TypeInfo>();
        }

        public IAsyncCommand Calculate
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

        public ObservableCollection<TypeInfo> TypeInfos
        {
            get => typeInfos;
            set
            {
                typeInfos = value;
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

        public async Task GetTypeInfos()
        {
            projectAnalyzer = new ProjectAnalyzer();
            projectAnalyzer.FileExtensionsToAnalyze = new List<string>() { @".cs" };
            projectAnalyzer.FoldersToIgnore = new List<string>() { @".git", @".vs", @"bin", @"obj" };

            if (string.IsNullOrWhiteSpace(Path))
            {
                return;
            }

            var r = System.IO.Path.GetFileName(Path);

            if (!string.IsNullOrEmpty(r))
            {
                var typeInfos = await projectAnalyzer.AnalyzeFileAsync(Path);

                if (typeInfos == null)
                {
                    return;
                }

                foreach (var typeInfo in typeInfos)
                {
                    TypeInfos.Add(typeInfo);
                }
            }
            else 
            {
                var filesTree = projectAnalyzer.BuildTree(Path);

                foreach (var filePath in ReadTree(filesTree))
                {
                    var typeInfos = await projectAnalyzer.AnalyzeFileAsync(filePath);

                    if (typeInfos == null)
                    {
                        continue;
                    }

                    foreach (var typeInfo in typeInfos)
                    {
                        TypeInfos.Add(typeInfo);
                    }
                }
            }
        }

        private List<string> ReadTree(Node<List<string>> treeToRead)
        {
            if (treeToRead == null || (!treeToRead?.Data?.Any() ?? true))
            {
                return null;
            }

            var data = new List<string>();

            foreach (var file in treeToRead.Data)
            {
                data.Add(file);
            }

            if (!treeToRead?.Children?.Any() ?? true)
            {
                return data;
            }

            foreach (var folder in treeToRead.Children)
            {
                var filesFromFolder = ReadTree(folder);
                if (filesFromFolder != null)
                {
                    data.AddRange(ReadTree(folder));
                }
            }

            return data;
        }
    }
}
