using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Controls
{
    public enum RobotFork
    {
        Fork1,
        Fork2
    }

    public enum RobotTransferAction
    {
        None,
        Get,
        Put
    }

    public sealed class RobotTransferActionCompletedEventArgs : EventArgs
    {
        private readonly RobotFork fork;
        private readonly RobotTransferAction action;
        private readonly double baseAngle;

        public RobotTransferActionCompletedEventArgs(RobotFork fork, RobotTransferAction action, double baseAngle)
        {
            this.fork = fork;
            this.action = action;
            this.baseAngle = baseAngle;
        }

        public RobotFork Fork
        {
            get { return fork; }
        }

        public RobotTransferAction Action
        {
            get { return action; }
        }

        public double BaseAngle
        {
            get { return baseAngle; }
        }
    }

    internal enum ForkActionState
    {
        Idle,
        RotateGet,
        RotatePut,
        GetExtend,
        PutExtend,
        GetHold,
        PutHold,
        GetRetract,
        PutRetract
    }

    internal sealed class ForkRuntime
    {
        private double progress;
        private double target;
        private bool waferVisible;
        private ForkActionState action;
        private double holdSeconds;
        private RobotTransferAction transferAction;

        public double Progress
        {
            get { return progress; }
        }

        public double Target
        {
            get { return target; }
        }

        public bool WaferVisible
        {
            get { return waferVisible; }
        }

        public ForkActionState Action
        {
            get { return action; }
        }

        public double HoldSeconds
        {
            get { return holdSeconds; }
        }

        public RobotTransferAction TransferAction
        {
            get { return transferAction; }
        }

        public void SetProgress(double value)
        {
            progress = Clamp01(value);
        }

        public void SetTarget(double value)
        {
            target = Clamp01(value);
        }

        public void SetWaferVisible(bool value)
        {
            waferVisible = value;
        }

        public void SetAction(ForkActionState value)
        {
            action = value;
        }

        public void SetTransferAction(RobotTransferAction value)
        {
            transferAction = value;
        }

        public void SetHoldSeconds(double value)
        {
            holdSeconds = Math.Max(0.0, value);
        }

        public void DecreaseHoldSeconds(double dt)
        {
            holdSeconds = Math.Max(0.0, holdSeconds - dt);
        }

        private static double Clamp01(double value)
        {
            return Math.Max(0.0, Math.Min(1.0, value));
        }
    }

    internal struct PointD
    {
        public double X;
        public double Y;

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointF ToPointF()
        {
            return new PointF((float)X, (float)Y);
        }
    }

    internal sealed class ForkPose
    {
        public PointD Base;
        public PointD Shoulder;
        public PointD Joint;
        public PointD ForkRoot;
        public PointD ForkTip;
    }

    [ToolboxItem(true)]
    [DefaultProperty("PushDistance")]
    [DefaultEvent("ActionCompleted")]
    [DesignerCategory("Code")]
    public sealed class RobotTransferControl : UserControl
    {
        private const int DefaultControlSize = 400;
        private const double BaseRadius = 72.0;
        private const double BaseMargin = 12.0;
        private const double ForkNeckWidth = 28.0;
        private const double ForkHeadWidth = 82.0;
        private const double ForkHeadHeight = 58.0;
        private const double ForkTineWidth = 14.0;
        private const double ForkNeckGap = 12.0;
        private const double ForkNeckStubHeight = 20.0;
        private const double ForkCornerRadius = 6.0;
        private const double WaferRadius = 22.0;
        private const double WaferCenterOffsetY = 29.0;
        private const double Arm1Width = 38.0;
        private const double Arm2Width = 34.0;
        private const int TimerIntervalMilliseconds = 16;
        private const double MinDeltaSeconds = 0.001;
        private const double MaxDeltaSeconds = 0.04;
        private const double DefaultForkMoveSpeed = 0.82;
        private const double DefaultBaseRotateSpeed = 120.0;
        private const double DefaultPushDistance = 138.0;
        private const double BaseAngleTolerance = 0.1;
        private const double ProgressMoveTolerance = 0.001;
        private const double ProgressSettleTolerance = 0.006;
        private const double RetractedProgressTolerance = 0.01;
        private const double TransferHoldSeconds = 0.18;
        private const double TwoLinkDistanceTolerance = 0.001;
        private const double FullCircleDegrees = 360.0;
        private const double HalfCircleDegrees = 180.0;
        private const double AngleNormalizeOffset = 540.0;
        private const double SmoothStepFactorA = 3.0;
        private const double SmoothStepFactorB = 2.0;
        private const double ForkHeadCurveHeight = 18.0;
        private const double ForkHeadCurveOffset = 8.0;

        private static readonly Color BaseColor = Color.FromArgb(192, 198, 206);
        private static readonly Color Fork1Arm1Color = Color.FromArgb(170, 178, 190);
        private static readonly Color Fork1Arm2Color = Color.FromArgb(210, 215, 222);
        private static readonly Color Fork1Color = Color.FromArgb(230, 226, 219);
        private static readonly Color Fork2Arm1Color = Color.FromArgb(162, 172, 184);
        private static readonly Color Fork2Arm2Color = Color.FromArgb(199, 206, 215);
        private static readonly Color Fork2Color = Color.FromArgb(215, 221, 228);
        private static readonly Color WaferColor = Color.FromArgb(225, 51, 196, 229);

        private readonly Timer timer;
        private readonly Stopwatch stopwatch;
        private readonly ForkRuntime fork1;
        private readonly ForkRuntime fork2;

        private double arm1Length = 90.0;
        private double arm2Length = 118.0;
        private double forkLength = 150.0;
        private double shoulderOffsetX = -28.0;
        private double shoulderOffsetY = -8.0;
        private double pushDistance = DefaultPushDistance;
        private double forkMoveSpeed = DefaultForkMoveSpeed;
        private double baseRotateSpeed = DefaultBaseRotateSpeed;

        private double baseAngle;
        private double targetBaseAngle;

        public event EventHandler<RobotTransferActionCompletedEventArgs> ActionCompleted;

        public RobotTransferControl()
        {
            fork1 = new ForkRuntime();
            fork2 = new ForkRuntime();
            stopwatch = new Stopwatch();

            ConfigurePainting();
            BackColor = Color.Transparent;
            Size = new Size(DefaultControlSize, DefaultControlSize);
            MinimumSize = new Size(80, 80);

            timer = new Timer();
            timer.Interval = TimerIntervalMilliseconds;
            timer.Tick += Timer_Tick;
        }

        [Category("Robot Transfer")]
        [Description("机械手底座当前角度，单位为度。")]
        [Browsable(false)]
        public double BaseAngle
        {
            get { return baseAngle; }
        }

        [Category("Robot Transfer")]
        [Description("机械手底座目标角度，单位为度。")]
        [Browsable(false)]
        public double TargetBaseAngle
        {
            get { return targetBaseAngle; }
        }

        [Category("Robot Transfer")]
        [Description("指示机械手是否正在旋转或移动叉臂。")]
        [Browsable(false)]
        public bool IsBusy
        {
            get { return !IsReady; }
        }

        [Category("Robot Transfer")]
        [Description("指示机械手是否已空闲、定位完成，并可开始新的取放动作。")]
        [Browsable(false)]
        public bool IsReady
        {
            get { return CanStart(); }
        }

        [Category("Robot Transfer Layout")]
        [Description("叉臂伸出距离，使用控件内部模型单位。")]
        [DefaultValue(DefaultPushDistance)]
        public double PushDistance
        {
            get { return pushDistance; }
            set { pushDistance = Math.Max(0.0, value); Invalidate(); }
        }

        [Category("Robot Transfer Animation")]
        [Description("叉臂伸出和缩回速度，单位为每秒进度值。")]
        [DefaultValue(DefaultForkMoveSpeed)]
        public double ForkMoveSpeed
        {
            get { return forkMoveSpeed; }
            set { forkMoveSpeed = Math.Max(0.01, value); }
        }

        [Category("Robot Transfer Animation")]
        [Description("底座旋转速度，单位为度/秒。")]
        [DefaultValue(DefaultBaseRotateSpeed)]
        public double BaseRotateSpeed
        {
            get { return baseRotateSpeed; }
            set { baseRotateSpeed = Math.Max(1.0, value); }
        }

        [Category("Robot Transfer Layout")]
        [Description("当前自动计算得到的绘制缩放比例。")]
        [Browsable(false)]
        public double RobotScale
        {
            get { return GetScale(); }
        }

        public bool Start(RobotFork fork, double angle, RobotTransferAction action)
        {
            if (!CanStart())
            {
                return false;
            }

            targetBaseAngle = angle;
            EnsureAnimationRunning();
            if (action == RobotTransferAction.None)
            {
                return true;
            }

            ForkRuntime runtime = GetFork(fork);
            runtime.SetWaferVisible(action == RobotTransferAction.Put);
            runtime.SetHoldSeconds(0.0);
            runtime.SetTransferAction(action);
            runtime.SetAction(action == RobotTransferAction.Get ? ForkActionState.RotateGet : ForkActionState.RotatePut);
            return true;
        }

        public bool Extend(RobotFork fork)
        {
            ForkRuntime runtime = GetFork(fork);
            if (runtime.Action != ForkActionState.Idle || !BaseRotationSettled())
            {
                return false;
            }

            runtime.SetTarget(1.0);
            EnsureAnimationRunning();
            return true;
        }

        public bool Retract(RobotFork fork)
        {
            ForkRuntime runtime = GetFork(fork);
            if (runtime.Action != ForkActionState.Idle)
            {
                return false;
            }

            runtime.SetTarget(0.0);
            EnsureAnimationRunning();
            return true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            double scale = GetScale();
            ForkPose pose1 = GetPose(fork1.Progress, RobotFork.Fork1, scale);
            ForkPose pose2 = GetPose(fork2.Progress, RobotFork.Fork2, scale);

            DrawBase(g, pose1.Base, scale);

            GraphicsState state = g.Save();
            try
            {
                ApplyBaseRotation(g, pose1.Base);

                DrawArm(g, pose2.Shoulder, pose2.Joint, Arm1Width, Fork2Arm1Color, scale);
                DrawArm(g, pose1.Shoulder, pose1.Joint, Arm1Width, Fork1Arm1Color, scale);

                DrawForkUpperAssembly(g, pose2, fork2.WaferVisible, Fork2Arm2Color, Fork2Color, scale);
                DrawForkUpperAssembly(g, pose1, fork1.WaferVisible, Fork1Arm2Color, Fork1Color, scale);
            }
            finally
            {
                g.Restore(state);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackColor == Color.Transparent && Parent != null)
            {
                GraphicsState state = e.Graphics.Save();
                try
                {
                    e.Graphics.TranslateTransform(-Left, -Top);
                    Rectangle parentClip = new Rectangle(Left, Top, Width, Height);
                    PaintEventArgs parentArgs = new PaintEventArgs(e.Graphics, parentClip);
                    InvokePaintBackground(Parent, parentArgs);
                }
                finally
                {
                    e.Graphics.Restore(state);
                }
                return;
            }

            base.OnPaintBackground(e);
        }

        private bool CanStart()
        {
            return BaseRotationSettled()
                && AllForksRetracted()
                && fork1.Action == ForkActionState.Idle
                && fork2.Action == ForkActionState.Idle;
        }

        private void ConfigurePainting()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        private bool AllForksRetracted()
        {
            return fork1.Target == 0.0
                && fork2.Target == 0.0
                && fork1.Progress < RetractedProgressTolerance
                && fork2.Progress < RetractedProgressTolerance;
        }

        private bool BaseRotationSettled()
        {
            return Math.Abs(ShortestAngleDiff(targetBaseAngle, baseAngle)) < BaseAngleTolerance;
        }

        private double GetScale()
        {
            if (Width <= 0 || Height <= 0)
            {
                return 1.0;
            }

            double halfWidth = Math.Max(1.0, Width * 0.5 - BaseMargin);
            double halfHeight = Math.Max(1.0, Height * 0.5 - BaseMargin);

            double topReach = Math.Abs(shoulderOffsetY) + pushDistance + forkLength;
            double bottomReach = Math.Max(BaseRadius, ForkHeadHeight);
            double sideReach = Math.Max(BaseRadius, Math.Abs(shoulderOffsetX) + arm1Length + Arm1Width);

            double scaleX = halfWidth / sideReach;
            double scaleTop = halfHeight / topReach;
            double scaleBottom = halfHeight / bottomReach;
            double scale = Math.Min(scaleX, Math.Min(scaleTop, scaleBottom));
            return Math.Max(0.1, scale);
        }

        private ForkRuntime GetFork(RobotFork fork)
        {
            return fork == RobotFork.Fork2 ? fork2 : fork1;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double dt = Math.Min(MaxDeltaSeconds, Math.Max(MinDeltaSeconds, stopwatch.Elapsed.TotalSeconds));
            stopwatch.Restart();

            UpdateFork(RobotFork.Fork1, fork1, dt);
            UpdateFork(RobotFork.Fork2, fork2, dt);
            UpdateBase(dt);
            Invalidate();
            StopAnimationIfIdle();
        }

        private void EnsureAnimationRunning()
        {
            stopwatch.Restart();
            if (!timer.Enabled)
            {
                timer.Start();
            }
        }

        private void StopAnimationIfIdle()
        {
            if (IsReady)
            {
                timer.Stop();
                stopwatch.Reset();
            }
        }

        private void UpdateBase(double dt)
        {
            double diff = ShortestAngleDiff(targetBaseAngle, baseAngle);
            if (Math.Abs(diff) > BaseAngleTolerance)
            {
                baseAngle += Math.Sign(diff) * Math.Min(Math.Abs(diff), baseRotateSpeed * dt);
            }
        }

        private void UpdateFork(RobotFork forkId, ForkRuntime fork, double dt)
        {
            UpdateForkProgress(fork, dt);
            UpdateForkState(forkId, fork, dt);
        }

        private void UpdateForkProgress(ForkRuntime fork, double dt)
        {
            double diff = fork.Target - fork.Progress;
            if (Math.Abs(diff) > ProgressMoveTolerance)
            {
                fork.SetProgress(fork.Progress + Math.Sign(diff) * Math.Min(Math.Abs(diff), forkMoveSpeed * dt));
            }
        }

        private void UpdateForkState(RobotFork forkId, ForkRuntime fork, double dt)
        {
            bool settledAtOne = ProgressSettled(fork.Progress, 1.0);
            bool settledAtZero = ProgressSettled(fork.Progress, 0.0);

            if (fork.Action == ForkActionState.RotateGet && BaseRotationSettled())
            {
                BeginExtend(fork, ForkActionState.GetExtend);
            }
            else if (fork.Action == ForkActionState.RotatePut && BaseRotationSettled())
            {
                BeginExtend(fork, ForkActionState.PutExtend);
            }
            else if (fork.Action == ForkActionState.GetExtend && settledAtOne)
            {
                BeginHold(fork, true, ForkActionState.GetHold);
            }
            else if (fork.Action == ForkActionState.PutExtend && settledAtOne)
            {
                BeginHold(fork, false, ForkActionState.PutHold);
            }
            else if ((fork.Action == ForkActionState.GetHold || fork.Action == ForkActionState.PutHold) && fork.HoldSeconds <= 0.0)
            {
                BeginRetract(fork);
            }
            else if ((fork.Action == ForkActionState.GetHold || fork.Action == ForkActionState.PutHold) && fork.HoldSeconds > 0.0)
            {
                fork.DecreaseHoldSeconds(dt);
            }
            else if ((fork.Action == ForkActionState.GetRetract || fork.Action == ForkActionState.PutRetract) && settledAtZero)
            {
                CompleteForkAction(forkId, fork);
            }
        }

        private static void BeginExtend(ForkRuntime fork, ForkActionState nextAction)
        {
            fork.SetTarget(1.0);
            fork.SetAction(nextAction);
        }

        private static void BeginHold(ForkRuntime fork, bool waferVisible, ForkActionState nextAction)
        {
            fork.SetWaferVisible(waferVisible);
            fork.SetAction(nextAction);
            fork.SetHoldSeconds(TransferHoldSeconds);
        }

        private static void BeginRetract(ForkRuntime fork)
        {
            fork.SetTarget(0.0);
            fork.SetAction(fork.Action == ForkActionState.GetHold ? ForkActionState.GetRetract : ForkActionState.PutRetract);
        }

        private void CompleteForkAction(RobotFork forkId, ForkRuntime fork)
        {
            RobotTransferAction completedAction = fork.TransferAction;
            fork.SetAction(ForkActionState.Idle);
            fork.SetTransferAction(RobotTransferAction.None);
            OnActionCompleted(new RobotTransferActionCompletedEventArgs(forkId, completedAction, baseAngle));
        }

        private void OnActionCompleted(RobotTransferActionCompletedEventArgs e)
        {
            EventHandler<RobotTransferActionCompletedEventArgs> handler = ActionCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static bool ProgressSettled(double progress, double target)
        {
            return Math.Abs(progress - target) < ProgressSettleTolerance;
        }

        private ForkPose GetPose(double progress, RobotFork fork, double scale)
        {
            PointD basePoint = new PointD(Width * 0.5, Height * 0.5);
            bool isFork2 = fork == RobotFork.Fork2;
            PointD shoulder = new PointD(
                basePoint.X + (isFork2 ? -shoulderOffsetX : shoulderOffsetX) * scale,
                basePoint.Y + shoulderOffsetY * scale);

            double t = Ease(progress);
            PointD retractedForkRoot = new PointD(basePoint.X, shoulder.Y);
            PointD extendedForkRoot = new PointD(basePoint.X, retractedForkRoot.Y - pushDistance * scale);
            PointD forkRoot = new PointD(
                Lerp(retractedForkRoot.X, extendedForkRoot.X, t),
                Lerp(retractedForkRoot.Y, extendedForkRoot.Y, t));
            PointD joint = SolveTwoLink(shoulder, forkRoot, isFork2 ? 1.0 : -1.0, scale);

            ForkPose pose = new ForkPose();
            pose.Base = basePoint;
            pose.Shoulder = shoulder;
            pose.Joint = joint;
            pose.ForkRoot = forkRoot;
            pose.ForkTip = new PointD(forkRoot.X, forkRoot.Y - forkLength * scale);
            return pose;
        }

        private PointD SolveTwoLink(PointD start, PointD target, double elbowSide, double scale)
        {
            double dx = target.X - start.X;
            double dy = target.Y - start.Y;
            double l1 = arm1Length * scale;
            double l2 = arm2Length * scale;
            double distance = Clamp(Math.Sqrt(dx * dx + dy * dy), Math.Abs(l1 - l2), l1 + l2 - 0.01);
            if (distance < TwoLinkDistanceTolerance)
            {
                return start;
            }

            double a = (l1 * l1 - l2 * l2 + distance * distance) / (2.0 * distance);
            double h = Math.Sqrt(Math.Max(0.0, l1 * l1 - a * a));
            double ux = dx / distance;
            double uy = dy / distance;
            PointD mid = new PointD(start.X + a * ux, start.Y + a * uy);
            return new PointD(mid.X - elbowSide * h * uy, mid.Y + elbowSide * h * ux);
        }

        private void DrawBase(Graphics g, PointD center, double scale)
        {
            float radius = (float)(BaseRadius * scale);
            using (Brush brush = new SolidBrush(BaseColor))
            {
                g.FillEllipse(brush, (float)center.X - radius, (float)center.Y - radius, radius * 2, radius * 2);
            }
        }

        private void ApplyBaseRotation(Graphics g, PointD basePoint)
        {
            g.TranslateTransform((float)basePoint.X, (float)basePoint.Y);
            g.RotateTransform((float)baseAngle);
            g.TranslateTransform((float)-basePoint.X, (float)-basePoint.Y);
        }

        private void DrawForkUpperAssembly(Graphics g, ForkPose pose, bool waferVisible, Color arm2Color, Color forkColor, double scale)
        {
            DrawArm(g, pose.Joint, pose.ForkRoot, Arm2Width, arm2Color, scale);
            DrawFork(g, pose.ForkRoot, pose.ForkTip, forkColor, scale);
            if (waferVisible)
            {
                DrawWafer(g, pose, scale);
            }
        }

        private void DrawArm(Graphics g, PointD from, PointD to, double width, Color color, double scale)
        {
            using (Pen pen = new Pen(color, Math.Max(1.0f, (float)(width * scale))))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                g.DrawLine(pen, from.ToPointF(), to.ToPointF());
            }
        }

        private void DrawFork(Graphics g, PointD root, PointD tip, Color color, double scale)
        {
            float neckW = (float)(ForkNeckWidth * scale);
            float headW = (float)(ForkHeadWidth * scale);
            float headH = (float)(ForkHeadHeight * scale);
            float tineW = (float)(ForkTineWidth * scale);
            float halfCenter = headW / 2 - tineW / 2;

            using (Brush brush = new SolidBrush(color))
            using (Pen pen = new Pen(color, Math.Max(1.0f, tineW)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                FillRoundRect(g, brush, (float)root.X - neckW / 2, (float)tip.Y + headH + (float)(ForkNeckGap * scale), neckW, (float)(root.Y - tip.Y - headH - ForkNeckGap * scale), (float)(ForkCornerRadius * scale));

                using (GraphicsPath path = CreateForkHeadPath(tip, halfCenter, headH, scale))
                {
                    g.DrawPath(pen, path);
                }

                FillRoundRect(g, brush, (float)tip.X - neckW / 2, (float)tip.Y + headH, neckW, (float)(ForkNeckStubHeight * scale), (float)(ForkCornerRadius * scale));
            }
        }

        private void DrawWafer(Graphics g, ForkPose pose, double scale)
        {
            float radius = (float)(WaferRadius * scale);
            using (Brush brush = new SolidBrush(WaferColor))
            {
                g.FillEllipse(brush, (float)pose.ForkTip.X - radius, (float)pose.ForkTip.Y + (float)(WaferCenterOffsetY * scale) - radius, radius * 2, radius * 2);
            }
        }

        private static GraphicsPath CreateForkHeadPath(PointD tip, float halfCenter, float headHeight, double scale)
        {
            float curveHeight = (float)(ForkHeadCurveHeight * scale);
            float curveOffset = (float)(ForkHeadCurveOffset * scale);
            float leftX = (float)tip.X - halfCenter;
            float rightX = (float)tip.X + halfCenter;
            float topY = (float)tip.Y;
            float bottomY = (float)tip.Y + headHeight;
            float curveStartY = bottomY - curveHeight;

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddLine(leftX, topY, leftX, curveStartY);
            path.AddBezier(leftX, curveStartY, leftX, bottomY, (float)tip.X - curveOffset, bottomY, (float)tip.X, bottomY);
            path.AddBezier((float)tip.X, bottomY, (float)tip.X + curveOffset, bottomY, rightX, bottomY, rightX, curveStartY);
            path.AddLine(rightX, curveStartY, rightX, topY);
            return path;
        }

        private static void FillRoundRect(Graphics g, Brush brush, float x, float y, float width, float height, float radius)
        {
            if (height <= 0 || width <= 0)
            {
                return;
            }

            using (GraphicsPath path = new GraphicsPath())
            {
                radius = Math.Min(radius, Math.Min(width, height) / 2.0f);
                float d = radius * 2;
                path.AddArc(x, y, d, d, 180, 90);
                path.AddArc(x + width - d, y, d, d, 270, 90);
                path.AddArc(x + width - d, y + height - d, d, d, 0, 90);
                path.AddArc(x, y + height - d, d, d, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }

        private static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        private static double Ease(double t)
        {
            return t * t * (SmoothStepFactorA - SmoothStepFactorB * t);
        }

        private static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private static double ShortestAngleDiff(double target, double current)
        {
            return ((target - current + AngleNormalizeOffset) % FullCircleDegrees) - HalfCircleDegrees;
        }
    }
}
