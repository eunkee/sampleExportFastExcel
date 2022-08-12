using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace testExportFastExcel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 그리드 선택 안되게
        private void DataGridView1_Click(object sender, EventArgs e)
        {
            DataGridView1.ClearSelection();
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            DataGridView1.Rows.Clear();
            for (int i = 0; i < 7; i++)
            {
                var rnd = new Random(Guid.NewGuid().GetHashCode());
                var year = rnd.Next(DateTime.Now.Year, DateTime.Now.Year + 1);
                var month = rnd.Next(1, 13);
                var days = rnd.Next(1, DateTime.DaysInMonth(year, month) + 1);

                DateTime dateTime = new DateTime(year, month, days,
                    rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), rnd.Next(0, 1000));

                string chars = "ABCDEFG";
                int sellerNumber = rnd.Next(1, chars.Length);
                string seller = $"Company{chars[sellerNumber]}";
                int count = rnd.Next(1, 100);

                DataGridViewRow AddRow = new DataGridViewRow();
                AddRow.CreateCells(DataGridView1);
                AddRow.Cells[0].Value = dateTime.ToString();
                AddRow.Cells[1].Value = seller;
                AddRow.Cells[2].Value = count.ToString();
                AddRow.Height = 25;
                DataGridView1.Rows.Add(AddRow);
            }

            DataGridView1.ClearSelection();
        }

        private void ButtonExport_Click(object sender, EventArgs e)
        {
            if (DataGridView1.Rows.Count <= 0)
            {
                return;
            }

            string nowDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string pathFilename;

            SaveFileDialog saveFile = new SaveFileDialog
            {
                Title = "Save Excel file",
                FileName = $"List_{nowDateTime}.xlsx",
                DefaultExt = "xlsx",
                Filter = "Xlsx files(*.xlsx)|*.xlsx"
            };

            // 파일 이름
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                pathFilename = saveFile.FileName.ToString();
            }
            else
            {
                return;
            }

            int rowCount = DataGridView1.Rows.Count;
            try
            {
                // 샘플 파일 Sheet1 까지 포함해서 반드시 존재해야
                FileInfo templateFile = new FileInfo($"{Application.StartupPath}\\sample.xlsx");
                FileInfo outputFile = new FileInfo(pathFilename);

                using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(templateFile, outputFile))
                {
                    List<MyClass> objectList = new List<MyClass>();

                    // 로우 진행
                    for (int rowNumber = 1; rowNumber < rowCount; rowNumber++)
                    {
                        DataGridViewRow row = DataGridView1.Rows[rowNumber - 1];

                        if (!int.TryParse(row.Cells[2].Value.ToString(), out int count))
                        {
                            count = -1;
                        }

                        // 값 할당
                        MyClass genericObject = new MyClass
                        {
                            Date = row.Cells[0].Value.ToString(),
                            MaterialName = row.Cells[1].Value.ToString(),
                            ReceivingCount = count
                        };

                        objectList.Add(genericObject);
                    }

                    // 시트 이름 중요
                    fastExcel.Write(objectList, "sheet1", true);

                    MessageBox.Show("Complete Export", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Fail Export", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
