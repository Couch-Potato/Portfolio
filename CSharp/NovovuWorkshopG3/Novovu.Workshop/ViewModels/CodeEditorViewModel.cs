using AvaloniaEdit.CodeCompletion;
using Dock.Model.Controls;
using Novovu.Workshop.DockSystem;
using Novovu.Workshop.TypeInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class CodeEditorViewModel: Document, IDockNameable
    {
        public List<ICompletionData> CompletionData = new List<ICompletionData>();

        public Views.CodeEditorView CodeEditor;
        private WScript _script = default;
        internal string ToOpen = "";
        public void OpenCode(string src, ref WScript script)
        {
            ToOpen = src;
            CodeEditor?.Open(src);
            OnSaved += CodeEditorViewModel_OnSaved;
            _script = script;
        }

        private void CodeEditorViewModel_OnSaved(string obj)
        {
            _script.Source = obj;
        }

        public void Modified()
        {
            Title = Title + "*";
        }
        public void Save(string s)
        {
            Title = Title.TrimEnd('*');
            OnSaved?.Invoke(s);
        }
        public void UnSave()
        {
            Title = Title + "*";
        }
        public event Action<string> OnSaved;
        public string Name => "Code Editor";
    }
}
