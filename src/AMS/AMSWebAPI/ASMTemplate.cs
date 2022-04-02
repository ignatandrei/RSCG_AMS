using System;
using System.Linq;
using System.Text;
partial class ASMTemplate
{
    StringBuilder sb = new StringBuilder();
    public void WriteText(string text)
    {
        sb.AppendLine(text);
    }
    public void WriteValue(int text)
    {     
        sb.AppendLine(""+text);
    }
    public void WriteValue(int? text)
    {
        if(text != null)
            sb.AppendLine("" + text);
    }
    public void WriteValue(DateTime text)
    {
        sb.AppendLine(text.ToString("yyyy MMMM dd HH:mm:ss"));
    }
    public void WriteValue(string text)
    {
        sb.AppendLine(text);
    }
    public string Render()
    {         
        this.RenderCore(); 
        return sb.ToString(); 
    }
} 