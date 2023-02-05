using Googler.Models;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Text;
using System.Xml.Linq;

namespace Googler.Controllers
{
    public enum NodeStatus
    {
        FindingType,
        FindingText,
        ClosingNode,
        CreatingNode
    }

    public class HtmlDocument
    {
        private readonly string _htmlText;
        private int _currentIndex;
        private HtmlNode? _documentRoot;
        private readonly List<char> _lowers = new() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public HtmlDocument(string htmlText)
        {
            _htmlText = htmlText;
            _currentIndex = 0;
        }

        public void LoadElements()
        {
            HtmlNode? prevNode = null;

            _currentIndex = 0;
            while (_currentIndex < _htmlText.Length)
            {
                char currentChar = _htmlText[_currentIndex];

                if (currentChar == '<')
                {
                    _currentIndex++;

                    StringBuilder s = new();
                    while (_htmlText[_currentIndex] != '>')
                    {
                        s.Append(_htmlText[_currentIndex]);
                        _currentIndex++;
                    }

                    // gather all the info
                    (bool isClosing, TagType? tagType) = ProcessTagText(s.ToString());

                    s.Clear();
                    if (isClosing)
                    {
                        prevNode = prevNode?.Parent;
                    }
                    // opening
                    else
                    {
                        string nodeText = string.Empty;
                        // check if there is any text
                        if (_htmlText[_currentIndex + 1] != '<')
                        {
                            // get the text, up to the next closing, or opening tag
                            while (_htmlText[_currentIndex] != '<')
                            {
                                s.Append(_htmlText[_currentIndex]);
                                _currentIndex++;
                            }
                            nodeText = s.ToString();
                            s.Clear();
                        }
                        // Create the node
                        if (tagType is not null)
                        {
                            HtmlNode node = new(tagType.Value, nodeText);
                            node.Parent = prevNode;
                            prevNode?.Children.Add(node);
                            prevNode = node;
                        }
                    }
                }
                // increment normally
                _currentIndex++;
            }
        }

        public (bool, TagType?) ProcessTagText(string s)
        {
            if (s.Length == 0)
            {
                throw new InvalidOperationException("Could not parse tag with zero length.");
            }
            bool isClosing = s[0] == '/';
            TagType? tagType = null;
            if (!isClosing)
            {
                int i = 0;
                StringBuilder builder = new();
                while (i < s.Length && s[i] != ' ' && _lowers.Contains(s[i]))
                {
                    builder.Append(s[i]);
                    i++;
                }
                string typeString = builder.ToString();
                if (typeString.Length == 0)
                {
                    throw new InvalidOperationException($"Error parsing tag text {s} for tag type");
                }
                tagType = HtmlNode.GetTagType(builder.ToString());
            }
            return (isClosing, tagType);
        }
    }
}