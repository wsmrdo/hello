using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace CheckData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string[] ReadFileS = null;
       // private float[] SumValue = new float[8];
        private string[] tempArray = new string[30000];
        private string[] tempArray2 = new string[30000];
        private int[] ReadValue = new int[30000];
        
        private string[] tempArrayBitFF = new string[1024];
        private string[] tempArrayHex = new string[1024];
        private void ReadData()
        {
            //String sss = null;
            //int aaa = 0;

            
#if sdfghj
            char Sp = '\\';
            //int aaaaa = 0;
            string[] SpString = ReadFileS[0].Split(Sp);
            ///#if dfghjkl
            StartDate = int.Parse(SpString[SpString.Length - 1].Substring(2, 8));

            SpString = ReadFileS[ReadFileS.Length - 1].Split(Sp);
            EndDate = int.Parse(SpString[SpString.Length - 1].Substring(2, 8));

            /*    DateTime ReadTime =  DateTime((StartDate / 10000), (StartDate / 100) %100, StartDate % 100, 0, 0, 0);
                 DateTime EndTime =  DateTime((EndDate / 10000), (EndDate /100) % 100, EndDate % 100, 0, 0, 0);
             * */
            int y = StartDate / 10000;
            int m = (StartDate / 100) % 100;
            int d = StartDate % 100;
            ReadDayFrom = StartDate;
            DateTime ReadTime = new DateTime(y, m, d, 0, 0, 0);
            y = EndDate / 10000;
            m = (EndDate / 100) % 100;
            d = EndDate % 100;
            ReadDays_From = EndDate;
            DateTime EndTime = new DateTime(y, m, d, 0, 0, 0);
            TimeSpan Days = EndTime - ReadTime;
            ///#endif        
            ///       TotalSec  
            TotalSec = (Days.Days + 1) * 3600 * 24;
            for (int i = 0; i < Constants.Max_X_Size; i++)
            {
                for (int j = 0; j < 8; j++) gData[i, j] = 0;
            }

#endif
            
            //for (int i = 0; i < ReadFileS.Length; i++)
            {
                System.IO.StreamReader file = new System.IO.StreamReader(ReadFileS[0]);
                int i = 0;
                do
                {
                    tempArray[i] = file.ReadLine();
                    //tempArray[1] = "abcd";
                    if (i!=0)
                    {
                        ReadValue[i] = Convert.ToInt32(tempArray[i].Substring(2, 4),16);
                        tempArray2[i] = ReadValue[i].ToString();
                    }
                    //file.ReadLine().ToString();
                    
                    i++;

                    // Check1Line(sss);

                      
                } while (file.EndOfStream == false);
                richTextBox1.Lines = tempArray;
                richTextBox2.Lines = tempArray2;
                //   file.BaseStream.Length > file.BaseStream.Position);
                file.Close();
           }
            
            
                
        }
        private int tempArrayBitcount = 0;
        private void ConvertBit()
        { 
            bool IsInputFlage = true; 
            bool WasInputFlage = true;
            int j = 0, i = 0;
            int TotalBitCount = 0;

            string[] tempArrayBit = new string[1024];

            for (i = 1; i < 1024; i++)
            {
                tempArrayBit[i] = "";
            }
            //richTextBox3.Clear();
            //tempArrayBit = richTextBox3.Lines;

            for(tempArrayBitcount =0, i=1; tempArrayBitcount < 1024; i++)
            {
                
                if (ReadValue[i] == 0) break;
                if (ReadValue[i] == 0xFFFF)
                {
                    tempArrayBit[tempArrayBitcount] = "Power Off to On ";
                    tempArrayBitcount++;
                    TotalBitCount = 0;
                    continue;
                }

                if (ReadValue[i - 1] == 0xFFFF)
                {
                    tempArrayBit[tempArrayBitcount] = "Start Dummy ";
                    tempArrayBitcount++;
                    TotalBitCount = 0;
                    continue;
                }

                if ((ReadValue[i] & 0x4000) == 0x4000)
                {
                    int BitCount = (ReadValue[i] & 0x3FFF);
                    TotalBitCount += BitCount;
                    for (j = 0; j < BitCount; j++)
                        tempArrayBit[tempArrayBitcount] +="1";
                    //tempArrayBitcount++;
                }
                else if ((ReadValue[i] & 0x8000) == 0x8000)
                {
                    IsInputFlage = false;
                    int BitCount = ((ReadValue[i] & 0x3FFF) + 2) / 5;
                    TotalBitCount += BitCount;
                    if(WasInputFlage != IsInputFlage)
                    {
                        WasInputFlage = IsInputFlage;
                    //    tempArrayBit[tempArrayBitcount] +="1";  
                        tempArrayBit[tempArrayBitcount] = tempArrayBitcount.ToString("D3") + "ToChip   :" + tempArrayBit[tempArrayBitcount];
                        tempArrayBitcount++;
                        TotalBitCount = 0;
                    }

                    for(j=1;j<BitCount ;j++)
                    {
                        tempArrayBit[tempArrayBitcount] +="0";
                    }
                    tempArrayBit[tempArrayBitcount] +="1";
                }else{
                    IsInputFlage = true;
                    int BitCount = ((ReadValue[i] & 0x3FFF) + 2) / 5;
                    TotalBitCount += BitCount;
                    if (WasInputFlage != IsInputFlage)
                    {
                        WasInputFlage = IsInputFlage;
                        tempArrayBit[tempArrayBitcount] += "1";
                        tempArrayBit[tempArrayBitcount] = tempArrayBitcount.ToString("D3") + "ToPrinter :" + tempArrayBit[tempArrayBitcount];
                        TotalBitCount = 0;
                        tempArrayBitcount++;
                    }
                    else
                    {
                        tempArrayBit[tempArrayBitcount] += "1";
                    }
   
                    for(j=1;j<BitCount ;j++)
                    {
                        tempArrayBit[tempArrayBitcount] +="0";
                    }
                }
            }
            if (TotalBitCount > 0)
            {
                if (IsInputFlage == false)
                {
                    tempArrayBit[tempArrayBitcount] = tempArrayBitcount.ToString("D3") + "ToPrinter :" + tempArrayBit[tempArrayBitcount];
                    tempArrayBitcount++;
                }
                else
                {
                    tempArrayBit[tempArrayBitcount] = tempArrayBitcount.ToString("D3") + "ToChip   :" + tempArrayBit[tempArrayBitcount];
                    tempArrayBitcount++;
                }
            }

            richTextBox3.Lines = tempArrayBit;
            tempArrayBitFF = tempArrayBit;
            
        }
        private char[] Bitvalues = new char[10000];
        //input.ToCharArray();
        private void ConVertBitToHex()
        {
            //int Value=0;
            int HexValue = 0, j= 0;
            int BitCount = 0;
            for (j = 0; j < 1024; j++)
            {
                tempArrayHex[j] = "";
            }
            for (j = 0; j < tempArrayBitcount; j++)
            {
                BitCount = 0;
                HexValue = 0;
                Bitvalues = tempArrayBitFF[j].ToCharArray();
                bool isTitleOk = false;
                for (int i = 0; i < tempArrayBitFF[j].Length; i++)
                {
                    if (isTitleOk)
                    {
                        if (Bitvalues[i] == '0')
                        {
                            HexValue *= 2;
                        }
                        else if (Bitvalues[i] == '1')
                        {
                            HexValue = HexValue * 2 + 1;
                        }

                        BitCount++;
                        if ((BitCount & 0x0007) == 0)
                        {
                            tempArrayHex[j] += (String.Format("{0:X2}", HexValue) + " ");
                            HexValue = 0;
                        }
                    }
                    else
                    {
                        // file.Write(tempArrayBit[i].Substring(i, 1));
                        if (Bitvalues[i] == ':') isTitleOk = true;
                        tempArrayHex[j] += tempArrayBitFF[j].Substring(i, 1);
                    }

                }
                while((BitCount & 0x0007) != 0)
                {
                    HexValue *= 2;
                    
                    BitCount++;
                    if ((BitCount & 0x0007) == 0)
                    {
                        tempArrayHex[j] += (String.Format("{0:X2}", HexValue) + " ");
                        HexValue = 0;
                       break;
                    }
                }
                //file.WriteLine(" ");
            }
            richTextBox4.Lines = tempArrayHex;
            
        }
        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            // StreamReader SW = null;
            Stream myStream = null;
            // File FF = null;
            // OpenFileDialog openFileDialog1 = new OpenFileDialog();
            // File CCC = File
            //string SSS = new byte[10240000];
            // new StreamReader(FilePath, true))
            //string text = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");

            // new StreamReader(FilePath, true))
            //openFileDialog1.InitialDirectory = "D:\\Job\\신광2차\\20170717";
            openFileDialog1.Filter = "data files (*.dat)|*.dat|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            //openFileDialog1.Multiselect = true;

            //  FF = openFileDialog1.OpenFile();

            /*   myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
               formGraphics = this.CreateGraphics();

               formGraphics.DrawLine(myPen, 0, 0, 200, 200);

                BackGroundDraw();
            
               myPen.Dispose();
               formGraphics.Dispose();
            
              */

            // openFileDialog1.FileName

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {

                        //string text = null;

                        //  String nnn = null;
                        myStream.Close();
                        ReadFileS = openFileDialog1.FileNames;

                        
                        ReadData();
                        //Make_Another();
                        //DrawScrean();
                        //trackBar1.Value = Constants.Max_X_Size - 2;
                        //trackBar1_Draw_Text();
                        // using (myStream)
                        // {

                        // }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

            //textInBar1.Text = Cursor.Position.X.ToString();
            //textOutBar1.Text = Cursor.Position.Y.ToString();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.Update();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            richTextBox2.Update();
            tempArray2 = richTextBox2.Lines;
            for (int i = 1; i < tempArray2.Length ; i++)
            {
                ReadValue[i] = int.Parse(tempArray2[i]);
                if (ReadValue[i] == 0) break;
            }
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            richTextBox3.Update();
           // for (i = 1; i < ; i++)
           // {
           //     tempArrayBit[i] = richTextBox3.Lines[i];
           // }
            tempArrayBitFF = richTextBox3.Lines;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox3.Clear();
            ConvertBit();
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox4.Clear();
            ConVertBitToHex();
        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(ReadFileS[0] + ".txt");
            for (int j = 0; j < tempArrayBitcount; j++)
            {
                file.WriteLine(tempArrayBitFF[j]);
            }
            file.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(ReadFileS[0] + ".Hex");
            for (int j = 0; j < tempArrayBitcount; j++)
            {
                file.WriteLine(tempArrayHex[j]);
            }
            file.Close(); 
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value > tempArrayBitcount) trackBar1.Value = tempArrayBitcount;
            textBox1.Text = tempArrayBitFF[trackBar1.Value];//.ToString;
            textBox2.Text = tempArrayHex[trackBar1.Value];//.ToString;
            
            textBox5.Text = trackBar1.Value.ToString();
            int Next = trackBar1.Value + 1;
            textBox6.Text = Next.ToString();
            textBox3.Text = tempArrayBitFF[Next];
            textBox4.Text = tempArrayHex[Next];
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox4_CursorChanged(object sender, EventArgs e)
        {
            //현재 커서위치 찾기
            int index = richTextBox4.SelectionStart;
            //커서위치에서 line 넘버 찾기
            int line = richTextBox4.GetLineFromCharIndex(index);

            //현재 line의 첫번째 char 위치부터, 현재 line의 text 끝까지 선택
            richTextBox1.Select(richTextBox4.GetFirstCharIndexFromLine(line), richTextBox1.Lines[line].Length);
            //선택영역 백그라운드 칼러 변경
            richTextBox4.SelectionBackColor = Color.Red;
            textBox9.Text = line.ToString();
            textBox10.Text = index.ToString();

        }

        private void richTextBox4_MouseUp(object sender, MouseEventArgs e)
        {
            //현재 커서위치 찾기
            int index = richTextBox4.SelectionStart;
            //커서위치에서 line 넘버 찾기
            int line = richTextBox4.GetLineFromCharIndex(index);

            //현재 line의 첫번째 char 위치부터, 현재 line의 text 끝까지 선택
            //richTextBox1.Select(richTextBox4.GetFirstCharIndexFromLine(line), richTextBox1.Lines[line].Length);
            //선택영역 백그라운드 칼러 변경
            //richTextBox4.SelectionBackColor = Color.Red;
            textBox9.Text = line.ToString();
            textBox10.Text = index.ToString();

        }

        private void richTextBox3_MouseUp(object sender, MouseEventArgs e)
        {
            //현재 커서위치 찾기
            int index = richTextBox3.SelectionStart;
            //커서위치에서 line 넘버 찾기
            int line = richTextBox3.GetLineFromCharIndex(index);

            //현재 line의 첫번째 char 위치부터, 현재 line의 text 끝까지 선택
            //richTextBox1.Select(richTextBox4.GetFirstCharIndexFromLine(line), richTextBox1.Lines[line].Length);
            //선택영역 백그라운드 칼러 변경
            //richTextBox4.SelectionBackColor = Color.Red;
            textBox7.Text = line.ToString();
            textBox8.Text = index.ToString();
        }

     }
}
