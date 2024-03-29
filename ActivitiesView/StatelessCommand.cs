﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ActivitiesView {
    class StatelessCommand : ICommand {
        private readonly Action<object> _action;

        public StatelessCommand(Action action) : this((_) => action()) {
        }

        public StatelessCommand(Action<object> action) {
            _action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            _action(parameter);
        }
    }
}
