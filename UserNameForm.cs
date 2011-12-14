using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PrinterAutoRegister
{
    public partial class UserNameForm : Form
    {
        //レジストリに記述するUserNameキーの値
        public string userName { private set; get; }
        //レジストリにUserNameキーの記述を行う場合true 行わない場合false
        public bool flug { private set; get; }

        public UserNameForm()
        {
            InitializeComponent();
            flug = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            userName = textBox1.Text;
            if (!userName.Equals(""))
            {
                flug = true;
            }
            if (flug)
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string mes = "ユーザーIDが設定されません.\n印刷時に毎回ユーザーIDの入力が必要になります.\n\nこのまま進みますか？";
            string tit = "プリンター設定";
            if (DialogResult.OK == MessageBox.Show(mes, tit, MessageBoxButtons.OKCancel, MessageBoxIcon.Information,MessageBoxDefaultButton.Button2))
            {
                flug = false;
                this.Close();
            }
        }
    }
}
