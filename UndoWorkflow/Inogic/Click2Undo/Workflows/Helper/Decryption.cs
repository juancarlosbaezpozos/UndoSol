using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Inogic.Click2Undo.Workflows.Helper
{
    internal class Decryption
    {
        private int[] digestBits = null;

        private long[] count = null;

        private List<long[]> states = new List<long[]>();

        private long[] buffer = null;

        private long[] transformBuffer = null;

        private Dictionary<string, object> licResults = null;

        private string dcgs;

        private int dcgi = 0;

        private string _key = "TYAEE-HWEBZ-SUIZZ-IXJUF-HWKAA-PMNGV-KNUID-WBZQT-RASRA-GIJEC-AUMZM-NYSBM";

        private int[] _keyArray = null;

        private int keySizeInBits = 256;

        private int blockSizeInBits = 128;

        private int[,] shiftOffsets = new int[9, 9];

        private int Nk = 8;

        private int Nb = 4;

        private int[,] roundsArray = new int[9, 9];

        private int Nr = 14;

        private int S11 = 7;

        private int S12 = 12;

        private int S13 = 17;

        private int S14 = 22;

        private int S21 = 5;

        private int S22 = 9;

        private int S23 = 14;

        private int S24 = 20;

        private int S31 = 4;

        private int S32 = 11;

        private int S33 = 16;

        private int S34 = 23;

        private int S41 = 6;

        private int S42 = 10;

        private int S43 = 15;

        private int S44 = 21;

        private int[] fItem = new int[3] { 10, 12, 14 };

        private int[] Rcon = new int[30]
        {
            1, 2, 4, 8, 16, 32, 64, 128, 27, 54,
            108, 216, 171, 77, 154, 47, 94, 188, 99, 198,
            151, 53, 106, 212, 179, 125, 250, 239, 197, 145
        };

        private int[] SBox = new int[256]
        {
            99, 124, 119, 123, 242, 107, 111, 197, 48, 1,
            103, 43, 254, 215, 171, 118, 202, 130, 201, 125,
            250, 89, 71, 240, 173, 212, 162, 175, 156, 164,
            114, 192, 183, 253, 147, 38, 54, 63, 247, 204,
            52, 165, 229, 241, 113, 216, 49, 21, 4, 199,
            35, 195, 24, 150, 5, 154, 7, 18, 128, 226,
            235, 39, 178, 117, 9, 131, 44, 26, 27, 110,
            90, 160, 82, 59, 214, 179, 41, 227, 47, 132,
            83, 209, 0, 237, 32, 252, 177, 91, 106, 203,
            190, 57, 74, 76, 88, 207, 208, 239, 170, 251,
            67, 77, 51, 133, 69, 249, 2, 127, 80, 60,
            159, 168, 81, 163, 64, 143, 146, 157, 56, 245,
            188, 182, 218, 33, 16, 255, 243, 210, 205, 12,
            19, 236, 95, 151, 68, 23, 196, 167, 126, 61,
            100, 93, 25, 115, 96, 129, 79, 220, 34, 42,
            144, 136, 70, 238, 184, 20, 222, 94, 11, 219,
            224, 50, 58, 10, 73, 6, 36, 92, 194, 211,
            172, 98, 145, 149, 228, 121, 231, 200, 55, 109,
            141, 213, 78, 169, 108, 86, 244, 234, 101, 122,
            174, 8, 186, 120, 37, 46, 28, 166, 180, 198,
            232, 221, 116, 31, 75, 189, 139, 138, 112, 62,
            181, 102, 72, 3, 246, 14, 97, 53, 87, 185,
            134, 193, 29, 158, 225, 248, 152, 17, 105, 217,
            142, 148, 155, 30, 135, 233, 206, 85, 40, 223,
            140, 161, 137, 13, 191, 230, 66, 104, 65, 153,
            45, 15, 176, 84, 187, 22
        };

        private int[] SBoxInverse = new int[256]
        {
            82, 9, 106, 213, 48, 54, 165, 56, 191, 64,
            163, 158, 129, 243, 215, 251, 124, 227, 57, 130,
            155, 47, 255, 135, 52, 142, 67, 68, 196, 222,
            233, 203, 84, 123, 148, 50, 166, 194, 35, 61,
            238, 76, 149, 11, 66, 250, 195, 78, 8, 46,
            161, 102, 40, 217, 36, 178, 118, 91, 162, 73,
            109, 139, 209, 37, 114, 248, 246, 100, 134, 104,
            152, 22, 212, 164, 92, 204, 93, 101, 182, 146,
            108, 112, 72, 80, 253, 237, 185, 218, 94, 21,
            70, 87, 167, 141, 157, 132, 144, 216, 171, 0,
            140, 188, 211, 10, 247, 228, 88, 5, 184, 179,
            69, 6, 208, 44, 30, 143, 202, 63, 15, 2,
            193, 175, 189, 3, 1, 19, 138, 107, 58, 145,
            17, 65, 79, 103, 220, 234, 151, 242, 207, 206,
            240, 180, 230, 115, 150, 172, 116, 34, 231, 173,
            53, 133, 226, 249, 55, 232, 28, 117, 223, 110,
            71, 241, 26, 113, 29, 41, 197, 137, 111, 183,
            98, 14, 170, 24, 190, 27, 252, 86, 62, 75,
            198, 210, 121, 32, 154, 219, 192, 254, 120, 205,
            90, 244, 31, 221, 168, 51, 136, 7, 199, 49,
            177, 18, 16, 89, 39, 128, 236, 95, 96, 81,
            127, 169, 25, 181, 74, 13, 45, 229, 122, 159,
            147, 201, 156, 239, 160, 224, 59, 77, 174, 42,
            245, 176, 200, 235, 187, 60, 131, 83, 153, 97,
            23, 43, 4, 126, 186, 119, 214, 38, 225, 105,
            20, 99, 85, 33, 12, 125
        };

        private int nbytes { get; set; }

        public string decryptKey(string licenseKey)
        {
            string empty = string.Empty;
            string text = string.Empty;
            digestBits = new int[16];
            count = new long[2];
            count[0] = 0L;
            count[1] = 0L;
            buffer = new long[64];
            transformBuffer = new long[16];
            int[] array = null;
            int num = 0;
            licResults = new Dictionary<string, object>();
            nbytes = 0;
            int[] array2 = null;
            int[] array3 = null;
            ASCIIEncoding aSCIIEncoding = null;
            byte[] array4 = null;
            string empty2 = string.Empty;
            string empty3 = string.Empty;
            roundsArray[4, 4] = 10;
            roundsArray[4, 6] = 12;
            roundsArray[4, 8] = 14;
            roundsArray[6, 4] = 12;
            roundsArray[6, 6] = 12;
            roundsArray[6, 8] = 14;
            roundsArray[8, 4] = 14;
            roundsArray[8, 6] = 14;
            roundsArray[8, 8] = 14;
            shiftOffsets[4, 1] = 1;
            shiftOffsets[4, 2] = 2;
            shiftOffsets[4, 3] = 3;
            shiftOffsets[6, 1] = 1;
            shiftOffsets[6, 2] = 2;
            shiftOffsets[6, 3] = 3;
            shiftOffsets[8, 1] = 1;
            shiftOffsets[8, 2] = 3;
            shiftOffsets[8, 3] = 4;
            try
            {
                setKey();
                array = disarm_codegroup(licenseKey);
                array2 = rijndaelDecrypt(array, _keyArray, "CBC");
                array3 = array2.Slice(0, 20);
                array2 = array2.Slice(20, array2.Length);
                if (array2.Length != 0)
                {
                    num = Convert.ToInt32((array3[16] << 24) | (array3[17] << 16) | (array3[18] << 8) | array3[19]);
                    if (num < 0 || num > array2.Length)
                    {
                        licResults.Add("Message",
                            "Message (Length " + array2.Length + ") truncated.  " + num + " characters expected.");
                        licResults.Add("isValidLic", false);
                        num = array2.Length;
                    }
                }

                md5_init();
                for (int i = 0; i < num; i++)
                {
                    aSCIIEncoding = new ASCIIEncoding();
                    byte b = Convert.ToByte(array2[i]);
                    array4 = new byte[1] { b };
                    empty2 = aSCIIEncoding.GetString(array4);
                    text += empty2;
                    md5_update(Convert.ToInt32(array2[i]));
                }

                md5_finish();
                for (int j = 0; j < digestBits.Length; j++)
                {
                    if (digestBits[j].ToString() != array3[j].ToString())
                    {
                        licResults.Add("Message ", "Message corrupted.  Checksum of decrypted message does not match.");
                        licResults.Add("isValidLic", false);
                        break;
                    }
                }

                return decode_utf8(text);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void setKey()
        {
            try
            {
                string text = encode_utf8(_key);
                if (text.Length == 1)
                {
                    text += text;
                }

                md5_init();
                for (int i = 0; i < _key.Length; i += 2)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(_key[i].ToString());
                    md5_update(bytes[0]);
                }

                md5_finish();
                string text2 = byteArrayToHex(digestBits);
                md5_init();
                for (int j = 1; j < _key.Length; j += 2)
                {
                    byte[] bytes2 = Encoding.ASCII.GetBytes(_key[j].ToString());
                    md5_update(Convert.ToInt32(bytes2[0]));
                }

                md5_finish();
                string text3 = byteArrayToHex(digestBits);
                string hexString = text2 + text3;
                _keyArray = hexToByteArray(hexString);
                hexString = byteArrayToHex(_keyArray);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string encode_utf8(string key)
        {
            bool flag = false;
            for (int i = 0; i < key.Length; i++)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(key[i].ToString());
                if (Convert.ToInt32(bytes[0]) == 157 || Convert.ToInt32(bytes[0]) > 255)
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                return key;
            }

            byte[] bytes2 = Encoding.ASCII.GetBytes("0x9D");
            return key + unicode_to_utf8(bytes2[0].ToString());
        }

        private string unicode_to_utf8(string s)
        {
            string empty = string.Empty;
            byte[] array = null;
            for (int i = 0; i < s.Length; i++)
            {
                array = Encoding.ASCII.GetBytes(s[i].ToString());
                int num = Convert.ToInt32(array[0]);
                if (num <= 127)
                {
                    array = Encoding.ASCII.GetBytes(num.ToString());
                }
            }

            return empty;
        }

        private void md5_init()
        {
            long[] array = new long[4];
            states = new List<long[]>();
            try
            {
                count[0] = (count[1] = 0L);
                array[0] = Convert.ToInt64(1732584193);
                array[1] = Convert.ToInt64(4023233417u);
                array[2] = Convert.ToInt64(2562383102u);
                array[3] = Convert.ToInt64(271733878);
                states.Add(array);
                for (int i = 0; i < digestBits.Length; i++)
                {
                    digestBits[i] = 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void md5_update(int b)
        {
            try
            {
                long num = and(shr(count[0], 3), 63);
                if (count[0] < Convert.ToInt64(uint.MaxValue) - 7)
                {
                    count[0] += 8L;
                }
                else
                {
                    count[1]++;
                    count[0] -= Convert.ToInt64(uint.MaxValue) + 1;
                    count[0] += 8L;
                }

                buffer[num] = and(b, 255);
                if (num >= 63)
                {
                    transform(buffer, 0);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private long and(long a, int b)
        {
            a = integer(a);
            b = integer(b);
            string empty = string.Empty;
            long num = a - Convert.ToInt64(2147483648u);
            long num2 = b - Convert.ToInt64(2147483648u);
            if (num >= 0)
            {
                if (num2 >= 0)
                {
                    return (num & num2) + Convert.ToInt64(2147483648u);
                }

                return num & b;
            }

            if (num2 >= 0)
            {
                return a & num2;
            }

            return a & b;
        }

        private int integer(int n)
        {
            return Convert.ToInt32(n % (Convert.ToInt64(uint.MaxValue) + 1));
        }

        private void transform(long[] buffer, int offset)
        {
            try
            {
                long num = 0L;
                long num2 = 0L;
                long num3 = 0L;
                long num4 = 0L;
                long[] array = transformBuffer;
                num = states[0][0];
                num2 = states[0][1];
                num3 = states[0][2];
                num4 = states[0][3];
                for (int i = 0; i < 16; i++)
                {
                    array[i] = and(Convert.ToInt64(buffer[i * 4 + offset]), 255);
                    for (int j = 1; j < 4; j++)
                    {
                        array[i] += shl(and(buffer[i * 4 + j + offset], 255), j * 8);
                    }
                }

                num = FF(num, num2, num3, num4, array[0], S11, Convert.ToInt64(3614090360u));
                num4 = FF(num4, num, num2, num3, array[1], S12, Convert.ToInt64(3905402710u));
                num3 = FF(num3, num4, num, num2, array[2], S13, Convert.ToInt64(606105819));
                num2 = FF(num2, num3, num4, num, array[3], S14, Convert.ToInt64(3250441966u));
                num = FF(num, num2, num3, num4, array[4], S11, Convert.ToInt64(4118548399u));
                num4 = FF(num4, num, num2, num3, array[5], S12, Convert.ToInt64(1200080426));
                num3 = FF(num3, num4, num, num2, array[6], S13, Convert.ToInt64(2821735955u));
                num2 = FF(num2, num3, num4, num, array[7], S14, Convert.ToInt64(4249261313u));
                num = FF(num, num2, num3, num4, array[8], S11, Convert.ToInt64(1770035416));
                num4 = FF(num4, num, num2, num3, array[9], S12, Convert.ToInt64(2336552879u));
                num3 = FF(num3, num4, num, num2, array[10], S13, Convert.ToInt64(4294925233u));
                num2 = FF(num2, num3, num4, num, array[11], S14, Convert.ToInt64(2304563134u));
                num = FF(num, num2, num3, num4, array[12], S11, Convert.ToInt64(1804603682));
                num4 = FF(num4, num, num2, num3, array[13], S12, Convert.ToInt64(4254626195u));
                num3 = FF(num3, num4, num, num2, array[14], S13, Convert.ToInt64(2792965006u));
                num2 = FF(num2, num3, num4, num, array[15], S14, Convert.ToInt64(1236535329));
                num = GG(num, num2, num3, num4, array[1], S21, Convert.ToInt64(4129170786u));
                num4 = GG(num4, num, num2, num3, array[6], S22, Convert.ToInt64(3225465664u));
                num3 = GG(num3, num4, num, num2, array[11], S23, Convert.ToInt64(643717713));
                num2 = GG(num2, num3, num4, num, array[0], S24, Convert.ToInt64(3921069994u));
                num = GG(num, num2, num3, num4, array[5], S21, Convert.ToInt64(3593408605u));
                num4 = GG(num4, num, num2, num3, array[10], S22, Convert.ToInt64(38016083));
                num3 = GG(num3, num4, num, num2, array[15], S23, Convert.ToInt64(3634488961u));
                num2 = GG(num2, num3, num4, num, array[4], S24, Convert.ToInt64(3889429448u));
                num = GG(num, num2, num3, num4, array[9], S21, Convert.ToInt64(568446438));
                num4 = GG(num4, num, num2, num3, array[14], S22, Convert.ToInt64(3275163606u));
                num3 = GG(num3, num4, num, num2, array[3], S23, Convert.ToInt64(4107603335u));
                num2 = GG(num2, num3, num4, num, array[8], S24, Convert.ToInt64(1163531501));
                num = GG(num, num2, num3, num4, array[13], S21, Convert.ToInt64(2850285829u));
                num4 = GG(num4, num, num2, num3, array[2], S22, Convert.ToInt64(4243563512u));
                num3 = GG(num3, num4, num, num2, array[7], S23, Convert.ToInt64(1735328473));
                num2 = GG(num2, num3, num4, num, array[12], S24, Convert.ToInt64(2368359562u));
                num = HH(num, num2, num3, num4, array[5], S31, Convert.ToInt64(4294588738u));
                num4 = HH(num4, num, num2, num3, array[8], S32, Convert.ToInt64(2272392833u));
                num3 = HH(num3, num4, num, num2, array[11], S33, Convert.ToInt64(1839030562));
                num2 = HH(num2, num3, num4, num, array[14], S34, Convert.ToInt64(4259657740u));
                num = HH(num, num2, num3, num4, array[1], S31, Convert.ToInt64(2763975236u));
                num4 = HH(num4, num, num2, num3, array[4], S32, Convert.ToInt64(1272893353));
                num3 = HH(num3, num4, num, num2, array[7], S33, Convert.ToInt64(4139469664u));
                num2 = HH(num2, num3, num4, num, array[10], S34, Convert.ToInt64(3200236656u));
                num = HH(num, num2, num3, num4, array[13], S31, Convert.ToInt64(681279174));
                num4 = HH(num4, num, num2, num3, array[0], S32, Convert.ToInt64(3936430074u));
                num3 = HH(num3, num4, num, num2, array[3], S33, Convert.ToInt64(3572445317u));
                num2 = HH(num2, num3, num4, num, array[6], S34, Convert.ToInt64(76029189));
                num = HH(num, num2, num3, num4, array[9], S31, Convert.ToInt64(3654602809u));
                num4 = HH(num4, num, num2, num3, array[12], S32, Convert.ToInt64(3873151461u));
                num3 = HH(num3, num4, num, num2, array[15], S33, Convert.ToInt64(530742520));
                num2 = HH(num2, num3, num4, num, array[2], S34, Convert.ToInt64(3299628645u));
                num = II(num, num2, num3, num4, array[0], S41, Convert.ToInt64(4096336452u));
                num4 = II(num4, num, num2, num3, array[7], S42, Convert.ToInt64(1126891415));
                num3 = II(num3, num4, num, num2, array[14], S43, Convert.ToInt64(2878612391u));
                num2 = II(num2, num3, num4, num, array[5], S44, Convert.ToInt64(4237533241u));
                num = II(num, num2, num3, num4, array[12], S41, Convert.ToInt64(1700485571));
                num4 = II(num4, num, num2, num3, array[3], S42, Convert.ToInt64(2399980690u));
                num3 = II(num3, num4, num, num2, array[10], S43, Convert.ToInt64(4293915773u));
                num2 = II(num2, num3, num4, num, array[1], S44, Convert.ToInt64(2240044497u));
                num = II(num, num2, num3, num4, array[8], S41, Convert.ToInt64(1873313359));
                num4 = II(num4, num, num2, num3, array[15], S42, Convert.ToInt64(4264355552u));
                num3 = II(num3, num4, num, num2, array[6], S43, Convert.ToInt64(2734768916u));
                num2 = II(num2, num3, num4, num, array[13], S44, Convert.ToInt64(1309151649));
                num = II(num, num2, num3, num4, array[4], S41, Convert.ToInt64(4149444226u));
                num4 = II(num4, num, num2, num3, array[11], S42, Convert.ToInt64(3174756917u));
                num3 = II(num3, num4, num, num2, array[2], S43, Convert.ToInt64(718787259));
                num2 = II(num2, num3, num4, num, array[9], S44, Convert.ToInt64(3951481745u));
                states[0][0] += num;
                states[0][1] += num2;
                states[0][2] += num3;
                states[0][3] += num4;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void md5_finish()
        {
            try
            {
                long[] array = new long[8];
                int[] array2 = new int[64];
                long num = 0L;
                long num2 = 0L;
                for (int i = 0; i < 4; i++)
                {
                    array[i] = and(shr(Convert.ToInt32(count[0]), i * 8), 255);
                }

                for (int j = 0; j < 4; j++)
                {
                    array[j + 4] = and(shr(Convert.ToInt32(count[1]), Convert.ToInt32(j * 8)), 255);
                }

                num = and(shr(Convert.ToInt32(count[0]), 3), 63);
                num2 = ((num < 56) ? (56 - num) : (120 - num));
                array2[0] = 128;
                for (int k = 0; k < num2; k++)
                {
                    md5_update(array2[k]);
                }

                for (int l = 0; l < 8; l++)
                {
                    md5_update(Convert.ToInt32(array[l]));
                }

                for (int m = 0; m < 4; m++)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        long value = and(shr(states[0][m], Convert.ToInt32(n * 8)), 255);
                        digestBits[m * 4 + n] = Convert.ToInt32(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private int[] hexToByteArray(string hexString)
        {
            List<int> list = new List<int>();
            try
            {
                if (hexString.Length > 1)
                {
                    if (hexString.IndexOf("0x") == 0 || hexString.IndexOf("0X") == 0)
                    {
                        hexString = hexString.Substring(2);
                    }

                    for (int i = 0; i < hexString.Length; i += 2)
                    {
                        string value = hexString.SliceString(i, i + 2);
                        list.Insert(i / 2, Convert.ToInt16(value, 16));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return list.ToArray();
        }

        private string byteArrayToHex(int[] digestBits)
        {
            string text = string.Empty;
            try
            {
                if (digestBits != null)
                {
                    for (int i = 0; i < digestBits.Length; i++)
                    {
                        string text2 = $"{digestBits[i]:X}";
                        text = text + ((digestBits[i] < 16) ? "0" : "") + text2;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return text;
        }

        private int[] disarm_codegroup(string licenseKey)
        {
            try
            {
                List<int> list = new List<int>();
                long num = 0L;
                long num2 = 0L;
                int num3 = 0;
                string text = string.Empty;
                dcgs = licenseKey;
                dcgi = 6;
                byte[] bytes = Encoding.ASCII.GetBytes("A");
                int num4 = Convert.ToInt16(bytes[0]);
                LEcuyer lEcuyer = new LEcuyer(Convert.ToInt64(195948557));
                num = 0L;
                while (num < 2)
                {
                    text = disarm_cg_insig();
                    if (text == "Y")
                    {
                        break;
                    }

                    if (text != "Z" && text != " ")
                    {
                        long num5 = lEcuyer.nextInt(23);
                        long num6 = text[0] - num4;
                        num6 = (num6 + (24 - num5)) % 24;
                        if (num == 0)
                        {
                            num2 = num6 << 4;
                            num++;
                        }
                        else
                        {
                            num2 |= num6;
                            list.Insert(num3++, Convert.ToInt32(num2));
                            num = 0L;
                        }
                    }
                }

                string text2 = "  Attempting decoding with data received.";
                if (num != 0)
                {
                    licResults.Add("Message", "Codegroup data truncated." + text2);
                }
                else if (text == "Y")
                {
                    for (num = 1L; num < 5; num++)
                    {
                        text = disarm_cg_insig();
                        if (text != "Y")
                        {
                            break;
                        }
                    }

                    if (num != 5)
                    {
                        licResults.Add("Message", "Codegroup end group incomplete." + text2);
                    }
                }
                else
                {
                    licResults.Add("Message", "Codegroup end group missing." + text2);
                }

                return list.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string disarm_cg_insig()
        {
            string empty = string.Empty;
            string text = string.Empty;
            try
            {
                if (dcgs != string.Empty)
                {
                    while (dcgi < dcgs.Length)
                    {
                        text = dcgs[dcgi++].ToString().ToUpper();
                        int num = Regex.Matches(text, "[a-zA-Z]").Count;
                        if (num > 0)
                        {
                            return text;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return text;
        }

        private int[] rijndaelDecrypt(int[] ciphertext, int[] key, string mode)
        {
            int[] array = null;
            int num = blockSizeInBits / 8;
            int[] array2 = null;
            try
            {
                array = keyExpansion(key);
                array2 = new int[ciphertext.Length];
                for (int num2 = ciphertext.Length / num - 1; num2 > 0; num2--)
                {
                    int[] array3 = decrypt(ciphertext.Slice(num2 * num, (num2 + 1) * num), array);
                    if (mode == "CBC")
                    {
                        for (int i = 0; i < num; i++)
                        {
                            array2[(num2 - 1) * num + i] = array3[i] ^ ciphertext[(num2 - 1) * num + i];
                        }
                    }
                    else
                    {
                        array2 = array3.Concat(array2).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return array2?.ToArray();
        }

        private int[] keyExpansion(int[] key)
        {
            List<int> list = new List<int>();
            int num = keySizeInBits / 32;
            int num2 = blockSizeInBits / 32;
            int num3 = roundsArray[num, num2];
            try
            {
                for (int i = 0; i < num; i++)
                {
                    list.Add(key[4 * i] | (key[4 * i + 1] << 8) | (key[4 * i + 2] << 16) | (key[4 * i + 3] << 24));
                }

                for (int j = num; j < num2 * (num3 + 1); j++)
                {
                    int num4 = list[j - 1];
                    if (j % num == 0)
                    {
                        num4 = (SBox[(num4 >> 8) & 0xFF] | (SBox[(num4 >> 16) & 0xFF] << 8) |
                                (SBox[(num4 >> 24) & 0xFF] << 16) | (SBox[num4 & 0xFF] << 24)) ^ Rcon[j / num - 1];
                    }
                    else if (num > 6 && j % num == 4)
                    {
                        num4 = (SBox[(num4 >> 24) & 0xFF] << 24) | (SBox[(num4 >> 16) & 0xFF] << 16) |
                               (SBox[(num4 >> 8) & 0xFF] << 8) | SBox[num4 & 0xFF];
                    }

                    list.Add(list[j - num] ^ num4);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return list.ToArray();
        }

        private int[] decrypt(int[] block, int[] expandedKey)
        {
            try
            {
                List<int> list = new List<int>();
                int[,] array = null;
                array = packBytes(block);
                int[] roundKey = expandedKey.Slice(Nb * Nr, expandedKey.Length);
                InverseFinalRound(array, roundKey);
                for (int num = Nr - 1; num > 0; num--)
                {
                    int[] roundKey2 = expandedKey.Slice(Nb * num, Nb * (num + 1));
                    InverseRound(array, roundKey2);
                }

                addRoundKey(array, expandedKey);
                return unpackBytes(array);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private int[] unpackBytes(int[,] packed)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                list.Add(packed[0, i]);
                list.Add(packed[1, i]);
                list.Add(packed[2, i]);
                list.Add(packed[3, i]);
            }

            return list.ToArray();
        }

        private void Round(int[,] state, int[] roundKey)
        {
            try
            {
                byteSub(state, "encrypt");
                shiftRow(state, "encrypt");
                mixColumn(state, "encrypt");
                addRoundKey(state, roundKey);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void InverseRound(int[,] state, int[] roundKey)
        {
            state = addRoundKey(state, roundKey);
            state = mixColumn(state, "decrypt");
            state = shiftRow(state, "decrypt");
            state = byteSub(state, "decrypt");
        }

        private void FinalRound(int[,] state, int[] roundKey)
        {
            byteSub(state, "encrypt");
            shiftRow(state, "encrypt");
            addRoundKey(state, roundKey);
        }

        private void InverseFinalRound(int[,] state, int[] roundKey)
        {
            state = addRoundKey(state, roundKey);
            state = shiftRow(state, "decrypt");
            state = byteSub(state, "decrypt");
        }

        private int[,] addRoundKey(int[,] state, int[] roundKey)
        {
            for (int i = 0; i < Nb; i++)
            {
                state[0, i] ^= roundKey[i] & 0xFF;
                state[1, i] ^= (roundKey[i] >> 8) & 0xFF;
                state[2, i] ^= (roundKey[i] >> 16) & 0xFF;
                state[3, i] ^= (roundKey[i] >> 24) & 0xFF;
            }

            return state;
        }

        private int[,] byteSub(int[,] state, string direction)
        {
            try
            {
                int[] array = null;
                array = ((!(direction == "encrypt")) ? SBoxInverse : SBox);
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < Nb; j++)
                    {
                        state[i, j] = array[state[i, j]];
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return state;
        }

        private string decode_utf8(string key)
        {
            if (key.Length > 0 && key[0] == '\u009d')
            {
                return utf8_to_unicode(key.Substring(1));
            }

            return key;
        }

        private string utf8_to_unicode(string utf8)
        {
            string text = string.Empty;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            while (num < utf8.Length)
            {
                num2 = utf8[num];
                if (num2 < 128)
                {
                    num2 = Convert.ToInt32(num2);
                    text += new string(new char[0]);
                    num++;
                }
                else if (num2 >= 192 && num2 < 224)
                {
                    num3 = utf8[num + 1];
                    num += 2;
                }
                else
                {
                    num3 = utf8[num + 1];
                    num4 = utf8[num + 2];
                    num += 3;
                }
            }

            return text;
        }

        private long shr(long a, int b)
        {
            a = integer(a);
            b = integer(b);
            if (a - 2147483648u >= 0)
            {
                a = Convert.ToInt32(a % 2147483648L);
                a >>= b;
                a += 1073741824 >> b - 1;
            }
            else
            {
                a >>= b;
            }

            return a;
        }

        private int xtime(int poly)
        {
            poly <<= 1;
            return (((uint)poly & 0x100u) != 0) ? (poly ^ 0x11B) : poly;
        }

        private int[,] packBytes(int[] octets)
        {
            int[,] array = new int[4, 4];
            if (octets == null || octets.Length == 0)
            {
                return array;
            }

            for (int i = 0; i < octets.Length; i += 4)
            {
                array[0, i / 4] = octets[i];
                array[1, i / 4] = octets[i + 1];
                array[2, i / 4] = octets[i + 2];
                array[3, i / 4] = octets[i + 3];
            }

            return array;
        }

        private int[,] shiftRow(int[,] state, string direction)
        {
            try
            {
                int[] array = new int[4];
                int[] array2 = new int[4];
                for (int i = 1; i < 4; i++)
                {
                    array = new int[4];
                    for (int j = 0; j < 4; j++)
                    {
                        array[j] = state[i, j];
                    }

                    array2 = cyclicShiftLeft(array, Nb - shiftOffsets[Nb, i]);
                    for (int k = 0; k < array2.Length; k += 4)
                    {
                        state[i, 0] = Convert.ToInt32(array2[k]);
                        state[i, 1] = Convert.ToInt32(array2[k + 1]);
                        state[i, 2] = Convert.ToInt32(array2[k + 2]);
                        state[i, 3] = Convert.ToInt32(array2[k + 3]);
                    }
                }

                return state;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private int[,] mixColumn(int[,] state, string direction)
        {
            int[] array = new int[4];
            try
            {
                for (int i = 0; i < Nb; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (direction == "encrypt")
                        {
                            array[j] = mult_GF256(state[j, i], 2) ^ mult_GF256(state[(j + 1) % 4, i], 3) ^
                                       state[(j + 2) % 4, i] ^ state[(j + 3) % 4, i];
                        }
                        else
                        {
                            array[j] = mult_GF256(state[j, i], 14) ^ mult_GF256(state[(j + 1) % 4, i], 11) ^
                                       mult_GF256(state[(j + 2) % 4, i], 13) ^ mult_GF256(state[(j + 3) % 4, i], 9);
                        }
                    }

                    for (int k = 0; k < 4; k++)
                    {
                        state[k, i] = array[k];
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return state;
        }

        private int mult_GF256(int x, int y)
        {
            int num = 0;
            int num2 = 0;
            num = 1;
            while (num < 256)
            {
                if ((x & num) != 0)
                {
                    num2 ^= y;
                }

                num *= 2;
                y = xtime(y);
            }

            return num2;
        }

        private int[] cyclicShiftLeft(int[] theArray, int positions)
        {
            int[] second = theArray.Slice(0, positions);
            theArray = theArray.Slice(positions, theArray.Length).Concat(second).ToArray();
            return theArray;
        }

        private long F(long x, long y, long z)
        {
            return or(and(x, y), and(not(x), z));
        }

        private long and(long a, long b)
        {
            try
            {
                a = integer(a);
                b = integer(b);
                string empty = string.Empty;
                long num = a - Convert.ToInt64(2147483648u);
                long num2 = b - Convert.ToInt64(2147483648u);
                if (num >= 0)
                {
                    if (num2 >= 0)
                    {
                        return (num & num2) + Convert.ToInt64(2147483648u);
                    }

                    return num & b;
                }

                if (num2 >= 0)
                {
                    return a & num2;
                }

                return a & b;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private long G(long x, long y, long z)
        {
            return or(and(x, z), and(not(z), y));
        }

        private long H(long x, long y, long z)
        {
            return xor(xor(x, y), z);
        }

        private long I(long x, long y, long z)
        {
            return xor(y, or(x, not(z)));
        }

        private long rotateLeft(long a, int n)
        {
            return or(shl(a, n), shr(a, 32 - n));
        }

        private long FF(long a, long b, long c, long d, long x, int s, long ac)
        {
            a = a + F(b, c, d) + x + ac;
            a = rotateLeft(a, s);
            a += b;
            return a;
        }

        private long GG(long a, long b, long c, long d, long x, int s, long ac)
        {
            a = a + G(b, c, d) + x + ac;
            a = rotateLeft(a, s);
            a += b;
            return a;
        }

        private long HH(long a, long b, long c, long d, long x, int s, long ac)
        {
            a = a + H(b, c, d) + x + ac;
            a = rotateLeft(a, s);
            a += b;
            return a;
        }

        private long II(long a, long b, long c, long d, long x, int s, long ac)
        {
            a = a + I(b, c, d) + Convert.ToInt64(x) + ac;
            a = rotateLeft(a, s);
            a += b;
            return a;
        }

        private long xor(long a, long b)
        {
            a = integer(a);
            b = integer(b);
            long num = a - Convert.ToInt64(2147483648u);
            long num2 = b - Convert.ToInt64(2147483648u);
            if (num >= 0)
            {
                if (num2 >= 0)
                {
                    return num ^ num2;
                }

                return (num ^ b) + Convert.ToInt64(2147483648u);
            }

            if (num2 >= 0)
            {
                return (a ^ num2) + Convert.ToInt64(2147483648u);
            }

            return a ^ b;
        }

        private long integer(long a)
        {
            return a % (Convert.ToInt64(uint.MaxValue) + 1);
        }

        private long not(long a)
        {
            a = integer(a);
            return Convert.ToInt64(uint.MaxValue) - a;
        }

        private long or(long a, long b)
        {
            try
            {
                a = integer(a);
                b = integer(b);
                long num = a - Convert.ToInt64(2147483648u);
                long num2 = b - Convert.ToInt64(2147483648u);
                if (num >= 0)
                {
                    if (num2 >= 0)
                    {
                        return (num | num2) + Convert.ToInt64(2147483648u);
                    }

                    return (num | b) + Convert.ToInt64(2147483648u);
                }

                if (num2 >= 0)
                {
                    return (a | num2) + Convert.ToInt64(2147483648u);
                }

                return a | b;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private long shl1(long a)
        {
            long num = a % Convert.ToInt64(2147483648u);
            if (a != 0L && Convert.ToInt64(2147483648u) == Convert.ToInt64(1073741824))
            {
                num -= Convert.ToInt64(1073741824);
                num *= 2;
                return num + Convert.ToInt64(2147483648u);
            }

            return num * 2;
        }

        private long shl(long a, int b)
        {
            a = integer(a);
            b = integer(b);
            for (int i = 0; i < b; i++)
            {
                a = shl1(a);
            }

            return a;
        }
    }
}