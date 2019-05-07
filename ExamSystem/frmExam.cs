using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExamSystem.Services;
using ExamSystem.Models;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace ExamSystem
{
    public partial class frmExam : Form
    {

        QuestionService _QuestionService = new QuestionService();
        CategoryService _CategoryService = new CategoryService();
        ChoiseService _ChoiseService = new ChoiseService();
        /// <summary>
        /// 题目(当前)
        /// </summary>
        private QUESTION _question;
        /// <summary>
        /// 题目类别(当前)
        /// </summary>
        private CATEGORY _category;
        /// <summary>
        /// 当前题目索引
        /// </summary>
        private int questionIndex = 0;
        /// <summary>
        /// 所有题目类别
        /// </summary>
        List<CATEGORY> AllCategorys;
        /// <summary>
        /// 所有题目
        /// </summary>
        private List<QUESTION> AllQuestions;
        List<QUESTION> nowQuestions;
        /// <summary>
        /// 用户错题
        /// </summary>
        List<QUESTION> userErrorQuestion = new List<QUESTION>();
        /// <summary>
        /// 选择
        /// </summary>
        Dictionary<int, string> dicOption;
        /// <summary>
        /// 答案
        /// </summary>
        Dictionary<int, string> dicAnswer;
        public frmExam()
        {
            InitializeComponent();
            AllQuestions = _QuestionService.GetList().ToList();

            AllCategorys = _CategoryService.GetList().ToList() ?? new List<CATEGORY>();
            foreach (CATEGORY item in AllCategorys)
            {
                ToolStripMenuItem tsmBtn = new ToolStripMenuItem();
                tsmBtn.Text = item.Name;
                tsmBtn.Tag = item;
                tsmBtn.Click += tsmBtnGetCategory_Click;
                tsmBtnGetCategory.DropDownItems.Add(tsmBtn);
            }

            LoadErrorCategorys();
        }

        void LoadErrorCategorys()
        {
            tsmBtnGetErrorCategory.DropDownItems.Clear();
            foreach (ErrorQUESTION item in Method.Serialize<ErrorQUESTION>.ReserializeMethod("Backup\\userErrorQuestion.btn") ?? new List<ErrorQUESTION>())
            {
                if (!userErrorQuestion.Contains(AllQuestions.Where(aq => aq.Id == item.Id).First()))
                {
                    userErrorQuestion.Add(AllQuestions.Where(aq => aq.Id == item.Id).First());
                }
            }
            List<QUESTION> tempErrorQuestions = userErrorQuestion.Distinct(new QUESTION()).ToList();
            tempErrorQuestions.Sort();
            foreach (CATEGORY item in GetAllErrorCategorys(tempErrorQuestions))
            {
                ToolStripMenuItem tsmBtn = new ToolStripMenuItem();
                tsmBtn.Text = item.Name;
                tsmBtn.Tag = item;
                tsmBtn.Click += tsmBtnGetErrorCategory_Click;
                tsmBtnGetErrorCategory.DropDownItems.Add(tsmBtn);
            }
        }

        /// <summary>
        /// 获取所有错题对应类别
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        List<CATEGORY> GetAllErrorCategorys(List<QUESTION> questions)
        {
            List<CATEGORY> categories = new List<CATEGORY>();
            foreach (var item in questions)
            {
                categories.Add(item.Category);
            }
            return categories;
        }

        private void frmExam_Load(object sender, EventArgs e)
        {
            GetCategory(Convert.ToInt32(Properties.Settings.Default["CategoryID"]), AllQuestions);
        }

        /// <summary>
        /// 绑定(刷新)界面数据
        /// </summary>
        /// <param name="question"></param>
        public void BindFormData(QUESTION question)
        {
            foreach (var item in dicOption.Where(d => d.Value != string.Empty && d.Key != questionIndex))
            {
                panelQuestions.Controls[Convert.ToInt32(item.Key)].BackColor = Color.Yellow;
            }
            foreach (var item in dicOption.Where(d => !(d.Value != string.Empty && d.Key != questionIndex)))
            {
                panelQuestions.Controls[Convert.ToInt32(item.Key)].BackColor = Color.FromArgb(225, 225, 225);
            }

            txtQuestion.Text = txtChoise.Text = string.Empty;
            panelOption.Controls.Clear();

            txtQuestion.Text = $"{questionIndex + 1}: {question.Name}";

            for (int i = 0; i < question.Choises.Count; i++)
            {
                string choiseIndex = Convert.ToChar('A' + i).ToString();
                txtChoise.AppendText($"{choiseIndex}:{question.Choises[i].Name}\r\n");
                /*是否存在多选(保留)*/

                RadioButton radio = new RadioButton() { Text = choiseIndex, Width = 30 };
                radio.CheckedChanged += RdoOrCheckButtonOption__CheckedChanged;
                panelOption.Controls.Add(radio);

                if (!string.IsNullOrWhiteSpace(dicOption[questionIndex]))
                {
                    if (dicOption[questionIndex].Substring(0, 1) == choiseIndex)
                    {
                        radio.Checked = true;
                    }

                    /*多选*/
                    //string[] strTemp = dicOption[questionIndex].Split('、');
                    //for (int j = 0; j < strTemp.Length; j++)
                    //{
                    //    if (strTemp[j].Substring(0,1)==choiseIndex)
                    //    {
                    //        radio.Checked = true;
                    //    }
                    //} 
                }
            }
        }

        /// <summary>
        /// 题目按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuestion_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            questionIndex = Convert.ToInt32(btn.Text) - 1;
            _question = btn.Tag as QUESTION;
            BindFormData(_question);
            btn.BackColor = Color.Aqua; //当前选中颜色
        }

        private void tsmBtnGetCategory_Click(object sender, EventArgs e)
        {
            questionIndex = 0;
            ToolStripMenuItem tsmBtn = sender as ToolStripMenuItem;
            _category = tsmBtn.Tag as CATEGORY;
            GetCategory(_category.Id, AllQuestions);
        }
        private void tsmBtnGetErrorCategory_Click(object sender, EventArgs e)
        {
            questionIndex = 0;
            ToolStripMenuItem tsmBtn = sender as ToolStripMenuItem;
            _category = tsmBtn.Tag as CATEGORY;
            GetCategory(_category.Id, userErrorQuestion);
        }
        /// <summary>
        /// 获取类别
        /// </summary>
        /// <param name="categoryID">类别ID</param>
        /// <param name="nQuestions">题目集合</param>
        void GetCategory(int categoryID, List<QUESTION> nQuestions)
        {
            nowQuestions = nQuestions.Where(q => q.CategoryId == categoryID && q.Choises.Count > 0).ToList() ?? new List<QUESTION>();
            //if (questions.Count < 75)
            //    MessageBox.Show("题库数量不足75,请及时补充题库。");
            if (nowQuestions.Count > 0)
            {
                dicOption = new Dictionary<int, string>();
                dicAnswer = new Dictionary<int, string>();
                int tempCount = nowQuestions.Count;
                for (int i = 0; i < tempCount; i++)
                {
                    int tempInt = new Random().Next(0, nowQuestions.Count);

                    if (panelQuestions.Controls.Count >= i + 1)
                    {
                        panelQuestions.Controls[i].Tag = nowQuestions[tempInt];
                    }
                    else
                    {
                        Button btn = new Button();
                        btn.Size = new Size(45, 45);
                        btn.Text = (i + 1).ToString();
                        btn.Tag = nowQuestions[tempInt];
                        btn.Font = new Font(Font.FontFamily, 17);
                        btn.Click += btnQuestion_Click;
                        panelQuestions.Controls.Add(btn);
                    }
                    dicOption.Add(i, "");
                    dicAnswer.Add(i, nowQuestions[tempInt].Answer);
                    nowQuestions.Remove(nowQuestions[tempInt]);

                    if (i >= 74)
                        break;
                }
                if (panelQuestions.Controls.Count!=dicAnswer.Count)
                {
                    for (int i = panelQuestions.Controls.Count; i > dicAnswer.Count; i--)
                    {
                        panelQuestions.Controls.RemoveAt(i - 1);
                    }
                }
            }

            foreach (Control control in panelQuestions.Controls)
            {
                if (control.Text != "1")
                {
                    continue;
                }
                _question = (QUESTION)control.Tag;
                control.BackColor = Color.Aqua;
                this.Text = $"咸鱼考试 -- {_question.Category.Name}";
                BindFormData(_question);
                break;
            }
        }

        private void RdoOrCheckButtonOption__CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton)
            {
                RadioButton radio = (RadioButton)sender;
                dicOption[questionIndex] = radio.Text + $":{_question.Choises[radio.TabIndex].Name}";
            }
            else if (sender is CheckBox)
            {
                string strGroup = "";
                dicOption[questionIndex] = "";
                foreach (Control control in panelOption.Controls)
                {
                    CheckBox checkBox = (CheckBox)control;
                    if (checkBox.Checked)
                    {
                        dicOption[questionIndex] += $"{strGroup + checkBox.Text}:{_question.Choises[checkBox.TabIndex].Name}";
                        strGroup = "、";
                    }
                }
            }
        }

        public void ChangeAnswer(RadioButton radioButton)
        {
            if (radioButton.Checked)
            {
                var chiose = _question.Choises.Where(o => o.Name == radioButton.Text.Split(':')[1].Trim()).FirstOrDefault();
                if (chiose != null)
                {
                    chiose.IsCheck = 0;
                    var list = _question.Choises.Where(o => o.Id != chiose.Id).ToList();
                    list.ForEach(o => o.IsCheck = 1);
                    _ChoiseService.Update(chiose);
                    _ChoiseService.Update(list);
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //questionIndex++;
            //if (questionIndex >= AllQuestions.Count)
            //{
            //    questionIndex = 0;
            //    _question = AllQuestions[questionIndex];
            //    BindFormData(_question);
            //    //MessageBox.Show("已经最后一题呢。", "考试提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //else
            //{
            //    _question = AllQuestions[questionIndex];
            //    BindFormData(_question);
            //}
            questionIndex++;
            if (questionIndex == dicAnswer.Count)
            {
                questionIndex = 0;
            }
            Method.MyReflection.CallObjectEvent((Button)panelQuestions.Controls[questionIndex], "OnClick");
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            questionIndex--;
            if (questionIndex == -1)
            {
                questionIndex = dicAnswer.Count - 1;
            }
            Method.MyReflection.CallObjectEvent((Button)panelQuestions.Controls[questionIndex], "OnClick");
            //if (questionIndex >= AllQuestions.Count)
            //{
            //    questionIndex = 0;
            //    _question = AllQuestions[questionIndex];
            //    BindFormData(_question);
            //}
            //else
            //{
            //    questionIndex--;
            //    if (questionIndex < 0)
            //    {
            //        questionIndex = AllQuestions.Count - 1;
            //        _question = AllQuestions[questionIndex];
            //        BindFormData(_question);
            //    }
            //    else
            //    {
            //        _question = AllQuestions[questionIndex];
            //        BindFormData(_question);
            //    }
            //}
        }

        private void FrmExam_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_category != null)
            {
                Properties.Settings.Default["CategoryID"] = _category.Id;
                Properties.Settings.Default.Save();
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (nowQuestions != null)
            {
                int errorCount = 0;
                string strGroup = "";
                string errorQuestion = "";

                for (int i = 0; i < dicAnswer.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(dicOption[i]/*.Substring(2)*/))
                    {
                        MessageBox.Show($"题{i + 1}未答,请确认所有题目确认无误后再交卷!");
                        Method.MyReflection.CallObjectEvent((Button)panelQuestions.Controls[i], "OnClick");
                        return;     //此处如果是break的话还会接着继续对比后面的答案
                    }
                    if (dicOption[i].Substring(2) != dicAnswer[i])
                    {
                        errorQuestion += strGroup + $"{i + 1}";
                        strGroup = "、";
                        errorCount++;
                        foreach (Control control in panelQuestions.Controls)
                        {
                            if (control.Text != $"{i + 1 }")
                                continue;
                            else
                            {
                                Button btn = (Button)control;
                                btn.BackColor = Color.Red;
                                if (!userErrorQuestion.Contains((QUESTION)btn.Tag))
                                {
                                    userErrorQuestion.Add((QUESTION)btn.Tag);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        userErrorQuestion.Remove(panelQuestions.Controls[i].Tag as QUESTION);
                    }
                }
                List<ErrorQUESTION> errQuestions = new List<ErrorQUESTION>();
                if (userErrorQuestion.Count > 0)
                {
                    foreach (QUESTION item in userErrorQuestion)
                    {
                        errQuestions.Add(new ErrorQUESTION()
                        {
                            Answer = item.Answer,
                            CategoryId = item.CategoryId,
                            Id = item.Id,
                            Name = item.Name,
                            Category = item.Category,
                            Choises = item.Choises
                        });
                    }
                }

                Method.Serialize<ErrorQUESTION>.SerializeMethod(errQuestions, "Backup\\userErrorQuestion.btn");

                if (errorCount == 0)
                {
                    MessageBox.Show("满分!");
                }
                else
                {
                    MessageBox.Show($"错误题:{errorQuestion},共错了{errorCount}题!");
                }
                LoadErrorCategorys();
            }
        }
    }
}
