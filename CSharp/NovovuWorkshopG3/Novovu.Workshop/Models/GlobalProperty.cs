using Novovu.Workshop.Shared.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Novovu.Workshop.Models
{
    public class GlobalProperty
    {
        public string PropertyName {get;set;}
        public Vector3 Vector { get => _vector; set { _vector = value; OnItemChanged?.Invoke(); } }
        private Vector3 _vector;
        public string String { get => _string; set { _string = value;OnItemChanged?.Invoke(); } }
        private string _string;

        private Shared.ExecutableProperty _command;

        public Shared.ExecutableProperty Command
        {
            get => _command;
            set
            {
                _command = value;
                OnItemChanged?.Invoke();
            }
        }

        public delegate void OnChanged();
        public string SelectedEnumItem { get; set; }
        public ObservableCollection<string> EnumItems { get; set; }
        public event OnChanged OnItemChanged;
        public enum Types
        {
            Vector3,
            String,
            Enum, 
            Command
        }

        public Types PropertyType { get; set; }

        public bool IsString { get => PropertyType == Types.String; }
        public bool IsVector3 { get => PropertyType == Types.Vector3; }
        public bool IsEnum{ get => PropertyType == Types.Enum; }

        public bool IsCommand { get => PropertyType == Types.Command; }
    }
}
