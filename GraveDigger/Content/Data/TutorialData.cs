using System.Collections.Generic;

namespace GraveDigger.Data
{
    public class TutorialSectionData
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Items { get; set; } = new();
    }

    public class TutorialContractData
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string DocumentTitle { get; set; } = string.Empty;
        public string Preamble { get; set; } = string.Empty;
        public List<TutorialSectionData> Sections { get; set; } = new();
        public string FooterDeclaration { get; set; } = string.Empty;
        public string TraderSignature { get; set; } = string.Empty;
        public string KeeperSignaturePlaceholder { get; set; } = string.Empty;
        public string SignButtonText { get; set; } = string.Empty;
    }
}