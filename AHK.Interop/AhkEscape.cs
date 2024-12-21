namespace AHK.Interop
{
    class AhkEscape
    {
       public static string Quote(string msg) {
            if (msg == null) throw new ArgumentNullException("msg");

            bool alreadyQuoted = msg.StartsWith("\"") && msg.EndsWith("\"");

            if (alreadyQuoted) {
                //remove quotes, and then escape
                msg = msg.Remove(0, 1);
                msg = msg.Remove(msg.Length - 1, 1);
                msg = "\"" + Escape(msg) + "\"";
            }
            else {
                msg = "\"" + Escape(msg) + "\"";
            }
            
            return msg;
        }

        public static string Escape(string msg) {
            if (msg == null) throw new ArgumentNullException("msg");

            return msg
                .Replace("`", "``")
                .Replace("\r", "`r")
                .Replace("\n", "`n")
                .Replace("\t", "`t")
                .Replace("\"", "\"\"");
        }
    }
}
