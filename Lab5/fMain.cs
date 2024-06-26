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
using TVLib;
using Lab3;
using Lab5;

namespace Lab5
{
    public partial class fMain : Form
    {
        public fMain()
        {
            InitializeComponent();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            gvTVs.AutoGenerateColumns = false;

            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Model";
            column.Name = "Модель";
            gvTVs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Display";
            column.Name = "Дисплей";
            gvTVs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Cores";
            column.Name = "Ядра";
            gvTVs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Resolution";
            column.Name = "Роздільна здатність";
            gvTVs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Platform";
            column.Name = "Платформа";
            gvTVs.Columns.Add(column);

            column = new DataGridViewCheckBoxColumn();
            column.DataPropertyName = "HasTuner";
            column.Name = "Тюнер";
            gvTVs.Columns.Add(column);

            column = new DataGridViewCheckBoxColumn();
            column.DataPropertyName = "HasAI";
            column.Name = "ШІ";
            gvTVs.Columns.Add(column);

            bindSrcTVs.Add(new TV("CoolPlasmaTV", "plasma", 4, "8240x4120", "Android", true, true));
            EventArgs eventArgs = new EventArgs(); OnResize(eventArgs);
        }

        private void fMain_Resize(object sender, EventArgs e)
        {
            int buttonsSize = 5 * btnAdd.Width + 2 * toolStripSeparator1.Width + 30;
            btnExit.Margin = new Padding(Width - buttonsSize, 0, 0, 0);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            TV tv = new TV();

            fTV ftv = new fTV(tv);
            if (ftv.ShowDialog() == DialogResult.OK)
            {
                bindSrcTVs.Add(tv);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            TV tv = (TV)bindSrcTVs.List[bindSrcTVs.Position];

            fTV ftv = new fTV(tv);
            if (ftv.ShowDialog() == DialogResult.OK)
            {
                bindSrcTVs.List[bindSrcTVs.Position] = tv;
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Видалити поточний запис?", "Видалення запису", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            { bindSrcTVs.RemoveCurrent(); }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Очистити таблицю?\n\nВсі дані будуть втрачені", "Очищення даних", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)== DialogResult.OK)
                bindSrcTVs.List.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Закрити застосунок?", "Вихід з програми", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void btnSaveAsText_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Текстові файли (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Зберегти дані у текстовому форматі";
            saveFileDialog.InitialDirectory = Application.StartupPath;
            StreamWriter sw;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);
                try
                {
                    foreach (TV tv in bindSrcTVs.List)
                    {
                        sw.Write(tv.Model + "\t" + tv.Display + "\t" +
                                 tv.Cores + "\t" + tv.Resolution + "\t" + tv.Platform + "\t" +
                                 tv.HasTuner + "\t" + tv.HasAI + "\t\n");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сталась помилка: \n{0}", ex.Message,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    sw.Close();
                }
            }

        }

        private void btnSaveAsBinary_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Файли даних (*.tvss)|*.tvs|All files (*.*)|*.*";
            saveFileDialog.Title = "Зберегти дані у бінарному форматі";
            saveFileDialog.InitialDirectory = Application.StartupPath;
            BinaryWriter bw;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                bw = new BinaryWriter(saveFileDialog.OpenFile());
                try 
                {    
                    foreach (TV tv in bindSrcTVs.List)
                    {
                        bw.Write(tv.Model);
                        bw.Write(tv.Display);
                        bw.Write(tv.Cores);
                        bw.Write(tv.Resolution);
                        bw.Write(tv.Platform);
                        bw.Write(tv.HasTuner);
                        bw.Write(tv.HasAI);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сталась помилка: \n{0}", ex.Message,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    bw.Close();
                }
            }

        }

        private void btnOpenFromText_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстові файли (*.txt)| *.txt|All files (*.*) | *.* ";
            openFileDialog.Title = "Прочитати дані у текстовому форматі";
            openFileDialog.InitialDirectory = Application.StartupPath;
            StreamReader sr;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bindSrcTVs.Clear(); sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                string s;
                try
                {
                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] split = s.Split('\t');
                        TV tv = new TV(split[0], split[1], int.Parse(split[2]), split[3], split[4]
                            , bool.Parse(split[5]), bool.Parse(split[6]));
                        bindSrcTVs.Add(tv);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сталась помилка: \n{0}", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    sr.Close();
                }
            }

        }

        private void btnOpenFromBinary_Click(object sender, EventArgs e)
        {    
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Текстові файли (*.tvs)| *.tvs|All files (*.*) | *.* ";
                openFileDialog.Title = "Прочитати дані у текстовому форматі";
                openFileDialog.InitialDirectory = Application.StartupPath;
                BinaryReader br;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bindSrcTVs.Clear();
                    br = new BinaryReader(openFileDialog.OpenFile());
                    try
                    {
                        TV tv;
                        while(br.BaseStream.Position < br.BaseStream.Length)
                        {
                            tv = new TV();
                            for (int i = 0; i < 8; i++)
                            {
                                switch (i)
                                {
                                    case 1:
                                        tv.Model = br.ReadString();
                                        break;
                                    case 2:
                                        tv.Display = br.ReadString();
                                        break;
                                    case 3:
                                        tv.Cores = br.ReadInt32();
                                        break;
                                    case 4:
                                        tv.Resolution = br.ReadString();
                                        break;
                                    case 5:
                                        tv.Platform = br.ReadString();
                                        break;
                                    case 6:
                                        tv.HasTuner = br.ReadBoolean();
                                        break;
                                    case 7:
                                        tv.HasAI = br.ReadBoolean();
                                        break;
                                }
                            }
                            bindSrcTVs.Add(tv);
                        }
                    }    
                    catch (Exception ex)
                    {
                        MessageBox.Show("Сталась помилка: \n{0}", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        br.Close();
                    }
                }
            }
    }
}
