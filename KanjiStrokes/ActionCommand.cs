using System;
using System.Windows.Input;

namespace KanjiStrokes {
	public sealed class ActionCommand : ICommand {

		private readonly Action _action;

		public ActionCommand(Action action) {
			_action = action;
		}

		public bool CanExecute(object parameter) => true;

		public void Execute(object parameter) => _action?.Invoke();

		public event EventHandler CanExecuteChanged;
	}

	public class ActionCommand<T> : ICommand {

		private readonly Action<T> _action;

		public ActionCommand(Action<T> action) {
			_action = action;
		}

		public bool CanExecute(object parameter) => true; //parameter is T;

		public void Execute(object parameter) => _action?.Invoke((T)parameter);

		public event EventHandler CanExecuteChanged;
	}
}
