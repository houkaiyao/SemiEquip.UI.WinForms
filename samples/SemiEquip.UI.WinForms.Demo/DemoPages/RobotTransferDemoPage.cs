using System;
using System.Drawing;
using System.Windows.Forms;
using SemiEquip.UI.WinForms.Controls;

namespace SemiEquip.UI.WinForms.Demo.DemoPages
{
    internal sealed class RobotTransferDemoPage : UserControl
    {
        private readonly RobotTransferControl _robotControl;
        private readonly NumericUpDown _angleInput;
        private readonly ComboBox _forkComboBox;
        private readonly ComboBox _actionComboBox;
        private readonly Label _statusLabel;
        private readonly Timer _statusTimer;
        private string _lastCompletedAction = "None";

        public RobotTransferDemoPage()
        {
            Font = new Font("Times New Roman", 9f, FontStyle.Regular, GraphicsUnit.Point);
            BackColor = Color.FromArgb(230, 235, 242);
            ForeColor = Color.FromArgb(32, 38, 46);
            Padding = new Padding(8);

            Panel previewPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 620,
                BackColor = Color.FromArgb(230, 235, 242)
            };

            _robotControl = new RobotTransferControl
            {
                Location = new Point(48, 48),
                Size = new Size(400, 400),
                BackColor = Color.Transparent
            };
            _robotControl.ActionCompleted += OnRobotActionCompleted;
            previewPanel.Controls.Add(_robotControl);

            Panel optionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Color.FromArgb(230, 235, 242)
            };

            Label titleLabel = CreateTitleLabel("Robot Transfer Control");
            titleLabel.Dock = DockStyle.Top;
            optionPanel.Controls.Add(titleLabel);

            GroupBox commandGroup = CreateGroupBox("Command", 0, 56, 420, 190);
            _angleInput = new NumericUpDown
            {
                Location = new Point(120, 30),
                Size = new Size(90, 24),
                Minimum = -360,
                Maximum = 360,
                Increment = 15,
                Value = 90
            };

            _forkComboBox = CreateComboBox(120, 66, 130);
            _forkComboBox.Items.Add(RobotFork.Fork1);
            _forkComboBox.Items.Add(RobotFork.Fork2);
            _forkComboBox.SelectedIndex = 0;

            _actionComboBox = CreateComboBox(120, 102, 130);
            _actionComboBox.Items.Add(RobotTransferAction.None);
            _actionComboBox.Items.Add(RobotTransferAction.Get);
            _actionComboBox.Items.Add(RobotTransferAction.Put);
            _actionComboBox.SelectedIndex = 1;

            Button startButton = CreateButton("Start", 270, 28);
            startButton.Click += delegate { StartSelectedAction(); };
            Button extendButton = CreateButton("Extend", 270, 64);
            extendButton.Click += delegate { _robotControl.Extend(GetSelectedFork()); UpdateStatus(); };
            Button retractButton = CreateButton("Retract", 270, 100);
            retractButton.Click += delegate { _robotControl.Retract(GetSelectedFork()); UpdateStatus(); };
            Button simpleGetButton = CreateButton("Simple GET", 270, 136);
            simpleGetButton.Click += delegate
            {
                _robotControl.Start(RobotFork.Fork1, 90.0, RobotTransferAction.Get);
                UpdateStatus();
            };

            commandGroup.Controls.Add(CreateFieldLabel("Angle", 16, 33));
            commandGroup.Controls.Add(CreateFieldLabel("Fork", 16, 69));
            commandGroup.Controls.Add(CreateFieldLabel("Action", 16, 105));
            commandGroup.Controls.Add(_angleInput);
            commandGroup.Controls.Add(_forkComboBox);
            commandGroup.Controls.Add(_actionComboBox);
            commandGroup.Controls.Add(startButton);
            commandGroup.Controls.Add(extendButton);
            commandGroup.Controls.Add(retractButton);
            commandGroup.Controls.Add(simpleGetButton);

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 270),
                Size = new Size(420, 150),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                TextAlign = ContentAlignment.TopLeft
            };

            optionPanel.Controls.Add(_statusLabel);
            optionPanel.Controls.Add(commandGroup);
            Controls.Add(optionPanel);
            Controls.Add(previewPanel);

            _statusTimer = new Timer();
            _statusTimer.Interval = 100;
            _statusTimer.Tick += delegate { UpdateStatus(); };
            _statusTimer.Start();
            UpdateStatus();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _statusTimer.Stop();
                _statusTimer.Dispose();
                _robotControl.ActionCompleted -= OnRobotActionCompleted;
            }

            base.Dispose(disposing);
        }

        private void StartSelectedAction()
        {
            _robotControl.Start(
                GetSelectedFork(),
                (double)_angleInput.Value,
                GetSelectedAction());
            UpdateStatus();
        }

        private RobotFork GetSelectedFork()
        {
            return _forkComboBox.SelectedItem is RobotFork
                ? (RobotFork)_forkComboBox.SelectedItem
                : RobotFork.Fork1;
        }

        private RobotTransferAction GetSelectedAction()
        {
            return _actionComboBox.SelectedItem is RobotTransferAction
                ? (RobotTransferAction)_actionComboBox.SelectedItem
                : RobotTransferAction.None;
        }

        private void OnRobotActionCompleted(object sender, RobotTransferActionCompletedEventArgs e)
        {
            _lastCompletedAction = string.Format("{0} {1} at {2:0.0} deg", e.Fork, e.Action, e.BaseAngle);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            _statusLabel.Text = string.Format(
                "IsReady: {0}\r\nIsBusy: {1}\r\nBaseAngle: {2:0.0}\r\nTargetBaseAngle: {3:0.0}\r\nPushDistance: {4:0.0}\r\nForkMoveSpeed: {5:0.00}\r\nBaseRotateSpeed: {6:0.0}\r\nLast Completed: {7}",
                _robotControl.IsReady,
                _robotControl.IsBusy,
                _robotControl.BaseAngle,
                _robotControl.TargetBaseAngle,
                _robotControl.PushDistance,
                _robotControl.ForkMoveSpeed,
                _robotControl.BaseRotateSpeed,
                _lastCompletedAction);
        }

        private static Label CreateTitleLabel(string text)
        {
            return new Label
            {
                AutoSize = false,
                Size = new Size(420, 32),
                Text = text,
                Font = new Font("Times New Roman", 12f, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static Label CreateFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                AutoSize = false,
                Location = new Point(x, y),
                Size = new Size(90, 22),
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static GroupBox CreateGroupBox(string text, int x, int y, int width, int height)
        {
            return new GroupBox
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height)
            };
        }

        private static ComboBox CreateComboBox(int x, int y, int width)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(x, y),
                Size = new Size(width, 24)
            };
        }

        private static Button CreateButton(string text, int x, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(110, 28)
            };
        }
    }
}
