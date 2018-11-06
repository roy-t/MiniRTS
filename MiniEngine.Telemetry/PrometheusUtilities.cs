using System.Text;

namespace MiniEngine.Telemetry
{
    public static class PrometheusUtilities
    {
        public static string ExpandTags(Tag[] tags)
        {
            if(tags == null || tags.Length == 0)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");

            for(var i = 0; i < tags.Length; i++)
            {
                var tag = tags[i];
                stringBuilder.Append(tag.Name);
                stringBuilder.Append("=\"");
                stringBuilder.Append(tag.Value);
                stringBuilder.Append("\"");

                if(i < tags.Length - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.Append("}");

            return stringBuilder.ToString();            
        }
    }
}

