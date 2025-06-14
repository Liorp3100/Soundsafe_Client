using NAudio.Wave; // Add this for your recording functionality
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class chat : Form
    {

        private Socket clientSocket; // Your existing socket connection
        private Thread receiveThread; // Thread for receiving messages
        private bool isConnected = true;

        // Your existing recording variables
        private WaveInEvent waveIn;
        private WaveFileWriter writer;
        private bool isRecording = false;
        private string outputFilePath = "C:\\Users\\Yonatan-PC\\Desktop\\recorded.wav"; // Your output file path

        public chat(string username, string otherUser, Socket socket)

        {
            InitializeComponent();
            this.clientSocket = socket;
            receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        // Your existing recording methods
        private void recorder1_Click(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                StartRecording();
                txtrec.Text = "Stop Recording";
            }
            else
            {
                StopRecording();
                txtrec.Text = "Start Recording";
            }
        }

        private void StartRecording()
        {
            waveIn = new WaveInEvent();
            writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
            waveIn.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
            };
            waveIn.StartRecording();
            isRecording = true;
        }

        private void StopRecording()
        {
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
            isRecording = false;
            txtMessage.Text = outputFilePath;
            opener.Text = "Open";
        }
        // Add this to your form initialization
        private void InitializeClient()
        {
            // Start receiving thread (if not already started)
            /*if (receiveThread == null || !receiveThread.IsAlive)
            {
                receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }*/
        }

        private void opener_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = outputFilePath,
                    UseShellExecute = true // Important for opening with default program
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening file: " + ex.Message);
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Your btnSend click event - sends the recorded file
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if there's a recorded file to send
                if (string.IsNullOrEmpty(outputFilePath) || !File.Exists(outputFilePath))
                {
                    MessageBox.Show("No recorded file found. Please record audio first.", "No File",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if currently recording
                if (isRecording)
                {
                    MessageBox.Show("Please stop recording before sending the file.", "Recording in Progress",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Send the recorded file
                SendWavFile(outputFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendWavFile(string filePath)
        {
            try
            {
                if (clientSocket == null || !clientSocket.Connected)
                {
                    MessageBox.Show("Not connected to server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Read the WAV file
                byte[] wavFileData = File.ReadAllBytes(filePath);

                // Send the action command first
                string action = "WAV_FILE|";
                byte[] actionBytes = Encoding.UTF8.GetBytes(action);
                clientSocket.Send(actionBytes);

                // Small delay to ensure the action is processed
                Thread.Sleep(100);

                // Send file size (4 bytes)
                byte[] sizeBytes = BitConverter.GetBytes(wavFileData.Length);
                clientSocket.Send(sizeBytes);

                // Send the actual WAV file data
                int totalSent = 0;
                int chunkSize = 8192; // Send in 8KB chunks

                while (totalSent < wavFileData.Length)
                {
                    int remainingBytes = wavFileData.Length - totalSent;
                    int bytesToSend = Math.Min(chunkSize, remainingBytes);

                    int sent = clientSocket.Send(wavFileData, totalSent, bytesToSend, SocketFlags.None);
                    totalSent += sent;
                }

                // Update UI on main thread
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Recorded audio sent successfully! ({wavFileData.Length} bytes)", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Error sending WAV file: {ex.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        // Thread for receiving messages from server
        // Thread for receiving messages from server
        // Better approach - Use Console.WriteLine for debugging instead of blocking MessageBox
        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];

            try
            {
             
                while (isConnected && clientSocket != null && clientSocket.Connected)
                {
                  
                    int received = clientSocket.Receive(buffer);

                    // Check if server closed the connection
                    if (received == 0)
                    {
                        MessageBox.Show("Server closed the connection.");
                        this.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show("Server closed the connection.");
                        });
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, received);
                    MessageBox.Show($"Received message: {message}");

                    // Handle different types of messages
                    if (message == "WAV_INCOMING")
                    {
                        MessageBox.Show("Receiving WAV file...");

                        ReceiveWavFile(); // This stays outside Invoke since it does socket operations

                        MessageBox.Show("WAV file received successfully.");
                    }
                    else if (message.StartsWith("ACCEPTED|"))
                    {
                        HandleAcceptedMessage(message);
                    }
                    else
                    {
                        HandleOtherMessages(message);
                    }
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Socket error: {ex.Message}");
                if (isConnected)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Connection lost: {ex.Message}", "Connection Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
            }
            catch (ObjectDisposedException)
            {
                MessageBox.Show("Socket was disposed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error in ReceiveMessages: {ex.Message}");
                if (isConnected)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Unexpected error: {ex.Message}", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            }
            finally
            {
                isConnected = false;
                MessageBox.Show("ReceiveMessages thread ended.");
            }
        }

        // Better debugging version of ReceiveWavFile method
        private void ReceiveWavFile()
        {
            try
            {
                MessageBox.Show("=== ENTERING ReceiveWavFile ===");

                // Receive file size (4 bytes)
                byte[] sizeBuffer = new byte[4];
                int totalReceived = 0;

                MessageBox.Show("About to receive file size...");
                while (totalReceived < 4)
                {
                    MessageBox.Show($"Receiving size bytes: {totalReceived}/4");
                    int received = clientSocket.Receive(sizeBuffer, totalReceived, 4 - totalReceived, SocketFlags.None);
                    if (received == 0)
                    {
                        MessageBox.Show("❌ Connection closed while receiving file size");
                        return;
                    }
                    totalReceived += received;
                    MessageBox.Show($"Size progress: {totalReceived}/4 bytes");
                }

                int fileSize = BitConverter.ToInt32(sizeBuffer, 0);
                MessageBox.Show($"✅ File size received: {fileSize} bytes");

                // Validate file size
                

                // Receive the WAV file data
                byte[] wavData = new byte[fileSize];
                totalReceived = 0;

                MessageBox.Show($"About to receive {fileSize} bytes of WAV data...");

                while (totalReceived < fileSize)
                {
                    MessageBox.Show($"📡 Receiving WAV chunk: {totalReceived}/{fileSize} bytes");

                    // Calculate how much to receive in this iteration
                    int remainingBytes = fileSize - totalReceived;
                    int bytesToReceive = Math.Min(remainingBytes, 8192); // Max 8KB chunks

                    MessageBox.Show($"Attempting to receive {bytesToReceive} bytes...");

                    int received = clientSocket.Receive(wavData, totalReceived, bytesToReceive, SocketFlags.None);

                    MessageBox.Show($"Actually received: {received} bytes");

                    if (received == 0)
                    {
                        MessageBox.Show($"❌ Connection closed! Only received {totalReceived}/{fileSize} bytes");
                        return;
                    }

                    totalReceived += received;
                    MessageBox.Show($"✅ Progress: {totalReceived}/{fileSize} bytes ({(totalReceived * 100.0 / fileSize):F1}%)");

                    // Check if we somehow got more data than expected
                    if (totalReceived > fileSize)
                    {
                        MessageBox.Show($"❌ ERROR: Received more data than expected! {totalReceived} > {fileSize}");
                        return;
                    }
                }

                // 🎯 THIS IS THE MESSAGE YOU'RE NOT SEEING:
                MessageBox.Show($"🎉 SUCCESS! Received complete WAV file: {totalReceived} bytes");

                // Save the file
                try
                {
                    string fileName = "received_wav.wav";
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string savePath = Path.Combine(documentsPath, "ReceivedWavFiles");

                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                        MessageBox.Show("Created directory: " + savePath);
                    }

                    string fullPath = Path.Combine(savePath, fileName);
                    File.WriteAllBytes(fullPath, wavData);

                    MessageBox.Show($"✅ WAV file saved to:\n{fullPath}\n\nFile size: {wavData.Length} bytes");
                }
                catch (Exception saveEx)
                {
                    MessageBox.Show($"❌ Error saving file: {saveEx.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ EXCEPTION in ReceiveWavFile: {ex.Message}\n\nStack trace:\n{ex.StackTrace}");
            }
        }

        private void SaveReceivedWavFile(byte[] wavData)
        {
            // Move everything inside Invoke to ensure it runs on UI thread
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    MessageBox.Show("you are in SaveReceivedWavFile");

                    string fileName = "received_wav.wav";
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string savePath = Path.Combine(documentsPath, "ReceivedWavFiles");

                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }

                    string fullPath = Path.Combine(savePath, fileName);
                    File.WriteAllBytes(fullPath, wavData);

                    MessageBox.Show($"Received WAV file saved to:\n{fullPath}\n\nSize: {wavData.Length} bytes",
                                  "WAV File Received", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving received WAV file: {ex.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        // Handle your existing ACCEPTED messages
        private void HandleAcceptedMessage(string message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    string[] parts = message.Split('|');
                    if (parts.Length == 2)
                    {
                        string acceptedUser = parts[1];

                        // Show notification that connection was accepted
                        MessageBox.Show($"Connection accepted by {acceptedUser}!\nYou can now send audio messages.",
                                      "Connection Accepted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Update UI to show connected status
                        // You can customize these based on your actual UI controls
                        if (this.Controls.ContainsKey("lblStatus"))
                        {
                            ((Label)this.Controls["lblStatus"]).Text = $"Connected to: {acceptedUser}";
                            ((Label)this.Controls["lblStatus"]).ForeColor = System.Drawing.Color.Green;
                        }

                        // Enable the send button since connection is established
                        if (btnSend != null)
                        {
                            btnSend.Enabled = true;
                            btnSend.Text = "Send Recording";
                        }

                        // Enable the recorder button
                        if (this.Controls.ContainsKey("recorder1"))
                        {
                            ((Button)this.Controls["recorder1"]).Enabled = true;
                        }

                        // Update any status text or log
                        if (this.Controls.ContainsKey("txtMessage"))
                        {
                            ((TextBox)this.Controls["txtMessage"]).Text += $"\r\n[{DateTime.Now:HH:mm:ss}] Connected to {acceptedUser}";
                        }

                        // You can also add the accepted user to a list if you have one
                        // For example, if you have a ListBox for connected users:
                        // if (this.Controls.ContainsKey("lstConnectedUsers"))
                        // {
                        //     ListBox lstUsers = (ListBox)this.Controls["lstConnectedUsers"];
                        //     if (!lstUsers.Items.Contains(acceptedUser))
                        //     {
                        //         lstUsers.Items.Add(acceptedUser);
                        //     }
                        // }
                    }
                    else
                    {
                        // Invalid message format
                        MessageBox.Show("Received invalid ACCEPTED message format.", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error handling ACCEPTED message: {ex.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        // Handle other messages
        private void HandleOtherMessages(string message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // Your existing message handling code
                // For example, updating a listbox or textbox with the message
            });
        }

        // Call this when you establish connection to server
        private void ConnectToServer()
        {
            try
            {
                // Your existing connection code
                // clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // clientSocket.Connect("your_server_ip", 5000);

                isConnected = true;
                InitializeClient(); // Start the receive thread
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Call this when disconnecting
        private void DisconnectFromServer()
        {
            try
            {
                isConnected = false;

                if (clientSocket != null && clientSocket.Connected)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle disconnect error if needed
            }
        }

        // Form closing event
        private void YourFormName_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectFromServer();
        }
    }
}