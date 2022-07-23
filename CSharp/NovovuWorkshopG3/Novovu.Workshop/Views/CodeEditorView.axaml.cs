using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using AvaloniaEdit.Rendering;
using Novovu.Workshop.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Novovu.Workshop.Views
{
    using Pair = KeyValuePair<int, IControl>;
    public class CodeEditorView : UserControl
    {
        private readonly AvaloniaEdit.TextEditor _textEditor;
        private CompletionWindow _completionWindow;
        private OverloadInsightWindow _insightWindow;

        private ElementGenerator _generator = new ElementGenerator();
        private IPopupImpl impl;
        public CodeEditorView()
        { 
        }
        public CodeEditorView(CodeEditorViewModel mmvm)
        {
            InitializeComponent();

            _textEditor = this.FindControl<AvaloniaEdit.TextEditor>("Editor");
            _textEditor.Background = Brushes.Transparent;
            _textEditor.ShowLineNumbers = true;
            using (Stream s = File.OpenRead("Grammar/JS.xml"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    _textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            _textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            _textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            _textEditor.TextArea.IndentationStrategy = new AvaloniaEdit.Indentation.CSharp.CSharpIndentationStrategy();
            this.KeyUp += CodeEditorView_KeyUp;

            _textEditor.TextArea.TextView.ElementGenerators.Add(_generator);

            impl = PlatformManager.CreateWindow().CreatePopup();

            this.AddHandler(PointerWheelChangedEvent, (o, i) =>
            {
                if (i.KeyModifiers != KeyModifiers.Control) return;
                if (i.Delta.Y > 0) _textEditor.FontSize++;
                else _textEditor.FontSize = _textEditor.FontSize > 1 ? _textEditor.FontSize - 1 : 1;
            }, RoutingStrategies.Bubble, true);

            _textEditor.Text = mmvm.ToOpen;
            ViewModel = mmvm;
            mmvm.CodeEditor = this;
        }

        bool Saved = true;
        private void CodeEditorView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && e.KeyModifiers == KeyModifiers.Control)
            {
                ViewModel?.Save(_textEditor.Text);
                Saved = true;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        public bool SaveStateChanged = false;
        void textEditor_TextArea_TextEntering(object sender, TextInputEventArgs e)
        {
            
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    //_completionWindow.CompletionList.RequestInsertion(e);
                }
            }

            
            _insightWindow?.Hide();

            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }
        CodeEditorViewModel ViewModel;
        public void Open(string src)
        {
            _textEditor.Text = src;
            
        }
        void textEditor_TextArea_TextEntered(object sender, TextInputEventArgs e)
        {
            
            if (Saved)
            {
                ViewModel?.UnSave();
                Saved = false;
            }
            if (e.Text == ".")
            {

                //_completionWindow = new CompletionWindow(_textEditor.TextArea);
                //_completionWindow.Closed += (o, args) => _completionWindow = null;

               // var data = _completionWindow.CompletionList.CompletionData;


                //_completionWindow.Show();
            }
            else if (e.Text == "(")
            {
                //_insightWindow = new OverloadInsightWindow(_textEditor.TextArea);
              //  _insightWindow.Closed += (o, args) => _insightWindow = null;
                
                //_insightWindow.Provider = new MyOverloadProvider(new[]
                //{
                //    ("Method1(int, string)", "Method1 description"),
                //    ("Method2(int)", "Method2 description"),
                //    ("Method3(string)", "Method3 description"),
                //});

               // _insightWindow.Show();
            }
        }

        private class MyOverloadProvider : IOverloadProvider
        {
            private readonly IList<(string header, string content)> _items;
            private int _selectedIndex;

            public MyOverloadProvider(IList<(string header, string content)> items)
            {
                _items = items;
                SelectedIndex = 0;
            }

            public int SelectedIndex
            {
                get => _selectedIndex;
                set
                {
                    _selectedIndex = value;
                    OnPropertyChanged();
                    // ReSharper disable ExplicitCallerInfoArgument
                    OnPropertyChanged(nameof(CurrentHeader));
                    OnPropertyChanged(nameof(CurrentContent));
                    // ReSharper restore ExplicitCallerInfoArgument
                }
            }

            public int Count => _items.Count;
            public string CurrentIndexText => null;
            public object CurrentHeader => _items[SelectedIndex].header;
            public object CurrentContent => _items[SelectedIndex].content;



            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class MyCompletionData : ICompletionData
        {
            public MyCompletionData(string text)
            {
                Text = text;
            }

            public IBitmap Image => null;

            public string Text { get; }

            // Use this property if you want to show a fancy UIElement in the list.
            public object Content => Text;

            public object Description => "Description for " + Text;

            public double Priority { get; } = 0;

            public void Complete(TextArea textArea, ISegment completionSegment,
                EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, Text);
            }
        }

        class ElementGenerator : VisualLineElementGenerator, IComparer<Pair>
        {
            public List<Pair> controls = new List<Pair>();

            /// <summary>
            /// Gets the first interested offset using binary search
            /// </summary>
            /// <returns>The first interested offset.</returns>
            /// <param name="startOffset">Start offset.</param>
            public override int GetFirstInterestedOffset(int startOffset)
            {
                int pos = controls.BinarySearch(new Pair(startOffset, null), this);
                if (pos < 0)
                    pos = ~pos;
                if (pos < controls.Count)
                    return controls[pos].Key;
                else
                    return -1;
            }

            public override VisualLineElement ConstructElement(int offset)
            {
                int pos = controls.BinarySearch(new Pair(offset, null), this);
                if (pos >= 0)
                    return new InlineObjectElement(0, controls[pos].Value);
                else
                    return null;
            }

            int IComparer<Pair>.Compare(Pair x, Pair y)
            {
                return x.Key.CompareTo(y.Key);
            }
        }
    }
}
