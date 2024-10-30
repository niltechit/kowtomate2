
namespace CutOutWiz.Services.InternalMessage
{
    public interface IInernalMessageTokenizer
    {
        string Replace(string template, IEnumerable<InternalMessageToken> tokens, bool htmlEncode);
    }
}