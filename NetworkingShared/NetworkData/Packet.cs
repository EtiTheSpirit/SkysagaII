using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace NetworkingShared.NetworkData {
	public class Packet {

		protected byte VersionAndIHL;
		protected byte DSCPAndECN;
		protected ushort FlagsAndFragOffset;

		/// <summary>
		/// Returns the version as a big-endian byte value where only the right-most four bits are used.
		/// </summary>
		public byte Version {
			get {
				return (byte)((VersionAndIHL & 0xF0) >> 4);
			}
		}

		/// <summary>
		/// Returns the IHL as a big-endian byte value where only the right-most four bits are used.
		/// </summary>
		public byte IHL {
			get {
				return (byte)(VersionAndIHL & 0x0F);
			}
		}

		/// <summary>
		/// Returns the DCSP as a big-endian byte value where only the right-most six bits are used.
		/// </summary>
		public byte DCSP {
			get {
				return (byte)((DSCPAndECN & 0xFC) >> 2);
			}
		}

		/// <summary>
		/// Returns the ECN as a big-endian byte value where only the right-most two bits are used.
		/// </summary>
		public byte ECN {
			get {
				return (byte)(DSCPAndECN & 0x03);
			}
		}

		/// <summary>
		/// Returns the flags as a big-endian byte value where only the right-most three bits are used.
		/// If the packet is too big and must be fragmented, these flags will state whether or not the packet is fragmented.
		/// The left-most bit (MSB in big-endian) is always zero.
		/// </summary>
		public byte Flags {
			get {
				return (byte)((FlagsAndFragOffset & 0xE000) >> 14);
			}
		}

		/// <summary>
		/// Tells the exact position of the fragment in the original IP Packet.
		/// </summary>
		public short FragmentOffset {
			get {
				return (short)(FlagsAndFragOffset & 0x1FFF);
			}
		}

		/// <summary>
		/// The length of the data inside of this packet including the header and payload.
		/// </summary>
		public ushort Length { get; protected set; }

		/// <summary>
		/// If this packet is fragmented, all sub-packets part of a unified packet will share the same identification.
		/// </summary>
		public ushort Identification { get; protected set; }

		/// <summary>
		/// The amount of times this packet can tranfer across routers. It is decremented each time this packet is processed.
		/// </summary>
		public byte TTL { get; protected set; }

		/// <summary>
		/// Tells the Network layer at the destination host which Protocol this packet uses. TCP is 6 and UDP is 17, for quick reference.
		/// </summary>
		public byte Protocol { get; protected set; }

		/// <summary>
		/// Stores a checksum of the entire header, which is used to check if the packet was corrupted during travel.
		/// </summary>
		public ushort HeaderChecksum { get; protected set; }

		/// <summary>
		/// 32-bit address of the Sender (or source) of the packet.
		/// </summary>
		public uint SourceAddress { get; protected set; }

		/// <summary>
		/// 32-bit address of the Receiver (or destination) of the packet.
		/// </summary>
		public uint DestinationAddress { get; protected set; }

		/// <summary>
		/// This is an optional field. It is used if the value of IHL is greater than 5. It contains values for various information such as Security, Record Route, Time Stamp, etc.
		/// </summary>
		public uint? Options { get; protected set; } = null;

		/// <summary>
		/// The data stored within this packet.
		/// </summary>
		public byte[] Data { get; protected set; } = null;

		public Packet(byte[] packet) {
			if (packet.Length < 20) {
				throw new ArgumentOutOfRangeException("The length of the packet data is shorter than the required amount for a valid IPv4 packet header.");
			}

			VersionAndIHL = packet[0];
			DSCPAndECN = packet[1];
			Length = BitConverter.ToUInt16(packet, 2);
			Identification = BitConverter.ToUInt16(packet, 4);
			FlagsAndFragOffset = BitConverter.ToUInt16(packet, 6);
			TTL = packet[8];
			Protocol = packet[9];
			HeaderChecksum = BitConverter.ToUInt16(packet, 10);
			SourceAddress = BitConverter.ToUInt32(packet, 12);
			DestinationAddress = BitConverter.ToUInt32(packet, 16);

			if (IHL > 5) {
				Options = BitConverter.ToUInt32(packet, 20);
				Length += 4; //Is this violating the law? It probably is. If you see this, I forgot to remove it after doing stupid shit.
			}

			if (packet.Length != Length) {
				throw new ArgumentOutOfRangeException("The length flag of this packet (" + Length + ") does not match the actual length of the input byte array (" + packet.Length + ")!");
			}

			if (Flags == 3) {
				//Drop the packet. Has the DontFragment flag and MoreFragments active at the same time.
				throw new ArgumentOutOfRangeException("Header flags are malformed (DontFragment flag active and MoreFragments flag active at the same time)!");
			}

			int headerLen = Options != null ? 24 : 20;
			Data = new ArraySegment<byte>(packet, headerLen, Length - headerLen).ToArray();
		}

	}

	public enum PacketFlags {
		DontFragment = 1,
		MoreFragments = 2,
	}
}
