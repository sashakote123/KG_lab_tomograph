using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tomograph
{
    public partial class Form1 : Form
    {

        Bin obj = new Bin();

        public Form1()
        {
            InitializeComponent();
        }



        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }




        bool loaded = false;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                Bin.readBIN(str);
                Viev.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
            trackBar1.Maximum = Bin.Z-1;

            trackBar2.Minimum = 1;
            trackBar3.Minimum = 1;
            trackBar2.Maximum = 2000;
            trackBar2.Value = 500;
            trackBar3.Maximum = 2000;

        }


        int currentLayer;
        int min=0;
        int width=5;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            min = trackBar2.Value;
            label1.Text = min.ToString();
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            width = trackBar3.Value;
            label2.Text = width.ToString();
            needReload = true;
        }


        bool needReload = false;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            
            if (radioButton1.Checked)
            {
                if (checkBox1.Checked)
                {
                    if (loaded)
                    {
                        if (needReload)
                        {
                            Viev.generateTextureImage(currentLayer, min, width);
                            Viev.Load2DTexture();
                            needReload = false;
                        }
                        Viev.DrawTexture();
                        glControl1.SwapBuffers();
                    }
                }
                else
                {
                    if (loaded)
                    {
                        if (needReload)
                        {
                            Viev.generateTextureImage2(currentLayer, min, width);
                            Viev.Load2DTexture();
                            needReload = false;
                        }
                        Viev.DrawTexture();
                        glControl1.SwapBuffers();
                    }
                }
                
            }

            if (radioButton2.Checked)
            {            
                if (loaded)
                {
                    Viev.DrawQuads(currentLayer, min, width);
                    glControl1.SwapBuffers();
                }
               
            }
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }


    }
}
