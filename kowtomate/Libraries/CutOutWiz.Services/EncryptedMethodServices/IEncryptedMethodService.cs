
namespace CutOutWiz.Services.EncryptedMethodServices
{
    public interface IEncryptedMethodService
    {
        string EncryptData(string plainText);
        string DecryptData(string cipherText);
    }
}
