using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BabyWireShark
{
    public partial class Form1 : Form
    {
        private string selectedPcapPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PCAP Files (*.pcap)|*.pcap";
            dialog.Title = "Select a PCAP File";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                selectedPcapPath = dialog.FileName;
                MessageBox.Show("File selected:\n" + selectedPcapPath);

                // Parse the pcap file using existing Wireshark class
                Dictionary<int, Packet> packets = new Dictionary<int, Packet>();
                
                using (FileStream pcapRead = new FileStream(selectedPcapPath, FileMode.Open, FileAccess.Read))
                {
                    byte[] pcapHeader = new byte[24];
                    pcapRead.Read(pcapHeader, 0, 24);
                    
                    uint linkType = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(pcapHeader.AsSpan(20));
                    
                    int counter = 1;
                    while (pcapRead.Position < pcapRead.Length)
                    {
                        Wireshark.packetDecoder(pcapRead, packets, linkType, counter);
                        counter++;
                    }
                }
                
                // Display in grid
                dataGridView1.Rows.Clear();
                foreach (var kvp in packets)
                {
                    dataGridView1.Rows.Add(
                        kvp.Key,
                        kvp.Value.timeSeconds,
                        kvp.Value.sourceAddress,
                        kvp.Value.destAddress,
                        kvp.Value.protocol,
                        kvp.Value.packetSize
                    );
                }
                
                MessageBox.Show($"Parsed {packets.Count} packets!");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "Basic Functionality:\n" +
            "- Open a .pcap file using the 'Open File' button.\n" +
            "- On file open, the backend will parse each packet.\n" +
            "- Human readable results will be converted from the .pcap hex data.\n",
            "Program Help",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
        }
    }
}
