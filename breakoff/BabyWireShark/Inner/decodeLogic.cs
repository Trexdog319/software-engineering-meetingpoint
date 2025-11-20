// Link Types to look for:
//  1 - Ethernet
//  113 - Linux SSL
//  101 - RAW IP
//  105 - IEEE 802.11 WIFI
//  0 - BSD Loopback
// Network Layer - Layer 3
//  IPv4 - 0x0800
//  IPv6 - 0x86DD
//  ARP - 0x0806
// TCP - Protocol 6
//  20+ (variable)
//  HTTP, SSH, FTP
// UDP - Protocol 17
//  Header: 8 bytes
//  DNS, DHCP, Streaming, VoIP, GAMING!!
// ICMP - Protocol 1
//  ping, traceroute, error
// Application Layer - Layer 7
//  DNS - UDP/TCP port 53
//  HTTP - TCP port 80
//  HTTPS/TLS - TCP port 443
//  DHCP - UDP ports 67/68
//  SSH - TCP port 22

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Collections.Specialized;
using System.Buffers.Binary;


public class Packet
{
    // Fields
    public string sourceAddress { get; set; }
    public string destAddress { get; set; }
    public string protocol { get; set; }
    public string etherType { get; set; }
    public string timeSeconds { get; set; }
    public string timeMicroseconds { get; set; }
    public string packetSize { get; set; }
    public byte[] data { get; set; }
}

public class Wireshark
{
    public static void MainTest()
    {
        Dictionary<int, Packet> packetDictionary = new Dictionary<int, Packet>();
        using (FileStream pcapRead = new FileStream("..\\..\\..\\ipv4frags.pcap", FileMode.Open, FileAccess.Read))
        {
            // Read pcap header into byte array
            byte[] pcapHeader = new byte[24];
            pcapRead.Read(pcapHeader, 0, 24);

            // Determine linktype from pcap header (bytes 20-24)
            uint linkType = BinaryPrimitives.ReadUInt32LittleEndian(pcapHeader.AsSpan(20));

            int counter = 1;
            while (pcapRead.Position < pcapRead.Length)
            {
                Console.WriteLine($"Now analyzing packet no. {counter}");
                packetDecoder(pcapRead, packetDictionary, linkType, counter);

                counter++;
            }

            Console.WriteLine($"1st Packet Size: {packetDictionary[1].packetSize}");
            Console.WriteLine($"2nd Packet Size: {packetDictionary[2].packetSize}");
            Console.WriteLine($"3rd Packet Size: {packetDictionary[3].packetSize}");
        }
    }

    public static void packetDecoder(FileStream readIn, Dictionary<int, Packet> list, uint linkType, int counter)
    {
        Packet packet = new Packet();

        // Read per-packet header into byte array
        byte[] pcapPPHeader = new byte[16];
        readIn.Read(pcapPPHeader, 0, 16);
        ppHeaderDecode(pcapPPHeader, packet);

        // Determine link and packet type
        if (linkType == 1)          // Ethernet
        {
            Console.WriteLine("Linktype for incoming packet is 'Ethernet'.");

            byte[] frameHeader = new byte[14];
            readIn.Read(frameHeader, 0, 14);
            ushort etherTypeBuffer = BinaryPrimitives.ReadUInt16BigEndian(frameHeader.AsSpan(12));
            string etherType = etherTypeBuffer.ToString("X4");
            if (etherType == "0800")
            {
                Console.WriteLine("Packet is IPv4");

                packet.etherType = etherType;
                ipv4Decode(readIn, packet);

                Console.WriteLine("Source Address: " + packet.sourceAddress);
                Console.WriteLine("Destination Address: " + packet.destAddress);
                Console.WriteLine("Protocol: " + packet.protocol);
                Console.WriteLine("Ethertype: " + packet.etherType);
                Console.WriteLine("Time: " + packet.timeSeconds);
                Console.WriteLine("Microseconds: " + packet.timeMicroseconds);
                Console.WriteLine("Packet size: " + packet.packetSize + "\n");
                //Console.WriteLine("Data: " + BitConverter.ToString(packet.data));
            }
        }

        list.Add(counter, packet);
    }

    public static void ipv4Decode(FileStream readIn, Packet packet)
    {
        // Get first byte, extract version and length
        byte vLByte = (byte)readIn.ReadByte();
        int version = vLByte >> 4;
        int ihl = vLByte & 0x0F;

        // Trash TOS
        readIn.ReadByte();

        // Get total length
        byte[] totalLengthBytes = new byte[2];
        readIn.Read(totalLengthBytes, 0, 2);
        ushort totalLength = BinaryPrimitives.ReadUInt16BigEndian(totalLengthBytes);
        int payloadLength = totalLength - (ihl * 4);

        // Store bytes 4-19 in buffer
        byte[] bufferBytes = new byte[16];
        readIn.Read(bufferBytes, 0, 16);

        byte pProtocol = bufferBytes[5];   // protocol is byte 9 -> [5]
        uint pSourceAddress = BinaryPrimitives.ReadUInt32BigEndian(bufferBytes.AsSpan(8));
        uint pDestinationAddress = BinaryPrimitives.ReadUInt32BigEndian(bufferBytes.AsSpan(12));

        packet.protocol = pProtocol.ToString("X4");
        packet.sourceAddress = pSourceAddress.ToString("X4");
        packet.destAddress = pDestinationAddress.ToString("X4");

        //Console.WriteLine(pProtocol.ToString("X4"));
        //Console.WriteLine(pSourceAddress.ToString("X4"));
        //Console.WriteLine(pDestinationAddress.ToString("X4"));

        // Read data up to end up payloadLength
        byte[] pData = new byte[payloadLength];
        readIn.Read(pData, 0, payloadLength);

        packet.data = pData;
    }

    public static void ppHeaderDecode(byte[] ppHeader, Packet packet)
    {
        uint pTimeSeconds = BinaryPrimitives.ReadUInt32LittleEndian(ppHeader.AsSpan(0));
        uint pTimeMicroSeconds = BinaryPrimitives.ReadUInt32LittleEndian(ppHeader.AsSpan(4));
        uint pPacketSize = BinaryPrimitives.ReadUInt32LittleEndian(ppHeader.AsSpan(8));

        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime timestamp = epoch.AddSeconds(pTimeSeconds).AddTicks(pTimeMicroSeconds*10);
        //Console.WriteLine(timestamp);

        packet.timeSeconds = timestamp.ToString();
        packet.timeMicroseconds = pTimeMicroSeconds.ToString();
        packet.packetSize = pPacketSize.ToString();
    }
}

