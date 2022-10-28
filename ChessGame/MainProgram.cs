using System;
using ChessGame;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace ChessGame
{
    class MainProgram
    {
        static MyForm form = new MyForm();
        static void Main(string[] args)
        {
            _ = new Board(form);

            Application.Run(form);
        }
    }
    
    class MyForm : Form
    {
        public MyForm()
        {
            Width = 900;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
        }

        private void Form_MouseMove(object sender, PaintEventArgs e)
        {
            //Graphics g = this.CreateGraphics(); 
            //string testString = "SAMSUNG";
            //Font verdana14 = new Font("DDTW00-CondensedSemiBold", 30); 
            //SizeF sz = g.MeasureString(testString, verdana14);
            //string stringDetails = "Height: " + sz.Height.ToString() + ", Width: " + sz.Width.ToString();

            //MessageBox.Show("String details: " + stringDetails);

            //g.DrawString(testString, verdana14, Brushes.HotPink, new PointF(200,200));
            //g.DrawRectangle(new Pen(Color.Blue, 3), 200, 200, sz.Width,sz.Height);
            //g.Dispose();

            Graphics g = e.Graphics;
            Image img = Image.FromFile(@"C:\Users\ADMIN\Downloads\Pictures\PHOTOS\LOL\Outfit.png");
            Bitmap curBitmap = new Bitmap(img);
            g.DrawImage(curBitmap, 0, 0, curBitmap.Width, curBitmap.Height);
            int brightnessScale = 50;
            
            for (int i = 100; i < 600; i++)
            {
                for (int j = 100; j < 400; j++)
                {
                    Color curColor = curBitmap.GetPixel(i, j);
                    
                    //int ret = (curColor.R + curColor.G + curColor.B) / 3;

                    int red = Truncate(curColor.R + brightnessScale);
                    int green = Truncate(curColor.G + brightnessScale);
                    int blue = Truncate(curColor.B + brightnessScale);
                    
                    curBitmap.SetPixel(i, j, Color.FromArgb(red, green, blue));
                }
            }

            g.DrawImage(curBitmap, 0, 0, curBitmap.Width, curBitmap.Height);
        }

        private int Truncate(int value)
        {
            if (value < 0)
                return 0;

            if (value > 255)
                return 255;

            return value;
        }
    }
}
