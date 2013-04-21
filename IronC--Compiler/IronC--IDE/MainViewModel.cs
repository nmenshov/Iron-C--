using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using IronC__Common;
using IronC__Common.Trees;
using IronC__Generator;
using IronC__Lexical;
using IronC__Semantics;
using IronC__Syntax;

namespace IronC__IDE
{
    public class MainViewModel: ViewModelBase
    {
        private const string TITLE = "IronC-- IDE";

        public RelayCommand<KeyEventArgs> KeyDownCommand { get; private set; }
        public RelayCommand CompileCommand { get; private set; }
        public RelayCommand RunCommand { get; private set; }
        public RelayCommand NewFileCommand { get; private set; }
        public RelayCommand LoadFileCommand { get; private set; }
        public RelayCommand SaveFileCommand { get; private set; }
        public RelayCommand SaveAsCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        private string _lastUsedFileName = string.Empty;

        private TextDocument _code = new TextDocument();
        public TextDocument Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);
            }
        }

        private string _title = TITLE;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        private IList<string> _errors;
        public IList<string> Errors
        {
            get { return _errors; }
            set
            {
                _errors = value;
                RaisePropertyChanged(() => Errors);
            }
        }

        public MainViewModel()
        {
            KeyDownCommand = new RelayCommand<KeyEventArgs>(KeyDownAction);
            CompileCommand = new RelayCommand(CompileAction);
            RunCommand = new RelayCommand(RunAction);
            NewFileCommand = new RelayCommand(NewFileAction);
            LoadFileCommand = new RelayCommand(LoadFileAction);
            SaveFileCommand = new RelayCommand(SaveFileAction);
            SaveAsCommand = new RelayCommand(SaveAsAction);
            ExitCommand = new RelayCommand(ExitAction);
        }

        private void CompileAction()
        {
            var tree = Compile();
            if (tree == null)
                return;

            var gen = new CodeGenerator(tree);
            gen.Generate(Path.GetFileNameWithoutExtension(_lastUsedFileName) + ".exe");
        }

        private void RunAction()
        {
            var tree = Compile();
            if (tree == null)
                return;

            var gen = new CodeGenerator(tree);
            gen.Generate(Path.GetFileNameWithoutExtension(_lastUsedFileName) + ".exe");
            using (var proc = Process.Start(Path.GetFileNameWithoutExtension(_lastUsedFileName) + ".exe"))
            {
                proc.WaitForExit();
            }
        }

        private ITree Compile()
        {
            var reader = new Reader("input.txt");
            var grammar = reader.ReadGrammar();

            var lexical = new LexicalAnalyzer(grammar, "LA.xml");
            var tokens = lexical.Convert(Code.Text);

            var syntax = new SyntaxAnalyzer(tokens);
            var tree = syntax.Analyze();

            if (syntax.Errors.Any())
            {
                Errors = syntax.Errors;
                return null;
            }

            var semantics = new SemanticAnalyzer(tree);
            semantics.DecorateAndValidateTree();

            if (semantics.Errors.Any())
            {
                Errors = semantics.Errors;
                return null;
            }

            return tree;
        }

        private void KeyDownAction(KeyEventArgs args)
        {
            if (args.Key == Key.F5)
                RunAction();
            else if (args.Key == Key.F6)
                CompileAction();
            else if (args.Key == Key.S && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
                SaveFileAction();
            else if (args.Key == Key.O && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
                LoadFileAction();
            else if (args.Key == Key.N && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
                NewFileAction();
        }

        private void NewFileAction()
        {
            if (string.IsNullOrEmpty(Code.Text))
                return;

            var res = MessageBox.Show("File isn't saved, save?", "New file", MessageBoxButton.YesNoCancel);
            if (res == MessageBoxResult.Yes)
            {
                SaveFileAction();
                Code = new TextDocument();
            }
            else if (res == MessageBoxResult.No)
                Code = new TextDocument();
        }

        private void LoadFileAction()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".c--";
            dlg.Filter = @"C-- programs (.c--)|*.c--|All files|*.*";

            if (dlg.ShowDialog() == true)
            {
                _lastUsedFileName = dlg.FileName;
                Title = TITLE + " " + Path.GetFileName(_lastUsedFileName);
                using (var reader = new StreamReader(dlg.OpenFile()))
                {
                    Code = new TextDocument(reader.ReadToEnd());
                }
            }
        }

        private void SaveFileAction()
        {
            if (!string.IsNullOrEmpty(_lastUsedFileName))
                SaveFile(_lastUsedFileName);
            else
                SaveAsAction();
        }

        private void SaveAsAction()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".c--";
            dlg.Filter = @"C-- programs (.c--)|*.c--|All files|*.*";

            if (dlg.ShowDialog() == true)
                SaveFile(dlg.FileName);
        }

        private void SaveFile(string fileName)
        {
            _lastUsedFileName = fileName;
            Title = TITLE + " " + Path.GetFileName(_lastUsedFileName);
            using (var writer = new StreamWriter(File.Open(fileName, FileMode.Create)))
            {
                writer.Write(Code.Text);
            }
        }

        private void ExitAction()
        {
            App.Current.Shutdown();
        }
    }
}
