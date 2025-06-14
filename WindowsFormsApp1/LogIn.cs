using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void X_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void showpass_CheckedChanged(object sender, EventArgs e)
        {
            txtpassword.PasswordChar = showpass.Checked ? '\0' : '*';
        }

        private void register_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void X_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string Username = txtUser.Text.Trim();
            string Password = txtpassword.Text.Trim();
            string message = $"LOGIN|{Username}|{Password}";

            try
            {
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await clientSocket.ConnectAsync(IPAddress.Loopback, 5000);

                byte[] dataToSend = Encoding.UTF8.GetBytes(message);
                await clientSocket.SendAsync(new ArraySegment<byte>(dataToSend), SocketFlags.None);

                byte[] buffer = new byte[1024];
                int bytesRead = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (serverResponse.Length == 16 && serverResponse.All(char.IsLetterOrDigit))
                {
                    txtKey.Text = serverResponse;
                    MessageBox.Show("Login successful.\nYour Serial Key: " + serverResponse, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await Task.Delay(5000);
                    Home homeForm = new Home(Username, clientSocket);
                    homeForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show(serverResponse, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            // פעולה במידת הצורך
        }
    }
}
