//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        private float oldHeadY;
        private bool position0Done;
        private bool position1Done;
        private bool position2Done;
        private bool position3Done;
        private bool position4Done;


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            oldHeadY = 0;
            position0Done = true;
            position1Done = false;
            position2Done = false;
            position3Done = false;
            position4Done = false;
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];
           
            
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            //Console.WriteLine(skeleton.Joints);
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);
 
            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
               // Console.WriteLine("Joint Type: " + joint.JointType + " X " + joint.Position.X + " Y " + joint.Position.Y + " Z " + joint.Position.Z);
                

                Brush drawBrush = null;
                
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;                    
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;                    
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }

            this.CheckForm(skeleton);
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);           
           
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }
            
            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// <summary>
        /// Handles the checking or unchecking of the seated mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != this.sensor)
            {
                if (this.checkBoxSeatedMode.IsChecked.GetValueOrDefault())
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                }
            }
        }

        private void CheckForm(Skeleton skeleton)
        {
            //Check if in front of the camera
            // this.CheckPosition(skeleton);

            Console.WriteLine("oldHeadY " + oldHeadY + " newHeadY " + skeleton.Joints[JointType.Head].Position.Y);

            Console.WriteLine("HipCenter " + skeleton.Joints[JointType.HipCenter].Position.Y + " HipLeft " + skeleton.Joints[JointType.HipLeft].Position.Y + " KneeLeft " + skeleton.Joints[JointType.KneeLeft].Position.Y);

            if (position0Done)
            {
                //Detect end position1
                if (position1Done)
                {
                    Console.WriteLine("Posición 1 Terminada");

                    //Check for position2Done              

                    if (position2Done)
                    {
                        Console.WriteLine("Posicion 2 terminada");

                        if (position3Done)
                        {
                            Console.WriteLine("Position 3 done");

                            if (position4Done)
                            {
                                Console.WriteLine("Position 4 done");
                            }
                            else
                            {
                                position4Done = Math.Abs(oldHeadY - skeleton.Joints[JointType.Head].Position.Y) < 0.05;
                            }

                        }
                        else
                        {
                            bool cond1 = Math.Abs(skeleton.Joints[JointType.HipLeft].Position.Y - skeleton.Joints[JointType.KneeLeft].Position.Y) > 0.05;
                            bool cond2 = Math.Abs(skeleton.Joints[JointType.HipRight].Position.Y - skeleton.Joints[JointType.KneeRight].Position.Y) > 0.05;
                            bool cond3 = skeleton.Joints[JointType.HipLeft].Position.Y > skeleton.Joints[JointType.KneeLeft].Position.Y;

                            position3Done = cond1 && cond2 && cond3;
                        }

                    }
                    else
                    {
                        bool cond1 = Math.Abs(skeleton.Joints[JointType.HipLeft].Position.Y - skeleton.Joints[JointType.KneeLeft].Position.Y) < 0.05;
                        bool cond2 = Math.Abs(skeleton.Joints[JointType.HipRight].Position.Y - skeleton.Joints[JointType.KneeRight].Position.Y) < 0.05;

                        position2Done = cond1 && cond2;

                        //Check form for position2
                        this.Position2Cond1(skeleton);

                    }

                }
                else
                {
                    if (oldHeadY != 0)
                    {
                        if (Math.Abs(oldHeadY - skeleton.Joints[JointType.Head].Position.Y) > 0.1)
                        {
                            position1Done = true;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        oldHeadY = skeleton.Joints[JointType.Head].Position.Y;
                    }

                    //Check form of position 1

                    //this.Position1Cond1(skeleton);

                }
            }
            else
            {
                

            }

            





        }

        private void CheckPosition(Skeleton skeleton)
        {
            if (Math.Abs(skeleton.Joints[JointType.HipLeft].Position.Z - skeleton.Joints[JointType.HipRight].Position.Z) < 0.03)
            {
                Console.WriteLine("You're doing great!Hip Left: " + skeleton.Joints[JointType.HipLeft].Position.Z + " Hip Right: " + skeleton.Joints[JointType.HipRight].Position.Z);
                statusBarText.Text = "Genial!";
            }
            else
            {
                Console.WriteLine("Hip Left: " + skeleton.Joints[JointType.HipLeft].Position.Z + " Hip Right: " + skeleton.Joints[JointType.HipRight].Position.Z);
                statusBarText.Text = "Hip Left: " + skeleton.Joints[JointType.HipLeft].Position.Z + " Hip Right: " + skeleton.Joints[JointType.HipRight].Position.Z;
            }
        }

        //Todo recto
        private void Position1Cond1(Skeleton skeleton)
        {
            var cond1 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.ShoulderLeft].Position.Z) < 0.1;
            var cond2 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.KneeLeft].Position.Z) < 0.1;
            var cond3 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.AnkleLeft].Position.Z) < 0.1;
            var cond4 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.ShoulderRight].Position.Z) < 0.1;
            var cond5 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.KneeRight].Position.Z) < 0.1;
            var cond6 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.AnkleRight].Position.Z) < 0.1;

            if (cond1 && cond2 && cond3 && cond4 && cond5 && cond6)
            {
                Console.WriteLine("Todo perfecto!");
            }
            else
            {
                if (!cond1)
                {
                    Console.WriteLine("ShoulderLeft");
                }
                if (!cond2)
                {
                    Console.WriteLine("KneeLeft");
                }
                if (!cond3)
                {
                    Console.WriteLine("AnkleLeft");
                }
                if (!cond4)
                {
                    Console.WriteLine("ShoulderRight");
                }
                if (!cond5)
                {
                    Console.WriteLine("KneeRight");
                }
                if (!cond6)
                {
                    Console.WriteLine("AnkleRight");
                }
            }
        }

        private void Position2Cond1(Skeleton skeleton)
        {
            var cond1 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.ShoulderCenter].Position.Z) > 0.1;
            var cond2 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.AnkleLeft].Position.Z) > 0.1;

            Console.WriteLine("HipCenter " + skeleton.Joints[JointType.HipCenter].Position.Z + " ShoulderCenter " + skeleton.Joints[JointType.ShoulderCenter].Position.Z);
        }
    }

   
}