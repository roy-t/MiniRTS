using System.Collections.Generic;

namespace MiniEngine.Gui
{
    public sealed class Node
    {
        public Node(string aggregate, bool terminal)
        {
            this.Aggregate = aggregate;
            this.IsTerminal = terminal;
            this.Outgoing = new SortedList<char, Node>();
        }

        public string Aggregate { get; }

        public bool IsTerminal { get; set; }

        public SortedList<char, Node> Outgoing { get; }

        public override string ToString()
        {
            var terminal = this.IsTerminal ? "" : "…";
            return $"{this.Aggregate}{terminal} [{string.Join(", ", this.Outgoing.Keys)}]";
        }
    }

    public sealed class Trie
    {
        private readonly Node RootNode;

        public Trie()
        {
            this.RootNode = new Node("", true);
        }

        public void Add(string text)
        {
            var currentNode = this.RootNode;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var isTerminal = i == text.Length - 1;

                if (currentNode.Outgoing.TryGetValue(c, out var nextNode))
                {
                    if (isTerminal)
                    {
                        nextNode.IsTerminal = true;
                    }
                }
                else
                {
                    nextNode = new Node(currentNode.Aggregate + c, isTerminal);
                    currentNode.Outgoing.Add(c, nextNode);
                }

                currentNode = nextNode;
            }
        }

        public string FindTextWithLongestCommonPrefix(string text)
        {
            var currentNode = this.RootNode;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (currentNode.Outgoing.TryGetValue(c, out var nextNode))
                {
                    currentNode = nextNode;
                }
                else
                {
                    break;
                }
            }

            if (currentNode.IsTerminal)
            {
                return currentNode.Aggregate;
            }
            else
            {
                return FindFirstTerminalNode(currentNode).Aggregate;
            }
        }

        private static Node FindFirstTerminalNode(Node node)
        {
            while (!node.IsTerminal)
            {
                node = node.Outgoing.Values[0];
            }

            return node;
        }
    }
}
