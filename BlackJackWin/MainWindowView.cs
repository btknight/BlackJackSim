using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;


namespace BlackJackWin
{
    /// <summary>
    /// View objects for the main window.
    /// </summary>
    public class MainWindowView : INotifyPropertyChanged
    {
        private Dispatcher MWDispatcher;

        /// <summary>
        /// Output from the modules or from the data model object (the executive).
        /// </summary>
        public string Output
        {
            get { return _Output; }
            private set {
                if (RouteToUIThread(() => Output = value)) { return; }
                if (_Output != value)
                {
                    _Output = value;
                    OnPropertyChanged("Output");
                }
            }
        }
        private string _Output = "";



        /// <summary>
        /// Value indicating whether it is permissible to start a test run.
        /// Set to true if no test is presently executing, false if it is.
        /// </summary>
        public bool CanExecute
        {
            get { return _CanExecute; }
            set
            {
                if (RouteToUIThread(() => CanExecute = value)) { return; }
                if (_CanExecute != value)
                {
                    _CanExecute = value;
                    OnPropertyChanged("CanExecute");
                }
            }
        }
        private bool _CanExecute = true;


        // ICommand implementations for Viewmodels must be presented as properties!
        /// <summary>
        /// Sets the ExecCommand object exposed to the MainWindow.
        /// </summary>
        public ExecCommand StartGames { get; private set; }


        /// <summary>
        /// Instantiates the view model.  Initializes the data model as well.
        /// </summary>
        public MainWindowView()
        {
            // Required to marshal window updates to the UI thread.
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                MWDispatcher = App.Current.MainWindow.Dispatcher;
            }
            Exec = new PEExecution(this);
            TestList = new ObservableCollection<PEModule>(Exec.Tests);
            StartGames = new ExecCommand(x => { Exec.Execute(); }, this, "CanExecute");
        }

        /// <summary>
        ///     Defines the event for INotifyPropertyChanged.  Window elements will subscribe to this event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Helper function used to signal subscribers to PropertyChanged that a change has, in fact, occurred.
        /// </summary>
        /// <param name="PropertyName">Name of the changed property</param>
        protected void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
        


        /// <summary>
        /// Helps route events to the UI thread.  View model properties may be set by any thread,
        /// but those properties must be set and signaled within the UI thread.
        /// This function helps route those updates to the correct thread.  If the update is attempted on the wrong thread,
        /// this function calls the main window's dispatcher to attempt the same operation on the correct thread.
        /// If the update is done on the correct thread, this function returns false to signal the property setter
        /// to go ahead, change the property, and send the property change notification.
        /// </summary>
        /// <param name="action">The set operation desired by the user</param>
        /// <returns>True if the update was routed to the main window thread, false if the update did not need to be routed.</returns>
        private bool RouteToUIThread(Action action)
        {
            if (MWDispatcher == null) { return false; }
            if (MWDispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                MWDispatcher.Invoke(action);
                return true;
            }
            return false;
        }
    }
}
