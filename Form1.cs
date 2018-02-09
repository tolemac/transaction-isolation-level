using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace IsolationLevel
{
    public partial class Form1 : Form
    {
        private string _connectionString =
            @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=IsolationTest;Integrated Security=True";

        private SqlConnection _conn;
        private SqlTransaction _trans;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var connectionString =
                @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True";
            var con = new SqlConnection(connectionString);
            DropAndCreate.DoIt(con);
            con.Close();
            con.Dispose();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _trans = _conn.BeginTransaction(GetIsolationLevel());
        }

        private System.Data.IsolationLevel GetIsolationLevel()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    return System.Data.IsolationLevel.ReadUncommitted;
                case 1:
                    return System.Data.IsolationLevel.ReadCommitted;
                case 2:
                    return System.Data.IsolationLevel.RepeatableRead;
                case 3:
                    return System.Data.IsolationLevel.Serializable;
                case 4:
                    return System.Data.IsolationLevel.Snapshot;
                case 5:
                    return System.Data.IsolationLevel.Chaos;

            }
            throw new Exception("isolation ???");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
            }
            _conn = new SqlConnection(_connectionString);
            _conn.Open();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _trans.Commit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _trans.Rollback();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var queryText = "select * from dbo.testtable where id = {0}";
            var cmd = _conn.CreateCommand();
            cmd.Transaction = _trans;
            var i = 1;
            var done = false;
            while (!done)
            {
                cmd.CommandText = string.Format(queryText, i++);
                var result = cmd.ExecuteReader();
                if (!result.HasRows)
                {
                    done = true;
                }
                else
                {
                    result.Read();
                    Log($"{result.GetInt64(0)} {result.GetInt64(1)}");
                }
                result.Close();
            }
        }

        private void Log(string s)
        {
            textBox1.AppendText(s);
            textBox1.AppendText(Environment.NewLine);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var wnd = new Form1();
            wnd.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var queryText = "update dbo.testtable set number = number + 1 where id = 1";
            var cmd = _conn.CreateCommand();
            cmd.Transaction = _trans;
            cmd.CommandText = queryText;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
}