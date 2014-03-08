//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System.IO;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Microsoft.Kinect;
    using System.Diagnostics;


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

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        int recordFlag,frameCount,trainmode,timerstarted,restarttesting;
        String xString, yString, zString,trainname,trainfilelist,xStringl, yStringl, zStringl;
        Stopwatch sw;
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
            recordFlag = 0;
            frameCount = 0;
            timerstarted = 0;
            trainmode = 0;
            restarttesting = 1;
            sw = new Stopwatch();
            button1.Visibility = Visibility.Hidden;
            
            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;
            
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
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
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                this.sensor.ColorFrameReady += this.ShowRGBImage;
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
               // this.statusBarText.Text = Properties.Resources.NoKinectReady;
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
        private void ShowRGBImage(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame rgbimg = e.OpenColorImageFrame())
            {
                if (rgbimg != null)
                {
                    Byte[] pixels = new Byte[rgbimg.PixelDataLength];
                    rgbimg.CopyPixelDataTo(pixels);

                    Int32 stride = rgbimg.Width * 4;

                    image1.Source = BitmapSource.Create(rgbimg.Width, rgbimg.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);

                }
            }
        }
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);

                    


                        if (skeletons != null)
                        {
                            Skeleton firstTrackedSkeleton = skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
                            if (firstTrackedSkeleton != null)
                            {
                                //MessageBox.Show("Recording Gesture Now", "Result");
                               




                                Joint j = firstTrackedSkeleton.Joints[JointType.HandRight];
                                Joint jleft = firstTrackedSkeleton.Joints[JointType.HandLeft];

                                if (j.TrackingState == JointTrackingState.Tracked && jleft.TrackingState == JointTrackingState.Tracked)
                                {

                                    if (trainmode == 0)
                                    {
                                        button1.Visibility = Visibility.Hidden;
                                        if (restarttesting == 0)
                                            return;

                                        int starttracking = 0;
                                        if (timerstarted == 0)
                                        {
                                            sw.Start(); // starts the stopwatch
                                            label3.Content = "";

                                            label3.Visibility = Visibility.Visible;
                                            timerstarted = 1;
                                        }
                                        else
                                        {
                                            long elapsed = sw.ElapsedMilliseconds;
                                            if (elapsed < 5000)
                                            {
                                                long remainingtime = 5000 - elapsed;
                                                label3.Content = "Recording in\n" + remainingtime / 1000 + " seconds";
                                            }
                                            else
                                            {
                                                //timerstarted = 0;
                                                starttracking = 1;
                                                label3.Visibility = Visibility.Hidden;
                                                sw.Stop();
                                                //sw.Reset();
                                            }


                                        }
                                        if (starttracking == 0)
                                        {
                                            return;
                                        }
                                        recordFlag = 1;
                                    }
                                    else
                                    {
                                        button1.Visibility = Visibility.Visible;
                                    }

                                   // Console.WriteLine("Right hand: " + j.Position.X + ", " + j.Position.Y + ", " + j.Position.Z);
                                    
                                    String lines = j.Position.X + ", " + j.Position.Y + ", " + j.Position.Z;
                                    String filename,filename2;
                                    Boolean fileappend = false ;
                                    if (trainmode == 1)
                                    {
                                        filename = "./train\\right\\" + trainname;
                                        filename2 = "./train\\left\\" + trainname;
                                        fileappend = true;
                                    }
                                    else
                                    {
                                        filename = "test";
                                        filename2 = "testleft";
                                        fileappend = false;
                                    }

                                   // textBox1.Text = "Right hand: " + lines;
                                    if (recordFlag == 1)
                                    {
                                        xString = xString + " " + j.Position.X*1000;
                                        yString = yString + " " + j.Position.Y*1000;
                                        zString = zString + " " + j.Position.Z*1000;
                                        xStringl = xStringl + " " + jleft.Position.X * 1000;
                                        yStringl = yStringl + " " + jleft.Position.Y * 1000;
                                        zStringl = zStringl + " " + jleft.Position.Z * 1000;
                                        frameCount++;
                                        label2.Content = frameCount * 2;
                                        

                                        if (frameCount == 60)
                                        {
                                            frameCount = 0;
                                            recordFlag = 0;
                                            System.IO.StreamWriter filex = new System.IO.StreamWriter( filename+"_x.txt", fileappend);
                                            filex.WriteLine(xString);
                                            filex.Close();

                                            System.IO.StreamWriter filey = new System.IO.StreamWriter(filename + "_y.txt", fileappend);
                                            filey.WriteLine(yString);
                                            filey.Close();

                                            System.IO.StreamWriter filez = new System.IO.StreamWriter(filename + "_z.txt", fileappend);
                                            filez.WriteLine(zString);
                                            filez.Close();

                                            System.IO.StreamWriter filex2 = new System.IO.StreamWriter(filename2 + "_x.txt", fileappend);
                                            filex2.WriteLine(xStringl);
                                            filex2.Close();

                                            System.IO.StreamWriter filey2 = new System.IO.StreamWriter(filename2 + "_y.txt", fileappend);
                                            filey2.WriteLine(yStringl);
                                            filey2.Close();

                                            System.IO.StreamWriter filez2 = new System.IO.StreamWriter(filename2 + "_z.txt", fileappend);
                                            filez2.WriteLine(zStringl);
                                            filez2.Close();

                                            xString = "";
                                            yString = "";
                                            zString = "";
                                            xStringl = "";
                                            yStringl = "";
                                            zStringl = "";

                                            testgesture();

                                        }
                                    }
                                }
                            }
                        }
                    

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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            recordFlag = 1;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            recordFlag = 0;
        }
        private string getDTWResult(string args)
        {
            Process p = new Process();
            p.StartInfo.FileName = "main.exe";
            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            textBox1.Text = output;
            return output;
        }

        private void testgesture()
        {
            if(trainmode==1)
            {   
                return;
            }
            string argx="test_x.txt "+getTrainFiles("x",0);
            string argy = "test_y.txt " + getTrainFiles("y", 0);
            string argz = "test_z.txt " + getTrainFiles("z", 0);

            string argxl = "testleft_x.txt " + getTrainFiles("x", 1);
            string argyl = "testleft_y.txt " + getTrainFiles("y", 1);
            string argzl = "testleft_z.txt " + getTrainFiles("z", 1);
           // textBox1.Text = argx;
            string xresult=getDTWResult(argx);
            string yresult = getDTWResult(argy);
            string zresult = getDTWResult(argz);

            string xresultl = getDTWResult(argxl);
            string yresultl = getDTWResult(argyl);
            string zresultl = getDTWResult(argzl);



            xresult=xresult.Remove(xresult.Length - 8);
            yresult=yresult.Remove(yresult.Length - 8);
            zresult=zresult.Remove(zresult.Length - 8);
            xresult = xresult.Substring(14);
            yresult = yresult.Substring(14);
            zresult = zresult.Substring(14);

            xresultl = xresultl.Remove(xresultl.Length - 8);
            yresultl = yresultl.Remove(yresultl.Length - 8);
            zresultl = zresultl.Remove(zresultl.Length - 8);
            xresultl = xresultl.Substring(13);
            yresultl = yresultl.Substring(13);
            zresultl = zresultl.Substring(13);
            string rightresult;
            string leftresult;
           //chekcing right
            if (xresult.CompareTo(yresult) == 0)
            {
                rightresult = xresult;
                //MessageBox.Show("Recognised Gesture - "+xresult, "Result");
            }
            else if(yresult.CompareTo(zresult) == 0)
            {
                rightresult = yresult;
               // MessageBox.Show("Recognised Gesture - " + yresult, "Result");
            }
            else if(zresult.CompareTo(xresult) == 0)
            {
                rightresult = zresult;
               // MessageBox.Show("Recognised Gesture - " + zresult, "Result");
            }
            else
            {//no result
                rightresult = "None";
               // MessageBox.Show("Recognised Gesture - None\n"/*+xresult+" \n"+yresult+" \n"+zresult*/, "Result");
            }
            //checking left
            if (xresultl.CompareTo(yresultl) == 0)
            {
                leftresult = xresultl;
                //MessageBox.Show("Recognised Gesture - "+xresult, "Result");
            }
            else if (yresultl.CompareTo(zresultl) == 0)
            {
                leftresult = yresultl;
                // MessageBox.Show("Recognised Gesture - " + yresult, "Result");
            }
            else if (zresultl.CompareTo(xresultl) == 0)
            {

                leftresult = zresultl;
                // MessageBox.Show("Recognised Gesture - " + zresult, "Result");
            }
            else
            {//no result
                leftresult = "None";
                // MessageBox.Show("Recognised Gesture - None\n"/*+xresult+" \n"+yresult+" \n"+zresult*/, "Result");
            }
            if (rightresult.CompareTo(leftresult) == 0)
            {
                label3.Content = "Recognised Gesture\n" + rightresult;
                label3.Visibility = Visibility.Visible;
                button2.Visibility = Visibility.Visible;
            }
            else
            {
                label3.Content = "Recognised Gesture\nNone";
                label3.Visibility = Visibility.Visible;
                button2.Visibility = Visibility.Visible;
            }
            timerstarted = 0;
            sw.Reset();
            restarttesting = 0;
            
           
        }



        private void button4_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Made in fulfillment of Study Project in CEERI by P. Deepak Prasanth, Mohit Minhas and Tarang Shah\nSpecial Thanks to Dr J L Raheja and Prof. Dhiraj Sangwan", "About");
            //getTrainFiles();
        }

        //private void checkBox1_Checked(object sender, RoutedEventArgs e)
        //{

        //    //Microst.VisualBasic.Interaction.InputBox("Did you knowe your question goes here?", "Title", "Default Text");
        //    String filename = Microsoft.VisualBasic.Interaction.InputBox("Enter the Train Gesture Name", "Train Gesture", "Gesture Name");
        //}

        private void checkBox1_Checked_1(object sender, RoutedEventArgs e)
        {
            trainname = Microsoft.VisualBasic.Interaction.InputBox("Enter the Train Gesture Name", "Train Gesture", "Gesture Name");
            if (trainname == "")
            {
                return;
            }

            trainmode = 1;
        }

        private string getTrainFiles(string c,int a)
        {
            string[] filePaths;
            if(a==0)
            filePaths = Directory.GetFiles(@"./train/right","*_"+c+".txt");
            else
            filePaths = Directory.GetFiles(@"./train/left", "*_" + c + ".txt");
            string stringlist="";
            for(int i=0;i<filePaths.Length;i++)
            {
                //filePaths[i]=filePaths[i].Substring(8);
                stringlist=stringlist+" "+filePaths[i];
            }
            stringlist.Trim();
      
            return stringlist;
            //textBox1.Text = filePaths[0];

        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            
            trainmode = 0;
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            label3.Visibility = Visibility.Hidden;
            button2.Visibility = Visibility.Hidden;
            restarttesting = 1;
        }

      

    }
}