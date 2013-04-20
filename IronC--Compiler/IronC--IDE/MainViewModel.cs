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
        public RelayCommand ErrorsCommand { get; private set; }

        private string _lastUsedFileName = string.Empty;
        private readonly ErrorsView _errorsView;

        private string _code = string.Empty;
        public string Code
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
            ErrorsCommand = new RelayCommand(ErrorsAction);
            _errorsView = new ErrorsView();
        }

        private void CompileAction()
        {
            var tree = Compile();
            if (tree == null)
            {
                ErrorsAction();
                return;
            }

            var gen = new CodeGenerator(tree);
            gen.Generate(Path.GetFileNameWithoutExtension(_lastUsedFileName) + ".exe");
            MoveFile();
        }

        private void RunAction()
        {
            var tree = Compile();
            if (tree == null)
            {
                ErrorsAction();
                return;
            }

            var gen = new CodeGenerator(tree);
            gen.Generate(Path.GetFileNameWithoutExtension(_lastUsedFileName) + ".exe");
            MoveFile();
            using (var proc = Process.Start(GetExePath()))
            {
                proc.WaitForExit();
            }
        }

        private void MoveFile()
        {
            if (File.Exists(GetExePath()))
                File.Delete(GetExePath());
            File.Move(Path.GetFileNameWithoutExtension(_lastUsedFileName) + ".exe", GetExePath());
        }

        private string GetExePath()
        {
            int fnlen = Path.GetFileName(_lastUsedFileName).Length;
            var dir = _lastUsedFileName.Remove(_lastUsedFileName.Length - fnlen, fnlen);
            return Path.Combine(dir, Path.GetFileNameWithoutExtension(_lastUsedFileName) + ".exe");
        }

        private ITree Compile()
        {
            var reader = new Reader("input.txt");
            var grammar = reader.ReadGrammar();

            var lexical = new LexicalAnalyzer(grammar, "LA.xml");
            var tokens = lexical.Convert(Code);

            var syntax = new SyntaxAnalyzer(tokens);
            var tree = syntax.Analyze();

            if (syntax.Errors.Any())
            {
                _errorsView.SetErrors(syntax.Errors);
                return null;
            }

            var semantics = new SemanticAnalyzer(tree);
            semantics.DecorateAndValidateTree();

            if (semantics.Errors.Any())
            {
                _errorsView.SetErrors(semantics.Errors);
                return null;
            }

            return tree;
        }

        private void ErrorsAction()
        {
            _errorsView.Show();
        }

        private void KeyDownAction(KeyEventArgs args)
        {
            if (args.Key == Key.F5)
                RunAction();
            else if (args.Key == Key.F6)
                CompileAction();
            else if (args.Key == Key.S && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
                SaveFileAction();
        }

        private void NewFileAction()
        {
            if (string.IsNullOrEmpty(Code))
                return;

            var res = MessageBox.Show("File isn't saved, save?", "New file", MessageBoxButton.YesNoCancel);
            if (res == MessageBoxResult.Yes)
            {
                SaveFileAction();
                Code = string.Empty;
            }
            else if (res == MessageBoxResult.No)
                Code = string.Empty;
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
                    Code = reader.ReadToEnd();
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
                writer.Write(Code);
            }
        }

        private void ExitAction()
        {
            App.Current.Shutdown();
        }
    }
}
