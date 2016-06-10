using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BlackJackWin
{
    /// <summary>
    /// An implementation of ICommand that will monitor a Boolean property in the view-model.  
    /// If that property is set to "true," the command will execute.  If it is "false," the command will NOT execute.
    /// </summary>
    /// <remarks>
    ///     I needed an ICommand implementation that could monitor a property in the ViewModel
    ///     and be able to send CanExecuteChanged events when that property changed.  The 
    ///     ExecutionDelegate is straightforward, but the property monitoring for CanExecute isn't.
    ///     Inspired by 
    ///     http://blogs.msdn.com/b/mikehillberg/archive/2009/03/20/icommand-is-like-a-chocolate-cake.aspx
    ///     (and who doesn't like chocolate cake?)
    /// </remarks>
    public class ExecCommand : ICommand
    {
        private Action<object> ExecutionDelegate { get; set; }
        private Predicate<object> CanExecuteDelegate { get; set; }
        private string MonitoredProperty { get; set; }
        private INotifyPropertyChanged MonitoredObject { get; set; }

        /// <summary>
        ///     Defines the ICommand event CanExecuteChanged.
        ///     This event will fire when the view model's CanExecute property changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Initializes the ExecCommand object.  Sets up the monitoring of the view model's CanExecute property.
        /// </summary>
        /// <param name="executionDelegate">Delegate function to be called when this object's Execute method is called</param>
        /// <param name="monitoredObject">Object to monitor to determine whether the Execute method can run</param>
        /// <param name="monitoredProperty">String of the name of the property to monitor</param>
        /// <seealso cref="PEWindowViewmodel.PEWindowViewmodel()"/>
        public ExecCommand(Action<object> executionDelegate, 
            INotifyPropertyChanged monitoredObject, string monitoredProperty)
        {
            this.ExecutionDelegate = executionDelegate;
            this.MonitoredObject = monitoredObject;
            this.MonitoredProperty = monitoredProperty;
            PropertyInfo p = MonitoredObject.GetType().GetProperty(monitoredProperty);
            if(p.CanRead == false)
            {
                throw new Exception(String.Concat("Insufficient access to read \"", monitoredProperty, "\""));
            }
            if(p.PropertyType != typeof(bool))
            {
                throw new Exception(String.Concat("Monitored property \"", monitoredProperty, "\" does not return type bool"));
            }
            p.GetValue(MonitoredObject, null);
            this.CanExecuteDelegate = x => { return (bool)p.GetValue(MonitoredObject, null); };
            MonitoredObject.PropertyChanged += new PropertyChangedEventHandler(MonitoredObject_PropertyChanged);
        }

        void MonitoredObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MonitoredProperty && CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        /// <summary>
        ///     Runs the command.
        /// </summary>
        /// <param name="parameter">Parameter to be passed to the execution method</param>
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                ExecutionDelegate(parameter);
            }
        }

        /// <summary>
        ///     Indicates whether the command can be run.
        /// </summary>
        /// <param name="parameter">Parameter to be passed to the execution method</param>
        /// <returns>True if the command can be executed, false if not</returns>
        public bool CanExecute(object parameter)
        {
            return CanExecuteDelegate(parameter);
        }
    }
}
