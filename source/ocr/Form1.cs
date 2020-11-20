using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ocr
{
    public partial class Form1 : Form
    {
        static string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        static string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        static ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);
        private static string ocrimage = "sample.jpg";
        private static string autooutput = "";
        private static int autodelay = 0;
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoDevice;


      


        public Form1()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            button5.Enabled = false;
            button2.Enabled = false;
            textBox5.Text = "> Debug window started.";
            textBox8.Text = "> Waiting to capture...";
            EnumerateCams();
          
            timer1.Enabled = false;
        }

        //---------------------------------------------------------

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox1.Text = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(textBox1.Text);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                button2.Enabled = true;
                button5.Enabled = true;
                addDebugMessage("Loaded image from file.");
            } else
            {
                addDebugMessage("Could not load picture from file.");
            }
        }
        //---------------------------------------------------------

        public void button2_Click(object sender, EventArgs e)
        {
            ocrimage = textBox1.Text;
            button1.Enabled = false;
            button2.Enabled = false;
            button2.Text = "Detecting...";
            addDebugMessage("Starting recognition on " + ocrimage + "...");
            BatchReadFileLocal(client, ocrimage);
            addDebugMessage("Successfully detected text.");
        }

        //---------------------------------------------------------



        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            textBox1.Text = "";
            addDebugMessage("Reset image preview.");
        }

        //----------------------------------------------------------




        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "" || textBox4.Text == "")
            {
                addDebugMessage("Please add a value to both boxes.");
            }
            else
            {
                string newkey = textBox3.Text;
                string newep = textBox4.Text;

                Environment.SetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY", newkey);
                Environment.SetEnvironmentVariable("COMPUTER_VISION_ENDPOINT", newep);
                subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
                endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");

                addDebugMessage("Set new environment variables for Azure credentials. May have to restart application to apply.");

            }
        }


        //----------------------------------------------------------


        private void button5_Click(object sender, EventArgs e)
        {
            string outputfile = @"E:\Incoming\" + "ocrtext.txt".AppendTimeStamp();
          
            using (StreamWriter writer = new StreamWriter(outputfile))
            {
                writer.Write(textBox2.Text);

            }
            addDebugMessage("Saved output text to file.");
            button5.Text = "Saved!";
            timer2.Interval = 1500;
            timer2.Start();
        }

        //-----------------------------------------------------------

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                textBox6.Text = openFileDialog1.FileName;
                addDebugMessage("Selected existing file for automatic capture.");
                autooutput = textBox6.Text;
            }
            else
            {
                addDebugMessage("Could not select existing file for automatic capture.");
            }
        }

        //---------------------------------------------------------------

        private void button7_Click(object sender, EventArgs e)
        {
            textBox6.Text = "automatic_textcapture.txt".AppendTimeStamp();
            textBox6.Text = @"E:\Incoming\" + textBox6.Text;
            autooutput = textBox6.Text;
        }

        //-------------------------------------------------------------


        private void button9_Click(object sender, EventArgs e)
        {

            timer1.Enabled = false;
            textBox8.Text = "> Waiting to capture...";
        }

        //----------------------------------------------------------------------

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (autodelay > 0)
            {
                autodelay -= 1;
                textBox8.Text = "> New capture in " + autodelay.ToString() + " seconds...";
            }
            else
            {
                textBox8.Text = "> Image captured and processed.";
                autodelay = Int32.Parse(textBox7.Text);
                Bitmap snapshotFrame = videoSourcePlayer1.GetCurrentVideoFrame();
                snapshotFrame.Save("capture.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                var imgtoanalyse = "capture.jpg";
                addDebugMessage("Starting recognition on " + imgtoanalyse + "...");
                BatchReadFileLocalAuto(client, imgtoanalyse, autooutput);
                addDebugMessage("Successfully detected text.");
            }
        }

        //-------------------------------------------------------------------

        private void button10_Click(object sender, EventArgs e)
        {
            EnumerateCams();
        }

        //----------------------------------------------------------------------

        private void button11_Click(object sender, EventArgs e)
        {
            if (videoDevice != null)
            {
                videoSourcePlayer1.VideoSource = videoDevice;
                videoSourcePlayer1.Start();
            }
            else
            {
                textBox8.Text = "> No working webcam selected.";
            }
        }

        //----------------------------------------------------------------------

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (videoDevices.Count != 0)
            {
                videoDevice = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);

            }
        }


        //----------------------------------------------------------------------

        private void button12_Click(object sender, EventArgs e)
        {
            videoSourcePlayer1.Stop();
        }

        //FUNCTIONS
        //---------------------------------------------------------

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        //---------------------------------------------------------

        public void addDebugMessage(string message)
        {
            
            textBox5.AppendText(Environment.NewLine + " > " + message);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox6.Text == "" || textBox7.Text == "")
            {
                textBox8.Text = "> Add a file and a delay, please.";
            } else {
                if (File.Exists(autooutput) == false)
                {
                    using (FileStream fs = File.Create(autooutput))
                    {
                        Byte[] title = new UTF8Encoding(true).GetBytes("Automatic Text Capture");
                        fs.Write(title, 0, title.Length);
                        Byte[] line = new UTF8Encoding(true).GetBytes("\n");
                        fs.Write(line,0,line.Length);
                        Byte[] line1 = new UTF8Encoding(true).GetBytes("\n");
                        fs.Write(line1, 0, line.Length);
                    }
                }
                autodelay = Int32.Parse(textBox7.Text);
                autooutput = textBox6.Text;
                textBox8.Text = "> New capture in " + autodelay.ToString() + " seconds...";
                timer1.Enabled = true;
              
            }
        }

        //-------------------------------------------------------------

  

        public async Task BatchReadFileLocal(ComputerVisionClient client, string localImage)
        {
          
            // Helps calucalte starting index to retrieve operation ID
            const int numberOfCharsInOperationId = 36;

            addDebugMessage($"Extracting text from local image {Path.GetFileName(localImage)}...");
            Console.WriteLine();
            using (Stream imageStream = File.OpenRead(localImage))
            {
                // Read the text from the local image
                BatchReadFileInStreamHeaders localFileTextHeaders = await client.BatchReadFileInStreamAsync(imageStream);
                addDebugMessage("Read text from local image.");
                // Get the operation location (operation ID)
                string operationLocation = localFileTextHeaders.OperationLocation;
                addDebugMessage("Got the operation location.");
                // Retrieve the URI where the recognized text will be stored from the Operation-Location header.
                string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
                addDebugMessage("Got the URI.");
                // Extract text, wait for it to complete.
                int i = 0;
                int maxRetries = 10;
                ReadOperationResult results;
                do
                {
                    results = await client.GetReadOperationResultAsync(operationId);
                    await Task.Delay(1000);
                    if (i == 9)
                    {
                        addDebugMessage("Server timed out.");
                    }
                }
                while ((results.Status == TextOperationStatusCodes.Running ||
                    results.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries);

                // Display the found text.
                Console.WriteLine();
                var textRecognitionLocalFileResults = results.RecognitionResults;      
                foreach (TextRecognitionResult recResult in textRecognitionLocalFileResults)
                {
                    foreach (Line line in recResult.Lines)
                    {
                        textBox2.AppendText(line.Text + Environment.NewLine);
                    }
                }
                
            }

            button1.Enabled = true;
            button2.Enabled = true;
            button2.Text = "Detect";
        }

        //------------------------------------------------------------------------

        public async Task BatchReadFileLocalAuto(ComputerVisionClient client, string localImage, string outputfile)
        {

            // Helps calucalte starting index to retrieve operation ID
            const int numberOfCharsInOperationId = 36;

            addDebugMessage($"Extracting text from local image {Path.GetFileName(localImage)}...");
            Console.WriteLine();
            using (Stream imageStream = File.OpenRead(localImage))
            {
                // Read the text from the local image
                BatchReadFileInStreamHeaders localFileTextHeaders = await client.BatchReadFileInStreamAsync(imageStream);
                addDebugMessage("Read text from local image.");
                // Get the operation location (operation ID)
                string operationLocation = localFileTextHeaders.OperationLocation;
                addDebugMessage("Got the operation location.");
                // Retrieve the URI where the recognized text will be stored from the Operation-Location header.
                string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
                addDebugMessage("Got the URI.");
                // Extract text, wait for it to complete.
                int i = 0;
                int maxRetries = 10;
                ReadOperationResult results;
                do
                {
                    results = await client.GetReadOperationResultAsync(operationId);
                    await Task.Delay(1000);
                    if (i == 9)
                    {
                        addDebugMessage("Server timed out.");
                    }
                }
                while ((results.Status == TextOperationStatusCodes.Running ||
                    results.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries);

                // Display the found text.
                Console.WriteLine();
                var textRecognitionLocalFileResults = results.RecognitionResults;
                foreach (TextRecognitionResult recResult in textRecognitionLocalFileResults)
                {
                    foreach (Line line in recResult.Lines)
                    {

                        StreamWriter sw = new StreamWriter(outputfile, true);
                        sw.WriteLine(line.Text);
                        sw.WriteLine(Environment.NewLine);
                        sw.Close();
                            
                    }
                }

            }
        }

        public void EnumerateCams()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count != 0)
            {
                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                {
                    comboBox1.Items.Add(device.Name);
                }
            }
            else
            {
                comboBox1.Items.Add("No devices found");
            }

            comboBox1.SelectedIndex = 0;

        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != null && !string.IsNullOrWhiteSpace(textBox2.Text)) 
            {
                Clipboard.SetText(textBox2.Text);
                button13.Text = "Copied!";
                timer3.Interval = 1500;
                timer3.Start();
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            button5.Text = "Save As Text File";
            timer2.Stop();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            button13.Text = "Copy Text";
            timer3.Stop();
        }
    }


    public static class MyExtensions
    {
        public static string AppendTimeStamp(this string fileName)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(fileName),
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(fileName)
                );
        }
    }
}
