﻿using System;
using System.IO;

namespace Client.World.Network
{
    public class OutPacket : BinaryWriter, Packet
    {
        public Header Header { get; private set; }

        private readonly MemoryStream Buffer;
        private byte[] FinalizedPacket;

        public OutPacket(WorldCommand command, int emptyOffset = 0)
            : base()
        {
            this.Header = new ClientHeader(command, this);

            Buffer = new MemoryStream();
            base.OutStream = Buffer;

            if (emptyOffset > 0)
                Write(new byte[emptyOffset]);
        }

        public byte[] Finalize(AuthenticationCrypto authenticationCrypto)
        {
            if (FinalizedPacket == null)
            {
                byte[] data = new byte[6 + Buffer.Length];
                byte[] size = ((ClientHeader)Header).EncryptedSize(authenticationCrypto);
                byte[] command = ((ClientHeader)Header).EncryptedCommand(authenticationCrypto);

                Array.Copy(size, 0, data, 0, 2);
                Array.Copy(command, 0, data, 2, 4);
                Array.Copy(Buffer.ToArray(), 0, data, 6, Buffer.Length);

                FinalizedPacket = data;
            }

            return FinalizedPacket;
        }

        public override string ToString()
        {
            return Header.Command.ToString();
        }
    }
}
