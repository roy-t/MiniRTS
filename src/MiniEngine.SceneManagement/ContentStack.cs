using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Configuration;

namespace MiniEngine.SceneManagement
{
    [System]
    public sealed class ContentStack : IDisposable
    {
        private readonly ContentManager Root;

        private readonly Stack<ContentStackFrame> Stack;

        public ContentStack(ContentManager root)
        {
            this.Root = root;
            this.Stack = new Stack<ContentStackFrame>();

            this.Stack.Push(this.CreateStackFrame("root"));
        }

        public T Load<T>(string assetName)
            where T : class => this.Stack.Peek().Load<T>(assetName);

        public void Link<T>(T asset)
            where T : class => this.Stack.Peek().Link(asset);

        public void Push(string tag)
            => this.Stack.Push(this.CreateStackFrame(tag));

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

        private ContentStackFrame CreateStackFrame(string tag)
        {
            var contentManager = new ContentManager(this.Root.ServiceProvider, this.Root.RootDirectory);
            return new ContentStackFrame(contentManager, tag);
        }

        private sealed class ContentStackFrame : IDisposable
        {
            private readonly ContentManager Content;
            private readonly List<object> Linked;

            public ContentStackFrame(ContentManager content, string tag)
            {
                this.Content = content;
                this.Linked = new List<object>();
                this.Tag = tag;
            }

            public string Tag { get; }

            public T Load<T>(string assetName)
                => this.Content.Load<T>(assetName);

            public void Link(object content)
                => this.Linked.Add(content);

            public void Dispose()
            {
                this.Content.Unload();

                for (var i = 0; i < this.Linked.Count; i++)
                {
                    var disposable = this.Linked[i] as IDisposable;
                    disposable?.Dispose();
                }

                this.Linked.Clear();
            }
        }
    }
}
