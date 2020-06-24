using System.Text;

namespace MiniEngine.Net
{
    public sealed class NetworkLogger
    {
        private StringBuilder Messages;
        private string messages;
        private bool isDirty;


        public NetworkLogger()
        {
            this.Messages = new StringBuilder();
            this.Clear();
        }

        public void Info(string message) => this.Write("info", message);
        public void Warn(string message) => this.Write("Warn", message);
        public void Error(string message) => this.Write("ERROR", message);

        public string BuildString()
        {
            if (this.isDirty)
            {
                this.messages = this.Messages.ToString();
            }

            return this.messages;
        }

        public void Clear()
        {
            this.Messages.Clear();
            this.messages = string.Empty;
            this.isDirty = false;
        }

        private void Write(string header, string message)
        {
            this.Messages.AppendLine($"[{header}] {message}");
            this.isDirty = true;
        }
    }
}
