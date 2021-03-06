using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Microsoft.Singularity.Smtp
{
    public class Session
    {
        private Socket socket;
        private byte[] ibuf;
        private int iend;
        private int ibeg;
        private byte[] obuf;
        private int odata;
        private int result;
        private bool verbose;

        public Session(Socket socket)
        {
            this.socket = socket;
            this.ibuf = new byte [4096];
            this.ibeg = 0;
            this.iend = 0;
            this.obuf = new byte [4096];
            this.odata = 0;
            this.result = 0;
            this.verbose = true;
        }

        private int HaveLine()
        {
            int left = iend - ibeg;
            for (int i = 0; i + 1< left; i++) {
                if (ibuf[ibeg + i] == '\r' && ibuf[ibeg + i + 1] == '\n') {
                    // return the # of character (not \r\n) in line.
                    return i;
                }
            }
            return -1;
        }

        private int ReadLine()
        {
            for (;;) {
                if (ibeg < iend) {

                    // See if there is a line already in the buffer.
                    int line = HaveLine();
                    if (line >= 0) {
                        return line;
                    }

                    // Make sure the remaining data is at the start of buffer.
                    if (ibeg > 0) {
                        Array.Copy(ibuf, ibeg, ibuf, 0, iend - ibeg);
                        iend -= ibeg;
                        ibeg = 0;
                    }
                }
                else {
                    ibeg = 0;
                    iend = 0;
                }

                int read = socket.Receive(ibuf, iend, ibuf.Length - iend, SocketFlags.None);
                if (read == 0) {
                    // Should probably throw an exception.
                    return -1;
                }
                iend += read;
            }
        }

        public int Result
        {
            get { return result; }
        }

        public bool Verbose
        {
            get { return verbose; }
            set { verbose = value; }
        }

        public string ReadLine7()
        {
            result = 0;
            int line = ReadLine();
            if (line >= 0) {
                string data = Encoding.ASCII.GetString(ibuf, ibeg, line);
                for (int i = 0; i < line && ibuf[ibeg + i] >= '0' && ibuf[ibeg + i] <= '9'; i++) {
                    result = result * 10 + (ibuf[ibeg + i] - '0');
                }
                ibeg += line + 2;
                if (verbose) {
                    Console.WriteLine("S: {0}", data);
                }
                return data;
            }
            if (verbose) {
                Console.WriteLine("S: <end>");
            }
            return null;
        }

        public int ReadLine8(byte[] buffer, int offset)
        {
            result = 0;
            int line = ReadLine();
            if (line >= 0) {
                Array.Copy(ibuf, ibeg, buffer, offset, line);
                ibeg += line + 2;
                if (verbose) {
                    Console.WriteLine("S: <8bit>");
                }
                return line;
            }
            if (verbose) {
                Console.WriteLine("S: <end>");
            }
            return -1;
        }

        public void WritePrefix(byte prefix)
        {
            obuf[odata++] = prefix;
        }

        public void WriteLine7(string line)
        {
            WriteLine7(line, null, null);
        }

        public void WriteLine7(string a, string b)
        {
            WriteLine7(a, b, null);
        }

        public void WriteLine7(string a, string b, string c)
        {
            if (verbose) {
                Console.WriteLine("C: {0}", a + b + c);
            }
            if (a != null) {
                odata += Encoding.ASCII.GetBytes(a, 0, a.Length, obuf, odata);
            }
            if (b != null) {
                odata += Encoding.ASCII.GetBytes(b, 0, b.Length, obuf, odata);
            }
            if (c != null) {
                odata += Encoding.ASCII.GetBytes(c, 0, c.Length, obuf, odata);
            }
            obuf[odata++] = (byte)'\r';
            obuf[odata++] = (byte)'\n';
            socket.Send(obuf, 0, odata, SocketFlags.None);
            odata = 0;
        }

        public void WriteLine8(byte[] line)
        {
            if (verbose) {
                Console.WriteLine("C: <8bit>");
            }
            if (odata > 0) {
                socket.Send(obuf, 0, odata, SocketFlags.None);
                odata = 0;
            }
            socket.Send(line);
            obuf[odata++] = (byte)'\r';
            obuf[odata++] = (byte)'\n';
            socket.Send(obuf, 0, odata, SocketFlags.None);
            odata = 0;
        }

        public int Quit()
        {
            int save = result;
            WriteLine7("QUIT");
            ReadLine7();
            socket.Shutdown(SocketShutdown.Both);

            if (save == 0) {
                return 999;
            }
            return save;
        }

        public int Close()
        {
            int save = result;
            socket.Shutdown(SocketShutdown.Both);

            if (save == 0) {
                return 999;
            }
            return save;
        }

        private void Dump(byte[] array, int beg, int end)
        {
            for (int i = beg; i < end; i += 16) {
                Console.Write("      :: ");
                for (int j = i; j < i + 16; j++) {
                    if (j < end) {
                        Console.Write("{0:x2}", array[j]);
                    }
                    else {
                        Console.Write("  ");
                    }
                }
                Console.Write("      :: ");
                for (int j = i; j < i + 16; j++) {
                    if (j < end) {
                        if (array[j] >= 32 && array[j] < 127) {
                            Console.Write("{0}", (char)array[j]);
                        }
                        else {
                            Console.Write(".");
                        }
                    }
                    else {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }

        public void Dump()
        {
            Console.WriteLine(":::: Input {0}..{1}", ibeg, iend);
            Dump(ibuf, ibeg, iend);
            Console.WriteLine(":::: Output {0}..{1}", 0, odata);
            Dump(obuf, 0, odata);
            Console.WriteLine(":::: Result {0}", result);
        }
    }
}
