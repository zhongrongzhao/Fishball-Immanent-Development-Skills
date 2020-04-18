using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 鱼蛋文件分割器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static private string what_do_you_want_to_do = "";
        /// <summary>
        /// 选择文件
        /// </summary>
        private void SelectFile(string what)
        {
            if(what.Equals("splitfile"))
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();   //显示选择文件对话框 
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.textBox1.Text = openFileDialog1.FileName;     //显示文件路径 
                }
            }

            if(what.Equals("combinfile"))
            {
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    string Selectfile = "";
                    string[] files = openFileDialog2.FileNames;
                    for (int i = 0; i < files.Length; i++)
                    {
                        Selectfile += "," + files[i].ToString();
                    }
                    if (Selectfile.StartsWith(","))
                    {
                        Selectfile = Selectfile.Substring(1);
                    }
                    if (Selectfile.EndsWith(","))
                    {
                        Selectfile.Remove(Selectfile.LastIndexOf(","), 1);
                    }
                    this.textBox1.Text = Selectfile;
                }

            }
        }
        

        /// <summary>
        /// 选择文件夹 
        /// </summary>
        private void SelectPath(string what)
        {
            if (what.Equals("splitfile"))
            {
                if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (this.folderBrowserDialog1.SelectedPath.Trim() != "")
                        this.textBox3.Text = this.folderBrowserDialog1.SelectedPath.Trim();
                }
            }
            if (what.Equals("combinfile"))
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBox3.Text = saveFileDialog1.FileName;
                }
            }
        }

        /// <summary>
        /// 分割文件
        /// </summary>
        /// <param name="strFlag">分割单位</param>
        /// <param name="intFlag">分割大小</param>
        /// <param name="strPath">分割后的文件存放路径</param>
        /// <param name="strFile">要分割的文件</param>
        /// <param name="PBar">进度条显示</param>
        public void SplitFile(string strFlag, int intFlag, string strPath, string strFile, ProgressBar PBar)
        {
            int iFileSize = 0;
            //根据选择来设定分割的小文件的大小
            switch (strFlag)
            {
                case "Byte":
                    iFileSize = intFlag;
                    break;
                case "KB":
                    iFileSize = intFlag * 1024;
                    break;
                case "MB":
                    iFileSize = intFlag * 1024 * 1024;
                    break;
                case "GB":
                    iFileSize = intFlag * 1024 * 1024 * 1024;
                    break;
            }
            //以文件的全路径对应的字符串和文件打开模式来初始化FileStream文件流实例
            FileStream SplitFileStream = new FileStream(strFile, FileMode.Open);
            //以FileStream文件流来初始化BinaryReader文件阅读器
            BinaryReader SplitFileReader = new BinaryReader(SplitFileStream);
            //每次分割读取的最大数据
            byte[] TempBytes;
            //小文件总数
            int iFileCount = (int)(SplitFileStream.Length / iFileSize);
            PBar.Maximum = iFileCount;
            if (SplitFileStream.Length % iFileSize != 0) iFileCount++;
            string[] TempExtra = strFile.Split('.');
            //循环将大文件分割成多个小文件
            for (int i = 1; i <= iFileCount; i++)
            {
                //确定小文件的文件名称
                string sTempFileName = strPath + @"\" + i.ToString().PadLeft(4, '0') + "." + TempExtra[TempExtra.Length - 1]; //小文件名
                //根据文件名称和文件打开模式来初始化FileStream文件流实例
                FileStream TempStream = new FileStream(sTempFileName, FileMode.OpenOrCreate);
                //以FileStream实例来创建、初始化BinaryWriter书写器实例
                BinaryWriter TempWriter = new BinaryWriter(TempStream);
                //从大文件中读取指定大小数据
                TempBytes = SplitFileReader.ReadBytes(iFileSize);
                //把此数据写入小文件
                TempWriter.Write(TempBytes);
                //关闭书写器，形成小文件
                TempWriter.Close();
                //关闭文件流
                TempStream.Close();
                PBar.Value = i - 1;
            }
            //关闭大文件阅读器
            SplitFileReader.Close();
            SplitFileStream.Close();
            MessageBox.Show("文件分割成功!");
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="list">要合并的文件集合</param>
        /// <param name="strPath">合并后的文件名称</param>
        /// <param name="PBar">进度条显示</param>
        public void CombinFile(string[] strFile, string strPath, ProgressBar PBar)
        {
            PBar.Maximum = strFile.Length;
            FileStream AddStream = null;
            //以合并后的文件名称和打开方式来创建、初始化FileStream文件流
            AddStream = new FileStream(strPath, FileMode.Append);
            //以FileStream文件流来初始化BinaryWriter书写器，此用以合并分割的文件
            BinaryWriter AddWriter = new BinaryWriter(AddStream);
            FileStream TempStream = null;
            BinaryReader TempReader = null;
            //循环合并小文件，并生成合并文件
            for (int i = 0; i < strFile.Length; i++)
            {
                //以小文件所对应的文件名称和打开模式来初始化FileStream文件流，起读取分割作用
                TempStream = new FileStream(strFile[i].ToString(), FileMode.Open);
                TempReader = new BinaryReader(TempStream);
                //读取分割文件中的数据，并生成合并后文件
                AddWriter.Write(TempReader.ReadBytes((int)TempStream.Length));
                //关闭BinaryReader文件阅读器
                TempReader.Close();
                //关闭FileStream文件流
                TempStream.Close();
                PBar.Value = i + 1;
            }
            //关闭BinaryWriter文件书写器
            AddWriter.Close();
            //关闭FileStream文件流
            AddStream.Close();
            MessageBox.Show("文件合并成功！");
        }

        /// <summary>
        /// 选择文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if(what_do_you_want_to_do == "")
            {
                MessageBox.Show("请选择”分割文件“或”合并文件“", "警告");
            }
            else
            {
                SelectFile(what_do_you_want_to_do);
            }
        }

        /// <summary>
        /// 选择文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (what_do_you_want_to_do == "")
            {
                MessageBox.Show("请选择”分割文件“或”合并文件“", "警告");
            }
            else
            {
                SelectPath(what_do_you_want_to_do);
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
            timer1.Start();
        }

     

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(textBox1.Text != ""  && textBox3.Text != "")
            {
                button2.Enabled = true;
                button4.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
                button4.Enabled = false;
            }
        }

        /// <summary>
        /// 分割/合并文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (what_do_you_want_to_do.Equals("splitfile"))
                    {
                        if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
                        {
                            MessageBox.Show("请填写好参数", "警告");
                        }
                        else
                        {
                            SplitFile(comboBox1.Text, Convert.ToInt32(textBox2.Text.Trim()), textBox3.Text, textBox1.Text, progressBar1);
                        }
                    }

                if (what_do_you_want_to_do.Equals("combinfile"))
                {
                    if (textBox1.Text == "" || textBox3.Text == "")
                    {
                        MessageBox.Show("请填写好参数", "警告");
                    }
                    else
                    {
                        if (textBox1.Text.IndexOf(",") == -1)
                        {
                            MessageBox.Show("至少两个文件才能合成", "警告");
                        }

                        else
                        {
                            string[] text = textBox1.Text.Split(',');
                            CombinFile(text, textBox3.Text, progressBar1);
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 打开导出文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            string v_OpenFolderPath = textBox3.Text; System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox2.Enabled = true;
            comboBox1.Enabled = true;
            label2.Enabled = true;
            what_do_you_want_to_do = "splitfile";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox2.Enabled = false;
            comboBox1.Enabled = false;
            label2.Enabled = false;
            what_do_you_want_to_do = "combinfile";
        }
    }
}
