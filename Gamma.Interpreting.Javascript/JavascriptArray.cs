using System.Text;

namespace Gamma.Interpreting.Javascript;

public class JavascriptArray : List<object>
{
    public JavascriptArray(IEnumerable<object> objects) : base(objects) { }

    public JavascriptArray() : base() { } 

    public override string ToString()
    {
        return $"[{string.Join(", ", this)}]";
    }
}