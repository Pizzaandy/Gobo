namespace CSharpier.DocTypes;

internal class Fill : Doc
{
	public IList<Doc> Contents { get; set; }
	public Fill(IList<Doc> contents)
	{
		this.Contents = contents;
	}
}
