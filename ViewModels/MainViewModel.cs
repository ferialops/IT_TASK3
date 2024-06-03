using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Microsoft.Win32;
namespace task3.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Assembly _assembly;
        private Type _selectedType;
        private MethodInfo _selectedMethod;

        public ObservableCollection<string> ClassNames { get; set; }
        public ObservableCollection<string> MethodNames { get; set; }
        public ObservableCollection<ParameterViewModel> Parameters { get; set; }

        public string SelectedClassName
        {
            get => _selectedClassName;
            set
            {
                _selectedClassName = value;
                OnPropertyChanged(nameof(SelectedClassName));
                LoadMethods();
            }
        }

        public string SelectedMethodName
        {
            get => _selectedMethodName;
            set
            {
                _selectedMethodName = value;
                OnPropertyChanged(nameof(SelectedMethodName));
                LoadParameters();
            }
        }

        public ICommand LoadAssemblyCommand { get; }
        public ICommand ExecuteMethodCommand { get; }

        private string _selectedClassName;
        private string _selectedMethodName;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            ClassNames = new ObservableCollection<string>();
            MethodNames = new ObservableCollection<string>();
            Parameters = new ObservableCollection<ParameterViewModel>();

            LoadAssemblyCommand = new RelayCommand(o => LoadAssembly());
            ExecuteMethodCommand = new RelayCommand(o => ExecuteMethod());
        }

        private void LoadAssembly()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "DLL Files (*.dll)|*.dll"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _assembly = Assembly.LoadFrom(openFileDialog.FileName);
                LoadClasses();
            }
        }

        private void LoadClasses()
        {
            ClassNames.Clear();
            var types = _assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(task3.Models.ОсветительныйПрибор)));

            foreach (var type in types)
            {
                ClassNames.Add(type.Name);
            }
        }

        private void LoadMethods()
        {
            MethodNames.Clear();
            _selectedType = _assembly.GetTypes().FirstOrDefault(t => t.Name == SelectedClassName);

            if (_selectedType != null)
            {
                var methods = _selectedType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    MethodNames.Add(method.Name);
                }
            }
        }

        private void LoadParameters()
        {
            Parameters.Clear();
            _selectedMethod = _selectedType.GetMethod(SelectedMethodName);

            if (_selectedMethod != null)
            {
                var parameters = _selectedMethod.GetParameters();
                foreach (var param in parameters)
                {
                    Parameters.Add(new ParameterViewModel { Name = param.Name, Type = param.ParameterType });
                }
            }
        }

        private void ExecuteMethod()
        {
            var instance = Activator.CreateInstance(_selectedType);
            var parameterValues = Parameters.Select(p => Convert.ChangeType(p.Value, p.Type)).ToArray();
            _selectedMethod.Invoke(instance, parameterValues);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}