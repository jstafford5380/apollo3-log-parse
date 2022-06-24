using CataParser.Constants;

namespace CataParser.Ext;

public static class StringEx
{
    public static UnitFlags ToUnitFlags(this string hexValue)
        => (UnitFlags) Convert.ToUInt64(hexValue, 16);

    public static byte[] HexToBytes(this string hexValue)
    {
        
        static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        if (hexValue.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        byte[] arr = new byte[hexValue.Length >> 1];

        for (int i = 0; i < hexValue.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hexValue[i << 1]) << 4) + (GetHexVal(hexValue[(i << 1) + 1])));
        }

        return arr;
    }
}
