using Antlr4.Runtime.Tree;
using Newtonsoft.Json;

namespace PrettierGML.Nodes
{
    internal abstract class GmlSyntaxNode
    {
        [JsonIgnore]
        public int StartIndex { get; set; } = -1;
        [JsonIgnore]
        public int EndIndex { get; set; } = -1;

        [JsonIgnore]
        public string OriginalText => "Get source text here!";

        [JsonIgnore]
        public GmlSyntaxNode? Parent { get; protected set; }

        [JsonIgnore]
        public List<GmlSyntaxNode> Children { get; protected set; } = new();

        [JsonIgnore]
        public bool IsEmpty => this is EmptyNode;

        public string Kind => GetType().Name;

        public GmlSyntaxNode() { }

        public GmlSyntaxNode(ISyntaxTree context)
        {
            StartIndex = context.SourceInterval.a;
            EndIndex = context.SourceInterval.b;
        }

        public static EmptyNode Empty => EmptyNode.Instance;

        public static NodeList List(ISyntaxTree context, IList<GmlSyntaxNode> contents) =>
            new(context, contents);

        protected GmlSyntaxNode AsChild(GmlSyntaxNode child)
        {
            Children.Add(child);
            child.Parent = this;
            return child;
        }

        public virtual Doc Print() => throw new NotImplementedException();

        public List<Doc> PrintChildren()
        {
            return Children.Select(child => child.Print()).ToList();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override int GetHashCode()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return serialized.GetHashCode();
        }

        //public static implicit operator Doc(GmlSyntaxNode node) => node.Print();
    }

    internal interface IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
    }
}
