using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp1
{
    public partial class Home : Form
    {
        private string currentUsername;
        private Socket clientSocket;

        public Home(string username, Socket existingSocket)
        {
            InitializeComponent();
            currentUsername = username;
            clientSocket = existingSocket;
            user.Text = currentUsername;
            ListenForNotifications();
        }

        private async void ListenForNotifications()
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int bytesRead = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // אם ההודעה היא בקשת חיבור (לדוגמה: "User Alice wants to talk to you.")
                        if (message.StartsWith("User") && message.Contains("wants to talk to you"))
                        {
                            DialogResult result = MessageBox.Show(message + "\nDo you accept the chat request?", "Chat Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                string fromUser = currentUsername;
                                string toUser = message.Split(' ')[1]; // לדוגמה מתוך "User Alice wants..."

                                string acceptMessage = $"ACCEPT|{fromUser}|{toUser}";
                                byte[] data = Encoding.UTF8.GetBytes(acceptMessage);
                                await clientSocket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
                            }
                        }
                        else if (message.StartsWith("ACCEPTED|"))
                        {
                            string otherUser = message.Split('|')[1];

                            this.Invoke((MethodInvoker)delegate {
                                chat chatForm = new chat(currentUsername, otherUser, clientSocket);  // אם צריך
                                chatForm.Show();
                                this.Hide();
                            });
                        }
                        else
                        {
                            // הודעות רגילות - תוכל להציג כמידע בלבד
                            MessageBox.Show(message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch
                {
                    // אם החיבור נסגר
                    break;
                }
            }
        }


        private void user_Click(object sender, EventArgs e)
        {
            // פעולה בלחיצה על שם המשתמש
        }

        private async void btnNotify_Click(object sender, EventArgs e)
        {
            string targetUsername = request.Text.Trim();

            if (string.IsNullOrEmpty(targetUsername))
            {
                MessageBox.Show("Please enter a username.");
                return;
            }

            string message = $"NOTIFY|{currentUsername}|{targetUsername}";
            byte[] data = Encoding.UTF8.GetBytes(message);

            try
            {
                await clientSocket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send notification: " + ex.Message);
            }
        }
    }
}
