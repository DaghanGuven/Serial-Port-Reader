using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Serial_Port_Reader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private SerialPort serialPort;
        private string currentPort = "";
        private string lastPort = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(sender, e);
            comboBox2.Text = "9600";
        }
        private ArrayList PortNames = new ArrayList();
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "Refreshing...";

            comboBox1.Items.Clear();

            PortNames = ArrayList.Adapter(SerialPort.GetPortNames());

            for (int j = 0; j < PortNames.Count; j++)
            {
                comboBox1.Items.Add(PortNames[j]);
            }

            PortNames = new ArrayList();

            button1.Enabled = true;
            button1.Text = "Refresh";
            comboBox1.Text = comboBox1.Items.Contains(lastPort) ? lastPort : comboBox1.Items.Count == 0 ? "" : comboBox1.Items[0].ToString();
        }
        private async void timer1_Tick(object sender, EventArgs e)
        {
            await ReadFromSerialPortAsync(currentPort);
        }

        private async Task ReadFromSerialPortAsync(string portName)
        {
            serialPort = new SerialPort(portName, baudRate: Convert.ToInt32(comboBox2.Text));
            serialPort.ReadTimeout = 5000;

            try
            {
                serialPort.Open();
                string result = await ReadLineAsync(serialPort);
                richTextBox1.Text = result + "\n"+richTextBox1.Text;
            }
            catch (Exception ex)
            {
                richTextBox1.Text = portName + ": " + ex.Message+"\n"+richTextBox1.Text;
            }
            finally
            {
                serialPort.Close();
            }
        }

        private async Task<string> ReadLineAsync(SerialPort serialPort)
        {
            using (StreamReader reader = new StreamReader(serialPort.BaseStream))
            {
                try
                {
                    return await reader.ReadLineAsync();
                }
                catch (TimeoutException)
                {
                    return null;
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                currentPort = comboBox1.Text;
                button3.Text = "STOP";
                button3.BackColor = System.Drawing.Color.FromArgb(192, 0, 0);
                button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
                button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(255, 128, 128);
                button3.Click += button3___Click;
                button3.Click -= button3_Click;
                timer1.Start();
            }
            else
            {
                MessageBox.Show("Please select a port.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3___Click(object sender, EventArgs e)
        {
            button3.Text = "START";
            button3.BackColor = System.Drawing.Color.FromArgb(0, 192, 0);
            button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(128, 255, 128);
            button3.Click += button3_Click;
            button3.Click -= button3___Click;
            timer1.Stop();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            lastPort = comboBox1.Text == "" ? lastPort : comboBox1.Text;
        }
    }
}
