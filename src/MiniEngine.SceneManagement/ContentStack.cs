using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Configuration;

namespace MiniEngine.SceneManagement
{
    [System]
    public sealed class ContentStack : IDisposable
    {
        private readonly ContentManager Content;

        private readonly Stack<ContentStackFrame> Stack;

        public ContentStack(ContentManager content)
        {
            this.Content = content;
            this.Stack = new Stack<ContentStackFrame>();

            this.Stack.Push(new ContentStackFrame("root"));
        }

        public T Load<T>(string assetName)
            where T : class
        {
            var asset = this.Content.Load<T>(assetName);
            this.Stack.Peek().Add(asset);

            return asset;
        }

        public void Link<T>(T asset)
            where T : class
            => this.Stack.Peek().Add(asset);

        public void Push(string tag) => this.Stack.Push(new ContentStackFrame(tag));

        public void Pop()
        {
            var content = this.Stack.Pop();
            content.Dispose();
        }

        public void Dispose()
        {
            while (this.Stack.Count > 0)
            {
                this.Stack.Pop().Dispose();
            }
        }

        private sealed class ContentStackFrame : IDisposable
        {
            private readonly List<object> Content;

            public ContentStackFrame(string tag)
            {
                this.Tag = tag;
                this.Content = new List<object>();
            }

            public string Tag { get; }

            public void Add(object content) => this.Content.Add(content);

            public void Dispose()
            {
                for (var i = 0; i < this.Content.Count; i++)
                {
                    var content = this.Content[i];
                    if (content is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                this.Content.Clear();
            }
        }
    }
}
