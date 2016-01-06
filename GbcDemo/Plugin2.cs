using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using SharpAccessory.Resources;

using SharpAccessory.GenericBusinessClient.Plugging;
using SharpAccessory.GenericBusinessClient.VisualComponents;

using SharpAccessory.GenericBusinessClient.DefaultPlugins;


namespace GbcDemo
{

    public class Plugin2 : Plugin
    {
        private ToolButton tbGoToDisplayResultsForm;

        private string previousPlugin;

        public Plugin2()
        {
            Text = "ICD - Search";

            InitializeComponent();

            tbGoToDisplayResultsForm = new ToolButton();
            tbGoToDisplayResultsForm.Image = TangoIconSet.LoadIcon(TangoIcon.Go_Previous);
            tbGoToDisplayResultsForm.Text = "List of selected records";
            tbGoToDisplayResultsForm.Click += delegate { PromptGoBack(); };
        }

        private readonly Regex _regex = new Regex(@"[ ]{2,}", RegexOptions.None);
        private Label label_Code;
        private TextBox textBox_Search;
        private Button button_Search;
        private DataGridView dataGridView_ICD;
        private Dictionary<string, string> dict_ICD = new Dictionary<string,string>();
        private BindingSource source = new BindingSource();

        private void InitializeComponent()
        {
            label_Code = new System.Windows.Forms.Label();
            textBox_Search = new System.Windows.Forms.TextBox();
            button_Search = new System.Windows.Forms.Button();
            dataGridView_ICD = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ICD)).BeginInit();
            // 
            // label_Code
            // 
            label_Code.AutoSize = true;
            label_Code.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label_Code.Location = new System.Drawing.Point(136, 18);
            label_Code.Name = "label_Code";
            label_Code.Size = new System.Drawing.Size(106, 13);
            label_Code.TabIndex = 0;
            label_Code.Text = "Code/Description";
            // 
            // textBox_Search
            // 
            textBox_Search.Location = new System.Drawing.Point(248, 15);
            textBox_Search.Name = "textBox_Search";
            textBox_Search.Size = new System.Drawing.Size(276, 20);
            textBox_Search.TabIndex = 1;
            // 
            // button_Search
            // 
            button_Search.Location = new System.Drawing.Point(530, 12);
            button_Search.Name = "button_Search";
            button_Search.Size = new System.Drawing.Size(75, 24);
            button_Search.TabIndex = 4;
            button_Search.Text = "Search";
            button_Search.UseVisualStyleBackColor = true;
           
            dataGridView_ICD.Location = new System.Drawing.Point(25, 57);
            dataGridView_ICD.Name = "dataGridView_ICD";
            dataGridView_ICD.Size = new Size(1100, 520);

            label_Code.Parent = Control;
            textBox_Search.Parent = Control;
            button_Search.Parent = Control;
            dataGridView_ICD.Parent = Control;

            readICD();

            button_Search.Click += new System.EventHandler(this.button_Search_Click);
        }

        private void readICD()
        {
            //new list to store the ICD
            List<string> icd = new List<string>();

            //read all lines from the text file and save them in a list
            var lines = File.ReadAllLines(@"icd10gm2014syst_edvascii_20130920.txt").ToList();
            List<char> accepted = new List<char> { '4', '5', '6' };

            //j = line number of the last "LZ" + 1
            int j = 0;
            //loop through the list of lines
            for (int i = 0; i < lines.Count; i++)
            {
                //"LZ" detected -> new records
                if (lines[i].Substring(0, 2).Equals("LZ"))
                {
                    //only add a new record to the list if it starts with 4, 5 or 6
                    if (accepted.Contains(lines[j][0]))
                    {
                        //get rid of the first 3 characters (e.g. "4T ")
                        string newRecord = lines[j].Substring(3);
                        //concatenate all lines of a record into one line
                        for (int a = j + 1; a < i; a++)
                        {
                            newRecord += " " + lines[a].Substring(3).Trim();
                        }
                        //add the new record into our list
                        icd.Add(newRecord);
                    }
                    //increase j by 1
                    j = i + 1;
                }
            }

            for (int a = 0; a < icd.Count; a++)
            {
                //get the code of disease from a record => from the beginning to the first blank space
                string code = icd[a].Substring(0, icd[a].IndexOf(' '));
                //get the description of disease from a record => it starts where the code ends
                string content = icd[a].Substring(code.Length);
                content = _regex.Replace(content, @" ").Trim();
                //add code & description to our data table
                dict_ICD.Add(code, content);
            }

            
            source.DataSource = dict_ICD;
            dataGridView_ICD.DataSource = source;

            dataGridView_ICD.Columns[1].Width = 1000;
        }

        private void button_Search_Click(object sender, EventArgs e)
        {
            //filter the records based on the search terms that the user types in.
            string term = textBox_Search.Text.Trim();

            source.DataSource = dict_ICD.Where(record => record.Key.Contains(term) || record.Value.Contains(term)); 
            dataGridView_ICD.DataSource = source;
            //set the current cell to the first cell
            dataGridView_ICD.CurrentCell = dataGridView_ICD.Rows[0].Cells[0];
        }

        protected override void OnShow(ShowEventArgs e)
        {
            base.OnShow(e);

            previousPlugin = e.PreviousPlugin;
        }

        protected string getSelected()
        {
            Dictionary<string, string> selected = new Dictionary<string, string>();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            Int32 selectedRowCount = dataGridView_ICD.Rows.GetRowCount(DataGridViewElementStates.Selected);
            
            if (selectedRowCount > 0)
            {
                for (int i = selectedRowCount - 1; i > -1; i--)
                {
                    sb.AppendLine(dataGridView_ICD.SelectedRows[i].Cells[0].Value.ToString());
                    sb.AppendLine(dataGridView_ICD.SelectedRows[i].Cells[1].Value.ToString());
                }
            }

            return sb.ToString();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override ToolButton[] GetToolButtons()
        {
            return new ToolButton[] { tbGoToDisplayResultsForm };
        }

        private void PromptGoBack()
        {
            ShowPlugin("Plugin1", getSelected());
        }

    }
}