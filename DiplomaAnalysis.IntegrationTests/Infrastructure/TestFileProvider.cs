using System.Security.Cryptography;
using System.Text;

namespace DiplomaAnalysis.IntegrationTests.Infrastructure;

public class TestFileProvider
{
    private readonly bool _decryptFiles;

    public TestFileProvider(bool decryptFiles)
    {
        _decryptFiles = decryptFiles;
    }

    public byte[] GetFile(string filePath) => _decryptFiles switch
    {
        true => GetFileAndDecrypt(filePath),
        false => File.ReadAllBytes(filePath)
    };

    private byte[] GetFileAndDecrypt(string filePath)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(EnvironmentVariables.ProductiveDataDecryptionKey);
        var salt = new byte[32];

        using var @in = new FileStream(filePath, FileMode.Open);
        @in.Read(salt, 0, salt.Length);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;

        var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CFB;

        using var cryptedInput = new CryptoStream(@in, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var @out = new MemoryStream();

        int read;
        var buffer = new byte[1048576];

        while ((read = cryptedInput.Read(buffer, 0, buffer.Length)) > 0)
        {
            @out.Write(buffer, 0, read);
        }

        return @out.ToArray();
    }
}
