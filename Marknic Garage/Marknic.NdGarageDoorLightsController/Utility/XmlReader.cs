using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Marknic.NdGarageDoorLightsController.Utility
{
    public class XmlReader
    {
        private readonly FileStream _fileStream;
        private readonly byte[] _fileData;
        private int _characterPosition;
        private readonly long _fileLength;
        private readonly string _fileAsString;

        public string Name { get; private set; }
        public string XmlNodeContents { get; private set; }
        public XmlNodeType NodeType { get; private set; }

        private XmlReader(FileStream fileStream)
        {
            _fileStream = fileStream;

            _fileLength = _fileStream.Length;

            _fileData = new byte[_fileLength];

            _fileStream.Read(_fileData, 0, (int) _fileLength);

            _fileAsString = new string(Encoding.UTF8.GetChars(_fileData));
        }


        public static XmlReader Create(FileStream fileStream)
        {
            return new XmlReader(fileStream);
        }


        public string GetAttribute(string attributeName)
        {
            var pattern = ".*" + attributeName + "\\s*\\=\\\"(\\w[\\w\\s]*)\\\".*";

            var keyValueMatch = Regex.Match(XmlNodeContents, pattern);

            var val = keyValueMatch.Groups[1].Value;

            return val;
        }


        /// <summary>
        /// Read the next XML Token (Element)
        /// </summary>
        /// <returns></returns>
        public bool ReadElement()
        {
            if (_characterPosition >= _fileLength)
            {
                return false;
            }

            // Skip to the next token
            while (_fileAsString[_characterPosition] != '<')
            {
                _characterPosition++;

                if (_characterPosition >= _fileLength)
                {
                    return false;
                }
            }

            var leftBracket = _characterPosition;

            if (_characterPosition >= _fileLength)
            {
                return false;
            }

            // Skip to the end of the token
            while (_fileAsString[_characterPosition] != '>')
            {
                _characterPosition++;

                if (_characterPosition >= _fileLength)
                {
                    throw new ApplicationException("Data is corrupt in the XML file (at token end)");
                }
            }

            var length = _characterPosition - leftBracket + 1;

            XmlNodeContents = _fileAsString.Substring(leftBracket, length);

            if (XmlNodeContents.Substring(0, 4) == "<!--")
            {
                NodeType = XmlNodeType.Comment;
            }
            else if ((XmlNodeContents.Length > 5) && (XmlNodeContents.Substring(0, 5) == "<?xml"))
            {
                NodeType = XmlNodeType.ProcessingInstruction;
            }
            else
            {
                NodeType = XmlNodeContents.Substring(0, 2) == "</" ? XmlNodeType.EndElement : XmlNodeType.Element;
            }

            if (NodeType == XmlNodeType.Element)
            {
                var nodeNamePos = 0;

                while (nodeNamePos < XmlNodeContents.Length)
                {
                    var nodeChar = XmlNodeContents[nodeNamePos];

                    if ((nodeChar == ' ') || (nodeChar == '>'))
                    {
                        Name = XmlNodeContents.Substring(1, nodeNamePos - 1).TrimIt();

                        break;
                    }
                    nodeNamePos++;
                }
            }

            return true;
        }
    }
}
