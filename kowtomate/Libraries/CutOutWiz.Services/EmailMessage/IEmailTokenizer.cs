
namespace CutOutWiz.Services.EmailMessage
{
    public interface IEmailTokenizer
    {
        string Replace(string template, IEnumerable<EmailToken> tokens, bool htmlEncode);
    }
}