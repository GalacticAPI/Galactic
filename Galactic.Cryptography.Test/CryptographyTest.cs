using System.Text;

namespace Galactic.Cryptography.Test;

[TestClass]
public sealed class AES256Test
{
    [TestMethod]
    public void GenerateIV_NotEmptyByteArray()
    {
        Assert.IsFalse(AES256.GenerateIV().Length == 0, "GenerateIV() should not be an empty byte array.");
    }

    [TestMethod]
    public void BytesToString_InputIsSingleValue_ReturnString()
    {
        Assert.IsTrue(AES256.BytesToString([0]) == "0", "BytesToString() should return the a single byte's string representation.");
    }

    [TestMethod]
    public void BytesToString_InputIsValidByteArray_ReturnString()
    {
        byte[] bytes = [byte.MinValue, byte.MaxValue];
        
        string minValue = byte.MinValue.ToString();
        string maxValue = byte.MaxValue.ToString();
        string byteString = minValue + ',' + maxValue;

        Assert.IsTrue(AES256.BytesToString(bytes) == byteString, "BytesToString() should return each byte in the array as a comma seperated string.");
    }

    [TestMethod]
    public void BytesToString_InputIsNull_ReturnNull()
    {
        Assert.IsTrue(AES256.BytesToString(null) == null, "BytesToString() should return null when a null input is given.");
    }

    [TestMethod]
    public void BytesToString_InputIsEmpty_ReturnNull()
    {
        Assert.IsTrue(AES256.BytesToString([]) == null, "BytesToString() should return null when an empty byte array input is given.");
    }

    [TestMethod]
    public void BytesToString_InputIsTooLarge_ReturnNull()
    {
        byte[] tooManyBytes = (byte[])Array.CreateInstance(typeof(byte), AES256.BytesToStringMaxLength);
        tooManyBytes.Initialize();

        Assert.IsTrue(AES256.BytesToString(tooManyBytes) == null, "BytesToString() should return null when a byte array input that is larger than BytesToStringMaxLength or exceeds system memory is given.");
    }
}
