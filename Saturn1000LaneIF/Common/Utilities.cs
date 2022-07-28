using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vjp.Saturn1000LaneIF.Common
{
    public class Utilities
    {
        private static Dictionary<string, string> iniDict;
        public static Dictionary<string, string> INIDict {
            get
            {
                if (iniDict == null) {
                    if (!File.Exists("config.ini"))
                    {
                        return new Dictionary<string, string>();
                    }
                    else
                    {
                        string[] lines = File.ReadAllLines("config.ini");
                        return (from entry in lines
                                let key = entry.Substring(0, entry.IndexOf("="))
                                let value = entry.Substring(entry.IndexOf("=") + 1)
                                select new { key, value }).ToDictionary(e => e.key, e => e.value);
                    }
                }
                else
                    return iniDict;
            }
            set { iniDict = value; }
        }

        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum ErrorType
        {
            SUCCESS = 0,
            UNEXPECTED_ERROR = -1,
            CONNECTION_ERROR = 1,
            TIMEOUT_ERROR = 2,
            PORTNAME_ERROR = 3,
        }

        public enum MARKED_BYTES
        {
            STX = 0x02,
            ETX = 0x03,
            ETB = 0x17,
            EOT = 0x04,
            ACK = 0x06,
            NAK = 0x15
        }

        // STX,ETX,Grant of BCC
        public static byte[] CreateMessage(byte[] msgArray, bool flg) {
            // STX
            byte[] startbit = { (byte)MARKED_BYTES.STX };
            // ETB
            byte[] endbit = { (byte)MARKED_BYTES.ETB };
            // BCC
            byte[] bcc = new byte[1];
            if (flg) {
                // ETX is given for the final message
                endbit[0] = (byte)MARKED_BYTES.ETX;
            }
            //BCC calculation
            msgArray = Enumerable.Concat(startbit, msgArray).ToArray();
            msgArray = Enumerable.Concat(msgArray, endbit).ToArray();
            bcc[0] = Convert.ToByte(CreateBcc(msgArray));
            msgArray = Enumerable.Concat(msgArray, bcc).ToArray();
            return msgArray;
        }

        // BCC creation
        public static byte CreateBcc(byte[] msg)
        {
            byte bcc = 0;
            for (int i = 0; i < msg.Length; i++)
            {
                bcc = (byte)(bcc ^ msg[i]);
            }
            return bcc;
        }
        
        public static byte CheckMessage(byte[] msgArray, byte receiveBcc)
        {
            byte buff;
            byte checkbcc = CreateBcc(msgArray);
            // BCC check
            if (receiveBcc == checkbcc)
            {
                // ACK
                buff = (byte)MARKED_BYTES.ACK;
            }
            else
            {
                // NAK
                buff = (byte)MARKED_BYTES.NAK;
            }
            return buff;
        }

        // Base64 encoding
        public static string EncodeBase64(string msg)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(msg));
        }

        // Base64 decoding
        public static string DecodeBase64(string msg)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(msg));
        }

        //  JIS8 unit coding
        public static byte[] EncodeJIS8(string msg)
        {
            return Encoding.UTF8.GetBytes(msg);
        }

        // JIS8 unit code decoding
        public static string DecodeJIS8(byte[] msgArray)
        {
            return Encoding.UTF8.GetString(msgArray);
        }

        public static void reloadConfig()
        {
            if (!File.Exists("config.ini"))
            {
                iniDict = new Dictionary<string, string>();
            }
            else
            {
                string[] lines = File.ReadAllLines("config.ini");
                iniDict = (from entry in lines
                           let key = entry.Substring(0, entry.IndexOf("="))
                           let value = entry.Substring(entry.IndexOf("=") + 1)
                           select new { key, value }).ToDictionary(e => e.key, e => e.value);
            }
        }
    }

}
