using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string Username = txtUser.Text;
            string Password = txtpassword.Text;
            string message = $"REGISTER|{Username}|{Password}";

            try
            {
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await Task.Factory.FromAsync(clientSocket.BeginConnect("127.0.0.1", 5000, null, null), clientSocket.EndConnect);

                byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
                await clientSocket.SendAsync(new ArraySegment<byte>(bytesToSend), SocketFlags.None);

                byte[] buffer = new byte[1024];
                int bytesRead = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (serverResponse.Length == 16 && serverResponse.All(char.IsLetterOrDigit))
                {
                    SerialKeyBox.Text = serverResponse;
                    MessageBox.Show("User registered successfully.\nSerial Key: " + serverResponse, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await Task.Delay(5000);

                    Home homeForm = new Home(Username, clientSocket);
                    homeForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show(serverResponse, "Server Response", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void showpass_CheckedChanged(object sender, EventArgs e)
        {
            txtpassword.PasswordChar = showpass.Checked ? '\0' : '*';
        }

        private void login_Click(object sender, EventArgs e)
        {
            LogIn loginForm = new LogIn();
            loginForm.Show();
            this.Hide();
        }

        private void SerialKeyBox_TextChanged(object sender, EventArgs e)
        {
            // פעולה במידת הצורך
        }
    }
}
