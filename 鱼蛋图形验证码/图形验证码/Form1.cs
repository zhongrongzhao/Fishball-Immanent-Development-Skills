/*
 *鱼蛋图形验证码
 * Bilibili：鱼蛋代码
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 图形验证码
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string a;       //传值
        
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {         
            CodeImage(CheckCode());
            a = CheckCode().Substring(0, CheckCode().Length - 1);
            textBox1.Text = "";
        }

        /// <summary>
        /// 生产随机字符
        /// </summary>
        /// <returns></returns>
        private string CheckCode()
        {
            int number;
            char code;
            string checkCode = String.Empty;
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                number = random.Next();
                if(number%2==0)
                {
                    code = (char)('0' + (char)(number % 10));
                }
                else
                {
                    code = (char)('A' + (char)(number % 26));
                }
                checkCode += "" + code.ToString();
            }
            return checkCode;
        }
        /// <summary>
        /// 让随机字符生成图片
        /// </summary>
        /// <param name="checkCode"></param>
        private void CodeImage(string checkCode)
        {
            if (checkCode == null || checkCode.Trim() == string.Empty)
                return;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 9.5)), 22);
            Graphics g = Graphics.FromImage(image);
            try
            {
                Random random = new Random();
                g.Clear(Color.White);
                for(int i=0; i<3; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
                }
                Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold));
                g.DrawString(checkCode, font, new SolidBrush(Color.Red), 2, 2);
                for(int i = 0; i < 150; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                this.pictureBox1.Width = image.Width;
                this.pictureBox1.Height = image.Height;
                this.pictureBox1.BackgroundImage = image;
            }
            catch { }
         }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(a))
            {
                MessageBox.Show("对了", "提示");
            }
            else
            {
                MessageBox.Show("错了", "提示");
            }
        }
    }
}
