using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassDiagramBuilder;
using ClassDiagramBuilder.Models.TypeAnalyzerModels;

namespace DesktopClassDiagramBuilder.ViewModels
{
    public class MainViewModel : PropertyChangedNotifier
    {
        public Graph<TypeInfo> Types { get; set; }
    }
}
