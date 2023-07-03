using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Triamec.Tam.Configuration;
using Triamec.Tam.Samples.Properties;
using Triamec.TriaLink;
using Triamec.TriaLink.Adapter;

// Rlid19 represents the register layout of drives of the current generation. A previous generation drive has layout 4.
using Axis = Triamec.Tam.Rlid19.Axis;

namespace Triamec.Tam.Samples {
    /// <summary>
    /// The main form of the TAM "XY Demo" application.
    /// </summary>
    internal partial class XYDemo : Form {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="XYDemo"/> class.
        /// </summary>
        public XYDemo() {
            InitializeComponent();
        }
        #endregion Constructor

        #region Hello world code

        /// <summary>
        /// The name of the axis this demo works with.
        /// </summary>
        // CAUTION!
        // Selecting the wrong axis can have unintended consequences.
        static readonly string yAxisName = Settings.Default.yAxisName;
        static readonly string xAxisName = Settings.Default.xAxisName;

        /// <summary>
        /// The distance to move when pressing one of the move buttons.
        /// </summary>
        // CAUTION!
        // The unit of this constant depends on the PositionUnit parameter provided with the TAM configuration.
        // Additionally, the encoder must be correctly configured.
        // Consider any limit stops.
        //static readonly double yMin = Settings.Default.yMin;
        //static readonly double yMax = Settings.Default.yMax;
        //static readonly double xMin = Settings.Default.xMin;
        //static readonly double xMax = Settings.Default.xMax;
        double yMin, yMax, xMin, xMax;
        double yMaxReduced, yMinReduced, xMinReduced, xMaxReduced;
        double xStepLength;
        static readonly int xNumberOfSteps = Settings.Default.xNumberOfSteps;
        static readonly int sleepTime = Settings.Default.sleepTime;
        TimeSpan moveTimeout = new TimeSpan(0,0,10);
        TimeSpan enableTimeout = new TimeSpan(0, 0, 10);
        //static readonly double yStartPosition = yMax;
        //static readonly double xStartPosition = xMin;
        //static readonly double xStepLength = (xMax - xMin)/xNumberOfSteps;


        TamTopology _topology;
        TamAxis _yAxis;
        TamAxis _xAxis;
        
        string _yUnit;
        string _xUnit;

        /// <summary>
        /// Prepares the TAM system.
        /// </summary>
        /// <exception cref="TamException">Startup failed.</exception>
        /// <exception cref="Triamec.Configuration.ConfigurationException">Failed to load the configuration.</exception>
        /// <remarks>
        /// 	<list type="bullet">
        /// 		<item><description>Creates a TAM topology,</description></item>
        /// 		<item><description>boots the Tria-Link,</description></item>
        /// 		<item><description>searches for a servo-drive,</description></item>
        /// 		<item><description>loads and applies a TAM configuration.</description></item>
        /// 	</list>
        /// </remarks>
        void Startup() {

            // Create the root object representing the topology of the TAM hardware.
            // We will dispose this object via components.
            _topology = new TamTopology();
            components.Add(_topology);

            TamSystem system;

            // Add the local TAM system on this PC to the topology.
            system = _topology.AddLocalSystem();

            // Boot the Tria-Link so that it learns about connected stations.
            system.Identify();

            // Don't load TAM configuration, assuming that the drive is already configured,
            // for example since parametrization is persisted in the drive.
            

            // Find the axis with the configured name in the Tria-Link.
            // The AsDepthFirstLeaves extension method performs a tree search an returns all instances of type TamAxis.
            // "Leaves" means that the search doesn't continue within TamAxis nodes.
            _yAxis = system.AsDepthFirstLeaves<TamAxis>().FirstOrDefault(a => a.Name == yAxisName);
            if (_yAxis == null) throw new TamException(Resources.NoAxisMessage);
            _xAxis = system.AsDepthFirstLeaves<TamAxis>().FirstOrDefault(a => a.Name == xAxisName);
            if (_xAxis == null) throw new TamException(Resources.NoAxisMessage);

            // Most drives get integrated into a real time control system. Accessing them via TAM API like we do here is considered
            // a secondary use case. Tell the axis that we're going to take control. Otherwise, the axis might reject our commands.
            // You should not do this, though, when this application is about to access the drive via the PCI interface.
            _yAxis.ControlSystemTreatment.Override(enabled: true);
            _xAxis.ControlSystemTreatment.Override(enabled: true);

            _yAxis.Drive.AddStateObserver(this);
            _xAxis.Drive.AddStateObserver(this);

            // Get the register layout of the axis
            // and cast it to the RLID-specific register layout.
            var yRegister = (Axis)_yAxis.Register;
            var xRegister = (Axis)_xAxis.Register;

            // Cache the position unit.
            _yUnit = yRegister.Parameters.PositionController.PositionUnit.Read().ToString();
            _xUnit = xRegister.Parameters.PositionController.PositionUnit.Read().ToString();

            // Cache Position Min and Max
            yMin = yRegister.Parameters.PathPlanner.PositionMinimum.Read();
            yMax = yRegister.Parameters.PathPlanner.PositionMaximum.Read(); 
            xMin = xRegister.Parameters.PathPlanner.PositionMinimum.Read();
            xMax = xRegister.Parameters.PathPlanner.PositionMaximum.Read();

            // Assign values based on Position Min and Max
            double xStroke = xMax - xMin;
            double yStroke = yMax - yMin;
            yMaxReduced = yMax - yStroke * 0.01;
            yMinReduced = yMin + yStroke * 0.01;
            xMinReduced = xMin + xStroke * 0.01;
            xMaxReduced = xMax - xStroke * 0.01;
            xStepLength = Math.Abs(xMaxReduced-xMinReduced) / xNumberOfSteps;

            // Start displaying the position in regular intervals.
            _timer.Start();
        }


        /// <exception cref="TamException">Disabling failed.</exception>
        void DisableDrive() {

            // Disable the axis controller.
            _yAxis.Control(AxisControlCommands.Disable);
            _xAxis.Control(AxisControlCommands.Disable);

            // Switch the power section off.
            _yAxis.Drive.SwitchOff();
            _xAxis.Drive.SwitchOff();
        }

        /// <summary>
        /// Moves in the specified direction.
        /// </summary>
        /// <param name="sign">A positive or negative value indicating the direction of the motion.</param>
        /// <exception cref="TamException">Moving failed.</exception>
        async Task MoveAxis() {
  
            // Set the drive operational, i.e. switch the power section on.
            _yAxis.Drive.SwitchOn();
            _xAxis.Drive.SwitchOn();

            // Reset any axis error and enable the axis controller.
            await _yAxis.Control(AxisControlCommands.ResetError).WaitForSuccessAsync(enableTimeout);
            await _xAxis.Control(AxisControlCommands.ResetError).WaitForSuccessAsync(enableTimeout);            

            // Enable axes if necessary
            if (_yAxis.ReadAxisState() == AxisState.Disabled) {
                await _yAxis.Control(AxisControlCommands.Enable).WaitForSuccessAsync(enableTimeout);
            }
            if (_xAxis.ReadAxisState() == AxisState.Disabled) {
                await _xAxis.Control(AxisControlCommands.Enable).WaitForSuccessAsync(enableTimeout);
            }

            // Start Demo move
            try {
                // Go to start position
                var yRequest = _yAxis.MoveAbsolute(yMaxReduced);
                var xRequest = _xAxis.MoveAbsolute(xMinReduced);
                await yRequest.WaitForSuccessAsync(moveTimeout);
                await xRequest.WaitForSuccessAsync(moveTimeout);
                await Task.Delay(sleepTime);
                // Loop move until stopped
                while (true) {
                    for (int i = 0; i < xNumberOfSteps; i++) {
                        _xAxis.MoveRelative(xStepLength).WaitForSuccess(moveTimeout);
                        await Task.Delay(sleepTime);
                        if (i % 2 == 0) {
                            _yAxis.MoveAbsolute(yMinReduced).WaitForSuccess(moveTimeout);
                            await Task.Delay(sleepTime);
                        } 
                        else {
                            _yAxis.MoveAbsolute(yMaxReduced).WaitForSuccess(moveTimeout);
                            await Task.Delay(sleepTime);
                        }
                    }
                    for (int i = 0; i < xNumberOfSteps; i++) {
                        _xAxis.MoveRelative(-xStepLength).WaitForSuccess(moveTimeout);
                        await Task.Delay(sleepTime);
                        if (i % 2 == 0) {
                            _yAxis.MoveAbsolute(yMaxReduced).WaitForSuccess(moveTimeout);
                            await Task.Delay(sleepTime);
                        } else {
                            _yAxis.MoveAbsolute(yMinReduced).WaitForSuccess(moveTimeout);
                            await Task.Delay(sleepTime);
                        }
                    }
                }
            } 
            catch(AxisCommandRejectedException) {
                //do nothing
            } 
            catch (CommandRejectedException) {
                //do nothing
            }


        }

        /// <summary>
        /// Measures the axis Y position and shows it in the GUI.
        /// </summary>
        void YreadPosition() {
            var register = (Axis)_yAxis.Register;
            var positionRegister = register.Signals.PositionController.MasterPosition;
            var position = positionRegister.Read();
            _yPositionBox.Text = $"{position:F2} {_yUnit}";
        }

        /// <summary>
        /// Measures the axis X position and shows it in the GUI.
        /// </summary>
        void XreadPosition() {
            var register = (Axis)_xAxis.Register;
            var positionRegister = register.Signals.PositionController.MasterPosition;
            var position = positionRegister.Read();
            _xPositionBox.Text = $"{position:F2} {_xUnit}";
        }

        async Task pollDigIn1() {
            var yRegister = (Axis)_yAxis.Register;
            bool inputStart = yRegister.Signals.General.DigitalInputBits.DigIn4.Read();
            bool inputStop = yRegister.Signals.General.DigitalInputBits.DigIn5.Read();
            if (inputStart && _StartButton.Enabled) {
                OnStart();
            }
            if (inputStop && _StopButton.Enabled) {
                OnStop();
            }
        }

        async Task OnStart() {
            try {
                _StopButton.Enabled = true;
                _StartButton.Enabled = false;
                await MoveAxis();
            } catch (TamException ex) {
                MessageBox.Show(ex.Message, Resources.MoveErrorCaption, MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0);
            }
        }

        async Task OnStop() {
            try {
                _StopButton.Enabled = false;
                _StartButton.Enabled = true;
                var yRequest = _yAxis.Stop();
                var xRequest = _xAxis.Stop();
                yRequest.WaitForSuccess(moveTimeout);
                xRequest.WaitForSuccess(moveTimeout);
                DisableDrive();
            } catch (TamException ex) {
                MessageBox.Show(ex.Message, Resources.MoveErrorCaption, MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0);
            }
        }

        #endregion Hello world code

        #region GUI handler methods
        #region Form handler methods

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            try {
                Startup();
                //_driveGroupBox.Enabled = true;
            } catch (TamException ex) {
                MessageBox.Show(this, ex.FullMessage(), Resources.StartupErrorCaption, MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e) {
            base.OnFormClosed(e);
            if (_yAxis != null) {
                try {
                    DisableDrive();
                    _yAxis.Drive.RemoveStateObserver(this);
                    _xAxis.Drive.RemoveStateObserver(this);
                } catch (TamException ex) {
                    MessageBox.Show(this, ex.Message, Resources.StartupErrorCaption, MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                }
            }
        }
        #endregion Form handler methods

        #region Button handler methods

        async void OnStartButtonClick(object sender, EventArgs e) {
            await OnStart();
        }

        async void OnStopButtonClick(object sender, EventArgs e) {
            await OnStop();
        }
        #endregion Button handler methods

        #region Menu handler methods

        void OnExitMenuItemClick(object sender, EventArgs e) => Close();
        #endregion Menu handler methods

        #region Timer methods
        void OnTimerTick(object sender, EventArgs e) {
            YreadPosition();
            XreadPosition();
            pollDigIn1();
        }

        #endregion Timer methods

        #endregion GUI handler methods

    }
}
