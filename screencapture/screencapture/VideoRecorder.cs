using System;
using System.Collections.Generic;
using System.Text;
using AForge.Video.FFMPEG;
using System.Drawing;
using System.Drawing.Imaging;
using EyeXFramework;
using Tobii.EyeX.Framework;

namespace screencapture
{
    class VideoRecorder
    {
        private VideoFileWriter videoWriter;
        private System.Timers.Timer frameTimer;
        const int intervalBetweenFrames = 40; // 1000 / 25fps

        private bool isRecording;
        private bool writing = false;
        double gazeX, gazeY;
        EyeXHost _eyeXHost;
        GazePointDataStream _lightlyFilteredGazeDataStream;

        private Size screenSize;

        public VideoRecorder()
        {
            Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            screenSize = new Size(bounds.Width, bounds.Height);
            frameTimer = new System.Timers.Timer(intervalBetweenFrames);
            frameTimer.Elapsed += ProcessFrame;
            frameTimer.AutoReset = true;
            isRecording = false;
        }

        public void StartRecording(String filenameToSave){
            videoWriter = new VideoFileWriter();
            videoWriter.Open(filenameToSave, screenSize.Width, screenSize.Height, 25, VideoCodec.MPEG2);
            frameTimer.Start();
            isRecording = true;
            StartEyeStream();
        }

        public void EndRecording(){
            isRecording = false;
            frameTimer.Stop();
            while (writing) { }     // prevent closing in the middle of writing
            videoWriter.Close();
            StopEyeStream();
        }

        private void StartEyeStream()
        {
            _eyeXHost = new EyeXHost();
            _lightlyFilteredGazeDataStream = _eyeXHost.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);
            _eyeXHost.Start();

                    // Write the data to the console.
            _lightlyFilteredGazeDataStream.Next += gazeDataStreamNext;
            Console.WriteLine("Eyex setup");
        }

        private void gazeDataStreamNext(object s, GazePointEventArgs e)
        {
            gazeX = e.X;
            gazeY = e.Y;
        }

        private void StopEyeStream()
        {
            _lightlyFilteredGazeDataStream.Dispose();
            _eyeXHost.Dispose();
        }

        private void ProcessFrame(Object source, System.Timers.ElapsedEventArgs e)
        {
           
            {
                Bitmap frameImage = new Bitmap(screenSize.Width, screenSize.Height);
                // record a frame of screen
                using (Graphics g = Graphics.FromImage(frameImage))
                {
                    g.CopyFromScreen(0, 0, 0, 0, screenSize, CopyPixelOperation.SourceCopy);
                    Color red = Color.FromArgb(0x60, 0xff, 0, 0);
                    Brush redBrush = new SolidBrush(red);
                    g.FillEllipse(redBrush, (int)gazeX-50, (int)gazeY-50, 100, 100);
                }
                if (isRecording)
                {
                    if (!writing)
                    {
                        writing = true;
                        videoWriter.WriteVideoFrame(frameImage);
                        writing = false;
                    }
                }
                frameImage.Dispose();
            }
        }
    }
}
